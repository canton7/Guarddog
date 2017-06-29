namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a chat message, such as a private message or message to a channel.
	/// </summary>
	public sealed class IrcMessageEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who sent the message.
		/// </summary>
		public IrcPeer From { get; private set; }

		/// <summary>
		///     Gets the target of the message. It could be sent to a channel or directly to the user who owns the IRC session.
		/// </summary>
		public IrcTarget To { get; private set; }

		/// <summary>
		///     Gets the message text.
		/// </summary>
		public string Text { get; private set; }

		internal IrcMessageEventArgs(IrcMessage message)
			: base(message)
		{
			From = message.From as IrcPeer;
			To = message.Parameters.Count > 0 ? new IrcTarget(message.Parameters[0]) : null;
			Text = message.Parameters.Count > 1 ? message.Parameters[1] : null;
		}
	}
}