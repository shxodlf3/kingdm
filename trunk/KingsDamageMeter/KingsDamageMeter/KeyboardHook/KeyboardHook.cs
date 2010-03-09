/**************************************************************************\
 * 
    This file is part of KingsDamageMeter.

    KingsDamageMeter is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    KingsDamageMeter is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with KingsDamageMeter. If not, see <http://www.gnu.org/licenses/>.
 * 
\**************************************************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KingsDamageMeter
{
    public static class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x101;
        private const byte VK_LSHIFT = 0xA0;
        private const byte VK_RSHIFT = 0xA1;
        private const byte VK_LCONTROL = 0xA2;
        private const byte VK_RCONTROL = 0xA3;
        private const byte VK_LMENU = 0xA4;
        private const byte VK_RMENU = 0xA5;
        private const byte VK_CAPITAL = 0x14;
        private const byte PRESSED_STATE = 0x80;

        private static IntPtr _Handle = IntPtr.Zero;

        public static event KeyEventHandler KeyDown;
        public static event KeyEventHandler KeyUp;

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static HookProc SafeHookProc = new HookProc(HookCallback);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        public static IntPtr Handle
        {
            get
            {
                return _Handle;
            }
        }

        public static bool ControlIsDown
        {
            get
            {
                if ((GetKeyState(VK_LCONTROL) & PRESSED_STATE) == PRESSED_STATE)
                {
                    return true;
                }

                if ((GetKeyState(VK_RCONTROL) & PRESSED_STATE) == PRESSED_STATE)
                {
                    return true;
                }

                return false;
            }
        }

        public static bool AltIsDown
        {
            get
            {
                if ((GetKeyState(VK_LMENU) & PRESSED_STATE) == PRESSED_STATE)
                {
                    return true;
                }

                if ((GetKeyState(VK_RMENU) & PRESSED_STATE) == PRESSED_STATE)
                {
                    return true;
                }

                return false;
            }
        }

        public static bool ShiftIsDown
        {
            get
            {
                if ((GetKeyState(VK_LSHIFT) & PRESSED_STATE) == PRESSED_STATE)
                {
                    return true;
                }

                if ((GetKeyState(VK_RSHIFT) & PRESSED_STATE) == PRESSED_STATE)
                {
                    return true;
                }

                return false;
            }
        }

        public static bool CapsLockEnabled
        {
            get
            {
                return (GetKeyState(VK_CAPITAL) != 0 ? true : false);
            }
        }

        public static void Start()
        {
            using (Process process = Process.GetCurrentProcess())
            using (ProcessModule module = process.MainModule)
            {
                _Handle = SetWindowsHookEx(WH_KEYBOARD_LL, SafeHookProc, GetModuleHandle(module.ModuleName), 0);
            }
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(_Handle);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                if (KeyDown != null && !IsControlKey(key))
                {
                    KeyDown(null, new KeyEventArgs(key));
                }
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                if (KeyUp != null && !IsControlKey(key))
                {
                    KeyUp(null, new KeyEventArgs(key));
                }
            }

            return CallNextHookEx(_Handle, nCode, wParam, lParam);
        }

        private static bool IsControlKey(Keys key)
        {
            switch (key)
            {
                case Keys.LControlKey:
                case Keys.RControlKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                case Keys.LMenu:
                case Keys.RMenu:
                    return true;
                default:
                    return false;
            }
        }
    }
}
