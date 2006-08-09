using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for Threads.
	/// </summary>
	[Serializable]
	public class ThreadInfoCollection : IEnumerable
	{
		public IEnumerator GetEnumerator()
		{
			return threads.Values.GetEnumerator();
		}
		public ThreadInfo this[ int threadId ]
		{
			get
			{
				lock ( threads )
				{
					ThreadInfo thread;
					if(!threads.TryGetValue(threadId,out thread))
					//ThreadInfo thread = (ThreadInfo)threads[threadId];
					//if (thread == null)
					{
						thread = new ThreadInfo( threadId );
						threads[ threadId ] = thread;
					}
					return thread;
				}
			}
		}
		private Dictionary<int,ThreadInfo> threads=new Dictionary<int,ThreadInfo>();
	}
}
