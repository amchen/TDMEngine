using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using WCeFramework.WCeDBInterface;

namespace TDMEngine
{

	class TDMMigrate
	{
		#region private data members
		string _errorMsg;
		int totalSteps = 0;
		MSAccessInterface _accessInterface;
		SQLDBInterface _sqlDBInterface;
		TDMConfig _tdmConfig;
		ProgressHandler _progressHandler;
		EqeSysTDMEvent _eqeSysTDMEvent;


		private enum MigrType
		{
			PRECMD = 0,
			STANDARD,
			POSTCMD,
		}
		#endregion

		#region constructor
		public TDMMigrate(TDMConfig tconfig, EqeSysTDMEvent eqeSysTDMEvent)
		{
			_tdmConfig = tconfig;
			_eqeSysTDMEvent = eqeSysTDMEvent;
			_progressHandler = new ProgressHandler(_eqeSysTDMEvent);
		}
		#endregion

		#region public methods
		public string GetErrorMsg() { return _errorMsg; }

		public void Cancel()
		{
			if (_sqlDBInterface != null)
			{
				_sqlDBInterface.Cancel();
				LogWriter.LogIt("Aborting TDMEngine..");
			}

		}

		public int DoMigrate()
		{
			int result = 0;
			string s = string.Empty;
			string errMsg = string.Empty;

			try
			{

				result = InitializeTDMEngine();
				if (result == 1)
				{
					LogWriter.LogIt("Migration steps=0. No migration needed.");
					return result;
				}
				else if (result == -1)
					return result;

				result = DoPrePostCmd("TDMPRECMD", MigrType.PRECMD);   // pre migration

				result = DoStandard();

				result = DoPrePostCmd("TDMPOSTCMD", MigrType.POSTCMD);   // post migration

				MigrCleanUpTasks();
				s = "======= TDMEngine exiting with " + _tdmConfig.TotalErrors + " error(s) =======";

				LogWriter.LogIt(s, false);
				if (_tdmConfig.TotalErrors > 0)
					_progressHandler.SetFailedMessage(s);
				else
					_progressHandler.SetSuccessMessage(s);
				//if (_tdmConfig.IsEqecatSystemDB())
				//{
				//    //Set DB Mode to READ_ONLY for EqecatSystem
				//    SetDBModeReadOnly(true);
				//}
			}
			catch (Exception)
			{
				string str = "\tTDMEngine -- Unexpected Exception";
				LogWriter.LogIt(str);
				result = 1;

				_progressHandler.SetFailedMessage(str);
				//set_process_failed(str);
			}

			return result;
		}
		#endregion

		#region private methods


