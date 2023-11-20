using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace _10sys
{
    public class KeyboardHook
    {
        public static Thread LookForShortcutThread;
        public static bool Closing = false;
        public static Dictionary<Keys,bool> KeysPressed = new();
        public KeyboardHook(Keys[] keys)
        {
            foreach (var key in keys)
            {
                KeysPressed.Add(key, false);
            }
            hookId = SetHook(HookCallback);
            LookForShortcutThread = new Thread(LookForShortcutLoop);
            LookForShortcutThread.Start();
            
        }
        ~KeyboardHook()
        {
            UnhookWindowsHookEx(hookId);
        }

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        

        private static IntPtr hookId = IntPtr.Zero;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private static LowLevelKeyboardProc _proc;
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            _proc = proc;
            return SetWindowsHookEx(WH_KEYBOARD_LL, _proc,  new IntPtr(0), 0);
            
        }
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var key = (Keys)vkCode;
                //Console.WriteLine(key+" --> down");
                if (KeysPressed.ContainsKey(key))
                {
                    KeysPressed[key] = true;
                }
                
            }
            if(nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var key = (Keys)vkCode;
                //Console.WriteLine(key+" <-- up");
                if (KeysPressed.ContainsKey(key))
                {
                    KeysPressed[key] = false;
                }
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }
         
        private void LookForShortcutLoop()
        {
            while (Closing == false)
            {
                Console.WriteLine(
                    string.Join("    ",KeysPressed.Select(kv => $"{kv.Key}={kv.Value}")));
                if(KeysPressed.Values.All(v => v))
                {
                    Closing = true;
                    Application.Exit();
                }
                Thread.Sleep(10);
            }
        }
    }
}