using System;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     This class represents a handler for a specific IRC code value. It can be used to "intercept" a response
	///     to a command and prevent other components from processing it.
	/// </summary>
	public sealed class IrcCodeHandler
	{
		internal IrcCode[] Codes { get; private set; }
		internal Func<IrcInfoEventArgs, bool> Handler { get; private set; }

		/// <summary>
		///     Primary constructor.
		/// </summary>
		/// <param name="handler">The function to handle the message. If the function returns true, the handler is removed.</param>
		/// <param name="codes">The IRC codes to handle.</param>
		public IrcCodeHandler(Func<IrcInfoEventArgs, bool> handler, params IrcCode[] codes)
		{
			Handler = handler;
			Codes = codes;
		}
	}
}