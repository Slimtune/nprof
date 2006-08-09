using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	[Serializable]
	public class ProcessInfoCollection : IEnumerable
	{
		public ProcessInfoCollection()
		{
			processes = new Dictionary<int,ProcessInfo>();
		}

		public IEnumerator GetEnumerator()
		{
			return processes.Values.GetEnumerator();
		}

		public void Add(ProcessInfo process)
		{
			processes[process.ID] = process;
		}

		public ProcessInfo this[int nProcessID]
		{
			get
			{
				lock (processes)
				{
					ProcessInfo pi = (ProcessInfo)processes[nProcessID];
					if (pi == null)
					{
						pi = new ProcessInfo(nProcessID);
						processes[nProcessID] = pi;
					}

					return pi;
				}
			}
		}
		private Dictionary<int,ProcessInfo> processes;
	}
}
