using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using Eqecat.ZipLibWrapper;

namespace TDMEngine
{
	static class Util
	{
		#region Static methods
		public static string TextFileExists(string file, string path)
		{
			// try known text file extensions.
			string strTextFile = path + "\\" + file + ".txt";

			if (File.Exists(strTextFile)) return strTextFile;

			strTextFile = path + "\\" + file + ".bar";
			if (File.Exists(strTextFile)) return strTextFile;

			strTextFile = path + "\\" + file + ".zip";
			if (File.Exists(strTextFile)) return strTextFile;

			// no text files with above file extensions found
			return "";
		}

		public static DateTime GetDataFileDate(string filename)
		{
			return File.GetCreationTime(filename);
		}

		public static void CleanUpFolder(string folder)
		{
			foreach (string f in Directory.GetFiles(folder, "*.*"))
			{
				if (File.Exists(f)) File.Delete(f);
			}
			foreach (string d in Directory.GetDirectories(folder))
			{

				CleanUpFolder(folder);
			}
		}

		public static string DetermineDelimiter(string fileName)
		{
			string delimChar = ",";
			List<string> presentDelims = new List<string>();
			if (Path.GetExtension(fileName).ToLower() == ".bar")
				delimChar = "|";

			//Now open the file and try out the available delimiters one by one.
			//Whichever occurs more than 3 times first is our delimiter.
			StreamReader sr = new StreamReader(fileName);
			string firstLine = sr.ReadLine();
			sr.Close();

			//turns out its difficult to enter tab in settings file
			StringCollection availableDelims = TDMEngineSettings.Default.DelimitersUsed;
			if (!availableDelims.Contains("\t"))
				availableDelims.Insert(0, "\t");

			foreach (string delim in availableDelims)
			{
				string[] linePart = firstLine.Split(new string[] { delim }, StringSplitOptions.None);

				if (linePart.Length > 1)
					presentDelims.Add(delim);

				if (linePart.Length >= TDMEngineSettings.Default.MinCountForDelimiters)
				{
					delimChar = delim;
					break;
				}
			}

			//finally check if the one decided is present at all
			if (presentDelims.Count > 0 && !presentDelims.Contains(delimChar))
				delimChar = presentDelims[0];

			return delimChar;
		}

		public static string UnzipFile(string fileName, string filePath)
		{
			string[] unzipFileNames = null;

			//Create a temporary folder for use
			string tempPath = Path.Combine(filePath, Path.GetFileNameWithoutExtension(fileName));
			if (Directory.Exists(tempPath))
				Directory.Delete(tempPath, true);

			if (ZipLibWrapper.UnzipFile(Path.Combine(filePath,fileName), tempPath))
				unzipFileNames = Directory.GetFiles(tempPath);

			return unzipFileNames[0];
		}

		public static void DeleteLineFromFile(string fileName)
		{
			String line = string.Empty;
			FileInfo f = new FileInfo(fileName);
			string delimiter = Util.DetermineDelimiter(fileName);
			string tempfile = f.FullName.TrimEnd(f.Extension.ToCharArray()) + "_temp" + f.Extension;

			int line_number = 0;
			using (StreamReader reader = new StreamReader(fileName))
			{
				using (StreamWriter writer = new StreamWriter(tempfile))
				{
					while ((line = reader.ReadLine()) != null)
					{
						line_number++;
						if (line_number != 1)
							writer.WriteLine(line.TrimEnd(delimiter.ToCharArray())); //trim the last character

					}
				}
			}
			if (File.Exists(tempfile))
			{
				File.Delete(fileName); File.Move(tempfile, fileName);
			}
		}

		public static void CreateFolder(string folderName)
		{
			int pos;

			//search from right to find path
			pos = folderName.IndexOf('\\');
			if (pos > 0)
			{
				CreateFolder(folderName.Substring(0, pos));
			}

			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

		}

		public static bool FolderExists(string folderName)
		{
			return Directory.Exists(folderName);

		}
		public static void DeleteFolder(string folderName)
		{
			Directory.Delete(folderName, true);

		}
		#endregion
	}
}
