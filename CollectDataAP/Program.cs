using ECcomm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CollectDataAP
{
    class Program
    {
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

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        [DllImport("gdi32.dll")]
        private unsafe static extern bool SetDeviceGammaRamp(Int32 hdc, void* ramp);

        private static bool initialized = false;
        private static Int32 hdc;


        //Hook id
        private const int WH_KEYBOARD_LL = 13;                    //Type of Hook - Low Level Keyboard
        private const int WH_KEYBOARD = 2;                    //Type of Hook - Low Level Keyboard

        //Key id
        private const int WM_KEYDOWN = 0x0100;                    //Value passed on KeyDown
        private const int WM_SYSKEYDOWN = 0x0104;                  //Value passed on  KeyDown for menu 
        private const int WM_KEYUP = 0x0101;                      //Value passed on KeyUp

        private static LowLevelKeyboardProc _proc = HookCallback; //The function called when a key is pressed
        private static IntPtr _hookID = IntPtr.Zero;

        //Key flag for hotkey 
        private static bool menuUp = false;                 //Bool to use as a flag for control key
        private static bool controlUp = false;                 //Bool to use as a flag for control key
        private static bool shiftUp = false;                 //Bool to use as a flag for control key

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        private static IntPtr HWND_BROADCAST = (IntPtr)0xffff;

        private static IntPtr handle;

        static void Main(string[] args)
        {
            Tommy.Tommy_Start();

            handle = Process.GetCurrentProcess().MainWindowHandle;

            _hookID = SetHook(_proc);  //Set our hook
            Application.Run();         //Start a standard application method loop 
        }


        private static void InitializeClass()
        {
            if (initialized)
                return;

            //Get the hardware device context of the screen, we can do
            //this by getting the graphics object of null (IntPtr.Zero)
            //then getting the HDC and converting that to an Int32.
            hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc().ToInt32();

            initialized = true;
        }

        public static unsafe bool SetBrightness(short brightness)
        {
            InitializeClass();

            if (brightness > 255)
                brightness = 255;

            if (brightness < 0)
                brightness = 0;

            short* gArray = stackalloc short[3 * 256];
            short* idx = gArray;

            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 256; i++)
                {
                    int arrayVal = i * (brightness + 128);

                    if (arrayVal > 65535)
                        arrayVal = 65535;

                    *idx = (short)arrayVal;
                    idx++;
                }
            }

            //For some reason, this always returns false?
            bool retVal = SetDeviceGammaRamp(hdc, gArray);

            //Memory allocated through stackalloc is automatically free'd
            //by the CLR.

            return retVal;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //SendMessageW(_hookID, WM_APPCOMMAND, _hookID, (IntPtr)APPCOMMAND_VOLUME_UP);
           // SendMessageW(HWND_BROADCAST, WM_APPCOMMAND, HWND_BROADCAST, (IntPtr)APPCOMMAND_VOLUME_UP);

            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)) //A Key was pressed down
            {
                int vkCode = Marshal.ReadInt32(lParam);           //Get the keycode
                string theKey = ((Keys)vkCode).ToString();        //Name of the key
                Console.WriteLine("Key make " + theKey);

                if (theKey.Contains("A"))
                {
                    SendMessageW(handle, WM_APPCOMMAND, IntPtr.Zero, (IntPtr)APPCOMMAND_VOLUME_UP);
                }
                else if (theKey.Contains("B"))
                {
                    SendMessageW(handle, WM_APPCOMMAND, IntPtr.Zero, (IntPtr)APPCOMMAND_VOLUME_DOWN);
                }
                else if (theKey.Contains("C"))
                {
                    SetBrightness(126);
                }
                else if (theKey.Contains("D"))
                {
                    SetBrightness(256);
                }
                else if (theKey.Contains("E"))
                {
                    SetBrightness(10);
                }
                else if (theKey.Contains("F"))
                {
                    System.Diagnostics.Process.Start("calc");

                }
                else if (theKey.Contains("G"))
                {
                    Process.Start("C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe");
                }


                if (theKey == "Escape")                           //If they press escape
                {
                    UnhookWindowsHookEx(_hookID);                 //Release our hook
                    Environment.Exit(0);                          //Exit our program
                }
            }
            else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)    //KeyUP
            {
                int vkCode = Marshal.ReadInt32(lParam);           //Get Keycode
                string theKey = ((Keys)vkCode).ToString();        //Get Key name
                Console.WriteLine("Key break " + theKey);
                
                if (menuUp == true)
                {
                    if(theKey.Contains("D0") || theKey.Contains("D3") || theKey.Contains("D4") || theKey.Contains("D5") || theKey.Contains("D6")
                        || theKey.Contains("D7") || theKey.Contains("D") || theKey.Contains("M") || theKey.Contains("RShiftKey") || theKey.Contains("RControlKey"))
                    {
                        
                    }
                    else
                    {
                        menuUp = false;
                        controlUp = false;
                        shiftUp = false;
                    }
                }
                
                if (menuUp == true && controlUp == true && shiftUp == true)
                {
                    if (theKey.Contains("D0"))
                    {
                        Console.WriteLine("Menu Key Pressed");
                        SendMessageW(HWND_BROADCAST, WM_APPCOMMAND, HWND_BROADCAST, (IntPtr)APPCOMMAND_VOLUME_UP);
                    }
                    else if (theKey.Contains("D3"))
                    {
                        Console.WriteLine("F1 Key Short Press");
                    }
                    else if (theKey.Contains("D4"))
                    {
                        Console.WriteLine("F1 Key Long Press");
                    }
                    else if (theKey.Contains("D5"))
                    {
                        Console.WriteLine("F2 Key Short Press");
                    }
                    else if (theKey.Contains("D6"))
                    {
                        Console.WriteLine("F2 Key Long Presss");
                    }
                    else if (theKey.Contains("D7"))
                    {
                        Console.WriteLine("F3 Key Short Press");
                    }
                    else if (theKey.Contains("D8"))
                    {
                        Console.WriteLine("F3 Key Long Presss");
                    }
                    else if (theKey.Contains("D"))
                    {
                        Console.WriteLine("Home Key Short Press");
                    }
                    else if (theKey.Contains("M"))
                    {
                        Console.WriteLine("Home Key Long Press");
                    }

                    menuUp = false;
                    controlUp = false;
                    shiftUp = false;
                }

                if (theKey.Contains("ShiftKey") || theKey.Contains("RShiftKey") || theKey.Contains("LShiftKey") && menuUp == true && controlUp == true)
                { 
                    Console.WriteLine("HotTab ShiftHey Key Break");
                    shiftUp = true;
                }

                if (theKey.Contains("ControlKey") || theKey.Contains("RControlKey") || theKey.Contains("LControlKey") && menuUp == true)
                {
                    Console.WriteLine("HotTab ControlKey Key Break");
                    controlUp = true;
                }

                if (theKey.Contains("Menu") || theKey.Contains("RMenu") || theKey.Contains("LMenu"))
                {
                    Console.WriteLine("HotTab Menu Key Break");
                    menuUp = true;
                }

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam); //Call the next hook
        }
    }

}


