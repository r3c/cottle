using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace   Demo
{
    class   Program
    {
        [STAThread]
        static void Main ()
        {
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault (false);
            Application.Run (new DemoForm ());
        }
    }
}
