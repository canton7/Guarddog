namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Represents an IRC server from which messages may be received.
	/// </summary>
	public sealed class IrcServer : IrcPrefix
	{
		/// <summary>
		///     Gets the name of the server.
		/// </summary>
		public string ServerName
		{
			get { return Prefix; }
		}

		internal IrcServer(string serverName)
			: base(serverName)
		{
		}
	}
}