using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Guarddog.IrcClient
{
    internal class CommandRegistrationManager
    {
        private readonly Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();
        private readonly List<string> prefixes;
        private readonly bool requirePrefix;

        public CommandRegistrationManager(List<string> prefixes, bool requirePrefix)
        {
            this.prefixes = prefixes ?? throw new ArgumentNullException(nameof(prefixes));
            this.requirePrefix = requirePrefix;
        }

        public string TryDispatchCommand(IrcPeer from, string text)
        {
            var matchedPrefix = this.prefixes.FirstOrDefault(x => text.StartsWith(x));
            if (!this.requirePrefix || matchedPrefix != null)
            {
                if (matchedPrefix != null)
                    text = text.Substring(matchedPrefix.Length);

                var parts = text.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    if (this.commands.TryGetValue(parts[0], out var registration))
                    {
                        if (registration.IsEnabled)
                            registration.Handler(from, parts[0], parts.Length > 1 ? parts[1] : string.Empty);
                    }
                    else
                    {
                        return $"Sorry, I don't recognise the command '{parts[0]}'";
                    }
                }
            }
            else
            {
                return "Sorry, commands start with " + string.Join(" or ", this.prefixes.Select(x => $"'{x}'"));
            }

            return null;
        }

        public void Add(CommandRegistration registration)
        {
            this.commands.Add(registration.Command, registration);
        }

        public void Remove(CommandRegistration registration)
        {
            if (this.commands.TryGetValue(registration.Command, out var existingRegistration))
            {
                if (existingRegistration == registration)
                    this.commands.Remove(registration.Command);
            }
        }
    }
}
