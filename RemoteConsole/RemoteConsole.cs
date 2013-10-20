using System;
using Actors;

namespace RemoteConsole
{
	public class RemoteConsole : IConsole
	{
		public RemoteConsole (RemoteActor s, int id)
		{
			this.a = s;
		}
		RemoteActor a;

		public void Write (string o)
		{
			a.Send("Write", o);
		}
		public void WriteLine (string s)
		{
			a.Send("WriteLine", s);
		}
		public void SetCursorPosition (int x, int y)
		{
			a.Send("SetCursorPosition", x, y);
		}
		public ConsoleKeyInfo ReadKey (bool intercept)
		{
			return a.SendReceive<ConsoleKeyInfo>("ReadKey", intercept);
		}
		public void Clear ()
		{
			a.Send("Clear");
		}
		public string ReadLine ()
		{
			return a.SendReceive<string>("ReadLine");
		}	
		public int CursorTop {
			get {
				return a.SendReceive<int>("CursorTop");
			}
		}
		public int BufferHeight {
			get {
				return a.SendReceive<int>("BufferHeight");
			}
		}
		public int WindowWidth {
			get {
				return a.SendReceive<int>("WindowWidth");
			}
		}
	}
}

