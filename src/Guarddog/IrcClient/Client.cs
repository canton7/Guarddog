using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.IrcClient
{
    public class Client : IDisposable
    {
        internal IrcSession Session { get; }
        internal ClientConfig Config { get; }

        internal string SupportedChannelModes { get; private set; } = string.Empty;

        public IReadOnlyDictionary<string, Channel> Channels { get; }

        public event EventHandler<MessagedEventArgs> PrivateMessaged;

        public Client(ClientConfig config)
        {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));

            this.Session = new IrcSession();

            this.Channels = this.Config.Channels.ToDictionary(x => x.Name, x => new Channel(x.Name, this, x));

            this.Session.RawMessageReceived += (o, e) => Console.WriteLine("<< " + e.Message);
            this.Session.RawMessageSent += (o, e) => Console.WriteLine(">> " + e.Message);
            this.Session.AddHandler(new IrcCodeHandler(e =>
            {
                Session.Nick(Session.Nickname + "_");
                return true;
            }, IrcCode.ErrNicknameInUse));
            this.Session.StateChanged += (o, e) =>
            {
                if (Session.State == IrcSessionState.Connected)
                {
                    foreach (var channel in this.Config.Channels)
                    {
                        this.Session.Join(channel.Name);
                    }
                    if (!string.IsNullOrWhiteSpace(this.Config.NickServServicesName) && !string.IsNullOrWhiteSpace(this.Config.NickservPassword))
                    {
                        this.Session.PrivateMessage(new IrcTarget(this.Config.NickServServicesName), $"identify {this.Config.NickservPassword}");
                    }
                }
            };
            this.Session.PrivateMessaged += (o, e) =>
            {
                if (!e.To.IsChannel && e.To.Name == this.Config.Nickname)
                    this.PrivateMessaged?.Invoke(this, new MessagedEventArgs(e.From, e.Text));
            };
            this.Session.AddHandler(new IrcCodeHandler(e =>
            {
                var parts = e.Text.Split(' ');
                foreach (var part in parts)
                {
                    var values = part.Split('=');
                    if (values[0] == "CHANMODES")
                    {
                        this.SupportedChannelModes = values[1];
                    }
                }
                return false;
            }, IrcCode.RPL_BOUNCE));
        }

        public void Open()
        {
            this.Session.Open(this.Config.Server, this.Config.Port, this.Config.IsSecure, this.Config.Nickname, this.Config.Username, this.Config.RealName, true, this.Config.Password);
        }

        public void Dispose()
        {
            this.Session.Dispose();
        }
    }
}
