using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using NProf.Glue.Profiler.Project;

namespace NProf.Utilities.DataStore
{
	public class SerializationHandler
	{
		public static string DataStoreDirectory;

		private static Hashtable _projectInfoToFileNameMap = new Hashtable();
		private static Hashtable _fileNameToProjectInfoMap = new Hashtable();

		private static string ProjectsDirectoryPath
		{
			get
			{
				string path = DataStoreDirectory + "\\Projects";
				Directory.CreateDirectory(path);

				return path;
			}
		}

		private static string ProjectsHistoryPath
		{
			get
			{
				string path = DataStoreDirectory;
				Directory.CreateDirectory(path);

				return path + "\\Projects.xml";
			}
		}

		private static UsedFileCollection ProjectsHistory
		{
			get
			{
				if(File.Exists(ProjectsHistoryPath))
				{
					FileStream inputFile = new FileStream(ProjectsHistoryPath, FileMode.Open);
					XmlSerializer s = new XmlSerializer(typeof(UsedFileCollection));

					UsedFileCollection rv = (UsedFileCollection)s.Deserialize(inputFile);

					inputFile.Close();

					return rv;
				}
				else
					return new UsedFileCollection();
			}
			set
			{
				FileStream outputFile = new FileStream(ProjectsHistoryPath, FileMode.Create);
				XmlSerializer s = new XmlSerializer(typeof(UsedFileCollection));

				value.Sort();
				s.Serialize(outputFile, value);

				outputFile.Close();
			}
		}

		static SerializationHandler() { }

		private static string GetUniqueFileName(string path, string extension)
		{
			return path + (path.EndsWith("\\") ? string.Empty : "\\") +
				Guid.NewGuid().ToString("D") + (extension.StartsWith(".") ? string.Empty : ".") + extension;
		}

		public static void SaveProjectInfo(ProjectInfo info)
		{
			string fileName = string.Empty;

			if(_projectInfoToFileNameMap.Contains(info)) // one that has already been opened
				fileName = (string)_projectInfoToFileNameMap[info];
			else // a new guy
			{
				fileName = GetUniqueFileName(ProjectsDirectoryPath, "xml");
				_projectInfoToFileNameMap.Add(info, fileName);
				_fileNameToProjectInfoMap.Add(fileName, info);
			}

			FileStream outputFile = new FileStream(fileName, FileMode.Create);

			XmlSerializer s = new XmlSerializer(typeof(ProjectInfo));
			s.Serialize(outputFile, info);

			outputFile.Close();

			MakeRecentlyUsed(fileName);
		}

		public static void MakeRecentlyUsed(ProjectInfo info)
		{ MakeRecentlyUsed(_projectInfoToFileNameMap[info] as ProjectInfo); }

		private static void MakeRecentlyUsed(string fileName)
		{
			if(fileName == null || fileName == string.Empty)
				return;

			UsedFileCollection ufc = ProjectsHistory;

			ufc[fileName].LastUsed = DateTime.Now;

			ProjectsHistory = ufc;
		}

		public static ProjectInfoCollection GetSavedProjectInfos()
		{
			SortedList list = new SortedList();
			UsedFileCollection usf = ProjectsHistory;

			foreach(string fileName in Directory.GetFiles(ProjectsDirectoryPath))
			{
				ProjectInfo info = null;

				if(_fileNameToProjectInfoMap.Contains(fileName))
					info = (ProjectInfo)_fileNameToProjectInfoMap[fileName];
				else
				{
					FileStream inputFile = new FileStream(fileName, FileMode.Open);

					XmlSerializer s = new XmlSerializer(typeof(ProjectInfo));
					info = (ProjectInfo)s.Deserialize(inputFile);

					inputFile.Close();

					_projectInfoToFileNameMap.Add(info, fileName);
					_fileNameToProjectInfoMap.Add(fileName, info);
				}

				list.Add(DateTime.MaxValue - usf[fileName].LastUsed, info);
			}

			ProjectInfoCollection rv = new ProjectInfoCollection();

			foreach(ProjectInfo info in list.Values)
				rv.Add(info);

			return rv;
		}
	}
}
