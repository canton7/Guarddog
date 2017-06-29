using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Represents a single IRC channel mode change and provides utility functions for converting collections of modes to a
	///     textual
	///     mode specification and vice-versa.
	/// </summary>
	public struct IrcChannelMode
	{
		/// <summary>
		///     Gets a value indicating whether the mode was set (true) or unset (false).
		/// </summary>
		public readonly bool Set;

		/// <summary>
		///     Gets the character that specifies the mode.
		/// </summary>
		public readonly char Mode;

		/// <summary>
		///     Gets the optional parameter attached to the mode.
		/// </summary>
		public readonly string Parameter;

		/// <summary>
		///     Initialize a channel mode.
		/// </summary>
		/// <param name="set">Indicates whether the mode is set (true) or unset (false).</param>
		/// <param name="mode">The character that specifies the mode.</param>
		/// <param name="parameter">The optional mode parameter.</param>
		public IrcChannelMode(bool set, char mode, string parameter = null)
		{
			Set = set;
			Mode = mode;
			Parameter = parameter;
		}

		/// <summary>
		///     Parse a mode spec into a collection of channel modes.
		/// </summary>
		/// <param name="modes">The mode spec to parse (for example, "-i+l 50" or "+snt").</param>
		/// <returns>Returns the collection of modes representing the change.</returns>
		public static ICollection<IrcChannelMode> ParseModes(string modeSpec)
		{
			return ParseModes(modeSpec.Split(' '));
		}

		/// <summary>
		///     Convert a collection of modes into a set of strings that can be sent to a server.
		/// </summary>
		/// <param name="modes">The collection of modes to convert.</param>
		/// <returns>
		///     Returns an array of strings that make up a mode change. The first string is the mode spec with mode
		///     characters only, and all subsequent strings are mode arguments. For example, the first string may contain "-i+l"
		///     and the second string may contain "100".
		/// </returns>
		public static string[] RenderModes(IEnumerable<IrcChannelMode> modes)
		{
			var output = new StringBuilder();
			var paramsOutput = new StringBuilder();
			var args = new List<string>();
			foreach (var mode in modes.Where((m) => m.Set))
			{
				if (output.Length == 0)
				{
					output.Append('+');
				}
				output.Append(mode.Mode);
				if (!string.IsNullOrEmpty(mode.Parameter))
				{
					args.Add(mode.Parameter);
				}
			}
			var hasMinus = false;
			foreach (var mode in modes.Where((m) => !m.Set))
			{
				if (!hasMinus)
				{
					output.Append('-');
					hasMinus = true;
				}
				output.Append(mode.Mode);
				if (!string.IsNullOrEmpty(mode.Parameter))
				{
					args.Add(mode.Parameter);
				}
			}
			var result = new string[args.Count + 1];
			result[0] = output.ToString();
			args.CopyTo(result, 1);
			return result;
		}

		internal static ICollection<IrcChannelMode> ParseModes(IEnumerable<string> parts)
		{
			var modeList = new List<IrcChannelMode>();
			var paramSetList = new List<bool>();
			var paramModeList = new List<char>();
			var paramList = new List<string>();

			var set = false;
			foreach (var part in parts)
			{
				var partTrimmed = part.Trim();

				if (!partTrimmed.StartsWith("+") &&
					!partTrimmed.StartsWith("-"))
				{
					paramList.Add(partTrimmed);
				}
				else
				{
					foreach (var chr in partTrimmed)
					{
						if (chr == '-')
						{
							set = false;
						}
						else if (chr == '+')
						{
							set = true;
						}
						else
						{
							switch (chr)
							{
								case 'O':
								case 'o':
								case 'v':
								case 'h':
								case 'k':
								case 'l':
								case 'b':
								case 'e':
								case 'I':
								case 'f':
								case 'j':
								case 'q':
									paramSetList.Add(set);
									paramModeList.Add(chr);
									break;
								default:
									modeList.Add(new IrcChannelMode(set, chr, null));
									break;
							}
						}
					}
				}
			}

			for (var i = 0; i < paramModeList.Count; i++)
			{
				modeList.Add(new IrcChannelMode(paramSetList[i], paramModeList[i], i < paramList.Count ? paramList[i] : null));
			}

			return modeList;
		}
	}
}