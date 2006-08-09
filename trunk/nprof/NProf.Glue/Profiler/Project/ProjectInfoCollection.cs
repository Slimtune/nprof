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
			newProjectIndex = 0;
			items = new ArrayList();
		}

		#region IListSource implementation

		IList IListSource.GetList()
		{
			return items;
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
			return items.GetEnumerator();
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
			get { return ( ProjectInfo )items[ nIndex ]; }
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
			int index = items.Count;

			this.Insert( index, pi );

			return index;
		}

		void IList.Clear()
		{
			items.Clear();
		}

		bool IList.Contains( object o )
		{
			return this.Contains( o as ProjectInfo );
		}

		public bool Contains( ProjectInfo pi )
		{
			return items.Contains(pi);
		}

		public int Count
		{
			get { return items.Count; }
		}

		int IList.IndexOf( object o )
		{
			return this.IndexOf( o as ProjectInfo );
		}

		public int IndexOf( ProjectInfo pi )
		{
			return items.IndexOf( pi );
		}

		void IList.Insert( int index, object o )
		{
			this.Insert( index, o as ProjectInfo );
		}

		public void Insert( int index, ProjectInfo pi )
		{
			if ( pi.Name == null )
				pi.Name = GetNewProjectName();

			items.Insert( index, pi );

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
			items.RemoveAt( nIndex );
			if ( ProjectRemoved != null )
				ProjectRemoved( this, pi, nIndex );
		}

		private string GetNewProjectName()
		{
			++newProjectIndex;
			return "Untitled Project #" + newProjectIndex;
		}

		#endregion

		#region ICollection implementation

		object ICollection.SyncRoot
		{
			get { return items.SyncRoot; }
		}

		bool ICollection.IsSynchronized
		{
			get { return false; }
		}

		void ICollection.CopyTo( Array ary, int startIndex )
		{
			items.CopyTo( ary, startIndex );
		}

		#endregion

		public event ProjectEventHandler ProjectAdded;
		public event ProjectEventHandler ProjectRemoved;

		public delegate void ProjectEventHandler( ProjectInfoCollection projects, ProjectInfo project, int nIndex );

		private ArrayList items;
		private int newProjectIndex;
	}
}
