using System;

namespace dabawd
{
	public class ScriptAction
	{
		readonly public string Name;
		readonly public ScriptActionEnum Action;

		public ScriptAction (string name, ScriptActionEnum action) {
			Name = name;
			Action = action;
		}
	}

	[Flags]
	public enum ScriptActionEnum {
		None = 1 << 0,
		Load = 1 << 1,
		Unload = 1 << 2,
		Reload = 1 << 3,
		LoadAll = 1 << 4,
		ReloadAll = 1 << 5,
	};
}

