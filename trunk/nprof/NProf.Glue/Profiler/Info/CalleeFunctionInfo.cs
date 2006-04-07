using System;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionInfo.
	/// </summary>
	[Serializable]
	public class CalleeFunctionInfo
	{
		public CalleeFunctionInfo()
		{
			_fsm = new FunctionSignatureMap();
		}

		public CalleeFunctionInfo( FunctionSignatureMap fsm, int nID, int nCalls, long lTotalTime, long lTotalRecursiveTime )
		{
			_nID = nID;
			_nCalls = nCalls;
			_lTotalTime = lTotalTime;
			_lTotalRecursiveTime = lTotalRecursiveTime;
			_fsm = fsm;
		}

		public int ID
		{
			get { return _nID; }
			set { _nID = value; }
		}

		public string Signature
		{
			get { return _fsm.GetFunctionSignature( ID ); }
		}

		public int Calls
		{
			get { return _nCalls; }
			set { _nCalls = value; }
		}

		public long TotalTime
		{
			get { return _lTotalTime; }
			set { _lTotalTime = value; }
		}

		public long TotalRecursiveTime
		{
			get { return _lTotalRecursiveTime; }
			set { _lTotalRecursiveTime = value; }
		}

		public double PercentOfTotalTimeInMethod
		{
			get { return ( double )_lTotalTime / ( double )_fi.ThreadTotalTicks * 100; }
		}

		public double PercentOfParentTimeInMethod
		{
			get { return ( double )_lTotalTime / ( double )_fi.TotalTicks * 100; }
		}

		internal FunctionInfo FunctionInfo
		{
			set { _fi = value; }
		}

		private int _nID;
		private int _nCalls;
		private FunctionInfo _fi;
		private FunctionSignatureMap _fsm;
		private long _lTotalTime;
		private long _lTotalRecursiveTime;
	}
}
