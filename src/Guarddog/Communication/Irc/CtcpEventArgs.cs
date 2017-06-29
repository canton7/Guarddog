namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a CTCP command sent from one client to another.
	/// </summary>
	public sealed class CtcpEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who sent the command.
		/// </summary>
		public IrcPeer From { get; private set; }

		/// <summary>
		///     Gets the target to which the command was sent. It could be sent to a channel or directly to the user who owns the
		///     IRC session.
		/// </summary>
		public IrcTarget To { get; private set; }

		/// <summary>
		///     Gets the CTCP command that was received.
		/// </summary>
		public CtcpCommand Command { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the received CTCP command is a response to a command that was previously sent.
		/// </summary>
		public bool IsResponse { get; private set; }

		internal CtcpEventArgs(IrcMessage message)
			: base(message)
		{
			From = message.From as IrcPeer;
			To = message.Parameters.Count > 0 ? new IrcTarget(message.Parameters[0]) : null;
			Command = message.Parameters.Count > 1 ? CtcpCommand.Parse(message.Parameters[1]) : null;
			IsResponse = message.Command == "NOTICE";
		}
	}
}