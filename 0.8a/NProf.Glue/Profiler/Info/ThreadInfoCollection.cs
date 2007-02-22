using System;
using System.Collections;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for Threads.
	/// </summary>
	public class ThreadInfoCollection : IEnumerable
	{
		public ThreadInfoCollection()
		{
			_htThreadInfo = new Hashtable();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _htThreadInfo.Values.GetEnumerator();
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
