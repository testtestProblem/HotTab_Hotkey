using System;
using System.Collections.Generic;
using System.Text;

namespace ECcomm
{
    public class HotTabECEventArgs : EventArgs
    {
        #region Const Event Number Declare

        public const int WM_APP = 0x8000;

        public const int ECEvent_VolumeUp = WM_APP + 0x25;
        public const int ECEvent_VolumeDown = WM_APP + 0x26;
        public const int ECEvent_Left = WM_APP + 0x27;
        public const int ECEvent_Right = WM_APP + 0x28;
        public const int ECEvent_OK = WM_APP + 0x29;

        public const int ECEvent_F1Short = WM_APP + 0x31;
        public const int ECEvent_F1Long = WM_APP + 0x32;
        public const int ECEvent_F2Short = WM_APP + 0x33;
        public const int ECEvent_F2Long = WM_APP + 0x34;
        public const int ECEvent_F3Short = WM_APP + 0x35;
        public const int ECEvent_F3Long = WM_APP + 0x36;
        public const int ECEvent_F4Short = WM_APP + 0x37;
        public const int ECEvent_F4Long = WM_APP + 0x38;
        public const int ECEvent_F5Short = WM_APP + 0x39;
        public const int ECEvent_F5Long = WM_APP + 0x3A;
        public const int ECEvent_F6Short = WM_APP + 0x3B;
        public const int ECEvent_F6Long = WM_APP + 0x3C;
        public const int ECEvent_F7Short = WM_APP + 0x3D;
        public const int ECEvent_F7Long = WM_APP + 0x3E;

        public const int ECEvent_Menu = WM_APP + 0x3F;
        public const int ECEvent_HomeShort = WM_APP + 0x40;
        public const int ECEvent_HomeLong = WM_APP + 0x41;

        public const int ECEvent_FM08VolumeUp = WM_APP + 0x42;
        public const int ECEvent_FM08VolumeDown = WM_APP + 0x43;
        public const int ECEvent_FM08VolumeMute = WM_APP + 0x44;
        public const int ECEvent_FM08BrightnessAuto = WM_APP + 0x45;
        public const int ECEvent_FM08BrightnessUp = WM_APP + 0x46;
        public const int ECEvent_FM08BrightnessDown = WM_APP + 0x47;
        public const int ECEvent_FM08Lamp = WM_APP + 0x48;

        public const int ECEvent_GroundDockingIn = WM_APP + 0x52;
        public const int ECEvent_AirDockingIn = WM_APP + 0x53;
        public const int ECEvent_DockingOut = WM_APP + 0x54;

        #endregion
    }
}
