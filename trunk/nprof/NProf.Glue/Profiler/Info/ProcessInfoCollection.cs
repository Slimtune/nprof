using System;
using System.Collections;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for ProcessInfoCollection.
	/// </summary>
	public class ProcessInfoCollection : IEnumerable
	{
		public ProcessInfoCollection()
		{
			_htProcessInfo = new Hashtable();
			_alProcessInfo = new ArrayList();
		}

		public IEnumerator GetEnumerator()
		{
			return _alProcessInfo.GetEnumerator();
		}

		public void Add( object o )
		{
			ProcessInfo pi = ( ProcessInfo )o;
			_htProcessInfo[ pi.ID ] = pi;
			_alProcessInfo.Add( pi );
		}

		public ProcessInfo this[ int nProcessID ]
		{
			get
			{
				lock ( _htProcessInfo )
				{
					ProcessInfo pi = ( ProcessInfo )_htProcessInfo[ nProcessID ];
					if ( pi == null )
					{
						pi = new ProcessInfo( nProcessID );
						_htProcessInfo[ nProcessID ] = pi;
					}

					return pi;
				}
			}
		}

		private Hashtable _htProcessInfo;	
		private ArrayList _alProcessInfo;
	}
}
