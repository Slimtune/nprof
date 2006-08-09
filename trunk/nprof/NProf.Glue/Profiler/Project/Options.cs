using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	[Serializable]
	public class Options
	{
		public Options()
		{
			isDebug = false;
		}

		public bool Debug
		{
			get { return isDebug; }
			set { isDebug = value; }
		}

		bool isDebug;
	}
}
