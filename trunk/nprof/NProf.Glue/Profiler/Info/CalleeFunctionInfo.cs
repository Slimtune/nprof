using System;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionInfo.
	/// </summary>
	public class CalleeFunctionInfo
	{
		public CalleeFunctionInfo( FunctionSignatureMap fsm, int nID, int nCalls, long lTotalTime )
		{
			_nID = nID;
			_nCalls = nCalls;
			_lTotalTime = lTotalTime;
			_fsm = fsm;
		}

		public int ID
		{
			get { return _nID; }
		}

		public string Signature
		{
			get { return _fsm.GetFunctionSignature( ID ); }
		}

		public int Calls
		{
			get { return _nCalls; }
		}

		public long TotalTime
		{
			get { return _lTotalTime; }
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
	}
}
