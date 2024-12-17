using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eqecat.AccessMdbRelated;
using WCeFramework.WCeDBInterface;
using System.Data;
using System.Collections;

namespace TDMEngine
{
	class MSAccessInterface
	{
		AccessDbInterface accessDbInterface = null;
		int _migrStepCount = 0;
		public string accessDBError = "";

		public MSAccessInterface(string tdmFile)
		{
			accessDBError = GetAccessDbInterface(tdmFile);
		}

		public string GetAccessDbInterface(string tdmFile)
		{		
			try
			{	
				AccessDbConnectionMdb accessDbConnectionMdb = new AccessDbConnectionMdb();
				accessDbInterface = new AccessDbInterface(accessDbConnectionMdb.GetMSAccess2007Connection (tdmFile));
				return "";
			}
			catch (Exception ex)
			{
				return ex.Message ;
			}
		}

		private int GetTotalSteps(string tdmTbl)
		{
			int cnt=0;
			string s = string.Format(TDMEngineSettings.Default.A_SQL_GetMigrStepCount, new string[] { tdmTbl }); 
			DataTable dt = accessDbInterface.ExecuteSql(s,null);
			if (dt != null && dt.Rows.Count > 0)
			{
				cnt = Int32.Parse(dt.Rows[0][0].ToString());
			}
			return cnt;
		}

		public int GetTotalMigrStepsCount()
		{
			return _migrStepCount;
		}

		public Dictionary<String, TDMDestBuilds> GetDestBuilds(string srcBuild, string destBuild)
		{
			Dictionary<String, TDMDestBuilds> destBuildTable = new Dictionary<String, TDMDestBuilds>();

			DataTable dt = null;
			try
			{
				string s = string.Format(TDMEngineSettings.Default.A_SQL_GetDestBuilds, new string[] { srcBuild, destBuild }); 
				LogWriter.LogIt(s);
				dt = accessDbInterface.ExecuteSql(s,null);
				if (dt.Rows.Count > 0)
				{
					foreach (DataRow dataRow in dt.Rows)
					{
						TDMDestBuilds tdmDestBuildInfo = new TDMDestBuilds();
						string destWceBuild = (string)dataRow["DESTBUILD"];
						tdmDestBuildInfo.DestWceBuild = destWceBuild;
						string destWceVersion = (string)dataRow["DESTWCE"];
						tdmDestBuildInfo.DestWceVersion = destWceVersion;
						tdmDestBuildInfo.SourceWceBuild = dataRow["SRCBUILD"] != System.DBNull.Value ? (string)dataRow["SRCBUILD"] : "";
						tdmDestBuildInfo.SourceWceBuild = dataRow["SRCWCE"] != System.DBNull.Value ? (string)dataRow["SRCWCE"] : "";
						tdmDestBuildInfo.DBSchema = dataRow["DBSCHEMA"] != System.DBNull.Value ? (string)dataRow["DBSCHEMA"] : "";
						tdmDestBuildInfo.TableName  = dataRow["TDMTBL"] != System.DBNull.Value ? (string)dataRow["TDMTBL"] : "";
						tdmDestBuildInfo.EDMDir = dataRow["EDMDIR"] != System.DBNull.Value ? (string)dataRow["EDMDIR"] : "";

						tdmDestBuildInfo.ArcSchema  = dataRow["ARCSCHEMA"] != System.DBNull.Value ? (string)dataRow["ARCSCHEMA"] : "";
						tdmDestBuildInfo.EqeVer = dataRow["SRCEQE"] != System.DBNull.Value ? (string)dataRow["SRCEQE"] : "";
						tdmDestBuildInfo.Desc  = dataRow["DESC"] != System.DBNull.Value ? (string)dataRow["DESC"] : "";
						tdmDestBuildInfo.FlCertVer  = dataRow["FL_CERTVER"] != System.DBNull.Value ? (string)dataRow["FL_CERTVER"] : "";

						tdmDestBuildInfo.NoteDelete = dataRow["NOTEDELETE"] != System.DBNull.Value ? (string)dataRow["NOTEDELETE"] : "";
						tdmDestBuildInfo.NotePreserve = dataRow["NOTEPRESERVE"] != System.DBNull.Value ? (string)dataRow["NOTEPRESERVE"] : "";
					//	tdmDestBuildInfo.PrevDate = dataRow["PREVDATE"] != System.DBNull.Value ? (DateTime )dataRow["PREVDATE"] : "";
						tdmDestBuildInfo.PrevDate = (DateTime)dataRow["PREVDATE"] ;
						String destKey = destWceVersion + " - " + destWceBuild.Replace(TDMEngineSettings.Default.StringToReplace, String.Empty);
						destBuildTable.Add(destKey, tdmDestBuildInfo);
						_migrStepCount += GetTotalSteps(tdmDestBuildInfo.TableName);
					}
				}
			}
			catch (Exception )
			{
				accessDBError =  "Error reading TDMCTRL";
			}
			finally
			{
				if (dt != null)
					dt.Clear();
			}

			return destBuildTable;
		}

