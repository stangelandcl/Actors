using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace RemoteConsole
{
    public class Win32HiddenConsole  : IDisposable
    {
        public Win32HiddenConsole(string exe, params string[] args)
        {            
            var a = string.Join(" ", args);
            var commandLine = "\"" + exe + "\" " + a;            
            Start(commandLine);
        }
                
        Win32.PROCESS_INFORMATION process;

        /// <summary>
        /// Console window of process
        /// </summary>
        public Win32Console Console { get; private set; }

        /// <summary>
        /// is process still running
        /// </summary>
        public bool IsAlive
        {
            get
            {
                uint exit;
                if (!Win32.GetExitCodeProcess(process.hProcess, out exit))
                    throw new Win32Exception();
                return exit == Win32.StillAlive;
            }
        }

        public void Detach()
        {
            if (!Win32.FreeConsole())
                throw new Win32Exception();
        }

        public void Attach()
        {
            if (!Win32.AttachConsole(process.dwProcessId))
                throw new Win32Exception();
        }
       

        void Start(string commandline)
        {
            var info = new Win32.STARTUPINFO();
            info.dwFlags = Win32.STARTF.STARTF_USESHOWWINDOW;
            info.wShowWindow = Win32.SHOWWINDOW.SW_HIDE;
            if (!Win32.CreateProcess(null, commandline, IntPtr.Zero, IntPtr.Zero, false,
                Win32.CreateProcessFlags.CREATE_NEW_PROCESS_GROUP, IntPtr.Zero, null, ref info, out process))
                throw new Win32Exception();
            Thread.Sleep(2 * 1000); // init console         
            Win32.FreeConsole(); // this will fail if no console is attached. that is okay.
            if (!Win32.AttachConsole(process.dwProcessId))
                throw new Win32Exception();
            IntPtr stdin, stdout, stderr;
            if ((stdout = Win32.GetStdHandle(Win32.StdHandle.Output)) == Win32.INVALID_HANDLE_VALUE)
                throw new Win32Exception();
            if ((stdin = Win32.GetStdHandle(Win32.StdHandle.Input)) == Win32.INVALID_HANDLE_VALUE)
                throw new Win32Exception();           
            if ((stderr = Win32.GetStdHandle(Win32.StdHandle.Error)) == IntPtr.Zero)
                throw new Win32Exception();
            if (!Win32.SetConsoleMode(stdin, 0))
                throw new Win32Exception();

            Console = new Win32Console(stdin, stdout, stderr, process.dwProcessId);
        }

        /// <summary>
        /// Kill console process
        /// </summary>
        public void Kill()
        {
            if (!Process.GetProcesses().Where(n => n.Id == (int)process.dwProcessId).Any())
                return;
            var proc = Process.GetProcessById((int)process.dwProcessId);
            if (proc != null)
                proc.Kill();
        }

        public void Dispose()
        {
            Kill();
            Console.Dispose();
        }
    }
 
}