		private bool ExecuteTDMCommand(string tdmType, string tname, string cmd, bool validDBVersion)
		{
			bool status = true;
			//Check if connection is closed
			if (!_sqlDBInterface.ConnectionStateOpen()) return false;

			//// Process command type
			if (tdmType == "IT")
			{    // insert table data
				if (validDBVersion)
					status = _sqlDBInterface.InsertTable(tname);
			}
			else if (tdmType == "LT")
			{    // load table
				if (validDBVersion)
					status = _sqlDBInterface.LoadTable(tname,  true);
			}
			else if (tdmType == "LTA")
			{   // load table ALWAYS
				status = _sqlDBInterface.LoadTable(tname, true);
			}
			else if (tdmType == "DT")
			{    // drop table
				status = _sqlDBInterface.DropTable(tname, true);
			}
			else if (tdmType == "CT")
			{    // create table
				if (validDBVersion)
					status = _sqlDBInterface.CreateTable(tname, true);
			}
			else if (tdmType == "AT")
			{    // alter table
				if (validDBVersion)
					status = _sqlDBInterface.AlterTable(tname, cmd);
			}
			else if (tdmType == "LX")
			{    // load indexes
				if (validDBVersion)
					status = _sqlDBInterface.LoadIndex(tname);
			}
			else if (tdmType == "CX")
			{    // create index
				if (validDBVersion)
					status = _sqlDBInterface.LoadIndex(tname, cmd);
			}
			else if (tdmType == "DX")
			{    // drop index
				if (validDBVersion)
					status = _sqlDBInterface.DropIndex(tname, cmd);
			}
			else if (tdmType == "LPF")
			{    // load all procedures
				//status = _sqlDBInterface.LoadAllProcedures();
			}
			else if (tdmType == "LP")
			{    // load all procedures
				//status = _sqlDBInterface.LoadProcedures();
			}
			else if (tdmType == "LV")
			{    // load all views
				status = _sqlDBInterface.LoadViews();
			}
			else if (tdmType == "CP")
			{    // create procedure
				//status = _sqlDBInterface.LoadProcedure(cmd);
			}
			else if (tdmType == "DP")
			{    // drop procedure
				status = _sqlDBInterface.DropProcedure(cmd);
			}
			else if (tdmType == "SQL")
			{   // SQL query with commit
				if (validDBVersion)
					status = _sqlDBInterface.ExecSQL(cmd);
			}
			else if (tdmType == "VDV")
			{   // verify database version
				//validDBVersion = VerifyDBVersion();
			}
			else if (tdmType == "UDV")
			{   // update database version
				//status = 0 == _errors ? true : false;
			}
			else if (tdmType == "SYS")
			{   // system command
				if (validDBVersion)
					status = _sqlDBInterface.ExecSYS(cmd);
			}
			else if (tdmType == "LOG")
			{   // log message
				string msg = "Message:  " + cmd;
				LogWriter.LogIt(msg);
			}
			else if (tdmType == "MSG")
			{   // database message
				status = _sqlDBInterface.DatabaseMessage(cmd);
			}
			else if (tdmType == "LR")
			{    // load all triggers (on*.sql)
				status = _sqlDBInterface.LoadTriggers();
			}
			else if (tdmType == "NOP")
			{   // no operation
				// do nothing here
			}

			else if (tdmType == "ToMDB")
			{     // copy table from database to .mdb
				status = _sqlDBInterface.ToMDB(cmd);
			}
			else if (tdmType == "FromMDB")
			{   // copy table from .mdb to database
				status = _sqlDBInterface.FromMDB(cmd);
			}
			else if (tdmType == "OPEN")
			{  // ShellExecute - open document with associated program
				status = _sqlDBInterface.TDMOpen(cmd);
			}
			else if (tdmType == "SQLOUT")
			{  // execute a procedure with a single out parameter
				LogWriter.LogIt(_sqlDBInterface.ExecuteProcedureWithOutParam(cmd, "@retVal"));
			}
			else if (tdmType == "ThreadExec")
			{
				_sqlDBInterface.ThreadExec(cmd);
			}
			else
			{  // unrecognized command
				LogWriter.LogIt("Warning (TDMEngine.exe): Unrecognized TDMTYPE command: " + tdmType);
			}

			return status;
		}

		private bool IsCorrectDB(string csTable)
		{
			bool success = false;

			if (csTable.Length == 0)
			{
				success = true;
			}
			else
			{
				if (_tdmConfig.IsEqecatSystemDB())
					success = _tdmConfig.IsSystemDBTable(csTable, true);

				if (_tdmConfig.IsEqecatCommonDB())
					success = _tdmConfig.IsCommonDBTable(csTable, true);

				if (_tdmConfig.IsEqecatUserDB())
				{
					if (_tdmConfig.MasterDB)
						success = _tdmConfig.IsCFDBTable(csTable, true);
					else
						success = _tdmConfig.IsCFIRDBTable(csTable, true);
				}

			}

			return success;
		}

	
		private string GetTableNameInProperCase(string tname, TDMMODE tdmMode)
		{
			TableMap t = null;
			if (tdmMode == TDMMODE.SYS_DB)
			{
				if (_tdmConfig.SysDBTableList.ContainsKey(tname.ToUpper())) t = _tdmConfig.SysDBTableList[tname.ToUpper()];
			}
			else if (tdmMode == TDMMODE.COM_DB)
			{
				if (_tdmConfig.ComDBTableList.ContainsKey(tname.ToUpper())) t = _tdmConfig.ComDBTableList[tname.ToUpper()];
			}
			else if (tdmMode == TDMMODE.CF_DB)
			{
				if (_tdmConfig.CFDBTableList.ContainsKey(tname.ToUpper())) t = _tdmConfig.CFDBTableList[tname.ToUpper()];
			}
			else if (tdmMode == TDMMODE.CF_DB_IR)
			{
				if (_tdmConfig.CFIRDBTableList.ContainsKey(tname.ToUpper())) t = _tdmConfig.CFIRDBTableList[tname.ToUpper()];
			}
			if (t != null)
				tname = t.TableName;
			return tname;
		}

