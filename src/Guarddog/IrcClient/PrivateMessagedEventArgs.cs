using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public class MessagedEventArgs : EventArgs
    {
        public IrcPeer From { get; }
        public string Message { get; }

        public MessagedEventArgs(IrcPeer from, string message)
        {
            this.From = from ?? throw new ArgumentNullException(nameof(from));
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
