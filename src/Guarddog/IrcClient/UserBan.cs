using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public enum UserBanType { Ban, Quiet }

    public class UserBan
    {
        public string Mask { get; }
        public string From { get; }
        public DateTime Date { get; }
        public char Type { get; }

        public UserBan(string mask, string from, DateTime date, char type)
        {
            this.Mask = mask ?? throw new ArgumentNullException(nameof(mask));
            this.From = from ?? throw new ArgumentNullException(nameof(from));
            this.Date = date;
            this.Type = type;
        }
    }
}
