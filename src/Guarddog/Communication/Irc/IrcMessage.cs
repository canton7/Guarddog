using System.Collections.Generic;
using System.Text;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Represents a raw IRC message received from or sent to the IRC server, in accordance with RFC 2812.
	/// </summary>
	public sealed class IrcMessage
	{
		/// <summary>
		///     Gets the prefix that indicates the source of the message.
		/// </summary>
		public IrcPrefix From { get; private set; }

		/// <summary>
		///     Gets the name of the command.
		/// </summary>
		public string Command { get; private set; }

		/// <summary>
		///     Gets the list of parameters.
		/// </summary>
		public IReadOnlyList<string> Parameters { get; private set; }

        public bool ExcludeSeparator { get; }

		internal IrcMessage(string command, params string[] parameters)
			: this(null, command, parameters)
		{
		}

        internal IrcMessage(string command, bool excludeSeparator, params string[] parameters)
            : this(null, command, parameters)
        {
            ExcludeSeparator = excludeSeparator;
        }

        internal IrcMessage(IrcPrefix prefix, string command, params string[] parameters)
		{
			From = prefix;
			Command = command;
			Parameters = parameters;
		}

		/// <summary>
		///     Convert the message into a string that can be sent to an IRC server or printed to a debug window.
		/// </summary>
		/// <returns>Returns the IRC message formatted in accordance with RFC 2812.</returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			if (From != null)
			{
				sb.Append(':').Append(From).Append(' ');
			}
			sb.Append(Command);
			for (var i = 0; i < Parameters.Count; i++)
			{
				if (string.IsNullOrEmpty(Parameters[i]))
				{
					continue;
				}

				sb.Append(' ');
				if (i == Parameters.Count - 1 && !ExcludeSeparator)
				{
					sb.Append(':');
				}
				sb.Append(Parameters[i]);
			}

			return sb.ToString();
		}

		internal static IrcMessage Parse(string data)
		{
			var sb = new StringBuilder();
			var para = new List<string>();
			var size = data.Length > 512 ? 512 : data.Length;
			var messageData = data.ToCharArray(0, size);
			var pos = 0;
			string prefix = null;
			string command = null;

			if (messageData[0] == ':')
			{
				for (pos = 1; pos < messageData.Length; pos++)
				{
					if (messageData[pos] == ' ')
					{
						break;
					}

					sb.Append(messageData[pos]);
				}
				prefix = sb.ToString();
				sb.Length = 0;
				pos++;
			}

			for (; pos < messageData.Length; pos++)
			{
				if (messageData[pos] == ' ')
				{
					break;
				}
				sb.Append(messageData[pos]);
			}
			command = sb.ToString();
			sb.Length = 0;
			pos++;

			var trailing = false;
			while (pos < messageData.Length)
			{
				if (messageData[pos] == ':')
				{
					trailing = true;
					pos++;
				}

				for (; pos < messageData.Length; pos++)
				{
					if (messageData[pos] == ' ' &&
						!trailing)
					{
						break;
					}
					sb.Append(messageData[pos]);
				}
				para.Add(sb.ToString());
				sb.Length = 0;
				pos++;
			}

			return new IrcMessage(IrcPrefix.Parse(prefix), command, para.ToArray());
		}
	}
}