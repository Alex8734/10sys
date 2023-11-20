using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace _10sys
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            
            var keyboardHook = new KeyboardHook(new[]{Keys.Escape});
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }
    }
}
