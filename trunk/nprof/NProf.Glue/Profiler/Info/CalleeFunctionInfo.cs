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

		private int _nID;
		private int _nCalls;
		private FunctionSignatureMap _fsm;
		private long _lTotalTime;
	}
}
