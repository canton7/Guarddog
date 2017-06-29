namespace IrcSays.Communication.Irc
{
	/// <summary>
	/// Describes all possible states of an IrcSession object.
	/// </summary>
	public enum IrcSessionState
	{
		/// <summary>
		/// The session is in the process of connecting. Either the server connection has not been established yet,
		/// or the user has not been registered.
		/// </summary>
		Connecting,

		/// <summary>
		/// The user has been registered with the IRC server and and has chosen a nickname. Commands can now be accepted.
		/// </summary>
		Connected,

		/// <summary>
		/// The session is not connected to any IRC server.
		/// </summary>
		Disconnected
	}
}