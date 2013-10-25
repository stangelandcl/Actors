using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RemoteConsole
{
    public struct KeyPress
    {
        public ControlKeyState Special;
        public VirtualKeys Code;
        public bool Pressed;
        public char Character;

        public static implicit operator ConsoleKeyInfo(KeyPress k)
        {
            return new ConsoleKeyInfo(k.Character, (ConsoleKey)k.Code, 
                (k.Special & ControlKeyState.SHIFT_PRESSED) != 0,
                (k.Special & ControlKeyState.LEFT_ALT_PRESSED) != 0 ||
                (k.Special & ControlKeyState.RIGHT_ALT_PRESSED) != 0,
                (k.Special & ControlKeyState.LEFT_CTRL_PRESSED) != 0 ||
                (k.Special & ControlKeyState.RIGHT_CTRL_PRESSED) != 0);              
        }

        public static implicit operator KeyPress(ConsoleKeyInfo k)
        {         
            var x = new KeyPress
            {
                Character = k.KeyChar,
                Code = (VirtualKeys)k.Key,
                Pressed = true,
            };
            if ((k.Modifiers & ConsoleModifiers.Alt) != 0)
                x.Special |= ControlKeyState.LEFT_ALT_PRESSED;
            if ((k.Modifiers & ConsoleModifiers.Control) != 0)
                x.Special |= ControlKeyState.LEFT_CTRL_PRESSED;
            if ((k.Modifiers & ConsoleModifiers.Shift) != 0)
                x.Special |= ControlKeyState.SHIFT_PRESSED;
            return x;
        }        
    }
}
