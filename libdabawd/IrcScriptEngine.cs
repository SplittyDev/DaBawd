using System;
using Neo.IronLua;

namespace libdabawd
{
	public class IrcScriptEngine : IDisposable
	{
		readonly Lua Engine;
		readonly LuaGlobal LuaEnvironment;

		public dynamic _ { get { return LuaEnvironment; } }

		public IrcScriptEngine () {
			Engine = new Lua (
				integerType: LuaIntegerType.Int32,
				floatType: LuaFloatType.Float
			);
			LuaEnvironment = Engine.CreateEnvironment<LuaGlobal> ();
		}

		public string DoSource (string source) {
			return LuaEnvironment.DoChunk (source, "__temp.lua").ToString ();
		}

		public void LoadSource (string source, string name) {
			LuaEnvironment.DoChunk (source, name);
		}

		public object Call (string function, params object[] args) {
			return LuaEnvironment.CallMember (function, args);
		}

		#region IDisposable implementation

		public void Dispose () {
			try {
				Engine.Dispose ();
			} catch (Exception e) {
				Console.WriteLine ("Error: {0}", e.Message);
			}
		}

		#endregion
	}
}

