using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for ThreadInfo.
	/// </summary>
	[XmlInclude( typeof( FunctionInfo ) )]
	[Serializable]
	public class ThreadInfo
	{
		public ThreadInfo()
		{
			this.functions = new FunctionInfoCollection();
			this.id = 0;
			this.startTime = 0;
			this.endTime = 0;
		}

		public ThreadInfo( int threadId ) : this()
		{
			this.id = threadId;
		}

		[XmlIgnore]
		public int ID
		{
			get { return id; }
		}

		[XmlIgnore]
		public long StartTime
		{
			get { return startTime; }
			set { startTime = value; }
		}

		[XmlIgnore]
		public long EndTime
		{
			get { return endTime; }
			set { endTime = value; }
		}

		[XmlIgnore]
		public long TotalTime
		{
			get { return endTime - startTime; }
		}

		public FunctionInfoCollection FunctionInfoCollection
		{
			get { return functions; }
		}

		public override string ToString()
		{
			return "Thread #" + id;
		}

		private int id;
		private long startTime, endTime;
		private FunctionInfoCollection functions;
	}
}