		public ArrayList GetTDMTblData(string tableName)
		{
			ArrayList MigrSteps = new ArrayList();
		
			DataTable dt = null;
			string colName=string.Empty ;
			try
			{
				if(tableName.Equals("TDMPRECMD")||tableName.Equals("TDMPOSTCMD")) // We have an extra column EDMDIR
						colName="EDMDIR," ;

				string s = string.Format(TDMEngineSettings.Default.A_SQL_GetTDMTableData, new string[] { tableName, colName }); 
 				dt = accessDbInterface.ExecuteSql(s,null);
				if (dt.Rows.Count > 0)
				{
					foreach (DataRow dataRow in dt.Rows)
					{
						TdmTableStep tdmTbl = new TdmTableStep();
						tdmTbl.Key = dataRow["KEY"] != System.DBNull.Value ? (Int32 )dataRow["KEY"] : 0;
						tdmTbl.Seq = dataRow["SEQ"] != System.DBNull.Value ? (Int32)dataRow["SEQ"] : 0;
						tdmTbl.Defect = dataRow["DEFECT"] != System.DBNull.Value ? (Int32)dataRow["DEFECT"] : 0;
						tdmTbl.Rem_sql = dataRow["REM_SQL"] != System.DBNull.Value ? (string)dataRow["REM_SQL"] : "";
						tdmTbl.Tname = dataRow["TNAME"] != System.DBNull.Value ? (string)dataRow["TNAME"] :"";
						tdmTbl.Cmd_sql = dataRow["CMD_SQL"] != System.DBNull.Value ? (string)dataRow["CMD_SQL"] : "";
						tdmTbl.Depend = dataRow["DEPEND"] != System.DBNull.Value ? (string)dataRow["DEPEND"] : "";
						tdmTbl.Notes = dataRow["NOTES"] != System.DBNull.Value ? (string)dataRow["NOTES"] : "";
						tdmTbl.Type = dataRow["TYPE"] != System.DBNull.Value ? (string)dataRow["TYPE"] : ""; 
						tdmTbl.Descr = dataRow["DESCR"] != System.DBNull.Value ? (string)dataRow["DESCR"] : "";
						if(tableName.Equals("TDMPRECMD")||tableName.Equals("TDMPOSTCMD"))
							tdmTbl.EdmDir = dataRow["EDMDIR"] != System.DBNull.Value ? (string)dataRow["EDMDIR"] : "";
						MigrSteps.Add(tdmTbl);											
					}
				}
			}
			catch (Exception ex)
			{
				accessDBError = "Error in " + tableName +". " + ex ;
			}
			finally
			{
				if (dt != null)
					dt.Clear();
			}

			return MigrSteps;
		}
	}

	class TdmTableStep
	{
		Int32 _key;
		Int32 _seq;
		string _rem_sql;
		Int32 _defect;
		string _descr; 
		string _type;
		string _tname;
		string _depend;
		string _cmd_sql;
		string _notes;
		string _edmdir = "";

		public Int32 Key
		{
			get { return _key; }
			set { _key = value; }
		}
		public Int32 Seq
		{
			get { return _seq; }
			set { _seq = value; }
		}

		public string Rem_sql
		{
			get { return _rem_sql; }
			set { _rem_sql = value; }
		}

		public Int32 Defect
		{
			get { return _defect; }
			set { _defect = value; }
		}
		
		public string Descr
		{
			get { return _descr; }
			set { _descr = value; }
		}
		public string Type
		{
			get { return _type; }
			set { _type = value; }
		}
		public string Tname
		{
			get { return _tname; }
			set { _tname = value; }
		}
		public string Depend
		{
			get { return _depend; }
			set { _depend = value; }
		}
		public string Cmd_sql
		{
			get { return _cmd_sql; }
			set { _cmd_sql = value; }
		}
		public string Notes
		{
			get { return _notes; }
			set { _notes = value; }
		}
		public string EdmDir
		{
			get { return _edmdir; }
			set { _edmdir = value; }
		}
	}
}
