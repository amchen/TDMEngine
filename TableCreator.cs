using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TDMEngine
{
	class TableCreator
	{
		#region private data members
		string _retVal = "";
		string _fieldName = "";
		string _fieldType = "";
		int _fieldWidth = 0;
		string _nullString = "";
		string _fieldDefault = "";
		bool _newDfltValAdd = true;
		string _colSubType = "";
		TDMConfig _tdmConfig;
		#endregion

		#region constructor
		public TableCreator(TDMConfig tdmConfig)
		{
			_tdmConfig = tdmConfig;
		}
		#endregion

		#region private method
		private void Init(string fieldName, string fieldType, string fieldWidth, string isNullable, string fieldDefault, string colSubType)
		{
			_fieldName = fieldName.Trim();
			_fieldType = fieldType.Length > 0 ? fieldType.Trim().Substring(0, 1).ToUpper() : "";
			_fieldWidth = Convert.ToInt32(fieldWidth.Length > 0 ? fieldWidth : "0");
			_nullString = (isNullable.Length > 0 && isNullable.Trim().Substring(0, 1).ToUpper() == "Y") ? "" : " NOT NULL";
			_fieldDefault = fieldDefault.Trim();
			_colSubType = colSubType.Trim();
			_retVal = _fieldName + " ";
		}

		private string CreateField(string fieldName, string fieldType, string fieldWidth, string isNullable, string fieldDefault, string hasAnotherClusteredIndexDefined, string colSubType)
		{
			Init(fieldName, fieldType, fieldWidth, isNullable, fieldDefault, colSubType);

			bool hasClustered = (hasAnotherClusteredIndexDefined.Length > 0 && hasAnotherClusteredIndexDefined.Trim().Substring(0, 1).ToUpper() == "Y") ? true : false;
			string nonClusteredString = hasClustered ? " NONCLUSTERED" : "";		// typically no other clustered index exists, but, if it does, uncluster this one

			if (_fieldType == "A")
			{
				_retVal += "INTEGER IDENTITY(1,1)";
			}
			else if (_fieldType == "B")
			{
				_retVal += "VARBINARY (MAX)";
			}
			else if (_fieldType == "C" || _fieldType == "U")
			{
				string cs = string.Empty;
				if (_fieldWidth == 1)
					cs = "CHAR (1)";
				else if (_fieldType == "U")
					cs = "VARCHAR (" + (_fieldWidth + 10).ToString() + ")";
				else
					cs = "VARCHAR (" + _fieldWidth + ")";

				_retVal += cs + _nullString + SetCharDefault();
			}
			else if (_fieldType == "F" || _fieldType == "G")
			{
				string floatString = string.Empty;
				string notNullString = " NOT NULL";

				string defaultString = string.Empty;

				if (_fieldType == "F")
					floatString = "FLOAT(24)";
				else
					floatString = "FLOAT(53)";

				if (_newDfltValAdd)
				{
					if (string.IsNullOrEmpty(_fieldDefault))
						defaultString = SetDefault0();
					else
						defaultString = SetDefault();
				}
				else if (!string.IsNullOrEmpty(notNullString))
					defaultString = SetDefault0();


				_retVal += floatString + notNullString + defaultString;
			}

			else if (_fieldType == "I")
			{
				_retVal += "INT" + _nullString + SetDefault0();
			}
			else if (_fieldType == "K")
			{
				_retVal += "INT" + _nullString + SetDefault0();
			}
			else if (_fieldType == "N")
			{
				_retVal += "BIGINT" + _nullString + SetDefault0();
			}
			else if (_fieldType == "S")
			{
				_retVal += "SMALLINT" + _nullString + SetDefault0(); ;
			}
			else if (_fieldType == "T")
			{
				_retVal += "VARCHAR (" + _fieldWidth + ")" + _nullString;
			}
			else if (_fieldType == "V")
			{
				_retVal += (_fieldWidth < 6000) ? "VARCHAR (" + _fieldWidth + ")" + _nullString + SetCharDefault() : "VARCHAR (MAX)" + _nullString + SetCharDefault();
			}

			return _retVal + ", ";
		}

		private string SetCharDefault()
		{
			if (_newDfltValAdd)
				return " DEFAULT " + "'" + _fieldDefault + "'";
			else
				return string.Empty;
		}

		private string SetDefault0()
		{
			return " DEFAULT 0";
		}

		private string SetDefault()
		{
			return (_fieldDefault.Length > 0 ? " DEFAULT " + _fieldDefault : "");
		}

		private string GetIndexScript(string tableName)
		{

			return CreateConstraintScript(tableName, "N");
		}

		private string GetPrimaryKeyConstraint(string tableName)
		{
			return CreateConstraintScript(tableName, "Y");
		}

		private string CreateConstraintScript(string tableName, string IsPrimary)
		{

			DataTable dictIdx = _tdmConfig.DataDictTables[(int)DataDictTabNames.DICTIDX];
			string filter = "";

			if (IsPrimary == "Y")
				filter = " and IsPrimary = 'Y'";
			else
				filter = " and IsPrimary <> 'Y'";
			IOrderedEnumerable<DataRow> rows = dictIdx.Select("TableName ='" + tableName + "' " + filter)
													  .OrderBy(n => n["IndexName"].ToString())
													  .ThenBy(n => Convert.ToInt32(n["FieldOrder"]));


			string fieldList = string.Empty;
			string lastIndexName = string.Empty;
			string cnstScript = string.Empty;
			bool IsUnique = false, IsClustered = false;
			string indexName = string.Empty;


			foreach (DataRow row in rows)
			{
				indexName = row["IndexName"].ToString();
				if (indexName == lastIndexName || lastIndexName.Length == 0)
				{
					IsUnique = row["IsUnique"].ToString().ToUpper() == "Y";
					IsClustered = row["IsCluster"].ToString().ToUpper() == "Y";
					fieldList = fieldList + row["FieldName"].ToString() + ",";
				}
				else
				{
					if (IsPrimary == "Y")
						break;
					else
					{
						fieldList = fieldList.TrimEnd(',');
						cnstScript += "CREATE " + (IsUnique ? "UNIQUE " : "") + (IsClustered ? "CLUSTERED " : "NONCLUSTERED ") +
										"INDEX " + indexName + " ON " + tableName + "(" + fieldList + ");";
						fieldList = "";
					}
				}

				lastIndexName = indexName;
			}

			if (IsPrimary == "Y")
			{
				fieldList = fieldList.TrimEnd(',');
				cnstScript = "CONSTRAINT [" + indexName + "] PRIMARY KEY " + (IsClustered ? "CLUSTERED " : "NONCLUSTERED ") + "( " + fieldList + " )";
			}
			else if (fieldList.Length > 0)
			{
				fieldList = fieldList.TrimEnd(',');
				cnstScript += "CREATE " + (IsUnique ? "UNIQUE " : "") + (IsClustered ? "CLUSTERED " : "NONCLUSTERED ") +
								"INDEX " + indexName + " ON " + tableName + "(" + fieldList + ");";
			}
			return cnstScript;
		}


		#endregion

		#region public methods
		public string CreateTableScript(string baseTableName, string newTableName)
		{
			string sqlCreate = "";
			string indexStr = "";

			sqlCreate = TDMEngineSettings.Default.CreateTablePrefix + " CREATE TABLE " + newTableName + "( ";

			DataTable dt = _tdmConfig.DataDictTables[(int)DataDictTabNames.DICTCOL];

			string selectClause = " TABLENAME = " + "'" + baseTableName + "'";
			IOrderedEnumerable<DataRow> found = dt.Select(selectClause).OrderBy(n => Convert.ToInt32(n["FieldNum"]));
			foreach (DataRow dr in found)
			{
				sqlCreate += CreateField(dr["FIELDNAME"].ToString(), dr["FIELDTYPE"].ToString(), dr["FIELDWIDTH"].ToString(),
					dr["NULLABLE"].ToString(), dr["DEFVALADD"].ToString(), "N", dr["COLSUBTYPE"].ToString());
			}

			string constraintStr = GetPrimaryKeyConstraint(baseTableName);
			if (constraintStr.Length > 0)
				sqlCreate = sqlCreate + constraintStr;
			else
				sqlCreate = sqlCreate.Substring(0, sqlCreate.Length - 2).Trim();

			sqlCreate = sqlCreate + ");";

			indexStr = GetIndexScript(baseTableName);

			return sqlCreate + indexStr;
		}
		#endregion
	}
}