		private bool ProcessTDMTable(string tableName, MigrType migrType)
		{
			bool status = false;
			bool validDBVersion = true;
			string msg = string.Empty;
			long nStep = 1, nTotal;

			ArrayList MigrSteps = _accessInterface.GetTDMTblData(tableName);
			if (_accessInterface.accessDBError.Length > 0)
			{
				LogWriter.LogIt(_accessInterface.accessDBError);
				return false;
			}
			nTotal = MigrSteps.Count;
			if (nTotal == 0)
				return true;

			_progressHandler.SetProgressRange(EnumPos.MIDDLE, nTotal);

			foreach (TdmTableStep migrStep in MigrSteps)
			{

				msg = "";
				if (migrType == MigrType.STANDARD)
				{
					//msg = (_tdmConfig.MasterDB ? "Primary" : "IR") + " Database: ";
					msg += _tdmConfig.CurrentBuildInfo.DestWceBuild.Replace(TDMEngineSettings.Default.StringToReplace, "") + ": ";
				}
				msg += " " + migrStep.Descr + "  ";
				if (migrStep.Tname.Length > 0)
					msg += migrStep.Tname;
				else
					msg += migrStep.Cmd_sql;

				msg += " (" + nStep + " of " + nTotal + ")";

				_sqlDBInterface.Notes = migrStep.Notes;
				_sqlDBInterface.TableName = migrStep.Tname;
				_sqlDBInterface.CMD = migrStep.Cmd_sql;

				_progressHandler.SetProgresMessage(EnumPos.MIDDLE, msg);

				LogWriter.LogIt(msg, true);
				// init the BOTTOM progress bar

				//_progressHandler.SetProgresMessage(EnumPos.BOTTOM, msg);
				//_progressHandler.SetProgressPosition(EnumPos.BOTTOM, 0);


				// always verify database version
				if (migrStep.Rem_sql.Length == 0)
				{

					if (migrType == MigrType.PRECMD || migrType == MigrType.POSTCMD)
					{
						// Set EDM path to DATADICT and datafiles
						_tdmConfig.EDMPath = _tdmConfig.TdmFilePath + "\\" + migrStep.EdmDir;
					}
					try
					{
						if (migrStep.Type == "LP")
						{
							// load all procedures
							status = _sqlDBInterface.LoadAllProcedures();
						}
						else if (migrStep.Type == "CP")
						{    // create procedure
							status = _sqlDBInterface.LoadProcedure(migrStep.Cmd_sql);
						}
						else if (migrStep.Type == "LPF")
						{   // load procedure folder
							status = _sqlDBInterface.LoadProcedureFolder(migrStep.Cmd_sql);
						}
						else if (migrStep.Type == "VDV")
						{
							// verify database version
							validDBVersion = _sqlDBInterface.VerifyDBVersion();

						}
						else if (migrStep.Type == "UDV")
						{   // update database version
							status = false;
							if (_tdmConfig.TotalErrors == 0)
								status = _sqlDBInterface.UpdateDBVersion();
							_sqlDBInterface.VerifyDBVersion();
						}
					}
					catch { return false; }

					if (IsCorrectDB(migrStep.Tname) || migrStep.Type == "DT")
					{
						string tName = GetTableNameInProperCase(migrStep.Tname, _tdmConfig.TDMMode);
						// Process command type
						status = ExecuteTDMCommand(migrStep.Type, tName, migrStep.Cmd_sql, validDBVersion);
					}
					else
					{
						msg = "Not executing the TDM command.";// +migrStep.Cmd_sql;
						LogWriter.LogIt(msg);
					}
				}
				else
				{
					msg = "";
					if (migrStep.Cmd_sql != "")
						msg = migrStep.Cmd_sql + "\r\n";
					msg += "This TDM command was skipped, clear the REM column to execute.\n";
					LogWriter.LogIt(msg);
				}
				nStep++; LogWriter.LogIt("");
			}
			return status;
		}

	
		private int InitializeTDMEngine()
		{
			bool status = true;
			// open tdm.mdb
			_accessInterface = new MSAccessInterface(_tdmConfig.TdmFilePath + "\\" + TDMEngineSettings.Default.TDMFileName);
			_tdmConfig.MSAccessInterface = _accessInterface;
			_errorMsg = _accessInterface.accessDBError;
			if (_errorMsg.Length > 0)
				return -1;

			// read TDMCTRL
			_tdmConfig.DestBuilds = _accessInterface.GetDestBuilds(_tdmConfig.CurrBuild, _tdmConfig.DestBuild);
			_errorMsg = _accessInterface.accessDBError;
			if (_accessInterface.accessDBError.Length > 0)
				return -1;


			//Get the count of total steps
			// if we are migrating both Master and Result databases,  double the count of total steps

			totalSteps = _accessInterface.GetTotalMigrStepsCount();
			if (_tdmConfig.IsMigrateResults())
				totalSteps *= 2;

			if (totalSteps == 0)
				return 1;

			CheckBCP();

	
			//Get tempFolder
			if (_tdmConfig.PrimaryDBTempPath.Length == 0)
			{

				_tdmConfig.PrimaryDBTempPath = GetWCeDBFolder() + TDMEngineSettings.Default.TDM_UnloadPath;
				_tdmConfig.IRDBTempPath = _tdmConfig.PrimaryDBTempPath;
				if (!Util.FolderExists(_tdmConfig.PrimaryDBTempPath))
				{
					Util.CreateFolder(_tdmConfig.PrimaryDBTempPath);
				}
				if (!Util.FolderExists(_tdmConfig.IRDBTempPath))
				{
					Util.CreateFolder(_tdmConfig.IRDBTempPath);
				}
			}

			return (status) ? 0 : -1;
		}

