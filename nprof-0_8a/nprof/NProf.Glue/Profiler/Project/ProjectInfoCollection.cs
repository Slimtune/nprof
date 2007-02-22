using System;
using System.Collections;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for ProjectInfoCollection.
	/// </summary>
	public class ProjectInfoCollection : IEnumerable
	{
		public ProjectInfoCollection()
		{
			_nNewProjectIndex = 0;
			_alItems = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _alItems.GetEnumerator();
		}

		public int Count
		{
			get { return _alItems.Count; }
		}

		public void RemoveAt( int nIndex )
		{
			ProjectInfo pi = this[ nIndex ];
			_alItems.RemoveAt( nIndex );
			if ( ProjectRemoved != null )
				ProjectRemoved( this, pi, nIndex );
		}

		public void Add( ProjectInfo pi )
		{
			if ( pi.Name == null )
				pi.Name = GetNewProjectName();

			_alItems.Add( pi );
			if ( ProjectAdded != null )
				ProjectAdded( this, pi, _alItems.Count - 1 );
		}

		public ProjectInfo this[ int nIndex ]
		{
			get { return ( ProjectInfo )_alItems[ nIndex ]; }
		}

		private string GetNewProjectName()
		{
			_nNewProjectIndex++;
			return "Untitled Project #" + _nNewProjectIndex;
		}

		public event ProjectEventHandler ProjectAdded;
		public event ProjectEventHandler ProjectRemoved;

		public delegate void ProjectEventHandler( ProjectInfoCollection projects, ProjectInfo project, int nIndex );

		private ArrayList _alItems;
		private int _nNewProjectIndex;
	}
}
