using System.Collections.Generic;
using System.Linq;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing a channel mode change event.
	/// </summary>
	public sealed class IrcChannelModeEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the user who performed the mode change.
		/// </summary>
		public IrcPeer Who { get; private set; }

		/// <summary>
		///     Gets the channel on which the modes were changed.
		/// </summary>
		public IrcTarget Channel { get; private set; }

		/// <summary>
		///     Gets the list of changed channel modes.
		/// </summary>
		public ICollection<IrcChannelMode> Modes { get; private set; }

		internal IrcChannelModeEventArgs(IrcMessage message)
			: base(message)
		{
			Who = message.From as IrcPeer;
			Channel = message.Parameters.Count > 0 ? new IrcTarget(message.Parameters[0]) : null;
			Modes = message.Parameters.Count > 1 ? IrcChannelMode.ParseModes(message.Parameters.Skip(1)) : null;
		}
	}
}