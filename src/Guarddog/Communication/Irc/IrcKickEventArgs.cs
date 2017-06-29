namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a kick event.
	/// </summary>
	public sealed class IrcKickEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who performed the kick.
		/// </summary>
		public IrcPeer Kicker { get; private set; }

		/// <summary>
		///     Gets the channel from which someone has been kicked.
		/// </summary>
		public IrcTarget Channel { get; private set; }

		/// <summary>
		///     Gets the nickname of the user who was kicked.
		/// </summary>
		public string KickeeNickname { get; private set; }

		/// <summary>
		///     Gets the associated kick text, typically describing the reason for the kick.
		/// </summary>
		public string Text { get; private set; }

		internal IrcKickEventArgs(IrcMessage message)
			: base(message)
		{
			Kicker = message.From as IrcPeer;
			Channel = message.Parameters.Count > 0 ? new IrcTarget(message.Parameters[0]) : null;
			KickeeNickname = message.Parameters.Count > 1 ? message.Parameters[1] : null;
			Text = message.Parameters.Count > 2 ? message.Parameters[2] : null;
		}
	}
}