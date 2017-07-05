using Guarddog.IrcClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.Modules.Akick
{
    public class AkickModuleChannelConfig
    {
        public Channel OpChannel { get; set; }
        public List<Channel> AdminChannels { get; } = new List<Channel>();
    }
}
