using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.Data.Bans
{
    public class BanRepository
    {
        private readonly ConnectionFactory factory = new ConnectionFactory();

        // Unique index is (Channel, Mask, Type)

        public void InsertBans(IEnumerable<BanRecord> bans)
        {
            using (var conn = this.factory.Create())
            {
                conn.Execute(@"INSERT INTO Bans(Channel, Mask, FromUser, Date, Type) VALUES(@Channel, @Mask, @FromUser, @Date, @Type)", bans);
            }
        }

        public void DeleteBan(BanRecord ban)
        {
            using (var conn = this.factory.Create())
            {
                conn.Execute(@"DELETE FROM Bans WHERE Channel = @Channel AND Mask = @Mask AND Type = @Type", ban);
            }
        }
    }
}
