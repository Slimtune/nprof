using System;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for ProjectInfo.
	/// </summary>
	public class ProjectInfo
	{
		public ProjectInfo()
		{
			_po = new Options();
			_strName = null;
			_rc = new RunCollection( this );
			_bSpecial = false;
		}

		public ProjectInfo( bool bSpecial ) : this()
		{
			_bSpecial = true;
		}

		public Options Options
		{
			get { return _po; }
		}

		public string Name
		{
			get { return _strName; }
			set { _strName = value; }
		}

		public string ApplicationName
		{
			get { return _strAppName; }
			set { _strAppName = value; }
		}

		public string Arguments
		{
			get { return _strArguments; }
			set { _strArguments = value; }
		}

		public string WorkingDirectory
		{
			get { return _strWorkingDirectory; }
			set { _strWorkingDirectory = value; }
		}

		public RunCollection Runs
		{
			get { return _rc; }
		}

		public Run CreateRun( Profiler p )
		{
			Run run = new Run( p, this );
			_rc.Add( run );
			
			return run;
		}

		public bool Special
		{
			get { return _bSpecial; }
		}

		private Options _po;
		private string _strAppName, _strArguments, _strWorkingDirectory;
		private string _strName;
		private RunCollection _rc;
		private bool _bSpecial;
	}
}
