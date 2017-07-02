using Microsoft.Data.Sqlite;
using System.Data.Common;

namespace Guarddog.Data
{
    public class ConnectionFactory
    {
        public DbConnection Create()
        {
            return new SqliteConnection("Data Source=database.sqlite");
        }
    }
}
