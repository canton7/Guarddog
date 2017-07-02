using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.Data.Migrations
{
    [Migration(1, "CreateBans")]
    public class CreateBans : Migration
    {
        protected override void Up()
        {
            // (Channel, Mask, Type) uniquely identify the ban. FromUser and Data change after a netsplit.
            // On conflict, the older info is more accurate - newer info might be from a netsplit.
            Execute(@"CREATE TABLE Bans (
                Id INTEGER PRIMARY KEY,
                Channel TEXT,
                Mask TEXT,
                FromUser TEXT,
                Date TEXT,
                Type TEXT,
                UNIQUE (Channel, Mask, Type) ON CONFLICT IGNORE
            );");
        }

        protected override void Down()
        {
            Execute(@"DROP TABLE Bans");
        }
    }
}
