using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.Data.Bans
{
    public class BanRecord
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public string Mask { get; set; }
        public string FromUser { get; set; }
        public DateTime Date { get; set; }
        public char Type { get; set; }
    }
}
