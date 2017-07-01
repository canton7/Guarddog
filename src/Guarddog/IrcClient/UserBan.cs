using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public enum UserBanType { Ban, Mute }

    public class UserBan
    {
        public string Mask { get; }
        public string From { get; }
        public DateTime Date { get; }
        public UserBanType Type { get; }

        public UserBan(string mask, string from, DateTime date, UserBanType type)
        {
            this.Mask = mask ?? throw new ArgumentNullException(nameof(mask));
            this.From = from ?? throw new ArgumentNullException(nameof(from));
            this.Date = date;
            this.Type = type;
        }
    }
}
