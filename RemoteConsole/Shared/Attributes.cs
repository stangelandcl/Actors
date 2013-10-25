using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteConsole
{
    [Flags]
    public enum Attributes : ushort
    {
        ForegroundBlue = 1,
        ForegroundGreen = 2,
        ForegroundRed = 4,
        ForegroundIntensity = 8,
        BackgroundBlue =0x10,
        BackgroundGreen = 0x20,
        BackgroundRed = 0x40,
        BackgroundIntensity = 0x80,
        CommonLvbLeadingByte = 0x100,
        CommonLvbTrailingByte = 0x200,
        CommonLvbGridHorizontal = 0x400,
        CommonLvbGridLeftVertical = 0x800,
        CommonLvbGridRightVertical = 0x1000,
        CommonLvbReverseVideo = 0x4000,
        CommonLvbUnderscore = 0x8000,
    }
}