		private string GetWCeDBFolder()
		{
			string dbFolderPath = "";

			System.IO.DirectoryInfo d = new System.IO.FileInfo(_tdmConfig.PrimaryMDB).Directory;
			if (_tdmConfig.TDMMode == TDMMODE.SYS_DB || _tdmConfig.TDMMode == TDMMODE.COM_DB)
				dbFolderPath = d.Parent.FullName;
			else
				dbFolderPath = d.Parent.Parent.FullName;
			_tdmConfig.WCEFolder = dbFolderPath;
			return dbFolderPath;
		}

		private void CheckBCP()
		{
			SQLDBInterface sqlDBInterface;
			try
			{
				//Connect to master database to get tableNames
				sqlDBInterface = new SQLDBInterface(_tdmConfig);
				if (!sqlDBInterface.CheckBcp())
				{
					throw new TDMEngineException("The SQL Server BCP utility is not installed on this server.");

				}
			}
			catch (Exception ex)
			{
				LogWriter.LogIt("Unable to Connect to Master Database." + "\n" + ex.Message);
				throw new TDMEngineException(ex.Message);

			}
		}

		private bool LoadTableLocations()
		{
			SQLDBInterface sqlDBInterface;
			try
			{
				//Connect to master database to get tableNames
				sqlDBInterface = new SQLDBInterface(_tdmConfig);
			}
			catch (Exception ex)
			{
				throw new TDMEngineException("Unable to Connect to Master Database." + "\n" + ex.Message);
			}

			_tdmConfig.SysDBTableList = sqlDBInterface.GetTableList(TDMMODE.SYS_DB);
			_tdmConfig.ComDBTableList = sqlDBInterface.GetTableList(TDMMODE.COM_DB);
			_tdmConfig.CFDBTableList = sqlDBInterface.GetTableList(TDMMODE.CF_DB);
			_tdmConfig.CFIRDBTableList = sqlDBInterface.GetTableList(TDMMODE.CF_DB_IR);
			_tdmConfig.SysLookupTableList = sqlDBInterface.GetLookupTableList();
			return true;
		}

		private void MigrCleanUpTasks()
		{
			Util.CleanUpFolder(_tdmConfig.PrimaryDBTempPath);
			Util.DeleteFolder(_tdmConfig.WCEFolder + TDMEngineSettings.Default.TDM_UnloadPath);
		}

		private int DoPrePostCmd(string tablename, MigrType mode)
		{

			string msg = string.Empty;
			msg = (mode == MigrType.PRECMD) ? "Pre-Migration" : "Post-Migration";
			LogWriter.LogIt("\r\n===============" + msg + "===============\r\n");

			StartMigrationProcess(tablename, mode);

			return 0;
		}

