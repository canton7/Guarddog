using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.IrcClient
{
    public class Channel
    {
        private readonly Client client;
        private readonly bool permanentlyOp;

        private bool muteSupported => this.client.SupportedChannelModes.Contains('q');
        private IrcSession session => this.client.Session;

        public string Name { get; }
        public bool IsJoined { get; private set; }
        public bool IsOp { get; private set; }
        private List<UserBan> wipBanList = new List<UserBan>();
        public IReadOnlyList<UserBan> BanList { get; private set; } = new List<UserBan>();

        public event EventHandler Joined;
        public event EventHandler<BanListReloadedEventArgs> BanListReloaded;
        public event EventHandler<BanChangedEventArgs> BanAdded;
        public event EventHandler<BanChangedEventArgs> BanRemoved;
        public event EventHandler<MessagedEventArgs> Messaged;

        internal Channel(string name, Client client, ChannelConfig channelConfig)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.permanentlyOp = channelConfig.PermanentlyOp ?? this.client.Config.PermanentlyOp;

            this.session.SelfJoined += (o, e) =>
            {
                if (e.Channel.IsChannel && e.Channel.Name == this.Name)
                {
                    this.IsJoined = true;
                    this.OnJoined();
                }
            };
            this.session.SelfParted += (o, e) =>
            {
                if (e.Channel.IsChannel && e.Channel.Name == this.Name)
                {
                    this.IsJoined = false;
                }
            };
            this.session.SelfKicked += (o, e) =>
            {
                if (e.Channel.IsChannel && e.Channel.Name == this.Name)
                {
                    this.IsJoined = false;
                }
            };
            this.session.PrivateMessaged += (o, e) =>
            {
                if (e.To.IsChannel && e.To.Name == this.Name)
                    this.Messaged?.Invoke(this, new MessagedEventArgs(e.From, e.Text));
            };
            this.session.ChannelModeChanged += (o, e) =>
            {
                if (e.Channel.IsChannel && e.Channel.Name == this.Name)
                {
                    foreach (var mode in e.Modes)
                    {
                        if (mode.Parameter == this.session.Nickname && mode.Mode == 'o')
                        {
                            this.IsOp = mode.Set;
                        }
                        else if (mode.Mode == 'b' || mode.Mode == 'q')
                        {
                            var newBanList = new List<UserBan>(this.BanList);
                            Action eventRaiser = null;
                            if (mode.Set)
                            {
                                var ban = new UserBan(mode.Parameter, e.Who.Prefix, DateTime.UtcNow, mode.Mode);
                                newBanList.Add(ban);
                                eventRaiser = () => this.BanAdded?.Invoke(this, new BanChangedEventArgs(ban));
                            }
                            else
                            {
                                var toRemove = newBanList.FirstOrDefault(x => x.Mask == mode.Parameter && x.Type == mode.Mode);
                                if (toRemove != null)
                                {
                                    newBanList.Remove(toRemove);
                                    eventRaiser = () => this.BanRemoved?.Invoke(this, new BanChangedEventArgs(toRemove));
                                }
                            }
                            this.BanList = newBanList;
                            eventRaiser?.Invoke();
                        }
                    }
                }
            };

            // These 4 form a chain - we always request bans, and when they finish we request mutes (if supported_
            this.session.AddHandler(new IrcCodeHandler(e =>
            {
                var parameters = e.Message.Parameters;
                if (parameters[1] == this.Name)
                {
                    var ban = new UserBan(parameters[2], parameters[3], DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parameters[4]) * 1000).DateTime, 'b');
                    this.wipBanList.Add(ban);
                }
                return false;
            }, IrcCode.RPL_BANLIST));
            this.session.AddHandler(new IrcCodeHandler(e =>
            {
                if (e.Message.Parameters[1] == this.Name)
                {
                    if (this.muteSupported)
                    {
                        this.session.Mode(this.Name, "+q");
                    }
                    else
                    {
                        this.BanList = this.wipBanList;
                        this.wipBanList = new List<UserBan>();
                        this.BanListReloaded?.Invoke(this, new BanListReloadedEventArgs(this.BanList));
                    }
                }
                return false;
            }, IrcCode.RPL_ENDOFBANLIST));

            if (this.muteSupported)
            {
                this.session.AddHandler(new IrcCodeHandler(e =>
                {
                    var parameters = e.Message.Parameters;
                    if (parameters[1] == this.Name && parameters[2] == "q")
                    {
                        var ban = new UserBan(parameters[3], parameters[4], DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parameters[5])).DateTime, 'q');
                        this.wipBanList.Add(ban);
                    }
                    return false;
                }, (IrcCode)728));
                this.session.AddHandler(new IrcCodeHandler(e =>
                {
                    if (e.Message.Parameters[1] == this.Name && e.Message.Parameters[2] == "q")
                    {
                        this.BanList = this.wipBanList;
                        this.wipBanList = new List<UserBan>();
                        this.BanListReloaded?.Invoke(this, new BanListReloadedEventArgs(this.BanList));
                    }
                    return false;
                }, (IrcCode)729));
            }
        }

        private void OnJoined()
        {
            this.session.Mode(this.Name, "+b");
            if (!string.IsNullOrWhiteSpace(this.client.Config.ChanServServicesName) && this.permanentlyOp)
            {
                this.session.PrivateMessage(new IrcTarget(this.client.Config.ChanServServicesName), $"OP {this.Name}");
            }

            this.Joined?.Invoke(this, EventArgs.Empty);
        }
    }
}
