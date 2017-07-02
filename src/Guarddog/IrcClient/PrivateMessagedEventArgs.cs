using IrcSays.Communication.Irc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Guarddog.IrcClient
{
    public class PrivateMessagedEventArgs : EventArgs
    {
        public IrcPeer From { get; }
        public string Message { get; }

        public PrivateMessagedEventArgs(IrcPeer from, string message)
        {
            this.From = from ?? throw new ArgumentNullException(nameof(from));
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}
