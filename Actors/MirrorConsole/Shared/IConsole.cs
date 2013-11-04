using System;

namespace Cls.Actors
{
	public interface IConsole 
	{
        CursorPosition CursorPosition { get; set; }
        Screen Screen { get; set; }
        KeyPress[] Keys { get; set; }
        void Clear();
	}
    
    public struct CursorPosition
    {
        public CursorPosition(int x, int y, bool isVisible)
        {
            X = x; Y = y; IsVisible = isVisible;
        }
        public int X;
        public int Y;
        public bool IsVisible;
    }
	
	public interface IConsoleCallback{
		void CancelKeyPress(ConsoleCancelEventArgs args);
	}
}




