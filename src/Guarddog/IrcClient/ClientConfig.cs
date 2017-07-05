using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public class ClientConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public bool IsSecure { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public string RealName { get; set; }
        public string Password { get; internal set; }
        public List<ChannelConfig> Channels { get; } = new List<ChannelConfig>();
        public string ChanServServicesName { get; set; } = "ChanServ";
        public string NickServServicesName { get; set; } = "NickServ";
        public string NickservPassword { get; set; }
        public bool PermanentlyOp { get; set; }
        public List<string> CommandPrefixes { get; } = new List<string>() { "!", "." };
        public bool RequireCommandPrefixInPrivmsg { get; set; } = false;
    }
}
