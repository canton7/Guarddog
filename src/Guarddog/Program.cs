using Guarddog.IrcClient;
using IrcSays.Communication.Irc;
using Microsoft.Data.Sqlite;
using SimpleMigrations;
using SimpleMigrations.Console;
using SimpleMigrations.DatabaseProvider;
using System;
using System.Linq;
using System.Reflection;

namespace Guarddog
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new SqliteConnection("Data Source=database.sqlite"))
            {
                var databaseProvider = new SqliteDatabaseProvider(connection);
                var migrator = new SimpleMigrator(Assembly.GetEntryAssembly(), databaseProvider, new ConsoleLogger());
                migrator.Load();
                if (args.ElementAtOrDefault(0) == "migrate")
                {
                    var runner = new ConsoleRunner(migrator);
                    runner.Run(args.Skip(1).ToArray());
                    return;
                }
                else
                {
                    migrator.MigrateToLatest();
                }
            }

            var clientConfig = new ClientConfig()
            {
                Server = "chat.freenode.net",
                Port = 6697,
                IsSecure = true,
                Nickname = "guarddog-test",
                RealName = "guarddog-test",
                Username = "guarddog-test",
                Channels = { "#botwar" },
            };

            using (var client = new Client(clientConfig))
            {
                client.Open();

                Console.ReadLine();
            }


            Console.WriteLine("Hello World!");
        }
    }
}