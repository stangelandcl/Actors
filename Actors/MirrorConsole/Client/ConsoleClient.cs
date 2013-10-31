using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace RemoteConsole
{
	public class ConsoleClient : IConsole
	{
        public void Clear() { Console.Clear(); }

       
        public Screen Screen
        {
            set
            {
                if (!IsSameSize(value))
                {
                    Console.SetBufferSize(value.Width, value.Height);
                    Console.SetWindowSize(value.Width, value.Height);
                }
                var sb = new StringBuilder();
                for (int y = 0; y < value.Height; y++)
                    for (int x = 0; x < value.Width; x++)
                        sb.Append(value[x, y]);
                Console.SetCursorPosition(0, 0);
                Console.Write(sb.ToString());
            }
            get { throw new NotImplementedException(); }
        }

        private static bool IsSameSize(RemoteConsole.Screen value)
        {
            return Console.BufferWidth == value.Width &&
                Console.BufferHeight == value.Height &&
                Console.WindowWidth == value.Width &&
                Console.WindowHeight == value.Height;
        }

        public KeyPress[] Keys
        {
            get
            {
                var keys = new List<KeyPress>();
                while (Console.KeyAvailable)
                    keys.Add(Console.ReadKey());
                return keys.ToArray();
            }
            set { throw new NotImplementedException(); }
        }

        public CursorPosition CursorPosition
        {
            get
            {
                return new CursorPosition(Console.CursorLeft, Console.CursorTop, Console.CursorVisible);
            }
            set
            {
                Console.SetCursorPosition(value.X, value.Y);
                Console.CursorVisible = value.IsVisible;
            }
        }
    }
}

