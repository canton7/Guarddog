using Guarddog.Data;
using Guarddog.Data.Bans;
using Guarddog.IrcClient;
using Guarddog.Modules;
using Guarddog.Modules.Akick;
using IrcSays.Communication.Irc;
using Microsoft.Data.Sqlite;
using SimpleMigrations;
using SimpleMigrations.Console;
using SimpleMigrations.DatabaseProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Guarddog
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connection = new ConnectionFactory().Create())
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
                Server = "ircd.antonymale.co.uk",
                Port = 6667,
                IsSecure = false,
                Nickname = "guarddog",
                RealName = "guarddog",
                Username = "guarddog",
                PermanentlyOp = true,
                Channels = {
                    new ChannelConfig() { Name = "#test" },
                    new ChannelConfig() { Name = "#admin" },
                },
                NickservPassword = "nickservpassword",
                RequireCommandPrefixInPrivmsg = true,
            };

            using (var client = new Client(clientConfig))
            {
                var banRepository = new BanRepository();
                var aKickModuleConfig = new AKickModuleConfig()
                {
                    Channels =
                    {
                        new AkickModuleChannelConfig()
                        {
                            OpChannel = client.Channels["#test"],
                            AdminChannels = { client.Channels["#admin"] },
                        },
                    }
                };

                var akick = new AkickModule(client, aKickModuleConfig, banRepository);
                akick.Load();

                client.Open();

                Console.ReadLine();
            }


            Console.WriteLine("Hello World!");
        }
    }
}