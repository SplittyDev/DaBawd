using System;
using System.Collections.Generic;

namespace libdabawd
{
	public class IrcApi
	{
		public Dictionary<string, IrcApiActionDelegate> Actions;

		public IrcApi () {
			Actions = new Dictionary<string, IrcApiActionDelegate> ();
		}

		public void Register (string name, IrcApiActionDelegate action) {
			if (!Actions.ContainsKey (name) || Actions.ContainsValue (action))
				Actions.Add (name, action);
		}

		public object call (string name) {
			return Actions [name] ();
		}

		public object call (string name, params object[] args) {
			return Actions [name] (args);
		}
	}
}

