using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public class BanListReloadedEventArgs : EventArgs
    {
        public IReadOnlyList<UserBan> BanList { get; }

        public BanListReloadedEventArgs(IReadOnlyList<UserBan> banList)
        {
            this.BanList = banList ?? throw new ArgumentNullException(nameof(banList));
        }
    }
}
