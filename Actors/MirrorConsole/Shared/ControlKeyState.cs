using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Actors
{
    [Flags]
    public enum ControlKeyState
    {

        RIGHT_ALT_PRESSED = 1,
        LEFT_ALT_PRESSED = 2,
        RIGHT_CTRL_PRESSED = 4,
        LEFT_CTRL_PRESSED = 8,
        SHIFT_PRESSED = 16,
        NUMLOCK_ON = 32,
        SCROLLLOCK_ON = 64,
        CAPSLOCK_ON = 128,
        ENHANCED_KEY = 0x100,
    };
}
