using System;
using System.Collections;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for RunCollection.
	/// </summary>
	[Serializable]
	public class RunCollection : IEnumerable
	{
		public RunCollection( ProjectInfo pi )
		{
			this.project = pi;
			items = new ArrayList();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		public int Count
		{
			get { return items.Count; }
		}

		public void Add( Run run )
		{
			items.Add( run );
			if ( RunAdded != null )
				RunAdded( project, this, run, items.Count - 1 );
		}

		public void RemoveAt( int index )
		{
			Run run = this[ index ];
			items.RemoveAt( index );
			if ( RunRemoved != null )
				RunRemoved( project, this, run, index );
		}

		public Run this[ int nIndex ]
		{
			get { return ( Run )items[ nIndex ]; }
		}
		[field:NonSerialized]
		public event RunEventHandler RunAdded;
		[field:NonSerialized]
		public event RunEventHandler RunRemoved;

		public delegate void RunEventHandler( ProjectInfo pi, RunCollection runs, Run run, int nIndex );

		private ArrayList items;
		private ProjectInfo project;
	}
}
