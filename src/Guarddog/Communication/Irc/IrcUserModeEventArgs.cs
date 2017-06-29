using System.Collections.Generic;
using System.Linq;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a user mode change event. This event always applies to the user who
	///     owns the IRC session.
	/// </summary>
	public sealed class IrcUserModeEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the list of changed user modes.
		/// </summary>
		public ICollection<IrcUserMode> Modes { get; private set; }

		internal IrcUserModeEventArgs(IrcMessage message)
			: base(message)
		{
			Modes = message.Parameters.Count > 1 ? IrcUserMode.ParseModes(message.Parameters.Skip(1)) : null;
		}
	}
}