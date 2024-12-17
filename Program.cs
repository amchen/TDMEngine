using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TDMEngine
{
	static class Program
	{		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>

		[STAThread]
		static void Main()
		{
			//Check Command line arguments. -d (dev mode)
			string[] s = Environment.GetCommandLineArgs();
			if (s.Length ==4)
			{
				EqeSysTDMProcess e = new EqeSysTDMProcess();
				e.process_client_request (s[0],s[1],s[2],s[3]);			
			}			
		}		
	}
}
