using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.eqecat.GenericFunctions.XlsxHelper;
using System.Data;

namespace TDMEngine
{
	public enum DataDictTabNames
	{
		DICTTBL=0,
		DICTCOL,
		DICTIDX,
		DICTCNST,
		DICTCLON
	}
	class DataDictXlsxReader
	{
		string _dataDictXlsxPath;
		DataTable[] _dataTables = new DataTable[Enum.GetNames(typeof(DataDictTabNames)).Length];
		public DataDictXlsxReader(string dataDictXlsxPath)
		{
			 _dataDictXlsxPath= dataDictXlsxPath;
		}

		public string[] _dataDictTableTabNames = new string[] 
		{ 
			Enum.GetName(typeof(DataDictTabNames), DataDictTabNames.DICTTBL),
			Enum.GetName(typeof(DataDictTabNames), DataDictTabNames.DICTCOL),
			Enum.GetName(typeof(DataDictTabNames), DataDictTabNames.DICTIDX),
			Enum.GetName(typeof(DataDictTabNames), DataDictTabNames.DICTCNST),
			Enum.GetName(typeof(DataDictTabNames), DataDictTabNames.DICTCLON),
		};


		private void Export(string outPath, string delimiterChar = "\t")
		{
			using (XlsxHelper xlsHelper = new XlsxHelper())
			{
				if (xlsHelper.OpenSpreadsheet(_dataDictXlsxPath))
				{
					for (int sheetIndex = (int)DataDictTabNames.DICTTBL; sheetIndex < _dataDictTableTabNames.Length; sheetIndex++)
					{
						xlsHelper.OpenSheet(_dataDictTableTabNames[sheetIndex]);
						string outPutFileName = outPath + "\\"+ _dataDictTableTabNames[sheetIndex] + ".txt";
						xlsHelper.WriteSheetToFile(outPutFileName);
					}
				}
			}
		}

		// get  data from  spreadsheet
		public DataTable[] ReadTableFromXlsx(string outPath)
		{
			using (XlsxHelper xlsHelper = new XlsxHelper())
			{
				if (xlsHelper.OpenSpreadsheet(_dataDictXlsxPath))
				{
					for (int sheetIndex = (int)DataDictTabNames.DICTTBL; sheetIndex < _dataDictTableTabNames.Length; sheetIndex++)
					{
						xlsHelper.OpenSheet(_dataDictTableTabNames[sheetIndex]);
						_dataTables[sheetIndex] = xlsHelper.GetSheetAsDataTable(1);
						_dataTables[sheetIndex].TableName = Enum.GetName(typeof(DataDictTabNames), sheetIndex);

						
						//string outPutFileName = outPath + "\\" + _dataDictTableTabNames[sheetIndex] + ".txt";
						//xlsHelper.WriteSheetToFile(outPutFileName, true, '\t');
						//Remove the first line -column names
						//Util.DeleteLineFromFile(outPutFileName);

					}
				}
			}
			 
			return _dataTables;
		}
	}
}
