using System;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
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
