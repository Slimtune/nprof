using System;

namespace NProf.Glue.Profiler
{
	/// <summary>
	/// Summary description for ProfilerOptions.
	/// </summary>
	public class ProfilerOptions
	{
		public ProfilerOptions()
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
