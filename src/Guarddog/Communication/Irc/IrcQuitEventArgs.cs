namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a quit event.
	/// </summary>
	public sealed class IrcQuitEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who quit.
		/// </summary>
		public IrcPeer Who { get; private set; }

		/// <summary>
		///     Gets the quit message.
		/// </summary>
		public string Text { get; private set; }

		internal IrcQuitEventArgs(IrcMessage message)
			: base(message)
		{
			Who = message.From as IrcPeer;
			Text = message.Parameters.Count > 0 ? message.Parameters[0] : null;
		}
	}
}