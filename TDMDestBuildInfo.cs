using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TDMEngine
{
	public class TDMDestBuilds
	{
		#region private members
		string _destBuilds = string.Empty;
		string _destWceVersion = string.Empty;
		string _dbSchema = string.Empty;
		int _deleteCheck = 0;
		string _noteDelete = string.Empty;
		string _notePreserve = string.Empty;
		string _tableName = string.Empty;
		string _EDMDir = string.Empty;
		string _sourceWceBuild = string.Empty;
		string _sourceWceVersion = string.Empty;
		string _arcSchema = string.Empty;
		string _eqeVer = string.Empty;
		string _desc = string.Empty;
		string _flCertVer = string.Empty;
		DateTime _prevDate ;
		#endregion

		public TDMDestBuilds()
		{
		}

		#region properties
		public String DestWceBuild
		{
			get { return _destBuilds; }
			set { _destBuilds = value; }
		}

		public String SourceWceBuild
		{
			get { return _sourceWceBuild; }
			set { _sourceWceBuild = value; }
		}
		public String SourceWceVersion
		{
			get { return _sourceWceVersion; }
			set { _sourceWceVersion = value; }
		}
		public String DestWceVersion
		{
			get { return _destWceVersion; }
			set { _destWceVersion = value; }
		}
		public String DBSchema
		{
			get { return _dbSchema; }
			set { _dbSchema = value; }
		}
		
		public String TableName
		{
			get { return _tableName; }
			set { _tableName = value; }
		}
		public String EDMDir
		{
			get { return _EDMDir; }
			set { _EDMDir = value; }
		}
		public DateTime  PrevDate
		{
			get { return _prevDate; }
			set { _prevDate = value; }
		}
		public int DeleteCheck
		{
			get { return _deleteCheck; }
			set { _deleteCheck = value; }
		}
		public String ArcSchema
		{
			get { return _arcSchema; }
			set { _arcSchema = value; }
		}

		public String EqeVer
		{
			get { return _eqeVer; }
			set { _eqeVer = value; }
		}

		public String Desc
		{
			get { return _desc; }
			set { _desc = value; }
		}

		public String FlCertVer
		{
			get { return _flCertVer; }
			set { _flCertVer = value; }
		}
		public String NoteDelete
		{
			get { return _noteDelete; }
			set { _noteDelete = value; }
		}

		public String NotePreserve
		{
			get { return _notePreserve; }
			set { _notePreserve = value; }
		}
		#endregion
	}
}
