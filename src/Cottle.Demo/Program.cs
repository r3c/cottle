using System;
using System.Windows.Forms;
using Cottle.Demo.Forms;

namespace Cottle.Demo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new DemoForm());
        }
    }
}