using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for ProcessInfo.
	/// </summary>
	[XmlInclude( typeof( ThreadInfo ) )]
	[Serializable]
	public class ProcessInfo
	{
		public ProcessInfo()
		{
			this.id = 0;
			this.threads = new ThreadInfoCollection();
			this.signatures = new FunctionSignatureMap();
		}

		public ProcessInfo( int id ) : this()
		{
			this.id = id;
		}

		public int ID
		{
			get { return id; }
		}

		[XmlIgnore]
		public int ProcessID
		{
			get { return processId; }
			set { processId = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public FunctionSignatureMap Functions
		{
			get { return signatures; }
			set { signatures = value; }
		}

		public ThreadInfoCollection Threads
		{
			get { return threads; }
			set { threads = value; }
		}

		public override string ToString()
		{
			return String.Format( "{0} ({1})", name, processId );
		}

		private int id;
		private int processId;
		private FunctionSignatureMap signatures;
		private ThreadInfoCollection threads;
		private string name;
	}
}
