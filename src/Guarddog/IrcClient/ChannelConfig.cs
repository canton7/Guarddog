using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public class ChannelConfig
    {
        public string Name { get; set; }
        public bool? PermanentlyOp { get; set; }
    }
}
