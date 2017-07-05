using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.Modules.Akick
{
    public class AKickModuleConfig
    {
        public string BanCommand { get; set; } = "ban";

        public List<AkickModuleChannelConfig> Channels { get; } = new List<AkickModuleChannelConfig>();
    }
}
