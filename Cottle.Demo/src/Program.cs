using System;
using System.Windows.Forms;

namespace Cottle.Demo
{
	class Program
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
