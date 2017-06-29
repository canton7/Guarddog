using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Represents a single IRC user mode change and provides utility functions for converting collections of modes to a
	///     textual
	///     mode specification and vice-versa.
	/// </summary>
	public struct IrcUserMode
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
		///     Initialize a user mode.
		/// </summary>
		/// <param name="set">Indicates whether the mode is set (true) or unset (false).</param>
		/// <param name="mode">The character that specifies the mode.</param>
		public IrcUserMode(bool set, char mode)
		{
			Set = set;
			Mode = mode;
		}

		/// <summary>
		///     Parse a given mode spec and returns a collection describing the change.
		/// </summary>
		/// <param name="modeSpec">A user mode specification (for example: "+im" or "+i-m").</param>
		/// <returns>Returns the collection of modes describing the mode change.</returns>
		public static ICollection<IrcUserMode> ParseModes(string modeSpec)
		{
			return ParseModes(modeSpec.Split(' '));
		}

		/// <summary>
		///     Converts a collection of modes into a mode specification string.
		/// </summary>
		/// <param name="modes">The collection of modes to convert.</param>
		/// <returns>Returns a mode specification string describing the modes.</returns>
		public static string RenderModes(IEnumerable<IrcUserMode> modes)
		{
			var output = new StringBuilder();
			foreach (var mode in modes.Where((m) => m.Set))
			{
				if (output.Length == 0)
				{
					output.Append('+');
				}
				output.Append(mode.Mode);
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
			}
			return output.ToString();
		}

		internal static ICollection<IrcUserMode> ParseModes(IEnumerable<string> parts)
		{
			var modeList = new List<IrcUserMode>();

			var set = false;
			foreach (var str in parts)
			{
				var s = str.Trim();
				if (!s.StartsWith("+") &&
					!s.StartsWith("-"))
				{
					continue;
				}
				foreach (var c in s)
				{
					if (c == '+')
					{
						set = true;
					}
					else if (c == '-')
					{
						set = false;
					}
					else
					{
						modeList.Add(new IrcUserMode(set, c));
					}
				}
			}
			return modeList;
		}
	}
}