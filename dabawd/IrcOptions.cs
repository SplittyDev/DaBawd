using System;
using System.Collections.Generic;
using Codeaddicts.libArgument.Attributes;
using System.Linq;

namespace dabawd
{
	public class IrcOptions
	{
		const string DEFAULT_NICK = "dabawdbot";
		const int DEFAULT_PORT = 6667;
		const int DEFAULT_SSL_PORT = 6697;

		[Argument ("-h", "--host", "/h", "/host")]
		public string Host;

		[Argument ("-n", "--nick", "/n", "/nick")]
		public string Nick;

		[Argument ("-s", "--server", "/s", "/server")]
		public string Server;

		[Argument ("-p", "--port", "/p", "/port")]
		public int Port;

		[Switch ("--ssl", "/ssl")]
		public bool UseSSL;

		public string GetHost () {
			if (!string.IsNullOrEmpty (Host))
				return Host;
			else if (!string.IsNullOrEmpty (Server)) {
				return Server.Any (c => c == ':')
					? Server.Any (c => c == '@')
						? Server.Split ('@').Skip (1).First ().Split (':').First ()
						: Server.Split (':').First ()
					: Server;
			}
			return string.Empty;
		}

		public string GetNick () {
			if (!string.IsNullOrEmpty (Nick))
				return Nick;
			else if (!string.IsNullOrEmpty (Server))
				return Server.Any (c => c == '@')
					? Server.Split ('@').First ()
					: DEFAULT_NICK;
			return DEFAULT_NICK;
		}

		public int GetPort () {
			if (Port != 0)
				return Port;
			else if (!string.IsNullOrEmpty (Server))
				return Server.Any (c => c == ':')
					? int.Parse (Server.Split (':').Skip (1).First ())
					: UseSSL ? DEFAULT_SSL_PORT : DEFAULT_PORT;
			return UseSSL ? DEFAULT_SSL_PORT : DEFAULT_PORT;
		}

		public bool GetSSL () {
			return UseSSL || GetPort () == DEFAULT_SSL_PORT;
		}
	}
}

