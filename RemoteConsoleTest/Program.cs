using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteConsole;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteConsoleTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var cmd = new Win32HiddenConsole("cmd.exe");
            Console.Clear();
            var console = cmd.Console;
            var real = new ConsoleClient();
            //var real = new WinFormsConsoleClient();
            Task.Factory.StartNew(() =>
            {
                while (cmd.IsAlive)
                {
                    real.Screen = console.Screen;
                    console.Keys = real.Keys;
                    Thread.Sleep(100);
                }
            });

            Thread.Yield();
           //  Application.Run(real);
        }
    }
}
