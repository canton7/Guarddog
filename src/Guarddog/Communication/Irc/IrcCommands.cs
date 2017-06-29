namespace IrcSays.Communication.Irc
{
	public static class IrcCommands
	{
		public const string Quote = "QUOTE";
		public const string Quit = "QUIT";
		public const string Nick = "NICK";
		public const string Notice = "NOTICE";
		public const string Join = "JOIN";
		public const string J = "J";
		public const string Part = "PART";
		public const string Ping = "PING";
		public const string Pong = "PONG";
		public const string PrivateMsg = "PRIVMSG";
		public const string Leave = "LEAVE";
		public const string Topic = "TOPIC";
		public const string Invite = "INVITE";
		public const string Kick = "KICK";
		public const string Motd = "MOTD";
		public const string Who = "WHO";
		public const string Whois = "WHOIS";
		public const string Whowas = "WHOWAS";
		public const string Away = "AWAY";
		public const string Userhost = "USERHOST";
		public const string Mode = "MODE";
		public const string Server = "SERVER";
		public const string Me = "ME";
		public const string Action = "ACTION";
		public const string Setup = "SETUP";
		public const string Clear = "CLEAR";
		public const string Msg = "MSG";
		public const string List = "LIST";
		public const string Op = "OP";
		public const string Deop = "DEOP";
		public const string Voice = "VOICE";
		public const string Devoice = "DEVOICE";
		public const string Help = "HELP";
		public const string Ctcp = "CTCP";
		public const string Query = "QUERY";
		public const string Ban = "BAN";
		public const string Unban = "UNBAN";
		public const string Ignore = "IGNORE";
		public const string Unignore = "UNIGNORE";
		public const string Dcc = "DCC";
		public const string Chat = "CHAT";
		public const string Send = "SEND";
		public const string Xmit = "XMIT";

		//unofficial commands
		public const string PurgeMessages = "PURGE";
		public const string PurgeAllMessages = "PURGEALL";
        public const string ZncMessages = "ZNC";
	}
}