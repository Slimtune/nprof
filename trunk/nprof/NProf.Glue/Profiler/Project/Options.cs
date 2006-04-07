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
			_bDebug = false;
		}

		public bool Debug
		{
			get { return _bDebug; }
			set { _bDebug = value; }
		}

		bool _bDebug;
	}
}
