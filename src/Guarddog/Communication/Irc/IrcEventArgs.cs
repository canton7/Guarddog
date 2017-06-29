using System;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing an IRC event.
	/// </summary>
	public class IrcEventArgs : EventArgs
	{
		/// <summary>
		///     Gets the raw IRC message received or sent.
		/// </summary>
		public IrcMessage Message { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether the message has been handled.
		/// </summary>
		public bool Handled { get; set; }

		internal IrcEventArgs(IrcMessage message)
		{
			Message = message;
		}
	}
}