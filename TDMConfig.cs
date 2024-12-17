using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace TDMEngine
{
	#region enum def
	public enum TDMMODE
	{
		STANDARD = 0,
		SYS_DB,
		COM_DB,
		CF_DB,
		CF_DB_IR,
	}
	public enum TDMOPTION
	{
		ALL = 0,
		MASTER_ONLY = 1,
		NUM_OPTION
	}
	#endregion

	class TDMConfig
	{
		#region private members
		string _primaryMDB = "";
		string _IRMDB = "";
		string _primaryDBFLocalPath = "";
		string _IRDBFLocalPath = "";
		string _primaryConStr = "";
		string _IRConStr = "";
		string _primaryDBName = "";
		string _IRDBName = "";
		bool _IsTrustedCon = false;
		string _tdmFilePath = "";
		string _currBuild = "";
		string _destBuild = "";
		string _currVersion = "";
		string _snapshotName = "";
		TDMMODE _tdmMode;
		TDMOPTION _tdmOption;
		bool _masterDB = true;
		string _userID = "";
		string _password = "";
		int _snapshotType = 0;
		string _snapshotUser = "";
		string _snapshotDesc = "";
		string _primaryServerName = "";
		string _IRServerName = "";
		string _EDMPath = "";
		string _analysisPath = "";
		string _PrimaryDBTempPath = "";
		string _IRDBTempPath = "";
		string _wceFolder = "";
		public MSAccessInterface MSAccessInterface = null;
		public Dictionary<String, TDMDestBuilds> DestBuilds;
		public Dictionary<string, TableMap> SysDBTableList;
		public Dictionary<string, TableMap> ComDBTableList;
		public Dictionary<string, TableMap> CFDBTableList;
		public Dictionary<string, TableMap> CFIRDBTableList;
		public Dictionary<string, TableMap> SysLookupTableList;
		private TDMDestBuilds _currentBuildInfo;
		private DataTable[] _dataDictTables;
		private long _totalErrors = 0;

		#endregion

		#region TDMConfig properties
		public string PrimaryMDB
		{
			get { return _primaryMDB; }
			set { _primaryMDB = value; }
		}
		public string IRMDB
		{
			get { return _IRMDB; }
			set { _IRMDB = value; }
		}
		public string PrimaryDBName
		{
			get { return _primaryDBName; }
			set { _primaryDBName = value; }
		}
		public string IRDBName
		{
			get { return _IRDBName; }
			set { _IRDBName = value; }
		}
		public string PrimaryServerName
		{
			get { return _primaryServerName; }
			set { _primaryServerName = value; }
		}
		public string IRServerName
		{
			get { return _IRServerName; }
			set { _IRServerName = value; }
		}

		public string CurrBuild
		{
			get { return _currBuild; }
			set { _currBuild = value; }
		}
		public string DestBuild
		{
			get { return _destBuild; }
			set { _destBuild = value; }
		}
		public string TdmFilePath
		{
			get { return _tdmFilePath; }
			set { _tdmFilePath = value; }
		}
		public string UserName
		{
			get { return _userID; }
			set { _userID = value; }
		}
		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}
		public TDMMODE TDMMode
		{
			get { return _tdmMode; }
			set { _tdmMode = value; }
		}
		public int SnapshotType
		{
			get { return _snapshotType; }
			set { _snapshotType = value; }
		}
		public string SnapshotName
		{
			get { return _snapshotName; }
			set { _snapshotName = value; }
		}
		public string SnapshotUser
		{
			get { return _snapshotUser; }
			set { _snapshotUser = value; }
		}
		public long TotalErrors
		{
			get { return _totalErrors; }
			set { _totalErrors = value; }
		}
		public string SnapshotDesc
		{
			get { return _snapshotDesc; }
			set { _snapshotDesc = value; }
		}
		public bool CheckDate
		{
			get { return TDMEngineSettings.Default.CheckDate; }
		}
		
		public TDMDestBuilds CurrentBuildInfo
		{
			get { return _currentBuildInfo; }
			set { _currentBuildInfo = value; }
		}
		public bool IsTrustedCon
		{
			get { return _IsTrustedCon; }
			set { _IsTrustedCon = value; }
		}
		public bool MasterDB
		{
			get { return _masterDB; }
			set { _masterDB = value; }
		}
		public string EDMPath
		{
			get { return _EDMPath; }
			set { _EDMPath = value; }
		}

		public string WCEFolder
		{
			get { return _wceFolder ; }
			set { _wceFolder = value; }
		}
		public string PrimaryDBTempPath
		{
			get { return _PrimaryDBTempPath; }
			set { _PrimaryDBTempPath = value; }
		}
		public string AnalysisPath
		{
			get { return _analysisPath; }
			set { _analysisPath = value; }
		}
		public string PrimaryConStr
		{
			get { return _primaryConStr; }
			set { _primaryConStr = value; }
		}
		public string IRConStr
		{
			get { return _IRConStr; }
			set { _IRConStr = value; }
		}
		public string IRDBTempPath
		{
			get { return _IRDBTempPath; }
			set { _IRDBTempPath = value; }
		}
		public DataTable[] DataDictTables
		{
			get { return _dataDictTables; }
			set { _dataDictTables = value; }
		}
		#endregion

		#region public methods
		public TDMConfig()
		{

		}

		public bool IsEqecatSystemDB()
		{
			return (_tdmMode == TDMMODE.SYS_DB);
		}

		public bool IsEqecatCommonDB()
		{
			return (_tdmMode == TDMMODE.COM_DB);
		}

		public bool IsEqecatUserDB()
		{
			return (_tdmMode == TDMMODE.CF_DB || _tdmMode == TDMMODE.CF_DB_IR);
		}

		public void PutString(string csString, char csDelimiter = '|')
		{
			string key, value;

			string[] options = csString.Split(csDelimiter);

			// parse the options
			foreach (string option in options)
			{
				key = option.Substring(0, option.IndexOf('='));
				value = option.Substring(option.IndexOf('=') + 1);

				if (key.Equals("MasterDBF", StringComparison.InvariantCultureIgnoreCase))
					_primaryMDB = value;
				else if (key.Equals("ResultDBF", StringComparison.InvariantCultureIgnoreCase))
					_IRMDB = value;
				else if (key.Equals("MasterDBFLocalPath", StringComparison.InvariantCultureIgnoreCase))
					_primaryDBFLocalPath = value;
				else if (key.Equals("ResultDBFLocalPath", StringComparison.InvariantCultureIgnoreCase))
					_IRDBFLocalPath = value;
				else if (key.Equals("MasterDSN", StringComparison.InvariantCultureIgnoreCase))
					_primaryConStr = value;
				else if (key.Equals("ResultDSN", StringComparison.InvariantCultureIgnoreCase))
					_IRConStr = value;
				else if (key.Equals("MasterDBN", StringComparison.InvariantCultureIgnoreCase))
					_primaryDBName = value;
				else if (key.Equals("ResultDBN", StringComparison.InvariantCultureIgnoreCase))
					_IRDBName = value;
				else if (key.Equals("MasterTempPath", StringComparison.InvariantCultureIgnoreCase))
					_PrimaryDBTempPath = value;
				else if (key.Equals("ResultTempPath", StringComparison.InvariantCultureIgnoreCase))
					_IRDBTempPath = value;
				else if (key.Equals("AnalysisPath", StringComparison.InvariantCultureIgnoreCase))
				{
					_analysisPath = value;
					SetTDMFilePath(value);
				}
				else if (key.Equals("CurrBuild", StringComparison.InvariantCultureIgnoreCase))
				{
					_currBuild = value;
					//_currVersion =  GetWCeVersion(m_csCurrBuild);
					_snapshotName = _snapshotName.Replace("@SnapshotName", _currVersion);
				}
				else if (key.Equals("DestBuild", StringComparison.InvariantCultureIgnoreCase))
					_destBuild = value;
				else if (key.Equals("TdmMode", StringComparison.InvariantCultureIgnoreCase))
					_tdmMode = (TDMMODE)Int32.Parse(value);
				else if (key.Equals("TDMOption", StringComparison.InvariantCultureIgnoreCase))
					_tdmOption = (TDMOPTION)Int32.Parse(value);
				else if (key.Equals("UserID", StringComparison.InvariantCultureIgnoreCase))
				{
					if (_userID == "")
						_IsTrustedCon = true;
					else
						_userID = value;
				}
				else if (key.Equals("Password", StringComparison.InvariantCultureIgnoreCase))
					_password = value;
				else if (key.Equals("SSName", StringComparison.InvariantCultureIgnoreCase))
					_snapshotName = value;
				else if (key.Equals("SSDesc", StringComparison.InvariantCultureIgnoreCase))
					_snapshotDesc = value;
				else if (key.Equals("SSType", StringComparison.InvariantCultureIgnoreCase))
					_snapshotType = Int32.Parse(value);
				else if (key.Equals("SSUser", StringComparison.InvariantCultureIgnoreCase))
				{
					_snapshotUser = value;
					_snapshotName = _snapshotName.Replace("@SnapshotUser", _snapshotUser);
					_snapshotDesc = _snapshotDesc.Replace("@SnapshotUser", _snapshotUser);
				}

			}
			_primaryServerName = ParseConnectionString(_primaryConStr);
			_IRServerName = ParseConnectionString(_IRConStr);
		}

		public bool IsMigrateResults()
		{
			return (_tdmOption != TDMOPTION.MASTER_ONLY);
		}

		private string ParseConnectionString(string connstr)
		{
			SqlConnectionStringBuilder connStr = new SqlConnectionStringBuilder(connstr);
			return connStr.DataSource;

		}

		public void SetTDMFilePath(string analysisPath)
		{


			// Check if we have the correct analysis path
			// If not, base it on the location of TDMEngine32.exe

			if (analysisPath.Length > 4) //Correct path

				_tdmFilePath = Path.Combine(analysisPath, "TDM");
			else
				_tdmFilePath = Path.Combine(Environment.CurrentDirectory, "TDM");

			string tdmMDBFile = Path.Combine(_tdmFilePath, TDMEngineSettings.Default.TDMFileName);

			// if TDM.mdb is not found, check alternate path
			if (!File.Exists(tdmMDBFile))
			{
				_tdmFilePath = Path.Combine(Directory.GetParent(analysisPath).FullName, "TDM");
				tdmMDBFile = Path.Combine(_tdmFilePath, TDMEngineSettings.Default.TDMFileName);
			}
			// if TDM.mdb is not found, check alternate path
			if (!File.Exists(tdmMDBFile))
			{
				_tdmFilePath = Path.Combine(Environment.CurrentDirectory, "ANALYSIS\\TDM");
				tdmMDBFile = Path.Combine(_tdmFilePath + TDMEngineSettings.Default.TDMFileName);

				// if TDM.mdb is still not found, error
				if (!File.Exists(tdmMDBFile))
				{
					string csError = "TDMFile TDM.mdb not found.";
					throw new TDMEngineException(csError);
				}
			}
		}

		public bool IsDictionaryTbl(string tablename)
		{
			foreach (DataDictTabNames tbl in Enum.GetValues(typeof(DataDictTabNames)))
			{
				if (tbl.ToString().ToUpper().Trim() == tablename.ToUpper().Trim())
					return true;
			}
			return false;
		}

		public bool IsSystemLookupTable(string csTable)
		{
			bool success = false;
			if (SysLookupTableList.ContainsKey(csTable.Trim().ToUpper()))
				success = true;

			return success;
		}

		public bool IsSystemDBTable(string csTable, bool logMsg = false)
		{
			bool success = false;

			if (SysDBTableList.ContainsKey(csTable.Trim().ToUpper()))
				success = true;
			if (logMsg) LogWriter.LogIt(csTable + " is " + ((success) ? "" : "not ") + "an EqecatSystem table");
			return success;
		}

		public bool IsCommonDBTable(string csTable, bool logMsg = false)
		{
			bool success = false;
			if (ComDBTableList.ContainsKey(csTable.Trim().ToUpper()))
				success = true;

			if (logMsg) LogWriter.LogIt(csTable + " is " + ((success) ? "" : "not ") + "an EqecatCommon table");

			return success;
		}

		public bool IsCFDBTable(string csTable, bool logMsg = false)
		{
			bool success = false;
			if (CFDBTableList.ContainsKey(csTable.Trim().ToUpper()))
				success = true;

			if (logMsg) LogWriter.LogIt(csTable + " is " + ((success) ? "" : "not ") + "a CFDB table");

			return success;
		}

		public bool IsCFIRDBTable(string csTable, bool logMsg = false)
		{
			bool success = false;
			if (CFIRDBTableList.ContainsKey(csTable.Trim().ToUpper()))
				success = true;

			if (logMsg) LogWriter.LogIt(csTable + " is " + ((success) ? "" : "not ") + "a CFDB_IR table");

			return success;
		}

		#endregion
	}
}

