using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for ThreadInfo.
	/// </summary>
	[XmlInclude( typeof( FunctionInfo ) )]
	public class ThreadInfo
	{
		public ThreadInfo()
		{
			_fic = new FunctionInfoCollection();
			_nID = 0;
			_lStartTime = 0;
			_lEndTime = 0;
		}

		public ThreadInfo( int nThreadID ) : this()
		{
			_nID = nThreadID;
		}

		public int ID
		{
			get { return _nID; }
		}

		public long StartTime
		{
			get { return _lStartTime; }
			set { _lStartTime = value; }
		}

		public long EndTime
		{
			get { return _lEndTime; }
			set { _lEndTime = value; }
		}

		public long TotalTime
		{
			get { return _lEndTime - _lStartTime; }
		}

		public FunctionInfoCollection FunctionInfoCollection
		{
			get { return _fic; }
		}

		public override string ToString()
		{
			return "Thread #" + _nID;
		}

		private int _nID;
		private long _lStartTime, _lEndTime;
		private FunctionInfoCollection _fic;
	}
}
