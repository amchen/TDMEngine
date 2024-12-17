using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCeFramework.WCeDBInterface;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace TDMEngine
{
	class SQLDBInterface
	{
		#region private data members
		SQLSrvrDataImportInterface _sqlDBInterface;
		TDMConfig _tdmConfig;
		string _tableName = "";
		string _notes = "";
		string _cmd = "";
		string _JobName = "";
		#endregion

		public string Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}
		public string TableName
		{
			get { return _tableName; }
			set { _tableName = value; }
		}
		public string CMD
		{
			get { return _cmd; }
			set { _cmd = value; }
		}
		#region private methods
		private string OpenConnection()
		{
			SqlConnectionStringBuilder connStr = new SqlConnectionStringBuilder();
			if (_tdmConfig.MasterDB)
			{
				connStr.DataSource = _tdmConfig.PrimaryServerName;
				connStr.InitialCatalog = _tdmConfig.PrimaryDBName;
			}
			else
			{
				connStr.DataSource = _tdmConfig.IRServerName;
				connStr.InitialCatalog = _tdmConfig.IRDBName;
			}

			if (!_tdmConfig.IsTrustedCon)
			{
				connStr.UserID = _tdmConfig.UserName;
				connStr.Password = _tdmConfig.Password;
			}
			else
				connStr.IntegratedSecurity = true;
			return connStr.ConnectionString;
		}

		private string GetFolderPath(string rootPath, string procname)
		{
			try
			{
				foreach (string d in Directory.GetDirectories(rootPath))
				{
					foreach (string f in Directory.GetFiles(d, procname + ".sql"))
					{
						return f;
					}
					GetFolderPath(d, procname);
				}
			}
			catch
			{

			}
			return "";
		}

		public void LogErrorMsg(string errorMsg)
		{
			LogWriter.LogError(errorMsg);
			_tdmConfig.TotalErrors++;
		}

		private bool ExecuteSQL(string sql)
		{
			return _sqlDBInterface.ExecuteSql(sql);
		}

		private void AddToList(string name, Dictionary<string, TableMap> TableList)
		{
			TableMap tmap = new TableMap();
			tmap.TableName = name;
			tmap.LoadTbl = "Y";
			TableList.Add(tmap.TableName, tmap);
		}

		private string GetDatafilePath(string tableName)
		{
			string dataFilePath = string.Empty;
			if (Enum.GetNames(typeof(DataDictTabNames)).Contains(tableName.ToUpper()))
				dataFilePath = _tdmConfig.PrimaryDBTempPath;
			else
			{
				string qry = String.Format(TDMEngineSettings.Default.SQL_GetTableGroup, new string[] { tableName });
				DataTable dt = _sqlDBInterface.ExecuteSql(qry, null);
				if (dt.Rows.Count > 0)
				{
					dataFilePath = Path.Combine(_tdmConfig.EDMPath, dt.Rows[0][0].ToString());
				}
			}
			return dataFilePath;
		}

		private bool TableToBeLoaded(string tableName, bool sysDbTable)
		{
			bool status = false;
			if (_tdmConfig.IsDictionaryTbl(tableName)) return true;

			//Check if it needs to be loaded
			if (sysDbTable && _tdmConfig.SysDBTableList.ContainsKey(tableName.ToUpper()))
			{
				TableMap t = _tdmConfig.SysDBTableList[tableName.ToUpper()];
				if (t.LoadTbl == "L")
					status = true;

			}
			else if (!sysDbTable)
			{
				//COM DB table which does not exist
				if (_tdmConfig.ComDBTableList.ContainsKey(tableName.ToUpper()))
				{
					TableMap t = _tdmConfig.ComDBTableList[tableName.ToUpper()];
					if (t.LoadTbl == "L")
						status = true;
				}
			}

			return status;
		}

		private string GetTempFolder()
		{
			string s = (_tdmConfig.MasterDB ? _tdmConfig.PrimaryDBTempPath : _tdmConfig.IRDBTempPath);
			return s;
			//return (_tdmConfig.MasterDB ? _tdmConfig.: _tdmConfig->m_csResultTempPath);
		}

		private bool CreateLookupTable(string tableName, string lkupTblName, bool dropfirst, bool createIndex)
		{
			bool status = true;
			string tableScript = string.Empty;

			//Generate Create tableScript
			tableScript = _sqlDBInterface.GetCreateTableScript(tableName, lkupTblName);
			if (createIndex)
				tableScript += ";" + _sqlDBInterface.GetCreateTableIndexScript(tableName);

			//Drop table first
			if (dropfirst)
				status = DropTable(lkupTblName);

			if (status)
			{
				if (!TableExists(lkupTblName))
					status = _sqlDBInterface.ExecuteSql(tableScript);
				else
					status = false;
			}
			return status;
		}

		private bool CreateTable(string tableName, bool dropfirst, bool createIndex)
		{
			bool status = true;
			//Generate Create tableScript
			string tableScript = string.Empty;
			if (_tdmConfig.IsDictionaryTbl(tableName))
			{
				TableCreator t = new TableCreator(_tdmConfig);
				tableScript = t.CreateTableScript(tableName, tableName);
			}
			else
			{
				tableScript = _sqlDBInterface.GetCreateTableScript(tableName);
				if (createIndex)
					tableScript += ";" + _sqlDBInterface.GetCreateTableIndexScript(tableName);
			}
			//Drop table first
			if (dropfirst)
				status = DropTable(tableName);
			if (status)
			{
				if (!TableExists(tableName))
					status = _sqlDBInterface.ExecuteSql(tableScript);
				else
					status = false;
			}
			return status;
		}

		private bool TableExists(string tableName)
		{
			//Check if table exists//
			string sSQL = string.Format(TDMEngineSettings.Default.SQL_CheckIfTableExists, new string[] { tableName });

			DataTable dt = _sqlDBInterface.ExecuteSql(sSQL, null);
			if (dt.Rows.Count == 0)
			{
				return false;
			}
			else
				return true;
		}

		private string ReplaceVariables(string sql)
		{

			sql = sql.Replace("@ckptName", _tdmConfig.SnapshotName);
			sql = sql.Replace("@ckptUser", _tdmConfig.SnapshotUser);
			sql = sql.Replace("@ckptDescript", _tdmConfig.SnapshotDesc);
			sql = sql.Replace("@ckptType", _tdmConfig.SnapshotType.ToString());

			sql = sql.Replace("@MasterDSN", _tdmConfig.PrimaryConStr);
			sql = sql.Replace("@MasterDBN", _tdmConfig.PrimaryDBName);
			if (_tdmConfig.IsMigrateResults())
			{
				sql = sql.Replace("@ResultDSN", _tdmConfig.IRConStr);
				sql = sql.Replace("@ResultDBN", _tdmConfig.IRDBName);
			}
			sql = sql.Replace("@UserID", _tdmConfig.UserName);
			sql = sql.Replace("@Password", _tdmConfig.Password);

			//csSQL.Replace("@TDMUtil", m_tdmConfig->m_csAppDirPath + "\\TDMUtil");
			sql = sql.Replace("@AnalysisFolder", _tdmConfig.AnalysisPath);

			sql = sql.Replace("@MasterOnly", _tdmConfig.IsMigrateResults() ? "0" : "1");

			sql = sql.Replace("@TDMLogFile", LogWriter.GetLogFileName());
			sql = sql.Replace("@MSGLogFile", TDMEngineSettings.Default.TDM_MsgLogFile);

			//// set paths
			sql = sql.Replace("@TempPath", GetTempFolder());
			sql = sql.Replace("@MasterTempPath", _tdmConfig.PrimaryDBTempPath);
			sql = sql.Replace("@ResultTempPath", _tdmConfig.IRDBTempPath);
			sql = sql.Replace("@EDMPath", _tdmConfig.EDMPath);

			sql = sql.Replace("@WCeDBFolder", _tdmConfig.WCEFolder);

			sql = sql.Replace("@NOTES", _notes);
			sql = sql.Replace("@TNAME", _tableName);
			sql = sql.Replace("@CMD", _cmd);

			return sql;
		}

		#endregion

		#region public methods
		public SQLDBInterface(TDMConfig tdmConfig)
		{
			_tdmConfig = tdmConfig;
			_sqlDBInterface = new SQLSrvrDataImportInterface(OpenConnection(), LogWriter.GetLogFileName());
		}

		public void StopDatabase()
		{
			_sqlDBInterface.StopDatabase();
		}

		public void Cancel()
		{
			_sqlDBInterface.StopDatabase();
		}

		public bool LoadProcedure(string procName)
		{
			bool success = false;
			string folderPath = _tdmConfig.EDMPath + "\\" + TDMEngineSettings.Default.StoredProcPath;

			//Search the pocedure path
			if (GetFolderPath(folderPath, procName) != "")
				success = _sqlDBInterface.LoadScriptFile(GetFolderPath(folderPath, procName));
			if (success)
				LogWriter.LogIt("Successfully loaded procedures  " + procName);
			else
				LogErrorMsg("Failed to load procedures  " + procName);
			return success;
		}

		public bool AlterTable(string tableName, string cmd)
		{
			string sSQL = String.Format(TDMEngineSettings.Default.SQL_AlterTable, new string[] { tableName, cmd });
			bool success = ExecuteSQL(sSQL);
			if (success)

				LogWriter.LogIt("Successfully altered table " + tableName);
			else
				LogErrorMsg("Failed to alter table " + tableName);
			return success;
		}

		public bool DropProcedure(string procedure)
		{
			string sSQL = String.Format(TDMEngineSettings.Default.SQL_DropProc, new string[] { procedure });
			bool success = ExecuteSQL(sSQL);
			if (success)
				LogWriter.LogIt("Successfully dropped procedure " + procedure + "\r\n");
			else
				LogErrorMsg("Failed to drop procedure " + procedure + "\r\n");
			return success;
		}

		public Dictionary<string, TableMap> GetTableList(TDMMODE tdmMode)
		{
			Dictionary<string, TableMap> TableList = new Dictionary<string, TableMap>();

			string dbType = "";
			if (tdmMode == TDMMODE.SYS_DB)
			{
				dbType = "SYS_DB";
				AddToList("ALLDB", TableList);
				AddToList("SYSTEMDB", TableList);
			}
			else if (tdmMode == TDMMODE.COM_DB)
			{
				dbType = "COM_DB";
				AddToList("ALLDB", TableList);
				AddToList("COMMONDB", TableList);
			}
			else if (tdmMode == TDMMODE.CF_DB)
			{
				dbType = "CF_DB";
				AddToList("ALLDB", TableList);
				AddToList("USERDB", TableList);
				AddToList("MASTERONLY", TableList);
				AddToList("BOTH", TableList);
				AddToList("BOTH_DB", TableList);
			}
			else if (tdmMode == TDMMODE.CF_DB_IR)
			{
				dbType = "CF_DB_IR";
				AddToList("ALLDB", TableList);
				AddToList("USERDB", TableList);
				AddToList("USERDBIR", TableList);
				AddToList("RESULTONLY", TableList);
				AddToList("RESULTSONLY", TableList);
				AddToList("BOTH_DB", TableList);
			}

			try
			{
				string s = String.Format(TDMEngineSettings.Default.SQL_GetAllTableNames, new string[] { dbType });
				DataTable dt = _sqlDBInterface.ExecuteSql(s, null);
				if (dt != null)
				{
					if (dt.Rows.Count > 0)
					{
						foreach (DataRow dataRow in dt.Rows)
						{
							TableMap tmap = new TableMap();
							tmap.TableName = dataRow[0].ToString().Trim();
							tmap.LoadTbl = dataRow[1].ToString().ToUpper().Trim();
							TableList.Add(tmap.TableName.ToUpper(), tmap);
						}
					}
				}
			}
			catch (Exception e)
			{
				LogWriter.LogError(e.Message);
			}
			return TableList;
		}

		public bool ExecSQL(string sql)
		{
			sql = ReplaceVariables(sql);
			bool success = ExecuteSQL(sql);
			LogWriter.LogIt(sql);
			if (!success)

				//else
				LogErrorMsg("Failed to execute SQL statement:\r\n" + sql);
			return success;
		}

		public bool ExecSQLWithoutLog(string sql)
		{
			bool success = ExecuteSQL(sql);
			if (!success)

				//else
				LogErrorMsg("Failed to execute SQL statement:\r\n" + sql);
			return success;
		}

		public bool DropTable(string tableName, bool logMessage = false)
		{
			string sSQL = String.Format(TDMEngineSettings.Default.SQL_DropTable, new string[] { tableName });
			bool success = ExecuteSQL(sSQL);
			if (logMessage)
			{
				if (success)
					LogWriter.LogIt("Successfully dropped table " + tableName + "\r\n");
				else
					LogErrorMsg("Failed to insert data into table " + tableName + "\r\n");
			}
			return success;
		}

		public bool DatabaseMessage(string msg)
		{
			string sSQL = String.Format(TDMEngineSettings.Default.SQL_DBMsg, new string[] { msg });
			return ExecuteSQL(sSQL);
		}

		public bool DropIndex(string tableName, string indexName)
		{
			bool success = false;
			if (indexName.Trim().Length == 0)
				LogErrorMsg("Index name is not provided");
			else if (tableName.Trim().Length == 0)
				LogErrorMsg("Table name is not provided");
			else
			{
				string sSQL = String.Format(TDMEngineSettings.Default.SQL_DropIndex, new string[] { tableName, indexName });
				success = _sqlDBInterface.ExecuteSql(sSQL);
			}
			if (success)
				LogWriter.LogIt("Successfully created index on " + tableName + "\r\n");
			else
				LogErrorMsg("Failed to create index on " + tableName + "\r\n");

			return success;
		}

		public bool InsertTable(string tableName)
		{
			bool success = false;
			// Load table without dropping it first
			LogWriter.LogIt("Insert data into table " + tableName);

			success = LoadTable(tableName, false);

			if (!success)
				LogErrorMsg("Failed to insert data into table " + tableName);

			return success;
		}

		public bool CreateTable(string tableName, bool createindex)
		{
			bool success = false;
			success = CreateTable(tableName, true, createindex);

			if (success)
			{
				LogWriter.LogIt("Successfully created table " + tableName + "\n");
			}
			else
			{
				LogErrorMsg("Failed to create table " + tableName);
			}

			return success;
		}

		public bool LoadIndex(string tableName, string indexName)
		{
			bool success = CreateIndex(tableName, indexName);

			if (success)
				LogWriter.LogIt("Successfully created index " + indexName + " on table " + tableName + "\n");
			else
				LogErrorMsg("Failed to create index " + indexName + " on table " + tableName);

			return success;
		}

		public bool LoadIndex(string tableName)
		{
			bool success = CreateIndex(tableName);

			if (success)
				LogWriter.LogIt("Successfully loaded indexes on table " + tableName + "\n");
			else
				LogErrorMsg("Failed to load index on table " + tableName);

			return success;
		}

		public bool CreateIndex(string tableName)
		{
			bool success = _sqlDBInterface.DropAllTableIndex(tableName);
			if (success)
			{
				return _sqlDBInterface.CreateAllTableIndex(tableName);
			}
			return false;
		}

		public bool CreateIndex(string tableName, string indexName)
		{
			string sql = _sqlDBInterface.GetCreateTableIndexScript(tableName, tableName);
			return _sqlDBInterface.ExecuteSql(sql);
		}

		public bool LoadProcedureFolder(string procedureFolder)
		{
			bool success = false;
			
			string folderPath = _tdmConfig.EDMPath + "\\" + TDMEngineSettings.Default.StoredProcPath + "\\" + procedureFolder;
			if (Directory.Exists(folderPath))
				success = LoadAllSQLFilesFromFolder(folderPath, "abs*.sql");
			else
			{
				LogWriter.LogIt(folderPath + " does not exist!!");
				success = false;
			}
			if (success)

				LogWriter.LogIt("Successfully loaded all Procedures from " + procedureFolder);
			else
				LogErrorMsg("Failed to load all Procedures from " + procedureFolder);

			
			return success;
		}


		public bool LoadAllProcedures()
		{
			bool success = true;
			bool status = true;
			string dbName = _sqlDBInterface.GetDatabaseName();

			string folderPath = _tdmConfig.EDMPath + "\\" + TDMEngineSettings.Default.StoredProcPath;

			if (_tdmConfig.TDMMode == TDMMODE.SYS_DB)
			{
				folderPath = folderPath + "\\" + "_SystemDB";
				success = LoadAllSQLFilesFromFolder(folderPath, "abs*.sql");

			}
			else if (_tdmConfig.TDMMode == TDMMODE.COM_DB)
			{
				folderPath = folderPath + "\\" + "_CommonDB";
				success = LoadAllSQLFilesFromFolder(folderPath, "abs*.sql");
			}

			else
			{
				if (Directory.Exists(folderPath))
				{
					foreach (string d in Directory.GetDirectories(folderPath))
					{
						status = LoadAllSQLFilesFromFolder(d, "abs*.sql");
						if (!status)
							success = false;
					}

				}
				else
				{
					LogWriter.LogIt(folderPath + " does not exist!!");
					success = false;
				}
			}
			if (success)

				LogWriter.LogIt("Successfully loaded all Procedures from " + folderPath);
			else
				LogErrorMsg("Failed to load Procedures");

			
			//Some procedures change the database//
			if (_sqlDBInterface.GetDatabaseName() != dbName)
				_sqlDBInterface.ChangeDatabase(dbName);

			return success;
		}

		public bool VerifyDBVersion()
		{
			bool success = false;
			string DB_NAME, DB_SCHEMA, WCEVERSION, EQEVERSION, BUILD, UPDATED_ON, SCRIPTUSED;
			String qry = TDMEngineSettings.Default.SQL_GetDBVersion;

			DataTable dt = _sqlDBInterface.ExecuteSql(TDMEngineSettings.Default.SQL_GetDBVersion, null);

			if (dt != null)
			{
				foreach (DataRow datarow in dt.Rows)
				{

					DB_NAME = datarow["DB_NAME"] != System.DBNull.Value ? (string)datarow["DB_NAME"] : "";
					DB_SCHEMA = datarow["DB_SCHEMA"] != System.DBNull.Value ? (string)datarow["DB_SCHEMA"] : "";
					WCEVERSION = datarow["WCEVERSION"] != System.DBNull.Value ? (string)datarow["WCEVERSION"] : "";
					EQEVERSION = datarow["EQEVERSION"] != System.DBNull.Value ? (string)datarow["EQEVERSION"] : "";
					BUILD = datarow["BUILD"] != System.DBNull.Value ? (string)datarow["BUILD"] : "";
					UPDATED_ON = datarow["UPDATED_ON"] != System.DBNull.Value ? (string)datarow["UPDATED_ON"] : "";
					SCRIPTUSED = datarow["SCRIPTUSED"] != System.DBNull.Value ? (string)datarow["SCRIPTUSED"] : "";

					LogWriter.LogIt("Current Database Version Information\r\n====================================");
					LogWriter.LogIt("DB_NAME      = " + DB_NAME);
					LogWriter.LogIt("DB_SCHEMA    = " + DB_SCHEMA);
					LogWriter.LogIt("WCEVERSION   = " + WCEVERSION);
					LogWriter.LogIt("EQEVERSION   = " + EQEVERSION);
					LogWriter.LogIt("BUILD        = " + BUILD);
					LogWriter.LogIt("UPDATED_ON   = " + UPDATED_ON);
					LogWriter.LogIt("SCRIPTUSED   = " + SCRIPTUSED);

					string destBuild = _tdmConfig.CurrentBuildInfo.DestWceBuild;
					destBuild = destBuild.Replace(TDMEngineSettings.Default.StringToReplace, "");
					LogWriter.LogIt("\r\nThe current BUILD is " + BUILD + ", the destination BUILD is " + destBuild);


					long srcDBSchema = long.Parse(DB_SCHEMA);
					long destDBSchema = long.Parse(_tdmConfig.CurrentBuildInfo.DBSchema);
					if (srcDBSchema < destDBSchema)
					{
						LogWriter.LogIt("The current DB_SCHEMA " + DB_SCHEMA + " is less than or equal to " + _tdmConfig.DestBuild + ", migration will continue.");
						success = true;
					}

					// current DB_SCHEMA is newer than destination
					else if (srcDBSchema > destDBSchema)
					{
						LogWriter.LogIt("\r\nThe current " + DB_SCHEMA + " is greater than " + _tdmConfig.DestBuild + ", no migration required at this time.");
					}
					// current DB_SCHEMA is equal to destination, update VERSION
					else
					{
						LogWriter.LogIt("\r\nThe current DB_SCHEMA is equal to the destination DB_SCHEMA " + _tdmConfig.DestBuild);
						UpdateDBVersion();
					}
				}
			}

			return success;
		}

		public bool UpdateDBVersion()
		{
			bool success = false;
			string dbSchema = _tdmConfig.CurrentBuildInfo.DBSchema;
			string arcSchema = _tdmConfig.CurrentBuildInfo.ArcSchema;
			string destWCE = _tdmConfig.CurrentBuildInfo.DestWceVersion;
			string eqeVer = _tdmConfig.CurrentBuildInfo.EqeVer;
			string destBuild = _tdmConfig.CurrentBuildInfo.DestWceBuild;
			string desc = _tdmConfig.CurrentBuildInfo.Desc;
			string fl_certver = _tdmConfig.CurrentBuildInfo.FlCertVer;

			string sSQL = String.Format(TDMEngineSettings.Default.SQL_SetDBVersion, new string[] { dbSchema, arcSchema, destWCE, eqeVer, destBuild, desc, fl_certver });
			LogWriter.LogIt("Update Database Version\r\n");
			success = ExecuteSQL(sSQL);

			if (!success)
				LogErrorMsg("Failed to update Version table\r\n");

			return success;
		}

		public bool CheckBcp()
		{
			bool flag = true;
			try
			{
				Process bcp = new Process();

				bcp.StartInfo.CreateNoWindow = false;
				bcp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				bcp.StartInfo.FileName = TDMEngineSettings.Default.BCP_APP_NAME;

				bcp.Start();
				bcp.WaitForExit();
				bcp.Close();

				bcp = null;
				flag = true;
			}
			catch (Exception ex)
			{
				flag = false;
			}
			if (!flag)
			{
				flag = false;
			}
			return flag;
		}

		public Dictionary<string, TableMap> GetLookupTableList()
		{
			Dictionary<string, TableMap> TableList = new Dictionary<string, TableMap>();
			try
			{
				string s = String.Format(TDMEngineSettings.Default.SQL_GetSystemLookup, new string[] { });
				DataTable dt = _sqlDBInterface.ExecuteSql(s, null);
				if (dt != null)
				{
					if (dt.Rows.Count > 0)
					{
						foreach (DataRow dataRow in dt.Rows)
						{
							TableMap tmap = new TableMap();
							tmap.TableName = dataRow[0].ToString().Trim();
							tmap.LoadTbl = dataRow[1].ToString().ToUpper().Trim();
							TableList.Add(tmap.TableName.ToUpper(), tmap);
						}
					}
				}
			}
			catch (Exception e)
			{
				LogWriter.LogError(e.Message);
			}
			return TableList;
		}

		public bool LoadDataDictTable(string tableName)
		{
			//Load data dictionary tables from datatable directly
			bool bReturn = CreateTable(tableName, true, false);//indexes will be created after bcp
			LogWriter.LogIt("Load table " + tableName);
			if (!bReturn) return false;
			return DataDictInsert(tableName);

		}
		private int GetDataTableIndex(string tablename)
		{
			int i = 0;
			if (tablename.ToUpper() == DataDictTabNames.DICTTBL.ToString().ToUpper())
				i = (int)DataDictTabNames.DICTTBL;
			else if (tablename.ToUpper() == DataDictTabNames.DICTCOL.ToString().ToUpper())
				i = (int)DataDictTabNames.DICTCOL;
			else if (tablename.ToUpper() == DataDictTabNames.DICTIDX.ToString().ToUpper())
				i = (int)DataDictTabNames.DICTIDX;
			else if (tablename.ToUpper() == DataDictTabNames.DICTCLON.ToString().ToUpper())
				i = (int)DataDictTabNames.DICTCLON;
			else if (tablename.ToUpper() == DataDictTabNames.DICTCNST.ToString().ToUpper())
				i = (int)DataDictTabNames.DICTCNST;
			return i;

		}
		private bool HasIdentityColumn(string tableName)
		{
			bool hasIdentity = false;
			DataTable dictCol = _tdmConfig.DataDictTables[(int)DataDictTabNames.DICTCOL];
			DataRow[] rows = dictCol.Select("TableName='" + tableName + "'");
			foreach (DataRow row in rows)
			{
				if (row["FieldType"].ToString().ToUpper() == "A")
				{
					hasIdentity = true;
					break;
				}
			}
			return hasIdentity;
		}

		private bool DataDictInsert(string tableName)
		{
			string sql = string.Empty;
			bool status = true;
			DataTable currentTableData = _tdmConfig.DataDictTables[GetDataTableIndex(tableName)];
			bool isIdentitySet = false;

			//See if identity column is involved
			if (HasIdentityColumn(tableName))
			{
				//Set identity insert on
				sql = "set identity_insert EqecatSystem.dbo." + tableName.Trim () + " on";
				status = _sqlDBInterface.ExecuteSql(sql);

				isIdentitySet = true;
				if (!status) return false;

			}

			//See if we have a valid table to work with
			if (currentTableData == null || currentTableData.Rows.Count == 0)
				LogWriter.LogIt("Please verify that input table " + tableName  + " is present and not empty!!!");

			sql = string.Empty;
			List<string> queryList = CreateInsertQueryListForTable(tableName);

			int BATCH_SIZE = 100;
			int rowCount = 0;
			foreach (string query in queryList)
			{
				++rowCount;
				sql += query + "\r\n";
				if (rowCount % BATCH_SIZE == 0)
				{
					status = _sqlDBInterface.ExecuteSql(sql);

					sql = string.Empty;

					if (!status)
						break;
				}
			}

			if (!string.IsNullOrEmpty(sql) && (status))
				status = _sqlDBInterface.ExecuteSql(sql);


			if (isIdentitySet)
			{
				sql = "set identity_insert EqecatSystem.dbo." + tableName.Trim() + " off";
				status = _sqlDBInterface.ExecuteSql(sql);
			}

			return status;
		}

		private string GetHeaderStringForCurrentTable(string tablename)
		{
			string headerString = string.Empty;
			DataTable dictCol = _tdmConfig.DataDictTables[(int)DataDictTabNames.DICTCOL];
			IOrderedEnumerable<DataRow> rowsForCurrentTable = dictCol.Select("TABLENAME ='" + tablename + "'").OrderBy(n => Convert.ToInt32(n["FIELDNUM"]));
			foreach (DataRow row in rowsForCurrentTable)
			{
				if (!string.IsNullOrEmpty(headerString))
					headerString += ",";

				headerString += row["FIELDNAME"].ToString();
			}

			return headerString;
		}

		private string GetDataTypeForColumn(string tableName, int columnIndex)
		{
			DataTable dictCol = _tdmConfig.DataDictTables[(int)DataDictTabNames.DICTCOL];
			DataRow[] rowsForCurrentTable = dictCol.Select("TABLENAME ='" + tableName + "'");

			return rowsForCurrentTable[columnIndex]["FIELDTYPE"].ToString();
		}

		private bool IsCharacterDataColumn(string tableName, int columnIndex)
		{
			string dataType = GetDataTypeForColumn(tableName, columnIndex);

			return (dataType.ToUpper() == "S" || dataType.ToUpper() == "C" || dataType.ToUpper() == "V");
		}

		private List<string> CreateInsertQueryListForTable(string tableName)
		{
			List<string> queryList = new List<string>();

			//Examine DictCol to generate Header Information			
			string headerString = GetHeaderStringForCurrentTable(tableName);

			foreach (DataRow row in _tdmConfig.DataDictTables[GetDataTableIndex(tableName)].Rows)
			{
				int colIndex = 0;
				string fieldValues = string.Empty;
				foreach (DataColumn col in _tdmConfig.DataDictTables[GetDataTableIndex(tableName)].Columns)
				{
					if (!string.IsNullOrEmpty(fieldValues))
						fieldValues += ",";

					if (IsCharacterDataColumn(tableName, colIndex))
						fieldValues += "'" + row[col].ToString().Replace("'", "''") + "'";
					else
						fieldValues += row[col].ToString();

					++colIndex;
				}

				//Finally compose the whole thing
				string queryString = "Insert Into " + tableName + "(" + headerString + ") Values (" + fieldValues + ");";

				queryList.Add(queryString);
			}

			return queryList;
		}

		public bool LoadLookupTable(string tableName, string lookuptableName, string dataFilePath, string delimiter, string ext)
		{
			bool bSuccess = true;
			string dataFile  = dataFilePath + "\\" + tableName + ext;
			 
			bSuccess = _sqlDBInterface.LoadTable(lookuptableName, dataFile, true, false, ext, delimiter);

			//Delete log
			if (bSuccess)
			{
				if (File.Exists(Path.Combine(dataFilePath, tableName + ".log")))
					File.Delete(Path.Combine(dataFilePath, tableName + ".log"));
			}
			return bSuccess;
		}
		public bool LoadTable(string tableName, string dataFilePath, string delimiter, string ext)
		{
			bool bSuccess = true;

			if (!_tdmConfig.IsDictionaryTbl(tableName))
				//bSuccess = _sqlDBInterface.LoadDictTbl(tableName, dataFilePath, delimiter, true, false);
			//else
			//{
				bSuccess = _sqlDBInterface.LoadTable(tableName, dataFilePath, true, false, ext, delimiter);
			//}

			//Delete log
			if (bSuccess)
			{
				if (File.Exists(Path.Combine(dataFilePath, tableName + ".log")))
					File.Delete(Path.Combine(dataFilePath, tableName + ".log"));
			}
			return bSuccess;
		}

		public bool LoadTable(string tableName, bool dropfirst)
		{
			string datafile = string.Empty;
			bool bReturn = false;
			bool bLoadTable = false;
			string dataFilePath = string.Empty;
			string ext = string.Empty;
			bool createFlag = false;
			bool systemLookup = false;
			string lookupTableName = string.Empty;

			//Check if system lookup
			if (_tdmConfig.IsSystemLookupTable(tableName))
			{
				systemLookup = true;
				TableMap t = _tdmConfig.SysLookupTableList[tableName.ToUpper()];
				lookupTableName = t.LoadTbl;
			}

			// drop/create table if it is a System table
			//If a com_db table and table does not exists then create and load table.
			bool sysDBTable = false;
			if (!dropfirst) bLoadTable = true;//Incase of insert data

			if (_tdmConfig.IsCommonDBTable(tableName) && (!TableExists(tableName))) createFlag = true;
			if (_tdmConfig.IsSystemDBTable(tableName)) sysDBTable = createFlag = true;
			if (_tdmConfig.IsDictionaryTbl(tableName)) { sysDBTable = createFlag = true; }
			if (createFlag || bLoadTable)
			{
				if (createFlag)
				{

					if (TableToBeLoaded(tableName, sysDBTable))
					{
						bLoadTable = true;
						if (systemLookup)
							bReturn = CreateLookupTable(tableName, lookupTableName, dropfirst, false);
						else
							bReturn = CreateTable(tableName, dropfirst, false);//indexes will be created after bcp
						LogWriter.LogIt("Load table " + tableName);
					}
					else
					{   //do not load
						//create index
						bReturn = CreateTable(tableName, dropfirst, true);//indexes will be created after bcp
						LogWriter.LogIt("Load table skipped ");
						return bReturn;
					}
				}
				//Get datafile name
				if (bLoadTable)
				{
					dataFilePath = GetDatafilePath(tableName);
					//Can be a .bar or .txt file
					datafile = Util.TextFileExists(tableName, dataFilePath);
					ext = Path.GetExtension(datafile).ToLower();
					if (ext.ToLower() == ".zip")
					{
						datafile = Util.UnzipFile(tableName + ext, dataFilePath);
						ext = Path.GetExtension(datafile).ToLower();
						dataFilePath = Path.Combine(dataFilePath, datafile);

					}
				}

				if (datafile.Length > 0)
				{
					if (_tdmConfig.CheckDate)
					{
						LogWriter.LogIt("Check Datafile Date is TRUE");
						if (Util.GetDataFileDate(datafile) > _tdmConfig.CurrentBuildInfo.PrevDate)
						{
							// datafile is newer than the previous build date	// load datafile
							bLoadTable = true;
						}
						else  // datafile is older than the previous build date
						{
							LogWriter.LogIt("Load table not needed: " + tableName + " file " + datafile + " is older than the build " + _tdmConfig.CurrentBuildInfo.PrevDate);
							return false;
						}
					}
					else
					{
						LogWriter.LogIt("Check Datafile Date is FALSE\n");
						bLoadTable = true;
					}
				}
				else
				{
					// there is no datafile
					bReturn = CreateTable(tableName, true);
					bLoadTable = false;

				}
				if (bLoadTable)
				{
					string delimiter = Util.DetermineDelimiter(datafile);
					if (systemLookup)
						bReturn = LoadLookupTable(tableName,lookupTableName,dataFilePath, delimiter, ext);
					else
						bReturn = LoadTable(tableName, dataFilePath, delimiter, ext);

				}

			}
			else
			{
				//Not a system table

				string tempFolder = GetTempFolder();

				string sSQL = string.Format(TDMEngineSettings.Default.SQL_Migr_ReloadTable_PROC, new string[] { tableName, tempFolder, _tdmConfig.UserName, _tdmConfig.Password });
				LogWriter.LogIt("Load user table " + tableName + " via Stored Procedure:");
				//set_progress_message(BOTTOM, "Loading table " + csTable + " using SQL");
				bReturn = _sqlDBInterface.ExecuteSql(sSQL);

			}

			if (bReturn)
			{
				if (bLoadTable)
					LogWriter.LogIt("Successfully loaded table " + tableName + "\r\n");
			}
			else
				LogErrorMsg("Failed to load table " + tableName + "\r\n");


			return bReturn;
		}

		public bool LoadViews()
		{
			bool success = false;
			string folderPath = _tdmConfig.EDMPath + "\\" + TDMEngineSettings.Default.StoredProcPath + "\\Views";
			success = LoadAllSQLFilesFromFolder(folderPath, "absvw_*.sql");
			if (success)

				LogWriter.LogIt("Successfully loaded all Views from " + folderPath);
			else
				LogErrorMsg("Failed to load Views");
			return success;
		}

		public bool ConnectionStateOpen()
		{
			ConnectionState cs = _sqlDBInterface.ConnectionState;
			if (cs != ConnectionState.Open)
				return false;
			return true;
		}

		private bool LoadAllSQLFilesFromFolder(String folderPath, string pattern)
		{
			bool status = true, retVal = true;

			if (!Directory.Exists(folderPath))
			{
				LogWriter.LogIt("Cannot find path " + folderPath);
				return false;
			}
			try
			{
				foreach (string file in Directory.GetFiles(folderPath, pattern))
				{
					//Check if connection is closed
					if (!ConnectionStateOpen())
						return false;
					status = _sqlDBInterface.LoadScriptFile(file);
					if (!status)
					{
						LogWriter.LogIt("Failed to load " + file, false);
						retVal = false;
					}
					else
					{

						LogWriter.LogIt("Successfully loaded " + file, false);
					}
				}

			}
			catch
			{

			}
			
			return retVal;
		}

		public bool ExecSYS(string cmd) { return true; }

		public bool LoadTriggers()
		{
			bool success = false;
			string folderPath = _tdmConfig.EDMPath + "\\" + TDMEngineSettings.Default.StoredProcPath + "\\Triggers";
			success = LoadAllSQLFilesFromFolder(folderPath, "On_*.sql");
			if (success)

				LogWriter.LogIt("Successfully loaded all Triggers from " + folderPath);
			else
				LogErrorMsg("Failed to load triggers");
			return success;
		}

		public bool ToMDB(string cmd) { return true; }

		public bool FromMDB(string cmd) { return true; }

		public bool TDMOpen(string cmd) { return true; }

		public string ExecuteProcedureWithOutParam(string proc, string outParamName, string inputParamName = "", string inputParamVal = "")
		{
			LogWriter.LogIt("Executing procedure " + proc);
			string outParam = "";
			try
			{

				DBParams dbParams = new DBParams();
				dbParams.AddOutParam(outParamName);
				if (inputParamName.Length > 0)
					dbParams.AddString(inputParamName, inputParamVal);

				DataTable dt = _sqlDBInterface.ExecuteProcedure(proc, dbParams);
				int x = dt.Rows.Count;
				Dictionary<string, string> d = dbParams.GetOutParameterValues();
				outParam = d[outParamName];
				//LogWriter.LogIt(proc + " returned " + outParam);
			}
			catch
			{
				LogErrorMsg("Error executing " + proc);
				return "";
			}
			return outParam.Trim();
		}

		public void ThreadExec(string cmd)
		{

			cmd = ReplaceVariables(cmd);

			Thread tdmThread = new Thread(new ThreadStart(DoJob));
			tdmThread.Start();


		}


		private void DoJob()
		{
			try
			{
				string arguments = _JobName.Substring(_JobName.IndexOf(' '));
				Process.Start(_JobName, arguments);
			}
			catch
			{
				LogErrorMsg("Failed to start ThreadExec: " + _JobName + "\r\n");

			}

		}
		#endregion



	}
}
