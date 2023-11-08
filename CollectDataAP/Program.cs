using ECcomm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 

namespace CollectDataAP
{
    class Program
    {
        // Import user32.dll functions to register and unregister hotkeys
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public const int MOD_ALT = 0x0001;
        public const int MOD_CONTROL = 0x0002;
        public const int MOD_SHIFT = 0x004;
        public const int MOD_NOREPEAT = 0x400;
        public const int WM_HOTKEY = 0x312;
        public const int DSIX = 0x36;

        public static void sendKeystroke()
        {
            const uint WM_KEYDOWN = 0x100;
            const uint WM_KEYUP = 0x0101;

            IntPtr hWnd;
            string processName = "putty";
            Process[] processList = Process.GetProcesses();
            KeyEventArgs e;
            foreach (Process P in processList)
            {
                if (P.ProcessName.Equals(processName))
                {
                    IntPtr edit = P.MainWindowHandle;
                    //PostMessage(edit, WM_KEYDOWN, (IntPtr)(Keys.Control), IntPtr.Zero);
                    //PostMessage(edit, WM_KEYDOWN, (IntPtr)(Keys.Menu), IntPtr.Zero);
                    PostMessage(edit, WM_KEYUP, (IntPtr)(Keys.Menu), IntPtr.Zero);
                    PostMessage(edit, WM_KEYUP, (IntPtr)(Keys.Control), IntPtr.Zero);
                }
            }
        }

        static void Main(string[] args)
        {
            _hookID = SetHook(_proc);  //Set our hook
            Application.Run();         //Start a standard application method loop 
        }

        ///////////////////////////////////////////////////////////
        //A bunch of DLL Imports to set a low level keyboard hook
        ///////////////////////////////////////////////////////////
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        ////////////////////////////////////////////////////////////////
        //Some constants to make handling our hook code easier to read
        ////////////////////////////////////////////////////////////////
        private const int WH_KEYBOARD_LL = 13;                    //Type of Hook - Low Level Keyboard
        private const int WH_KEYBOARD = 2;                    //Type of Hook - Low Level Keyboard

        private const int WM_KEYDOWN = 0x0100;                    //Value passed on KeyDown
        private const int WM_SYSKEYDOWN = 0x0104;                  //Value passed on  KeyDown for menu 
        private const int WM_KEYUP = 0x0101;                      //Value passed on KeyUp
        private static LowLevelKeyboardProc _proc = HookCallback; //The function called when a key is pressed
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool CONTROL_DOWN = false;                 //Bool to use as a flag for control key


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            sendKeystroke();
            
