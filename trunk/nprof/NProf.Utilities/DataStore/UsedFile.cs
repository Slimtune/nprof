using System;
using System.Collections;

namespace NProf.Utilities.DataStore
{
	public class UsedFile : IComparable
	{
		private string _fileName;
		private string _projectName;
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

		public string ProjectName
		{
			get
			{
				return _projectName;
			}
			set
			{
				_projectName = value;
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
		{
		}

		internal UsedFile(string fileName, string projectName, DateTime lastUsed)
		{
			_fileName = fileName;
			_projectName = projectName;
			_lastUsed = lastUsed;
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
