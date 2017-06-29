namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments for an invite event.
	/// </summary>
	public sealed class IrcInviteEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who sent the invite.
		/// </summary>
		public IrcPeer From { get; private set; }

		/// <summary>
		///     Gets the channel to which the target was invited.
		/// </summary>
		public string Channel { get; private set; }

		internal IrcInviteEventArgs(IrcMessage message)
			: base(message)
		{
			From = message.From as IrcPeer;
			Channel = message.Parameters.Count > 1 ? message.Parameters[1] : null;
		}
	}
}