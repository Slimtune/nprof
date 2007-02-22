using System;

namespace NProf.Glue.Profiler.Core
{
	/// <summary>
	/// Summary description for CorEventDispatcher.
	/// </summary>
	public class CorEventDispatcher
	{
		public CorEventDispatcher()
		{
		}

		public delegate void AppDomainEvent( int nAppDomainID );
		public delegate void ThreadEvent( int nThreadID );
	}
}
