namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Represents another user on the IRC network, identified by a prefix (nick!user@host), and exposes the separated
	///     properties of the prefix.
	/// </summary>
	public sealed class IrcPeer : IrcPrefix
	{
		/// <summary>
		///     Gets the user's nickname.
		/// </summary>
		public string Nickname { get; private set; }

		/// <summary>
		///     Gets the user's username.
		/// </summary>
		public string Username { get; private set; }

		/// <summary>
		///     Gets the user's hostname.
		/// </summary>
		public string Hostname { get; private set; }

		internal IrcPeer(string nickUserHost)
			: base(nickUserHost)
		{
			var parts = nickUserHost.Split('@');

			if (parts.Length > 1)
			{
				Hostname = parts[1];
			}

			if (parts.Length > 0)
			{
				parts = parts[0].Split('!');
				if (parts.Length > 0)
				{
					Username = parts[1];
				}
				if (parts.Length > 0)
				{
					Nickname = parts[0];
				}
			}
		}
	}
}