using System;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionInfo.
	/// </summary>
	public class FunctionInfo
	{
		public FunctionInfo( ThreadInfo ti, int nID, FunctionSignature fs, int nCalls, long lTotalTime, long lTotalSuspendedTime, CalleeFunctionInfo[] acfi )
		{
			_ti = ti;
			_nID = nID;
			_fs = fs;
			_nCalls = nCalls;
			_lTotalTime = lTotalTime;
			_lTotalSuspendedTime = lTotalSuspendedTime;
			_acfi = acfi;
		}

		public int ID
		{
			get { return _nID; }
		}

		public FunctionSignature Signature
		{
			get { return _fs; }
		}

		public CalleeFunctionInfo[] CalleeInfo
		{
			get { return _acfi; }
		}

		public int Calls
		{
			get { return _nCalls; }
		}

		public long TotalTime
		{
			get { return _lTotalTime; }
		}

		public long TotalSuspendedTime
		{
			get { return _lTotalSuspendedTime; }
		}

		public double TimeSuspended
		{
			get 
			{
				if ( _lTotalTime == 0 )
					return 0;

				return ( ( double )_lTotalSuspendedTime / ( double )_lTotalTime ) * 100;
			}
		}

		public double TimeInMethod
		{
			get
			{
				if ( _ti.TotalTime == 0 )
					return 0;

				long lTotalChildrenTime = 0;
				foreach ( CalleeFunctionInfo cfi in _acfi )
					lTotalChildrenTime += cfi.TotalTime;

				return ( ( ( double )_lTotalTime - ( double )lTotalChildrenTime ) / ( double )_ti.TotalTime ) * 100;
			}
		}

		public double TimeInMethodAndChildren
		{
			get
			{
				if ( _ti.TotalTime == 0 )
					return 0;

				return ( ( double )_lTotalTime / ( double )_ti.TotalTime ) * 100;
			}
		}

		public double TimeInChildren
		{
			get
			{
				if ( _lTotalTime == 0 )
					return 0;

				long lTotalChildrenTime = 0;
				foreach ( CalleeFunctionInfo cfi in _acfi )
					lTotalChildrenTime += cfi.TotalTime;

				return ( ( double )lTotalChildrenTime / ( double )_lTotalTime ) * 100;
			}
		}

		private int _nID;
		private int _nCalls;
		private long _lTotalTime;
		private long _lTotalSuspendedTime;
		private FunctionSignature _fs;
		private CalleeFunctionInfo[] _acfi;
		private ThreadInfo _ti;
	}
}
