using System;

namespace IrcSays.Communication.Network
{
	public class SocksException : Exception
	{
		public SocksException(string message)
			: base(message)
		{
		}
	}
}