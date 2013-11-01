using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace Actors
{
    public class Win32Console : IConsole, IDisposable
    {
        public IntPtr StdOut;
        public IntPtr StdIn;
        public IntPtr StdErr;
        uint? oldMode;
        uint processId;

        public Win32Console()
        {            
            if ((StdOut = Win32.GetStdHandle(Win32.StdHandle.Output)) == Win32.INVALID_HANDLE_VALUE)
                throw new Win32Exception();
            if ((StdIn = Win32.GetStdHandle(Win32.StdHandle.Input)) == Win32.INVALID_HANDLE_VALUE)
                throw new Win32Exception();
            //if (!Win32.SetConsoleMode(stdin, 0))
            //    throw new Win32Exception();
            if ((StdErr = Win32.GetStdHandle(Win32.StdHandle.Error)) == IntPtr.Zero)
                throw new Win32Exception();
           
            uint mode;
            if (!Win32.GetConsoleMode(StdIn, out mode))
                throw new Win32Exception();
            oldMode = mode;
            if (!Win32.SetConsoleMode(StdIn, 0))
                throw new Win32Exception();
            processId = (uint)Process.GetCurrentProcess().Id;
        }

        public Win32Console(IntPtr stdin, IntPtr stdout, IntPtr stderr, uint processId)
        {
            this.processId = processId;
            this.StdIn = stdin;
            this.StdOut = stdout;
            this.StdErr = stderr;

            SetSize();
        }

        private void SetSize(int width = 120, int height= 35)
        {
            var size = new Win32.SMALL_RECT(0, 0, (short)(width - 1), (short)(height - 1));
            var old = GetConsoleSize();
            if (old.Item1.X > width || old.Item1.Y > height)
            {
                var minsize = new Win32.SMALL_RECT(0, 0, (short)Math.Min(old.Item1.X - 1, width - 1), (short)Math.Min(old.Item1.Y - 1, height - 1));
                if (!Win32.SetConsoleWindowInfo(StdOut, true, ref minsize))
                    throw new Win32Exception();
            }
            if (!Win32.SetConsoleScreenBufferSize(StdOut, new Win32.COORD((short)(size.Right + 1), (short)(size.Bottom + 1))))
                throw new Win32Exception();
            if (!Win32.SetConsoleWindowInfo(StdOut, true, ref size))
                throw new Win32Exception();
        }

        public void Clear()
        {
            var pos = new Win32.COORD(0, 0);
            uint written;
            Win32.CONSOLE_SCREEN_BUFFER_INFO screen_attr;
            if (Win32.GetConsoleScreenBufferInfo(StdOut, out screen_attr)) throw new Win32Exception();
            var size = screen_attr.dwSize.X * screen_attr.dwSize.Y;
            if (!Win32.FillConsoleOutputCharacter(StdOut, ' ',(uint) size, pos, out written)) throw new Win32Exception();
            if (!Win32.SetConsoleCursorPosition(StdOut, pos)) throw new Win32Exception();
        }

        public Screen Screen
        {
            get
            {
                var size = GetConsoleSize();
                var buffer = new Win32.CHAR_INFO[size.Item1.X * size.Item1.Y];
                var read = new Win32.SMALL_RECT(0, 0, (short)(size.Item1.X - 1), (short)(size.Item1.Y - 1));
                if (!Win32.ReadConsoleOutput(StdOut, buffer, size.Item1, new Win32.COORD(0, 0), ref read))
                    throw new Win32Exception();
                var chars = new Screen(size.Item1.X, size.Item1.Y);
                for (int i = 0; i < size.Item1.Y; i++)
                    for (int j = 0; j < size.Item1.X; j++)
                    {
                        chars[j, i] = buffer[i * chars.Width + j].UnicodeChar;
                        chars.SetAttribute(j, i, buffer[i * chars.Width + j].Attributes);
                    }
                return chars;
            }
            set
            {
                var size = GetConsoleSize();
                if (size.Item1.X != value.Width || size.Item1.Y != value.Height)
                    SetSize(value.Width, value.Height);

                var buffer = new Win32.CHAR_INFO[value.Width * value.Height];
                for (int i = 0; i < value.Height; i++)
                    for (int j = 0; j < value.Width; j++)
                        buffer[i * value.Width + j] = new Win32.CHAR_INFO
                        {
                            UnicodeChar = value[j, i],
                            Attributes = value.GetAttribute(j, i),
                        };

                var read = new Win32.SMALL_RECT(0, 0, (short)(value.Width - 1), (short)(value.Height - 1));
                if (!Win32.WriteConsoleOutput(StdOut, buffer, new Win32.COORD((short)value.Width, (short)value.Height),
                    new Win32.COORD(0, 0), ref read))
                    throw new Win32Exception();
            }
        }

        public KeyPress[] Keys
        {
            get
            {
                var keys = new Win32.INPUT_RECORD[256];
                uint read;
                if (!Win32.ReadConsoleInput(StdIn, keys, (uint)keys.Length, out read))
                    throw new Win32Exception();
                return keys.Take((int)read).Where(n => n.EventType == Win32.InputEventTypes.KEY_EVENT)
                    .Select(n => n.KeyEvent)
                    .Select(n => new KeyPress
                    {
                        Special = n.dwControlKeyState,
                        Code = n.wVirtualKeyCode,
                        Character = n.UnicodeChar,
                        Pressed = n.bKeyDown
                    })
                    .ToArray();
            }
            set
            {
                if(value.Any(n=>((n.Special & ControlKeyState.LEFT_CTRL_PRESSED) != 0 || (n.Special & ControlKeyState.RIGHT_CTRL_PRESSED ) != 0)
                    && n.Code == VirtualKeys.C))
                    Win32.GenerateConsoleCtrlEvent(Win32.CtrlBreakEvent, processId);


                var buffer = value
                    .Select(n => (KeyPress)n)
                    .Select(n => new Win32.INPUT_RECORD
                {
                    EventType = Win32.InputEventTypes.KEY_EVENT,
                    KeyEvent = new Win32.KEY_EVENT_RECORD
                    {
                        bKeyDown = n.Pressed,
                        dwControlKeyState = n.Special,
                        UnicodeChar = n.Character,
                        wVirtualKeyCode = n.Code,
                    }
                }).ToArray();
                uint written;
                if (!Win32.WriteConsoleInput(StdIn, buffer, (uint)buffer.Length, out written))
                    throw new Win32Exception();
            }
        }

        private Tuple<Win32.COORD, Win32.COORD> GetConsoleSize()
        {
            Win32.CONSOLE_SCREEN_BUFFER_INFO screen;
            if (!Win32.GetConsoleScreenBufferInfo(StdOut, out screen))
                throw new Win32Exception();
            var size = Tuple.Create(new Win32.COORD
            {
                X = (short)(screen.srWindow.Right - screen.srWindow.Left + 1),
                Y = (short)(screen.srWindow.Bottom - screen.srWindow.Top + 1)
            },
                new Win32.COORD { X = screen.dwCursorPosition.X, Y = screen.dwCursorPosition.Y });
            return size;
        }

        public CursorPosition CursorPosition
        {
            get
            {
                var x = GetConsoleSize();
                Win32.CONSOLE_CURSOR_INFO i;
                if(!Win32.GetConsoleCursorInfo(StdOut, out i))
                    throw new Win32Exception();

                return new CursorPosition(x.Item2.X, x.Item2.Y, i.bVisible);
            }
            set
            {
                if(!Win32.SetConsoleCursorPosition(StdOut, new Win32.COORD((short)value.X, (short)value.Y)))
                    throw new Win32Exception();
                Win32.CONSOLE_CURSOR_INFO i = new Win32.CONSOLE_CURSOR_INFO();
                i.bVisible = value.IsVisible;
                i.dwSize = 100;
                if (!Win32.SetConsoleCursorInfo(StdOut, ref i))
                    throw new Win32Exception();

            }
        }

        public void Dispose()
        {
            if (oldMode.HasValue && !Win32.SetConsoleMode(StdIn, oldMode.Value))
                    throw new NotImplementedException();
            Win32.FreeConsole();
        }
    }
}
