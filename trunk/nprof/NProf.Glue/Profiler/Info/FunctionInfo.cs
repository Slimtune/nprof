using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionInfo.
	/// </summary>
	[Serializable]
	public class FunctionInfo
	{
		public FunctionInfo()
		{
		}

		public FunctionInfo( ThreadInfo ti, int nID, FunctionSignature signature, int calls, long totalTime, long totalRecursiveTime, long totalSuspendedTime, CalleeFunctionInfo[] callees )
		{
			this.thread = ti;
			this.id = nID;
			this.signature = signature;
			this.calls = calls;
			this.totalTime = totalTime;
			this.totalRecursiveTime = totalRecursiveTime;
			this.totalSuspendedTime = totalSuspendedTime;
			this.callees = callees;

			foreach ( CalleeFunctionInfo callee in callees )
				callee.FunctionInfo = this;
		}

		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		public FunctionSignature Signature
		{
			get { return signature; }
			set { signature = value; }
		}

		public CalleeFunctionInfo[] CalleeInfo
		{
			get { return callees; }
			set { callees = value; }
		}

		public int Calls
		{
			get { return calls; }
		}

		[XmlIgnore]
		public long ThreadTotalTicks
		{
			get { return thread.TotalTime; }
		}
		[XmlIgnore]
		public long TotalTicks
		{
			get { return totalTime; }
		}
		[XmlIgnore]
		public long TotalRecursiveTicks
		{
			get { return totalRecursiveTime; }
		}
		[XmlIgnore]
		public long TotalSuspendedTicks
		{
			get { return totalSuspendedTime; }
		}
		[XmlIgnore]
		public long TotalChildrenTicks
		{
			get
			{
				long totalChildrenTime = 0;
				foreach ( CalleeFunctionInfo callee in callees )
					totalChildrenTime += callee.TotalTime;

				return totalChildrenTime;
			}
		}
		[XmlIgnore]
		public long TotalChildrenRecursiveTicks
		{
			get
			{
				long totalChildrenRecursiveTime = 0;
				foreach ( CalleeFunctionInfo callee in callees )
					totalChildrenRecursiveTime += callee.TotalRecursiveTime;

				return totalChildrenRecursiveTime;
			}
		}

		/// <summary>
		/// Percent on time, based on thread ticks, that method is suspended.
		/// </summary>
		[XmlIgnore]
		public double PercentOfTotalTimeSuspended
		{
			get 
			{
				if ( ThreadTotalTicks == 0 )
					return 0;

				return ( 
					( double )totalSuspendedTime 
					/ 
					( double )ThreadTotalTicks ) * 100;
			}
		}

		/// <summary>
		/// Percent on time, based on method ticks (not thread ticks), that method is suspended.
		/// </summary>
		[XmlIgnore]
		public double PercentOfMethodTimeSuspended
		{
			get 
			{
				if ( TotalTicks == 0 )
					return 0;

				return ( 
					( double )totalSuspendedTime 
					/ 
					( double )TotalTicks ) * 100;
			}
		}
		
		/// <summary>
		/// Percent of time, based on total thread ticks, spent in method (not including children).
		/// </summary>
		[XmlIgnore]
		public double PercentOfTotalTimeInMethod
		{
			get
			{
				if ( ThreadTotalTicks == 0 )
					return 0;

				return ( 
					( double )( TotalTicks + TotalRecursiveTicks - TotalChildrenTicks - TotalChildrenRecursiveTicks ) 
					/ 
					( double )ThreadTotalTicks ) * 100;
			}
		}

		/// <summary>
		/// Percent of time, based on total thread ticks, spent in method and children.
		/// </summary>
		[XmlIgnore]
		public double PercentOfTotalTimeInMethodAndChildren
		{
			get
			{
				if ( ThreadTotalTicks == 0 )
					return 0;

				return ( 
					( double )TotalTicks 
					/ 
					( double )ThreadTotalTicks ) * 100;
			}
		}

		/// <summary>
		/// Percent of time, based on method ticks (not thread ticks), spent in children.
		/// </summary>
		[XmlIgnore]
		public double PercentOfMethodTimeInChildren
		{
			get
			{
				if ( TotalTicks == 0 )
					return 0;

				return ( 
					( double )( TotalChildrenTicks ) 
					/ 
					( double )( TotalTicks + TotalRecursiveTicks ) ) * 100;
			}
		}

		/// <summary>
		/// Percent of time, based on method ticks (not thread ticks), spent in method.
		/// </summary>
		[XmlIgnore]
		public double PercentOfMethodTimeInMethod
		{
			get
			{
				if (TotalTicks == 0)
					return 0;

				return (
					(double)(TotalTicks + TotalRecursiveTicks - TotalChildrenTicks - TotalChildrenRecursiveTicks)
					/
					(double)(TotalTicks + TotalRecursiveTicks)) * 100;
			}
		}

		/// <summary>
		/// Percent of time, based on thread ticks, spent in children.
		/// </summary>
		[XmlIgnore]
		public double PercentOfTotalTimeInChildren
		{
			get
			{
				if ( TotalTicks == 0 )
					return 0;

				long totalChildrenTime = 0;
				foreach ( CalleeFunctionInfo callee in callees )
					totalChildrenTime += callee.TotalTime;

				return ( ( 
					( double )( TotalChildrenTicks + TotalChildrenRecursiveTicks - TotalRecursiveTicks ) ) 
					/ 
					( double )ThreadTotalTicks ) * 100;
			}
		}

		private int id;
		private int calls;
		private long totalTime;
		private long totalRecursiveTime;
		private long totalSuspendedTime;
		private FunctionSignature signature;
		private CalleeFunctionInfo[] callees;
		private ThreadInfo thread;
	}
}
