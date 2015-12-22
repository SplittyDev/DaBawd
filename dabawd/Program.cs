using System;
using System.IO;
using Codeaddicts.libArgument;
using libdabawd;
using System.Linq;
using System.Collections.Generic;

namespace dabawd
{
	class MainClass
	{
		public static void Main (string[] args) {
			new MainClass (ArgumentParser.Parse<IrcOptions> (args)).Main ();
		}

		readonly public IrcApi API;
		readonly string ScriptDir;
		readonly IrcOptions Options;
		readonly IrcBot Bot;
		readonly Queue<ScriptAction> ScriptActions;

		public MainClass (IrcOptions options) {
			Options = options;
			Bot = new IrcBot ();
			API = new IrcApi ();
			ScriptActions = new Queue<ScriptAction> ();
			ScriptDir = "../../scripts";

			#region API
			API.Register ("loadscript", (args => {
				ScriptActions.Enqueue (new ScriptAction ((string) args [0], ScriptActionEnum.Load));
				return null;
			}));
			API.Register ("loadscripts", (args => {
				ScriptActions.Enqueue (new ScriptAction (null, ScriptActionEnum.LoadAll));
				return null;
			}));
			API.Register ("reloadscript", (args => {
				ScriptActions.Enqueue (new ScriptAction ((string) args [0], ScriptActionEnum.Reload));
				return null;
			}));
			API.Register ("reloadscripts", (args => {
				ScriptActions.Enqueue (new ScriptAction (null, ScriptActionEnum.ReloadAll));
				return null;
			}));
			API.Register ("unloadscript", (args => {
				ScriptActions.Enqueue (new ScriptAction ((string) args [0], ScriptActionEnum.Unload));
				return null;
			}));
			API.Register ("sendmsg", (args => {
				Bot.Client.Message ((string) args [1], (string) args [0]);
				return null;
			}));
			API.Register ("listscripts", (args => {
				var files = Directory.GetFiles (ScriptDir, "*.lua", SearchOption.AllDirectories);
				return string.Join (" ", files.Select (f => Path.GetFileNameWithoutExtension (f)).ToArray ());
			}));
			#endregion
		}

		public void Main () {
			Bot.Client.Connected += Bot_Client_Connected;
			Bot.Client.Disconnected += Bot_Client_Disconnected;
			Bot.Client.ChannelJoined += Bot_Client_ChannelJoined;
			Bot.Client.ChannelMessage += Bot_Client_ChannelMessage;
			Connect ();
		}

		void Connect () {
			LoadScripts ();
			Bot.Connect (
				server: Options.GetHost (),
				port: Options.GetPort (),
				ssl: Options.GetSSL ()
			).Login (Options.GetNick ()).Work ();
		}

		void LoadScripts () {
			var files = Directory.GetFiles (ScriptDir, "*.lua", SearchOption.AllDirectories);
			foreach (var path in files)
				Bot.LoadScript (path);
		}

		void LoadScript (string name) {
			Bot.LoadScript (Path.Combine (ScriptDir, string.Format ("{0}.lua", name)));
		}

		void UnloadScripts () {
			for (var i = 0; i < Bot.ScriptEngines.Count; i++)
				Bot.ScriptEngines.ElementAt (i).Value.Dispose ();
			Bot.ScriptEngines.Clear ();
		}

		void UnloadScript (string name) {
			Bot.UnloadScript (name);
		}

		void ReloadScripts () {
			UnloadScripts ();
			LoadScripts ();
		}

		void ReloadScript (string name) {
			UnloadScript (name);
			LoadScript (name);
		}

		static void Bot_Client_Connected (string hostname, int port) {
			Log ("Connected to {0}:{1}", hostname, port);
		}

		static void Bot_Client_Disconnected (object sender, EventArgs e) {
			Log ("Disconnected");
		}

		static void Bot_Client_ChannelJoined (string channel) {
			Log ("Joined {0}", channel);
		}

		void Bot_Client_ChannelMessage (string channel, string message, string sender) {
			Console.WriteLine ("[{0}] {1}: {2}", channel, sender, message);
			foreach (var engine in Bot.ScriptEngines)
				engine.Value.Call ("onmessage", API, channel, message, sender);
			for (var i = 0; i < ScriptActions.Count; i++) {
				var action = ScriptActions.Dequeue ();
				switch (action.Action) {
				case ScriptActionEnum.Load:
					LoadScript (action.Name);
					break;
				case ScriptActionEnum.Reload:
					ReloadScript (action.Name);
					break;
				case ScriptActionEnum.Unload:
					UnloadScript (action.Name);
					break;
				case ScriptActionEnum.LoadAll:
					LoadScripts ();
					break;
				case ScriptActionEnum.ReloadAll:
					ReloadScripts ();
					break;
				}
			}
		}

		static void Log (string format, params object[] args) {
			Console.WriteLine (format, args);
		}
	}
}
