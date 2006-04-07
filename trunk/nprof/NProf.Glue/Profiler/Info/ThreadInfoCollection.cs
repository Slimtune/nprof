using System;
using System.Collections;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for Threads.
	/// </summary>
	[Serializable]
	public class ThreadInfoCollection : IEnumerable
	{
		public ThreadInfoCollection()
		{
			_htThreadInfo = new Hashtable();
		}

		public IEnumerator GetEnumerator()
		{
			return _htThreadInfo.Values.GetEnumerator();
		}

		public void Add( object o )
		{
			ThreadInfo ti = ( ThreadInfo )o;
			_htThreadInfo[ ti.ID ] = ti;
		}

		public ThreadInfo this[ int nThreadID ]
		{
			get
			{
				lock ( _htThreadInfo )
				{
					ThreadInfo ti = ( ThreadInfo )_htThreadInfo[ nThreadID ];
					if ( ti == null )
					{
						ti = new ThreadInfo( nThreadID );
						_htThreadInfo[ nThreadID ] = ti;
					}

					return ti;
				}
			}
		}

		private Hashtable _htThreadInfo;
	}
}
