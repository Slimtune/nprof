using System;
using System.Collections;

namespace NProf.Utilities.DataStore
{
	public class UsedFileCollection
	{
		private ArrayList _usedFiles = new ArrayList();
		private Hashtable _fileNameMap = new Hashtable();

		public UsedFile this[string fileName]
		{
			get
			{
				if(!_fileNameMap.Contains(fileName))
				{
					UsedFile uf = new UsedFile(fileName);
					_fileNameMap.Add(fileName, uf);
					_usedFiles.Add(uf);
				}

				return ((UsedFile)_fileNameMap[fileName]);
			}
		}

		public UsedFile[] Data
		{
			get
			{
				return (UsedFile[])_usedFiles.ToArray(typeof(UsedFile));
			}
			set
			{
				foreach(UsedFile uf in value)
				{
					_usedFiles.Add(uf);
					_fileNameMap.Add(uf.FileName, uf);
				}
			}
		}

		public void Sort()
		{
			_usedFiles.Sort();
		}
	}

	public class UsedFile : IComparable
	{
		private string _fileName;
		private DateTime _lastUsed;

		public string FileName
		{
			get
			{
				return _fileName;
			}
			set
			{
				_fileName = value;
			}
		}

		public DateTime LastUsed
		{
			get
			{
				return _lastUsed;
			}
			set
			{
				_lastUsed = value;
			}
		}

		public UsedFile()
		{ }
		public UsedFile(string fileName)
		{
			_fileName = fileName;
			_lastUsed = DateTime.MinValue;
		}
	
		public int CompareTo(object obj)
		{
			UsedFile left = this;
			UsedFile right = (UsedFile)obj;

			if(left == null || right == null || left.Equals(right))
				return 0;

			return left.LastUsed.CompareTo(right.LastUsed);
		}
	}
}
