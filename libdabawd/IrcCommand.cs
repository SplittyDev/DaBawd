using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libdabawd
{
	/// <summary>
	/// Command.
	/// </summary>
	public class IrcCommand {

		/// <summary>
		/// The name.
		/// </summary>
		public string Name;

		/// <summary>
		/// The arguments.
		/// </summary>
		public string[] Args;

		public IrcCommand (string text) {
			Parse (text);
		}

		/// <summary>
		/// Parse the specified text.
		/// </summary>
		/// <param name="text">Text.</param>
		public void Parse (string text) {
			text = text.Trim ();
			if (text.Contains (" ")) {
				Name = new string (text.TakeWhile (c => c != ' ').ToArray ());
				var argstr = new string (text.Skip (text.IndexOf (' ') + 1).ToArray ());
				var accum = new StringBuilder ();
				var i = 0;
				var awaitquote = false;
				var lst = new List<string> ();
				while (i < argstr.Length) {
					if (argstr [i] == '"' && awaitquote) {
						i++;
						awaitquote = false;
					} else if (argstr [i] == '"' && !awaitquote) {
						i++;
						awaitquote = true;
					}
					if (i == argstr.Length)
						break;
					if (argstr [i] == ' ' && !awaitquote) {
						lst.Add (accum.ToString ());
						accum.Clear ();
					} else {
						accum.Append (argstr [i]);
					}
					++i;
				}
				lst.Add (accum.ToString ());
				Args = lst.ToArray ();
			} else {
				Name = text;
				Args = new string[0];
			}
		}

		/// <summary>
		/// To arguments string.
		/// </summary>
		/// <returns>The arguments string.</returns>
		public string ToArgsString () {
			return string.Join (" ", Args);
		}

		public static explicit operator string (IrcCommand com) {
			var accum = new StringBuilder ();
			accum.Append (com.Name);
			foreach (var arg in com.Args)
				accum.AppendFormat ( "\"{0}\"", arg);
			return accum.ToString ().Trim ();
		}

		public static explicit operator IrcCommand (string str) {
			return new IrcCommand (str);
		}

		public override string ToString () {
			return (string)this;
		}
	}
}
