using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace _10sys
{
    public class Windows
    {
        [DllImport("User32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        internal static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);
        public static Int32 WM_SYSCOMMAND = 0x0112;
        public static Int32 SC_RESTORE = 0xF120;
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public const int SW_SHOWMAXIMIZED = 3;
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public static bool FocusWindow(string windowName)
        {
         
            var prc = Windows.GetForegroundWindow();
            var proc = Process.GetProcessesByName(windowName).FirstOrDefault();
            
            if (proc == null)
            {
                Console.WriteLine("proc not found!");
                return false;
            }
            
            var pointer = proc!.MainWindowHandle;
            
            SetForegroundWindow(pointer);
            SendMessage(pointer, Windows.WM_SYSCOMMAND, Windows.SC_RESTORE, 0);
            
            // Make the window fullscreen
            ShowWindow(pointer, SW_SHOWMAXIMIZED);
            
            return true;
        }

    }
}
