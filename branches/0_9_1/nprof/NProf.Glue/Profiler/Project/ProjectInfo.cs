using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for ProjectInfo.
	/// </summary>
	public class ProjectInfo
	{
		public ProjectInfo() : this(ProjectType.File) { } // JC: added default constructor for serialization

		public ProjectInfo( ProjectType pt )
		{
			_po = new Options();
			_strName = null;
			_rc = new RunCollection( this );
			_pt = pt;
		}

		public Options Options
		{
			get { return _po; }
			set { _po = value; } // JC: added for serialization support
		}

		public string Name
		{
			get { return _strName; }
			set { _strName = value; Fire_ProjectInfoChanged(); }
		}

		public string ApplicationName
		{
			get { return _strAppName; }
			set { _strAppName = value; Fire_ProjectInfoChanged(); }
		}

		public string Arguments
		{
			get { return _strArguments; }
			set { _strArguments = value; Fire_ProjectInfoChanged(); }
		}

		public string WorkingDirectory
		{
			get { return _strWorkingDirectory; }
			set { _strWorkingDirectory = value; Fire_ProjectInfoChanged(); }
		}

		[XmlIgnore()] // JC: prevent saving of runs collection
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

		public ProjectType ProjectType
		{
			get { return _pt; }
			set { _pt = value; }
		}

		private void Fire_ProjectInfoChanged()
		{
			if ( ProjectInfoChanged != null )
				ProjectInfoChanged( this );
		}

		public event ProjectEventHandler ProjectInfoChanged;

		public delegate void ProjectEventHandler( ProjectInfo project );

		private Options _po;
		private string _strAppName, _strArguments, _strWorkingDirectory;
		private string _strName;
		private RunCollection _rc;
		private ProjectType _pt;
	}
}
