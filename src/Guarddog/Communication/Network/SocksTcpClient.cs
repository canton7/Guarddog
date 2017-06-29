using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IrcSays.Communication.Network
{
	public class SocksTcpClient
	{
		private const int SocksTimeout = 30000;

		private class AsyncResult : IAsyncResult
		{
			public object AsyncState { get; private set; }

			public WaitHandle AsyncWaitHandle
			{
				get { return Event; }
			}

			public bool CompletedSynchronously
			{
				get { return false; }
			}

			public bool IsCompleted
			{
				get { return AsyncWaitHandle.WaitOne(0); }
			}

			public EventWaitHandle Event { get; private set; }
			public AsyncCallback Callback { get; private set; }
			public string Hostname { get; set; }
			public int Port { get; set; }
			public Exception Exception { get; set; }
			public TcpClient Client { get; set; }

			public AsyncResult(AsyncCallback callback, object state)
			{
				Event = new ManualResetEvent(false);
				AsyncState = state;
			}
		}

		private readonly ProxyInfo _info;

		public SocksTcpClient(ProxyInfo proxy)
		{
			_info = proxy;
		}

        public async Task<TcpClient> ConnectAsync(string hostName, int port, CancellationToken token)
        {
            var client = new TcpClient();
            using (token.Register(() => client.Dispose()))
            {
                await client.ConnectAsync(_info.ProxyHostname, _info.ProxyPort);
            }
            token.ThrowIfCancellationRequested();

            var stream = client.GetStream();
            await DoHandshakeAsync(stream, hostName, port);
            return client;
        }

		private async Task DoHandshakeAsync(NetworkStream stream, string hostName, int port)
		{
			var useAuth = _info.ProxyUsername != null && _info.ProxyPassword != null;

			stream.WriteByte(0x05); // SOCKS v5
			stream.WriteByte(useAuth ? (byte) 0x2 : (byte) 0x1); // auth protocols supported
			stream.WriteByte(0x00); // no auth
			if (useAuth)
			{
				stream.WriteByte(0x02); // user/pass
			}

			var buffer = new byte[256];
			await ReadAsync(stream, buffer, 2);

			var method = buffer[1];
			if (method == 0x2)
			{
				stream.WriteByte(0x1);
				WriteString(stream, _info.ProxyUsername);
				WriteString(stream, _info.ProxyPassword);
				await ReadAsync(stream, buffer, 2);
				if (buffer[1] != 0x0)
				{
					throw new SocksException(string.Format("Proxy authentication failed with code {0}.", buffer[1].ToString()));
				}
			}

			stream.WriteByte(0x5); // SOCKS v5
			stream.WriteByte(0x1); // TCP stream
			stream.WriteByte(0x0); // reserved
			stream.WriteByte(0x3); // domain name
			WriteString(stream, hostName); // hostname
			stream.WriteByte((byte) ((port >> 8) & 0xFF)); // port high byte
			stream.WriteByte((byte) (port & 0xff)); // port low byte

			await ReadAsync(stream, buffer, 4);
			switch (buffer[1])
			{
				case 0x1:
					throw new SocksException("General failure.");
				case 0x2:
					throw new SocksException("Connection not allowed by ruleset.");
				case 0x3:
					throw new SocksException("Network unreachable.");
				case 0x4:
					throw new SocksException("Host unreachable.");
				case 0x5:
					throw new SocksException("Connection refused by destination host.");
				case 0x6:
					throw new SocksException("TTL expired.");
				case 0x7:
					throw new SocksException("Command not supported / protocol error.");
				case 0x8:
					throw new SocksException("Address type not supported.");
			}
			switch (buffer[3])
			{
				case 0x1:
					await ReadAsync(stream, buffer, 4);
					break;
				case 0x3:
					await ReadStringAsync(stream, buffer);
					break;
				case 0x4:
					await ReadAsync(stream, buffer, 16);
					break;
			}
			await ReadAsync(stream, buffer, 2);
		}

		private void WriteString(NetworkStream stream, string str)
		{
			if (str.Length > 255)
			{
				str = str.Substring(0, 255);
			}
			stream.WriteByte((byte) str.Length);
			var buffer = Encoding.ASCII.GetBytes(str);
			stream.Write(buffer, 0, buffer.Length);
		}

		private async Task<string> ReadStringAsync(NetworkStream stream, byte[] buffer)
		{
			await ReadAsync(stream, buffer, 1);
			int length = buffer[0];
			await ReadAsync(stream, buffer, length);
			return Encoding.ASCII.GetString(buffer, 0, length);
		}

		private async Task<int> ReadAsync(NetworkStream stream, byte[] buffer, int count)
		{
            var cts = new CancellationTokenSource(SocksTimeout);
            int read;
            try
            {
                read = await stream.ReadAsync(buffer, 0, count, cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new SocksException("The proxy did not respond in a timely manner.");
            }
			if (read < 2)
			{
				throw new SocksException("Unable to negotiate with the proxy.");
			}
			return read;
		}
	}
}