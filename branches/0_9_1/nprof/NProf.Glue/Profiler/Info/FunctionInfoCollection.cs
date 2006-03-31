using System;
using System.Collections;
using System.Xml.Serialization;

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

		public void Add( object o )
		{
			Add( ( FunctionInfo )o );
		}

		public void Add( FunctionInfo fi )
		{
			_htFunctionInfo[ fi.ID ] = fi;
		}

		public FunctionInfo this[ int nFunctionID ]
		{
			get { return ( FunctionInfo )_htFunctionInfo[ nFunctionID ]; }
		}

		public IEnumerator GetEnumerator()
		{
			return _htFunctionInfo.Values.GetEnumerator();
		}

		Hashtable _htFunctionInfo;
	}
}
