using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.IrcClient
{
    public class Client : IDisposable
    {
        private readonly IrcSession session;
        private readonly ClientConfig config;

        public IReadOnlyDictionary<string, Channel> Channels { get; }

        public Client(ClientConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            this.session = new IrcSession();

            this.Channels = this.config.Channels.ToDictionary(x => x, x => new Channel(x, this.session));

            this.session.RawMessageReceived += (o, e) => Console.WriteLine("<< " + e.Message);
            this.session.RawMessageSent += (o, e) => Console.WriteLine(">> " + e.Message);
            this.session.AddHandler(new IrcCodeHandler(e =>
            {
                session.Nick(session.Nickname + "_");
                return true;
            }, IrcCode.ErrNicknameInUse));
            this.session.StateChanged += (o, e) =>
            {
                if (session.State == IrcSessionState.Connected)
                {
                    foreach (var channel in this.config.Channels)
                    {
                        this.session.Join(channel);
                    }
                }
            };
        }

        public void Open()
        {
            this.session.Open(this.config.Server, this.config.Port, this.config.IsSecure, this.config.Nickname, this.config.Username, this.config.RealName, true, this.config.Password);
        }

        public void Dispose()
        {
            this.session.Dispose();
        }
    }
}
