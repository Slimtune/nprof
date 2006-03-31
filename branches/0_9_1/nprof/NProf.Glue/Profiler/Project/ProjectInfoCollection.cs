using System;
using System.Collections;
using System.ComponentModel;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Summary description for ProjectInfoCollection.
	/// </summary>
	public class ProjectInfoCollection : IList, IListSource
	{
		public ProjectInfoCollection()
		{
			_nNewProjectIndex = 0;
			_alItems = new ArrayList();
		}

		#region IListSource implementation

		IList IListSource.GetList()
		{
			return _alItems;
		}

		bool IListSource.ContainsListCollection
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _alItems.GetEnumerator();
		}

		#endregion

		#region IList implementation

		object IList.this[ int index ]
		{
			get { return this[index]; }
			set { throw new NotImplementedException(); }
		}

		public ProjectInfo this[ int nIndex ]
		{
			get { return ( ProjectInfo )_alItems[ nIndex ]; }
		}

		bool IList.IsReadOnly
		{
			get { return false; }
		}

		bool IList.IsFixedSize
		{
			get { return false; }
		}

		int IList.Add( object o )
		{
			return this.Add( o as ProjectInfo );
		}

		public int Add( ProjectInfo pi )
		{
			int index = _alItems.Count;

			this.Insert( index, pi );

			return index;
		}

		void IList.Clear()
		{
			_alItems.Clear();
		}

		bool IList.Contains( object o )
		{
			return this.Contains( o as ProjectInfo );
		}

		public bool Contains( ProjectInfo pi )
		{
			return _alItems.Contains(pi);
		}

		public int Count
		{
			get { return _alItems.Count; }
		}

		int IList.IndexOf( object o )
		{
			return this.IndexOf( o as ProjectInfo );
		}

		public int IndexOf( ProjectInfo pi )
		{
			return _alItems.IndexOf( pi );
		}

		void IList.Insert( int index, object o )
		{
			this.Insert( index, o as ProjectInfo );
		}

		public void Insert( int index, ProjectInfo pi )
		{
			if ( pi.Name == null )
				pi.Name = GetNewProjectName();

			_alItems.Insert( index, pi );

			if ( ProjectAdded != null )
				ProjectAdded( this, pi, index );
		}

		void IList.Remove( object o )
		{
			this.Remove( o as ProjectInfo );
		}

		public void Remove( ProjectInfo pi )
		{
			this.RemoveAt( this.IndexOf( pi ) );
		}

		public void RemoveAt( int nIndex )
		{
			ProjectInfo pi = this[ nIndex ];
			_alItems.RemoveAt( nIndex );
			if ( ProjectRemoved != null )
				ProjectRemoved( this, pi, nIndex );
		}

		private string GetNewProjectName()
		{
			++_nNewProjectIndex;
			return "Untitled Project #" + _nNewProjectIndex;
		}

		#endregion

		#region ICollection implementation

		object ICollection.SyncRoot
		{
			get { return _alItems.SyncRoot; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		void ICollection.CopyTo( Array ary, int startIndex )
		{
			_alItems.CopyTo( ary, startIndex );
		}

		#endregion

		public event ProjectEventHandler ProjectAdded;
		public event ProjectEventHandler ProjectRemoved;

		public delegate void ProjectEventHandler( ProjectInfoCollection projects, ProjectInfo project, int nIndex );

		private ArrayList _alItems;
		private int _nNewProjectIndex;
	}
}
