using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RemoteConsole
{
    public class WinFormsConsoleClient : Form, IConsole
    {
        public WinFormsConsoleClient()
        {
            Width = 800;
            Height = 600;
            box = new TextBox();
            Controls.Add(box);
            box.Multiline = true;
            box.Dock = DockStyle.Fill;
            box.KeyDown += new KeyEventHandler(box_KeyDown);
            box.KeyUp += new KeyEventHandler(box_KeyUp);
        }

        void box_KeyUp(object sender, KeyEventArgs e)
        {          
            var key = new KeyPress
            {
                Character = GetChar(e),
                Code = (VirtualKeys)e.KeyCode,
                Pressed = false,
            };
            if (e.Alt)
                key.Special |= ControlKeyState.LEFT_ALT_PRESSED;
            if (e.Control)
                key.Special |= ControlKeyState.LEFT_CTRL_PRESSED;
            if (e.Shift)
                key.Special |= ControlKeyState.SHIFT_PRESSED;
            lock (keys)
                keys.Add(key);
        }

        private static char GetChar(KeyEventArgs e)
        {
            var c = (char)Win32.MapVirtualKey((uint)e.KeyCode, Win32.MAPVK_VK_TO_CHAR);
            if (c != 0 && !e.Shift && c >= 'A' && c <= 'z')
                c += (char)('a' - 'A');
            return c;
        }        

        void box_KeyDown(object sender, KeyEventArgs e)
        {          
            var key = new KeyPress
            {
                Character = GetChar(e),
                Code = (VirtualKeys)e.KeyCode,
                Pressed = true,                
            };
            if (e.Alt)
                key.Special |= ControlKeyState.LEFT_ALT_PRESSED;
            if (e.Control)
                key.Special |= ControlKeyState.LEFT_CTRL_PRESSED;
            if (e.Shift)
                key.Special |= ControlKeyState.SHIFT_PRESSED;
            lock (keys)
                keys.Add(key);
        }

       
        List<KeyPress> keys = new List<KeyPress>();
        TextBox box;

        public Screen Screen
        {
            set
            {
                if (InvokeRequired || box.InvokeRequired)
                {
                    Invoke(new Action(() => Screen = value));
                    return;
                }
                var w = value.Width * 8;
                var h = value.Height * 16;
                if (Width != w || Height != h)
                {
                    Width = w;
                    Height = h;
                }
                var lines = new string[value.Height];
                for (int y = 0; y < value.Height; y++)
                {
                    var sb = new StringBuilder();
                    for (int x = 0; x < value.Width; x++)
                        sb.Append(value[x, y]);
                    lines[y] = sb.ToString();
                }
                box.Lines = lines;
            }
            get { throw new NotImplementedException(); }
        }

        public KeyPress[] Keys
        {
            get
            {
                lock (keys)
                {
                    var x = keys.ToArray();
                    keys.Clear();
                    return x;
                }
            }
            set { throw new NotImplementedException(); }
        }

        public void Clear()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(Clear));
                return;
            }
            box.Clear();
        }

        public CursorPosition CursorPosition
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
