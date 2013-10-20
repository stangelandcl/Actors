using System;

namespace RemoteConsole
{
	public class LocalConsole : IConsole
	{
		public void Write (string o)
		{
			Console.Write(o);
		}

		public void WriteLine (string s)
		{
			Console.WriteLine(s);
		}

		public void SetCursorPosition (int x, int y)
		{
			Console.SetCursorPosition(x, y);
		}

		public ConsoleKeyInfo ReadKey (bool intercept)
		{
			return Console.ReadKey(intercept);
		}

		public void Clear ()
		{
			Console.Clear();
		}

		public string ReadLine ()
		{
			return Console.ReadLine();
		}

		public int CursorTop {
			get {
				return Console.CursorTop;
			}
		}

		public int BufferHeight {
			get {
				return Console.BufferHeight;
			}
		}

		public int WindowWidth {
			get {
				return Console.WindowWidth;
			}
		}
	}
}

