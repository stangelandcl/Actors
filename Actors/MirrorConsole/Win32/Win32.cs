using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Cls.Actors
{
    class Win32
    {
        public enum InputEventTypes : short
        {
            KEY_EVENT = 1,
            MOUSE_EVENT = 2,
            WINDOW_BUFFER_SIZE_EVENT = 4,
            MENU_EVENT = 8,
            FOCUS_EVENT = 16
        }

        public enum SHOWWINDOW : short
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11,
        }

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint ucode, uint mapType);
        public const uint MAPVK_VK_TO_CHAR = 2;
        [DllImport("kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, out  uint lpMode);
            

        [DllImport("kernel32.dll")]
        public static extern bool FillConsoleOutputCharacter(IntPtr hConsoleOutput,
           char cCharacter, uint nLength, COORD dwWriteCoord, out uint
           lpNumberOfCharsWritten);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleInputW", CharSet = CharSet.Unicode)]
        public static extern bool ReadConsoleInput(
                IntPtr hConsoleInput,
                [Out] INPUT_RECORD[] lpBuffer,
                uint nLength,
                out uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent,
           uint dwProcessGroupId);
        public const uint CtrlCEvent = 0;
        public const uint CtrlBreakEvent = 1;

        [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
        public struct KEY_EVENT_RECORD
        {
            [FieldOffset(0), MarshalAs(UnmanagedType.Bool)]
            public bool bKeyDown;
            [FieldOffset(4), MarshalAs(UnmanagedType.U2)]
            public ushort wRepeatCount;
            [FieldOffset(6), MarshalAs(UnmanagedType.U2)]
            public VirtualKeys wVirtualKeyCode;
            [FieldOffset(8), MarshalAs(UnmanagedType.U2)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(12), MarshalAs(UnmanagedType.U4)]
            public ControlKeyState dwControlKeyState;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct MOUSE_EVENT_RECORD
        {
            [FieldOffset(0)]
            public COORD dwMousePosition;
            [FieldOffset(4)]
            public uint dwButtonState;
            [FieldOffset(8)]
            public uint dwControlKeyState;
            [FieldOffset(12)]
            public uint dwEventFlags;
        }



        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            public COORD dwSize;

            public WINDOW_BUFFER_SIZE_RECORD(short x, short y)
            {
                this.dwSize = new COORD(x, y);
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)]
            public InputEventTypes EventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
            [FieldOffset(4)]
            public MENU_EVENT_RECORD MenuEvent;
            [FieldOffset(4)]
            public FOCUS_EVENT_RECORD FocusEvent;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct FOCUS_EVENT_RECORD
        {
            public uint bSetFocus;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MENU_EVENT_RECORD
        {
            public uint dwCommandId;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_CURSOR_INFO {
          public uint dwSize;
          public bool  bVisible;
        } 

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleCursorInfo(
            IntPtr hConsoleOutput,
            out CONSOLE_CURSOR_INFO lpConsoleCursorInfo);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleCursorInfo(
            IntPtr hConsoleOutput,
            [In] ref CONSOLE_CURSOR_INFO lpConsoleCursorInfo
            );

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCursorPosition(IntPtr hConsoleOutput, COORD dwCursorPosition);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetExitCodeProcess(IntPtr hProcess, out uint lpExitCode);
        public const uint StillAlive = 259;

        /* Reads data from a console input buffer and removes it from the buffer. */
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleInputW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WriteConsoleInput(
            IntPtr hConsoleInput,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT_RECORD[] lpBuffer,
            uint nLength,
            out uint lpNumberOfEventsWritten);


        /* Writes character and color attribute data to a specified rectangular block of character cells in a console screen buffer.
       The data to be written is taken from a correspondingly sized rectangular block at a specified location in the source buffe */
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool WriteConsoleOutput(
            IntPtr hConsoleOutput,
            /* This pointer is treated as the origin of a two-dimensional array of CHAR_INFO structures
            whose size is specified by the dwBufferSize parameter.*/
            [MarshalAs(UnmanagedType.LPArray), In] CHAR_INFO[] lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpWriteRegion);

        [DllImport("kernel32.dll", EntryPoint = "ReadConsoleOutputW", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ReadConsoleOutput(
            IntPtr hConsoleOutput,
            /* This pointer is treated as the origin of a two-dimensional array of CHAR_INFO structures
            whose size is specified by the dwBufferSize parameter.*/
            [MarshalAs(UnmanagedType.LPArray), Out] CHAR_INFO[] lpBuffer,
            COORD dwBufferSize,
            COORD dwBufferCoord,
            ref SMALL_RECT lpReadRegion);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute,
           [In] ref SMALL_RECT lpConsoleWindow);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput,
           COORD dwSize);

        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);
        [DllImport("kernel32.dll")]
        public static extern bool CreateProcess(string lpApplicationName,
           string lpCommandLine, IntPtr security,
           IntPtr thread, bool bInheritHandles,
           CreateProcessFlags dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
           [In] ref STARTUPINFO lpStartupInfo,
           out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(uint dwProcessId);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(StdHandle nStdHandle);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfo(
            IntPtr hConsoleOutput,
            out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo
            );

        [DebuggerDisplay("Char={UnicodeChar}")]
        [StructLayout(LayoutKind.Explicit)]
        public  struct CHAR_INFO
        {
            [FieldOffset(0)]
            internal char UnicodeChar;
            [FieldOffset(0)]
            internal char AsciiChar;
            [FieldOffset(2)] //2 bytes seems to work properly
            internal Attributes Attributes;
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public COORD(short x, short y)
            {
                X = x; Y = y;
            }
            public short X;
            public short Y;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_SCREEN_BUFFER_INFO
        {

            public COORD dwSize;
            public COORD dwCursorPosition;
            public short wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public SMALL_RECT(short left, short top, short right, short bottom)
            {
                Left = left; Top = top; Right = right; Bottom = bottom;
            }
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;

        }

        public enum StdHandle
        {
            Input = -10,
            Output = -11,
            Error = -12
        }


        [StructLayout(LayoutKind.Sequential)]
        public  struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [Flags]
        public enum STARTF : uint
        {
            STARTF_USESHOWWINDOW = 0x00000001,
            STARTF_USESIZE = 0x00000002,
            STARTF_USEPOSITION = 0x00000004,
            STARTF_USECOUNTCHARS = 0x00000008,
            STARTF_USEFILLATTRIBUTE = 0x00000010,
            STARTF_RUNFULLSCREEN = 0x00000020,  // ignored for non-x86 platforms
            STARTF_FORCEONFEEDBACK = 0x00000040,
            STARTF_FORCEOFFFEEDBACK = 0x00000080,
            STARTF_USESTDHANDLES = 0x00000100,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public STARTF dwFlags;
            public SHOWWINDOW wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [Flags]
        public enum CreateProcessFlags : uint
        {
            DEBUG_PROCESS = 0x00000001,
            DEBUG_ONLY_THIS_PROCESS = 0x00000002,
            CREATE_SUSPENDED = 0x00000004,
            DETACHED_PROCESS = 0x00000008,
            CREATE_NEW_CONSOLE = 0x00000010,
            NORMAL_PRIORITY_CLASS = 0x00000020,
            IDLE_PRIORITY_CLASS = 0x00000040,
            HIGH_PRIORITY_CLASS = 0x00000080,
            REALTIME_PRIORITY_CLASS = 0x00000100,
            CREATE_NEW_PROCESS_GROUP = 0x00000200,
            CREATE_UNICODE_ENVIRONMENT = 0x00000400,
            CREATE_SEPARATE_WOW_VDM = 0x00000800,
            CREATE_SHARED_WOW_VDM = 0x00001000,
            CREATE_FORCEDOS = 0x00002000,
            BELOW_NORMAL_PRIORITY_CLASS = 0x00004000,
            ABOVE_NORMAL_PRIORITY_CLASS = 0x00008000,
            INHERIT_PARENT_AFFINITY = 0x00010000,
            INHERIT_CALLER_PRIORITY = 0x00020000,
            CREATE_PROTECTED_PROCESS = 0x00040000,
            EXTENDED_STARTUPINFO_PRESENT = 0x00080000,
            PROCESS_MODE_BACKGROUND_BEGIN = 0x00100000,
            PROCESS_MODE_BACKGROUND_END = 0x00200000,
            CREATE_BREAKAWAY_FROM_JOB = 0x01000000,
            CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x02000000,
            CREATE_DEFAULT_ERROR_MODE = 0x04000000,
            CREATE_NO_WINDOW = 0x08000000,
            PROFILE_USER = 0x10000000,
            PROFILE_KERNEL = 0x20000000,
            PROFILE_SERVER = 0x40000000,
            CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000,
        }

    }
}
