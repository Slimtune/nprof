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
			_pi = pi;
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

		public void Add( Run run )
		{
			_alItems.Add( run );
			if ( RunAdded != null )
				RunAdded( _pi, this, run, _alItems.Count - 1 );
		}

		public void RemoveAt( int nIndex )
		{
			Run run = this[ nIndex ];
			_alItems.RemoveAt( nIndex );
			if ( RunRemoved != null )
				RunRemoved( _pi, this, run, nIndex );
		}

		public Run this[ int nIndex ]
		{
			get { return ( Run )_alItems[ nIndex ]; }
		}
		[field:NonSerialized]
		public event RunEventHandler RunAdded;
		[field:NonSerialized]
		public event RunEventHandler RunRemoved;

		public delegate void RunEventHandler( ProjectInfo pi, RunCollection runs, Run run, int nIndex );

		private ArrayList _alItems;
		private ProjectInfo _pi;
	}
}
