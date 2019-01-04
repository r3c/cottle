using System;
#if NET472
using System.Windows.Forms;
#endif

namespace Cottle.Demo
{
	class Program
	{
		[STAThread]
		static void Main ()
		{
			#if NET472
			Application.EnableVisualStyles ();
			Application.SetCompatibleTextRenderingDefault (false);
			Application.Run (new DemoForm ());
			#else
			throw new Exception ("application was compiled with unsupported framework");
			#endif
		}
	}
}
