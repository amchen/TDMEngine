using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TDMEngine
{
	static class LogWriter
	{
		static string _logFileName = string.Empty;

		public static string GetLogFileName()
		{
			return _logFileName;
		}
		public static void Init(string logFileName)
		{
			try
			{
				_logFileName = logFileName;
				if (File.Exists(_logFileName))
				{
					//string newLog = Path.GetDirectoryName(_logFileName) + @"\" +
						//Path.GetFileNameWithoutExtension(_logFileName) + DateTime.Now.ToString("dd-MM-yy-hh-mm-ss-fff") + ".txt";

					//File.Move(_logFileName, newLog);
					File.Delete(_logFileName);
				}

				LogIt("TDMEngine Started..\r\n",true);
			}
			catch (Exception exp)
			{
				LogIt("!!Warning!!Unable to create log file at the requested path." + exp.Message);

			}
		}

		public static void ShowLog()
		{
			if (File.Exists(_logFileName))
			{
				ProcessStartInfo pStartInfo = new ProcessStartInfo();
				pStartInfo.FileName = _logFileName;
				Process process = new Process();
				process.StartInfo = pStartInfo;
				process.Start();
			}
		}

		public static void LogIt( string message , bool timestamp=false)
		{
			try
			{
				StreamWriter writer = new StreamWriter(_logFileName, true);
				if (timestamp )
					message = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff") + "\t" + message ;
				
				writer.WriteLine(message);
				writer.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}


		public static void LogError(string message)
		{
			LogIt("!!Error!!: " + message);
		}

		
	}
}
