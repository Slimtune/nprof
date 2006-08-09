using System;
using System.Xml.Serialization;
using NProf.Glue.Profiler.Info;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for ProfilerRun.
	/// </summary>
	[XmlInclude( typeof( ProcessInfo ) )]
	[Serializable]
	public class Run
	{
		public Run()
		{
			this.messages = new RunMessageCollection();
			this.processes = new ProcessInfoCollection();
		}

		public Run( Profiler p, ProjectInfo pi )
		{
			this.profiler = p;
			this.start = DateTime.Now;
			this.end = DateTime.MaxValue;
			this.runState = RunState.Initializing;
			this.processes = new ProcessInfoCollection();
			this.project = pi;
			this.messages = new RunMessageCollection();
			this.isSuccess = false;
		}

		/*public Run( DateTime dtStart, DateTime dtEnd, RunState rs, ThreadInfoCollection tic )
		{
			// When is this called?
			throw new NotImplementedException( "Fix me!" );
			_dtStart = dtStart;
			_dtEnd = dtEnd;
			_rs = rs;
			threadCollection = tic;
		}*/

		public bool Start()
		{
			start = DateTime.Now;

			return profiler.Start( project, this, new Profiler.ProcessCompletedHandler( OnProfileComplete ) );
		}

		public bool CanStop
		{
			get { return State == RunState.Initializing || 
						( project.ProjectType == ProjectType.AspNet && State != RunState.Finished ); } 
		}

		public bool Stop()
		{
			profiler.Stop();

			return true;
		}

		[XmlIgnore]
		public ProjectInfo Project
		{
			get { return project; }
			set { project = value; }
		}

		[XmlIgnore]
		public DateTime StartTime
		{
			get { return start; }
			set { start = value; }
		}

		[XmlIgnore]
		public DateTime EndTime
		{
			get { return end; }
			set { end = value; }
		}

		public RunState State
		{
			get { return runState; }
			
			set 
			{ 
				RunState rsOld = runState;
				runState = value;
				if ( StateChanged != null )
					StateChanged( this, rsOld, runState );
			}
		}

		[XmlIgnore]
		public RunMessageCollection Messages
		{
			get { return messages; }
			set { messages = value; }
		}

		public ProcessInfoCollection Processes
		{
			get { return processes; }
			set { processes = value; }
		}

		public bool Success
		{
			get { return isSuccess; }
			set { isSuccess = value; }
		}

		[Serializable]
		public enum RunState
		{
			Initializing,
			Running,
			Finished,
		}

		[XmlIgnore]
		public Profiler.ProcessCompletedHandler ProcessCompletedHandler
		{
			get { return new Profiler.ProcessCompletedHandler( OnProfileComplete ); }
		}

		private void OnProfileComplete( Run run )
		{
			State = RunState.Finished;
			end = DateTime.Now;
		}

		[field:NonSerialized]
		public event RunStateEventHandler StateChanged;

		public delegate void RunStateEventHandler( Run run, RunState rsOld, RunState rsNew );

		private bool isSuccess;
		private DateTime start;
		private DateTime end;
		private RunState runState;
		private ProjectInfo project;
		private Profiler profiler;
		private RunMessageCollection messages;
		private ProcessInfoCollection processes;
	}
}
