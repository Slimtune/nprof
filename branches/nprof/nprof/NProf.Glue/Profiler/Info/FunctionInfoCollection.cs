using System;
using System.Collections;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionCollection.
	/// </summary>
	public class FunctionInfoCollection : IEnumerable
	{
		public FunctionInfoCollection()
		{
			_htFunctionInfo = new Hashtable();
		}

		public void Add( FunctionInfo fi )
		{
			_htFunctionInfo[ fi.ID ] = fi;
		}

		public FunctionInfo this[ int nFunctionID ]
		{
			get { return ( FunctionInfo )_htFunctionInfo[ nFunctionID ]; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _htFunctionInfo.Values.GetEnumerator();
		}

		Hashtable _htFunctionInfo;
	}
}
