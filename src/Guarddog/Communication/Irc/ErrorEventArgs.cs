using System;

namespace IrcSays.Communication.Irc
{
	/// <summary>
	///     Provides event arguments describing an IRC error.
	/// </summary>
	public sealed class ErrorEventArgs : EventArgs
	{
		/// <summary>
		///     Gets the original exception that resulted from the error.
		/// </summary>
		public Exception Exception { get; private set; }

		public ErrorEventArgs(Exception ex)
		{
			Exception = ex;
		}
	}
}