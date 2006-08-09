using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for ProjectInfo.
	/// </summary>
	[Serializable]
	public class ProjectInfo
	{
		public ProjectInfo() : this(ProjectType.File) { } // JC: added default constructor for serialization

		public ProjectInfo( ProjectType projectType )
		{
			this.options = new Options();
			this.name = null;
			this.runs = new RunCollection( this );
			this.projectType = projectType;
		}

		public Options Options
		{
			get { return options; }
			set { options = value; } // JC: added for serialization support
		}

		public string Name
		{
			get { return name; }
			set { name = value; Fire_ProjectInfoChanged(); }
		}

		public string ApplicationName
		{
			get { return appName; }
			set { appName = value; Fire_ProjectInfoChanged(); }
		}

		public string Arguments
		{
			get { return arguments; }
			set { arguments = value; Fire_ProjectInfoChanged(); }
		}

		public string WorkingDirectory
		{
			get { return workingDirectory; }
			set { workingDirectory = value; Fire_ProjectInfoChanged(); }
		}

		[XmlIgnore()] // JC: prevent saving of runs collection
		public RunCollection Runs
		{
			get { return runs; }
		}

		public Run CreateRun( Profiler p )
		{
			Run run = new Run( p, this );
			runs.Add( run );
			
			return run;
		}

		public ProjectType ProjectType
		{
			get { return projectType; }
			set { projectType = value; }
		}

		private void Fire_ProjectInfoChanged()
		{
			if ( ProjectInfoChanged != null )
				ProjectInfoChanged( this );
		}
		[field:NonSerialized]
		public event ProjectEventHandler ProjectInfoChanged;

		public delegate void ProjectEventHandler( ProjectInfo project );

		private Options options;
		private string appName;
		private string arguments;
		private string workingDirectory;
		private string name;
		private RunCollection runs;
		private ProjectType projectType;
	}
}
