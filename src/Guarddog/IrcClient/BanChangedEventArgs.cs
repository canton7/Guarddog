using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public class BanChangedEventArgs : EventArgs
    {
        public UserBan Ban { get; }

        public BanChangedEventArgs(UserBan ban)
        {
            this.Ban = ban ?? throw new ArgumentNullException(nameof(ban));
        }
    }
}
