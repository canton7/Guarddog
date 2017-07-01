using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.IrcClient
{
    public class Channel
    {
        private readonly IrcSession session;

        public string Name { get; }
        public bool IsJoined { get; private set; }
        public bool IsOp { get; private set; }
        private List<UserBan> wipBanList = new List<UserBan>();
        public IReadOnlyList<UserBan> BanList { get; private set; } = new List<UserBan>();

        internal Channel(string name, IrcSession session)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.session = session ?? throw new ArgumentNullException(nameof(session));

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
                            var type = mode.Mode == 'b' ? UserBanType.Ban : UserBanType.Mute;
                            var newBanList = new List<UserBan>(this.BanList);
                            if (mode.Set)
                            {
                                newBanList.Add(new UserBan(mode.Parameter, e.Who.Prefix, DateTime.UtcNow, type));
                            }
                            else
                            {
                                newBanList.RemoveAll(x => x.Mask == mode.Parameter && x.Type == type);
                            }
                            this.BanList = newBanList;
                        }
                    }
                }
            };
            this.session.AddHandler(new IrcCodeHandler(e =>
            {
                var parameters = e.Message.Parameters;
                if (parameters[1] == this.Name)
                {
                    var ban = new UserBan(parameters[2], parameters[3], DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parameters[4])).DateTime, UserBanType.Ban);
                    this.wipBanList.Add(ban);
                }

                return false;
            }, IrcCode.RPL_BANLIST));
            this.session.AddHandler(new IrcCodeHandler(e =>
            {
                if (e.Message.Parameters[1] == this.Name)
                {
                    this.BanList = this.wipBanList.Concat(this.BanList.Where(x => x.Type == UserBanType.Mute)).ToList();
                    this.wipBanList = new List<UserBan>();
                }
                return false;
            }, IrcCode.RPL_ENDOFBANLIST));
            this.session.AddHandler(new IrcCodeHandler(e =>
            {
                var parameters = e.Message.Parameters;
                if (parameters[1] == this.Name && parameters[2] == "q")
                {
                    var ban = new UserBan(parameters[3], parameters[4], DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(parameters[5])).DateTime, UserBanType.Mute);
                    this.wipBanList.Add(ban);
                }
                return false;
            }, IrcCode.RPL_MUTELIST));
            this.session.AddHandler(new IrcCodeHandler(e =>
            {
                if (e.Message.Parameters[1] == this.Name && e.Message.Parameters[2] == "q")
                {
                    this.BanList = this.wipBanList.Concat(this.BanList.Where(x => x.Type == UserBanType.Ban)).ToList();
                    this.wipBanList = new List<UserBan>();
                }
                return false;
            }, IrcCode.RPL_ENDOFMUTELIST));
        }

        private void OnJoined()
        {
            this.session.Mode(this.Name, "+b");
            this.session.Mode(this.Name, "+q");
        }
    }
}