		private bool LoadDataDictionaryTables(string dataDictXlsxPath)
		{
			bool status = true;
			DataDictXlsxReader dataDictReader = new DataDictXlsxReader(dataDictXlsxPath);
			_tdmConfig.DataDictTables = dataDictReader.ReadTableFromXlsx(_tdmConfig.PrimaryDBTempPath);

			foreach (string tbl in dataDictReader._dataDictTableTabNames)
			{
				_progressHandler.SetProgresMessage(EnumPos.MIDDLE, "Load Table " + tbl);
				string tname = GetTableNameInProperCase(tbl, TDMMODE.SYS_DB);
				status = _sqlDBInterface.LoadDataDictTable(tname);
				if (!status)
					return false;

			}
			return true;

		}

		private int DoStandard()
		{

			int bReturn = 0;
			bool status = true;
			int nStep = 1;
			string msg = string.Empty;

			msg = "Standard Migration ";
			LogWriter.LogIt("\r\n===============" + msg + "===============\r\n");
			//Loop TDMTBLs until we've reached the destination Build
			try
			{
				foreach (TDMDestBuilds build in _tdmConfig.DestBuilds.Values)
				{
					msg = "Migrating from " + build.SourceWceBuild + " to " + build.DestWceBuild + "(Build " + nStep + " of " + totalSteps + " )";
					_progressHandler.SetProgresMessage(EnumPos.TOP, msg);

					_tdmConfig.CurrentBuildInfo = build;

					_tdmConfig.EDMPath = _tdmConfig.TdmFilePath + "\\" + build.EDMDir;


					status = StartMigrationProcess(build.TableName, MigrType.STANDARD);
					if (!status)
						bReturn = -1;
				}

			}
			catch (Exception e)
			{
				bReturn = -1;
				LogWriter.LogIt(e.Message);

			}
			return bReturn;
		}

		private bool StartMigrationProcess(string table, MigrType migrType)
		{
			// Migrate Master database
			_tdmConfig.MasterDB = true;

			//Connect to master database
			try
			{
				_sqlDBInterface = new SQLDBInterface(_tdmConfig);
			}
			catch (Exception ex)
			{
				LogWriter.LogIt("Unable to Connect to master Database." + "\n" + ex.Message);
				return false;
			}
		

			if (migrType == MigrType.STANDARD)
			{
				string dataDictFile = _tdmConfig.EDMPath + "\\" + TDMEngineSettings.Default.DataDictFileName;

				//Read data dictionary
				if (_tdmConfig.TDMMode == TDMMODE.SYS_DB)
				{
					try
					{

						if (!LoadDataDictionaryTables(dataDictFile))
							return false;


					}
					catch (Exception e)
					{
						_sqlDBInterface.LogErrorMsg("Error reading Data Dict tables.\n" + e.Message);
						return false;
					}
				}

			}
			LoadTableLocations();

			// process TDMTBL
			ProcessTDMTable(table, migrType);

			//Close connection
			_sqlDBInterface.StopDatabase();

			// Migrate Result database only if we need to//
			if (_tdmConfig.IsMigrateResults())
			{
				LogWriter.LogIt("\r\n=============== IR Database ===============\r\n");
				_tdmConfig.MasterDB = false;
				//Connect to results database
				try
				{
					_sqlDBInterface = new SQLDBInterface(_tdmConfig);
				}
				catch (Exception ex)
				{
					LogWriter.LogIt("Unable to Connect to results Database." + "\n" + ex.Message);
					return false;
				}

				// process TDMTBL
				ProcessTDMTable(table, migrType);

				//Close connection
				_sqlDBInterface.StopDatabase();

			}


			return true;
		}

		#endregion
	}

	class TableMap
	{

		string _tableName;
		string _loadTbl;

		public string TableName
		{
			get { return _tableName; }
			set { _tableName = value; }
		}
		public string LoadTbl
		{
			get { return _loadTbl; }
			set { _loadTbl = value; }
		}

		#region constructor
		public TableMap(string tableName, string loadTbl)
		{
			_tableName = tableName;
			_loadTbl = loadTbl;
		}
		public TableMap() { }
		#endregion
	}
}
