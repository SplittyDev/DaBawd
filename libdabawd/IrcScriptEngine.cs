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
			try {
				Engine = new Lua (
					integerType: LuaIntegerType.Int32,
					floatType: LuaFloatType.Float
				);
				LuaEnvironment = Engine.CreateEnvironment<LuaGlobal> ();
			} catch (Exception e) {
				Console.WriteLine ("Error: {0}", e.Message);
			}
		}

		public void LoadSource (string source, string name) {
			try {
				LuaEnvironment.DoChunk (source, name);
			} catch (Exception e) {
				Console.WriteLine ("Error: {0}", e.Message);
			}
		}

		public object Call (string function, params object[] args) {
			LuaResult result;
			try {
				result = LuaEnvironment.CallMember (function, args);
			} catch (Exception e) {
				Console.WriteLine ("Error: {0}", e.Message);
				result = LuaResult.Empty;
			}
			return result;
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

