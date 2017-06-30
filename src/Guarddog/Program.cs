using Guarddog.IrcClient;
using IrcSays.Communication.Irc;
using System;

namespace Guarddog
{
    class Program
    {
        static void Main(string[] args)
        {
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