            //if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) //A Key was pressed down
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)) //A Key was pressed down
            {
                int vkCode = Marshal.ReadInt32(lParam);           //Get the keycode
                string theKey = ((Keys)vkCode).ToString();        //Name of the key
                Console.Write(theKey);                            //Display the name of the key
                if (theKey.Contains("ControlKey"))                //If they pressed control
                {
                    CONTROL_DOWN = true;                          //Flag control as down
                }
                else if (CONTROL_DOWN && theKey == "B")           //If they held CTRL and pressed B
                {
                    Console.WriteLine("\n***HOTKEY PRESSED***");  //Our hotkey was pressed
                }
                else if (theKey == "Escape")                      //If they press escape
                {
                    UnhookWindowsHookEx(_hookID);                 //Release our hook
                    Environment.Exit(0);                          //Exit our program
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP) //KeyUP
            {
                int vkCode = Marshal.ReadInt32(lParam);        //Get Keycode
                string theKey = ((Keys)vkCode).ToString();     //Get Key name
                if (theKey.Contains("ControlKey"))             //If they let go of control
                {
                    CONTROL_DOWN = false;                      //Unflag control
                }
                if((theKey.Contains("RShiftKey")|| theKey.Contains("LShiftKey")) && (theKey.Contains("RMenu") || theKey.Contains("LMenu")) && (theKey.Contains("LControlKey") || theKey.Contains("RControlKey"))  && (theKey.Contains("D0")  ))
                {
                    Console.WriteLine("HotTab Menu Key Pressed");
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam); //Call the next hook
        }


        /*
        private static void RegisterHotKey(Keys key, KeyModifiers modifiers)
        {
            const int id = 1; // Unique identifier for the hotkey

            if (RegisterHotKey(IntPtr.Zero, id, (uint)modifiers, (uint)key))
            {
                Console.WriteLine($"Hotkey (Ctrl+Alt+{key}) registered.");
            }
            else
            {
                Console.WriteLine("Failed to register the hotkey.");
            }
        }*/


        /*
        protected  override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case HotTabECEventArgs.ECEvent_F1Short:
                    Console.WriteLine("F1 Key Short Press");
                    break;

                case HotTabECEventArgs.ECEvent_F1Long:
                    Console.WriteLine("F1 Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_F2Short:
                    Console.WriteLine("F2 Key Short Press");
                    break;

                case HotTabECEventArgs.ECEvent_F2Long:
                    Console.WriteLine("F2 Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_F3Short:
                    Console.WriteLine("F3 Key Short Press");
                    break;

                case HotTabECEventArgs.ECEvent_F3Long:
                    Console.WriteLine("F3 Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_F4Short:
                    Console.WriteLine("F4 Key Short Press");
                    break;

                case HotTabECEventArgs.ECEvent_F4Long:
                    Console.WriteLine("F4 Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_F5Short:
                    Console.WriteLine("F5 Key Short Press");
                    break;

                case HotTabECEventArgs.ECEvent_F5Long:
                    Console.WriteLine("F5 Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_F6Short:
                    Console.WriteLine("F6 Key Short Press");
                    break;

                case HotTabECEventArgs.ECEvent_F6Long:
                    Console.WriteLine("F6 Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_F7Short:
                    Console.WriteLine("F7 Key Short Press");
                    break;

                case HotTabECEventArgs.ECEvent_F7Long:
                    Console.WriteLine("F7 Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_Menu:
                    Console.WriteLine("Menu Key Press");
                    break;

                case HotTabECEventArgs.ECEvent_HomeShort:
                    Console.WriteLine("Home Key Short Press)");
                    break;

                case HotTabECEventArgs.ECEvent_HomeLong:
                    Console.WriteLine("Home Key Long Press");
                    break;

                case HotTabECEventArgs.ECEvent_VolumeUp:
                    Console.WriteLine("Volume UP(Up) Press");
                    break;

                case HotTabECEventArgs.ECEvent_VolumeDown:
                    Console.WriteLine("Volume Down(Down) Press");
                    break;

                case HotTabECEventArgs.ECEvent_Left:
                    Console.WriteLine("Left Key Press");
                    break;

                case HotTabECEventArgs.ECEvent_Right:
                    Console.WriteLine("Right Key Press");
                    break;

                case HotTabECEventArgs.ECEvent_OK:
                    Console.WriteLine("OK Key Press");
                    break;

                case HotTabECEventArgs.ECEvent_FM08Lamp:
                    Console.WriteLine("[FM08]Lamp Key Press");
                    break;
                case HotTabECEventArgs.ECEvent_FM08BrightnessAuto:
                    Console.WriteLine("[FM08]Bright Auto Key Press");
                    break;
                case HotTabECEventArgs.ECEvent_FM08BrightnessDown:
                    Console.WriteLine("[FM08]Bright Down Key Press");
                    break;
                case HotTabECEventArgs.ECEvent_FM08BrightnessUp:
                    Console.WriteLine("[FM08]Bright Up Key Press");
                    break;
                case HotTabECEventArgs.ECEvent_FM08VolumeMute:
                    Console.WriteLine("[FM08]Volume Mute Key Press");
                    break;
                case HotTabECEventArgs.ECEvent_FM08VolumeUp:
                    Console.WriteLine("[FM08]Volume Up Key Press");
                    break;
                case HotTabECEventArgs.ECEvent_FM08VolumeDown:
                    Console.WriteLine("[FM08]Volume Down Key Press");
                    break;

                case HotTabECEventArgs.ECEvent_GroundDockingIn:
                    Console.WriteLine("Ground Dock In(only support IBCMC)");
                    break;
                case HotTabECEventArgs.ECEvent_AirDockingIn:
                    Console.WriteLine("Air Docking In(only support IBCMC)");
                    break;
                case HotTabECEventArgs.ECEvent_DockingOut:
                    Console.WriteLine("Docking AC Out(only support IBCMC)");
                    break;
            }*/

        //base.WndProc(ref m);
    }
    
}
    

