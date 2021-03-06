using System;
using NProf.Glue.Profiler.Info;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for ProfilerRun.
	/// </summary>
	public class Run
	{
		public Run( Profiler p, ProjectInfo pi )
		{
			_p = p;
			_dtStart = DateTime.Now;
			_dtEnd = DateTime.MaxValue;
			_rs = RunState.Initializing;
			_tic = new ThreadInfoCollection();
			_pi = pi;
			_rmcMessages = new RunMessageCollection();
			_bSuccess = false;
		}

		/*public Run( DateTime dtStart, DateTime dtEnd, RunState rs, ThreadInfoCollection tic )
		{
			// When is this called?
			throw new NotImplementedException( "Fix me!" );
			_dtStart = dtStart;
			_dtEnd = dtEnd;
			_rs = rs;
			_tic = tic;
		}*/

		public bool Start()
		{
			_dtStart = DateTime.Now;

			return _p.Start( _pi, this, new Profiler.ProcessCompletedHandler( OnProfileComplete ) );
		}

		public bool CanStop
		{
			get { return State == RunState.Initializing || 
						( _pi.ProjectType == ProjectType.AspNet && State != RunState.Finished ); } 
		}

		public bool Stop()
		{
			_p.Stop();

			return true;
		}

		public ProjectInfo Project
		{
			get { return _pi; }
		}

		public DateTime StartTime
		{
			get { return _dtStart; }
		}

		public DateTime EndTime
		{
			get { return _dtEnd; }
			set { _dtEnd = value; }
		}

		public RunState State
		{
			get { return _rs; }
			
			set 
			{ 
				RunState rsOld = _rs;
				_rs = value;
				if ( StateChanged != null )
					StateChanged( this, rsOld, _rs );
			}
		}

		public RunMessageCollection Messages
		{
			get { return _rmcMessages; }
			set { _rmcMessages = value; }
		}

		public ThreadInfoCollection ThreadInfoCollection
		{
			get { return _tic; }
			set { _tic = value; }
		}

		public bool Success
		{
			get { return _bSuccess; }
			set { _bSuccess = value; }
		}

		public enum RunState
		{
			Initializing,
			Running,
			Finished,
		}

		public Profiler.ProcessCompletedHandler ProcessCompletedHandler
		{
			get { return new Profiler.ProcessCompletedHandler( OnProfileComplete ); }
		}

		private void OnProfileComplete( Run run )
		{
			State = RunState.Finished;
			_dtEnd = DateTime.Now;
		}

		public event RunStateEventHandler StateChanged;

		public delegate void RunStateEventHandler( Run run, RunState rsOld, RunState rsNew );

		private bool _bSuccess;
		private DateTime _dtStart;
		private DateTime _dtEnd;
		private RunState _rs;
		private ThreadInfoCollection _tic;
		private ProjectInfo _pi;
		private Profiler _p;
		private RunMessageCollection _rmcMessages;
	}
}
