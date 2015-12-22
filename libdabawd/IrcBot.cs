using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace libdabawd
{
	public class IrcBot
	{
		public readonly IrcClient Client;
		public readonly Dictionary<string, IrcScriptEngine> ScriptEngines;

		public dynamic this [string script] {
			get { return ScriptEngines [script]._; }
		}

		public IrcBot () {
			Client = new IrcClient ();
			ScriptEngines = new Dictionary<string, IrcScriptEngine> ();
		}

		public IrcBot Connect (string server, int port, bool ssl = false) {
			Client.Connect (server, port, ssl);
			return this;
		}

		public IrcBot ConnectSsl (string server, int port = 6697) {
			Connect (server, port, ssl: true);
			return this;
		}

		public IrcBot Login (string nick, string username = "", string realname = "") {
			Client.LogIn (
				username: string.IsNullOrEmpty (username) ? nick : username,
				realname: string.IsNullOrEmpty (realname) ? nick : realname,
				nickname: nick
			);
			return this;
		}

		public IrcBot LoadScript (string path) {
			using (var file = File.OpenRead (path))
			using (var reader = new StreamReader (file)) {
				Console.WriteLine ("Loading script: {0}", file.Name);
				try {
					Console.WriteLine ("Creating lua engine");
					var engine = new IrcScriptEngine ();
					Console.WriteLine ("Loading script source");
					var name = Path.GetFileNameWithoutExtension (file.Name);
					engine.LoadSource (reader.ReadToEnd (), name);
					Console.WriteLine ("Registering script");
					ScriptEngines.Add (name, engine);
				} catch (Exception e) {
					Console.WriteLine ("Error: {0}", e.Message);
				}
			}
			return this;
		}

		public IrcBot UnloadScript (string name) {
			var engine = ScriptEngines.FirstOrDefault (kvp => kvp.Key == name);
			if (engine.Value == null)
				return this;
			try {
				Console.WriteLine ("Unloading script: {0}", name);
				engine.Value.Dispose ();
				ScriptEngines.Remove (engine.Key);
			} catch (Exception e) {
				Console.WriteLine ("Error: {0}", e.Message);
			}
			return this;
		}

		public void Work () {
			Thread.Sleep (Timeout.Infinite);
		}
	}
}

