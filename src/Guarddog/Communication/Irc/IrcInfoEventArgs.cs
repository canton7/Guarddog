using System.Linq;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing miscellaneous information received from an IRC server. A numeric code
	///     is assigned to each message, typically describing the results of a command or an error that occurred.
	/// </summary>
	public sealed class IrcInfoEventArgs : IrcEventArgs
	{
		/// <summary>
		///     Gets the numeric IRC code.
		/// </summary>
		public IrcCode Code { get; private set; }

		/// <summary>
		///     Gets the text following the numeric code.
		/// </summary>
		public string Text { get; private set; }

		/// <summary>
		///     Gets a value indicating whether the numeric code indicates than an error has occurred.
		/// </summary>
		public bool IsError { get; private set; }

		internal IrcInfoEventArgs(IrcMessage message)
			: base(message)
		{
			int code;
			if (int.TryParse(message.Command, out code))
			{
				Code = (IrcCode) code;
			}

			Text = message.Parameters.Count > 1 ? string.Join(" ", message.Parameters.Skip(1).ToArray()) : null;
			IsError = (int) Code >= 400;
		}
	}
}