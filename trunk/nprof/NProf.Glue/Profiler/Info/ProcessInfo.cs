using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for ProcessInfo.
	/// </summary>
	[XmlInclude( typeof( ThreadInfo ) )]
	public class ProcessInfo
	{
		public ProcessInfo()
		{
			_nID = 0;
			_tic = new ThreadInfoCollection();
			_fsm = new FunctionSignatureMap();
		}

		public ProcessInfo( int nID ) : this()
		{
			_nID = nID;
		}

		public int ID
		{
			get { return _nID; }
		}

		public int ProcessID
		{
			get { return _nProcessID; }
			set { _nProcessID = value; }
		}

		public string Name
		{
			get { return _strName; }
			set { _strName = value; }
		}

		public FunctionSignatureMap Functions
		{
			get { return _fsm; }
			set { _fsm = value; }
		}

		public ThreadInfoCollection Threads
		{
			get { return _tic; }
			set { _tic = value; }
		}

		public override string ToString()
		{
			return String.Format( "{0} ({1})", _strName, _nProcessID );
		}

		private int _nID;
		private int _nProcessID;
		private FunctionSignatureMap _fsm;
		private ThreadInfoCollection _tic;
		private string _strName;
	}
}
