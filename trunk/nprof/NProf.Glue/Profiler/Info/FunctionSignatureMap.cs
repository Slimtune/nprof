using System;
using System.Collections;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionSignatureMap.
	/// </summary>
	[Serializable]
	public class FunctionSignatureMap
	{
		public FunctionSignatureMap()
		{
			_htSignatureMap = new Hashtable();
		}

		public void MapSignature( int nFunctionID, FunctionSignature fs )
		{
			lock ( _htSignatureMap.SyncRoot )
				_htSignatureMap[ nFunctionID ] = fs;
		}

		public string GetFunctionSignature( int nFunctionID )
		{
			lock ( _htSignatureMap.SyncRoot )
			{
				FunctionSignature fs = ( FunctionSignature )_htSignatureMap[ nFunctionID ];
				if ( fs == null )
					return "Unknown!";

				return fs.Signature;
			}
		}

		private Hashtable _htSignatureMap;
	}
}
