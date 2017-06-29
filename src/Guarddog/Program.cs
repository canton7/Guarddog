using IrcSays.Communication.Irc;
using System;

namespace Guarddog
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var session = new IrcSession())
            {
                session.Open("chat.freenode.net", 6697, false, "guarddog-bot", "guarddog-bot", "guarddog-bot", true);

                Console.ReadLine();
            }

            Console.WriteLine("Hello World!");
        }
    }
}