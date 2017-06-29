namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a part (leave) event.
	/// </summary>
	public sealed class IrcPartEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who left the channel.
		/// </summary>
		public IrcPeer Who { get; private set; }

		/// <summary>
		///     Gets the channel that the user left.
		/// </summary>
		public IrcTarget Channel { get; private set; }

		/// <summary>
		///     Gets the part text, if any exists.
		/// </summary>
		public string Text { get; private set; }

		internal IrcPartEventArgs(IrcMessage message)
			: base(message)
		{
			Who = message.From as IrcPeer;
			Channel = message.Parameters.Count > 0 ? new IrcTarget(message.Parameters[0]) : null;
			Text = message.Parameters.Count > 1 ? message.Parameters[1] : null;
		}
	}
}