namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Represents an abstract IRC prefix which could refer to either a user or a server.
	/// </summary>
	public abstract class IrcPrefix
	{
		/// <summary>
		///     Gets the raw prefix.
		/// </summary>
		public string Prefix { get; private set; }

		internal IrcPrefix(string prefix)
		{
			Prefix = prefix;
		}

		/// <summary>
		///     Gets the raw prefix.
		/// </summary>
		/// <returns>Returns the raw prefix.</returns>
		public override string ToString()
		{
			return Prefix;
		}

		internal static IrcPrefix Parse(string prefix)
		{
			if (string.IsNullOrEmpty(prefix))
			{
				return null;
			}

			var idx1 = prefix.IndexOf('!');
			var idx2 = prefix.IndexOf('@');
			if (idx1 > 0 &&
				idx2 > 0 &&
				idx2 > idx1 + 1)
			{
				return new IrcPeer(prefix);
			}
			return new IrcServer(prefix);
		}
	}
}