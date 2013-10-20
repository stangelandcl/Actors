using System;

namespace RemoteConsole
{
	public interface IConsole
	{
		void Write(string o);
		void WriteLine(string s);	
		void SetCursorPosition(int x, int y);
		int CursorTop { get;}
		int BufferHeight { get;}
		ConsoleKeyInfo ReadKey(bool intercept);         
		int WindowWidth { get;}
		void Clear();
		string ReadLine();
	}
	
	public interface IConsoleCallback{
		void CancelKeyPress(ConsoleCancelEventArgs args);
	}
}




