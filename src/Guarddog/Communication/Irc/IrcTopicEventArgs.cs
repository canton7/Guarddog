namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a topic change event.
	/// </summary>
	public sealed class IrcTopicEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who changed the topic.
		/// </summary>
		public IrcPeer Who { get; private set; }

		/// <summary>
		///     Gets the channel in which the topic was changed.
		/// </summary>
		public IrcTarget Channel { get; private set; }

		/// <summary>
		///     Gets the new topic text.
		/// </summary>
		public string Text { get; private set; }

		internal IrcTopicEventArgs(IrcMessage message)
			: base(message)
		{
			Who = message.From as IrcPeer;
			Channel = message.Parameters.Count > 0 ? new IrcTarget(message.Parameters[0]) : null;
			Text = message.Parameters.Count > 1 ? message.Parameters[1] : null;
		}
	}
}