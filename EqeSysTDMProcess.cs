using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDMEngine
{
	public class EqeSysTDMProcess
	{
		TDMConfig _tdmConfig = null;
		TDMMigrate _tdmMigrate = null;
		EqeSysTDMEvent _eqeSysTDMEvent = null;

		public EqeSysTDMProcess()
		{
			_eqeSysTDMEvent = new EqeSysTDMEvent();
		}

		public int CreateTdmProcess(string command_str, string logFileName)
		{
			int retCode = 0;

			_tdmConfig = new TDMConfig();
			_tdmMigrate = new TDMMigrate(_tdmConfig, _eqeSysTDMEvent);

			_tdmConfig.PutString(command_str);
			LogWriter.Init(logFileName);

			try
			{


				ParseCommandString();
				retCode = _tdmMigrate.DoMigrate();
			}


			catch (MigrationFailedException ex)
			{
				retCode = 1;    //  1 is failure
				LogWriter.LogIt(ex.Message);
			}
			catch (Exception)
			{
				//catch (CancelRunException *e)
				retCode = -1;
				LogWriter.LogIt("TDMEngine was canceled. Exiting process_client_request...");

			}
			if (retCode == -1)    //  0 is success
			{
				string errorMsg;


				errorMsg = _tdmMigrate.GetErrorMsg();

				errorMsg = TDMEngineSettings.Default.TDMFailureMsg;

				throw new TDMEngineException(errorMsg);
			}

			return retCode;
		}

		public void KillTDMProcess()
		{
			_tdmMigrate.Cancel();
		}

		public int process_client_request(string command_str, string datasource, string user_name, string password)
		{
			string logFile="";
			return CreateTdmProcess(command_str,logFile);
		}

		public EqeSysTDMEvent GetTDMServer()
		{
			return _eqeSysTDMEvent;
		}

		void ParseCommandString()
		{
			string s = "TDM Configuration Options\r\n=========================\r\n";

			LogWriter.LogIt(s);

			s = "Source Build                = " + _tdmConfig.CurrBuild + "\r\n";
			s += "Destination Build           = " + _tdmConfig.DestBuild + "\r\n";
			s += "TDM Path                    = " + _tdmConfig.TdmFilePath + "\r\n";
			s += "Primary Database Filename   = " + _tdmConfig.PrimaryMDB + "\r\n";
			s += "IR Database Filename        = " + _tdmConfig.IRMDB + "\r\n";
			s += "Primary Database ServerName = " + _tdmConfig.PrimaryServerName + "\r\n";
			s += "IR Database ServerName      = " + _tdmConfig.IRServerName + "\r\n";
			s += "Primary Database Name       = " + _tdmConfig.PrimaryDBName + "\r\n";
			s += "IR Database Name            = " + _tdmConfig.IRDBName + "\r\n";
			s += "Username                    = " + _tdmConfig.UserName + "\r\n";

			if (_tdmConfig.TDMMode == TDMMODE.STANDARD)
				s += "TDM Mode                    = Standard Migration \r\n";
			else if (_tdmConfig.TDMMode == TDMMODE.SYS_DB)
				s += "TDM Mode                    = System Database \r\n";
			else if (_tdmConfig.TDMMode == TDMMODE.COM_DB)
				s += "TDM Mode                    = Common Database \r\n";
			else if (_tdmConfig.TDMMode == TDMMODE.CF_DB)
				s += "TDM Mode                    = Primary Database \r\n";
			else if (_tdmConfig.TDMMode == TDMMODE.CF_DB_IR)
				s += "TDM Mode                    = IR Database \r\n";

			if (_tdmConfig.IsMigrateResults())
				s += "TDM Option                  = Primary and IR databases \r\n";
			else
				s += "TDM Option                  = Primary database only \r\n";

			s += "Snapshot Name               = " + _tdmConfig.SnapshotName + "\r\n";
			s += "Snapshot User               = " + _tdmConfig.SnapshotUser + "\r\n";
			s += "Snapshot Description        = " + _tdmConfig.SnapshotDesc + "\r\n";
			s += "Snapshot Type               = " + _tdmConfig.SnapshotType + "\r\n";
			s += "CheckDate                   = " + _tdmConfig.CheckDate + "\r\n";
			s += "\r\n";
			LogWriter.LogIt(s);
		}
	}
}
