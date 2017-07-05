using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public delegate void CommandRegistrationHandler(IrcPeer from, string command, string text);

    public class CommandRegistration
    {
        public bool IsEnabled { get; set; }

        public string Command { get; }
        public CommandRegistrationHandler Handler { get; }

        public CommandRegistration(string command, CommandRegistrationHandler handler)
        {
            this.Command = command ?? throw new ArgumentNullException(nameof(command));
            this.Handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }
    }
}
