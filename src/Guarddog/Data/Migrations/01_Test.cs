using SimpleMigrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.Data.Migrations
{
    [Migration(1, "Test")]
    public class Test : Migration
    {
        protected override void Up()
        {
            Execute(@"CREATE TABLE foo (id SERIAL);");
        }

        protected override void Down()
        {
            Execute(@"DROP TABLE foo");
        }
    }
}
