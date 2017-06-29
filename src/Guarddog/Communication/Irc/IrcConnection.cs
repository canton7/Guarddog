using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using IrcSays.Communication.Network;
using System.Threading.Tasks;

namespace IrcSays.Communication.Irc
{
	internal sealed class IrcConnection : IDisposable
	{
		private const int HeartbeatInterval = 300000;
        private readonly SynchronizationContext _syncContext;

        private string _server;
		private int _port;
		private bool _isSecure;
		private ProxyInfo _proxy;

		private TcpClient _tcpClient;
		private BlockingCollection<IrcMessage> _writeQueue;
        private CancellationTokenSource _closeCts;

		public event EventHandler Connected;
		public event EventHandler Disconnected;
		public event EventHandler Heartbeat;
		public event EventHandler<ErrorEventArgs> Error;
		public event EventHandler<IrcEventArgs> MessageReceived;
		public event EventHandler<IrcEventArgs> MessageSent;

		public IrcConnection()
		{
			_syncContext = SynchronizationContext.Current;
		}

		public void Open(string server, int port, bool isSecure, ProxyInfo proxy = null)
		{
			if (string.IsNullOrEmpty(server))
			{
				throw new ArgumentNullException(nameof(server));
			}
			if (port <= 0 || port > 65535)
			{
				throw new ArgumentOutOfRangeException(nameof(port));
			}

			if (_closeCts != null)
			{
				Close();
			}

			_server = server;
			_port = port;
			_isSecure = isSecure;
			_proxy = proxy;
			_writeQueue = new BlockingCollection<IrcMessage>();
            _closeCts = new CancellationTokenSource();

            SocketMain(_closeCts.Token);
		}

		public void Close()
		{
			if (_closeCts != null)
			{
                _closeCts.Cancel();
				OnDisconnected();
				_closeCts = null;
			}
		}

		public void QueueMessage(string message)
		{
			QueueMessage(IrcMessage.Parse(message));
		}

		public void QueueMessage(IrcMessage message)
		{
			if (_writeQueue == null)
			{
				throw new InvalidOperationException("The connection is not open.");
			}

			_writeQueue.Add(message);
		}

		public void Dispose()
		{
			Close();
		}

		private async void SocketMain(CancellationToken token)
		{
            try
            {
                await SocketLoopAsync(token);
            }
            catch (IOException ex)
            {
                Dispatch(OnError, ex);
            }
            catch (SocketException ex)
            {
                Dispatch(OnError, ex);
            }
            catch (SocksException ex)
            {
                Dispatch(OnError, ex);
            }
            catch (OperationCanceledException) { }

			_tcpClient?.Dispose();
            _tcpClient = null;
		}

		private async Task SocketLoopAsync(CancellationToken token)
		{
			if (!string.IsNullOrEmpty(_proxy?.ProxyHostname))
			{
				var proxy = new SocksTcpClient(_proxy);
                _tcpClient = await proxy.ConnectAsync(_server, _port, token);
			}
			else
			{
                _tcpClient = new TcpClient();
                using (token.Register(() => _tcpClient.Dispose()))
                {
                    await _tcpClient.ConnectAsync(_server, _port);
                }
                token.ThrowIfCancellationRequested();
            }

            Stream stream = _tcpClient.GetStream();

			if (_isSecure)
			{
				// Just accept all server certs for now; we'll take advantage of the encryption
				// but not the authentication unless users ask for it.
				var sslStream = new SslStream(stream, true, (sender, cert, chain, sslPolicyErrors) => true);
				await sslStream.AuthenticateAsClientAsync(_server);
				stream = sslStream;
			}

			Dispatch(OnConnected);

            async Task ReadLoopAsync()
            {
                byte[] readBuffer = new byte[512];
                var gotCR = false;
                var input = new List<byte>(512);

                while (_tcpClient.Connected)
                {
                    var count = await stream.ReadAsync(readBuffer, 0, readBuffer.Length, token);
                    if (count == 0)
                    {
                        _tcpClient.Dispose();
                    }
                    else
                    {
                        for (var i = 0; i < count; i++)
                        {
                            switch (readBuffer[i])
                            {
                                case 0xa:
                                    if (gotCR)
                                    {
                                        var incoming = IrcMessage.Parse(Encoding.UTF8.GetString(input.ToArray()));
                                        Dispatch(OnMessageReceived, incoming);
                                        input.Clear();
                                    }
                                    break;
                                case 0xd:
                                    break;
                                default:
                                    input.Add(readBuffer[i]);
                                    break;
                            }
                            gotCR = readBuffer[i] == 0xd;
                        }
                    }
                }
            };

            Task WriteLoopAsync()
            {
                return Task.Run(async () =>
                {
                    byte[] writeBuffer = new byte[Encoding.UTF8.GetMaxByteCount(512)];

                    while (_tcpClient.Connected)
                    {
                        var outgoing = _writeQueue.Take(token);
                        var output = outgoing.ToString();
                        int count = Encoding.UTF8.GetBytes(output, 0, output.Length, writeBuffer, 0);
                        count = Math.Min(510, count);
                        writeBuffer[count] = 0xd;
                        writeBuffer[count + 1] = 0xa;
                        await stream.WriteAsync(writeBuffer, 0, count + 2);
                        Dispatch(OnMessageSent, outgoing);
                    }
                }, token);
            };

            async Task HeartbeatLoopAsync()
            {
                while (_tcpClient.Connected)
                {
                    await Task.Delay(HeartbeatInterval, token);
                    Dispatch(OnHeartbeat);
                }
            };

            try
            {
                await Task.WhenAll(ReadLoopAsync(), WriteLoopAsync(), HeartbeatLoopAsync());
            }
            finally
            {
                Dispatch(OnDisconnected);
            }
		}

		private void Dispatch<T>(Action<T> action, T arg)
		{
			if (_syncContext != null)
			{
				_syncContext.Post(o => action((T) o), arg);
			}
			else
			{
				action(arg);
			}
		}

		private void Dispatch(Action action)
		{
			if (_syncContext != null)
			{
				_syncContext.Post(o => ((Action) o)(), action);
			}
			else
			{
				action();
			}
		}

		private void OnConnected()
		{
			var handler = Connected;
			handler?.Invoke(this, EventArgs.Empty);
		}

		private void OnDisconnected()
		{
			var handler = Disconnected;
			handler?.Invoke(this, EventArgs.Empty);
		}

		private void OnHeartbeat()
		{
			var handler = Heartbeat;
			handler?.Invoke(this, EventArgs.Empty);
		}

		private void OnError(Exception ex)
		{
			var handler = Error;
			handler?.Invoke(this, new ErrorEventArgs(ex));
			Close();
		}

		private void OnMessageReceived(IrcMessage message)
		{
			var handler = MessageReceived;
			handler?.Invoke(this, new IrcEventArgs(message));
		}

		private void OnMessageSent(IrcMessage message)
		{
			var handler = MessageSent;
			handler?.Invoke(this, new IrcEventArgs(message));
		}
	}
}