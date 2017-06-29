namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a join event.
	/// </summary>
	public sealed class IrcJoinEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who joined.
		/// </summary>
		public IrcPeer Who { get; private set; }

		/// <summary>
		///     Gets the channel that the user joined.
		/// </summary>
		public IrcTarget Channel { get; private set; }

		internal IrcJoinEventArgs(IrcMessage message)
			: base(message)
		{
			Who = message.From as IrcPeer;
			Channel = message.Parameters.Count > 0 ? new IrcTarget(message.Parameters[0]) : null;
		}
	}
}