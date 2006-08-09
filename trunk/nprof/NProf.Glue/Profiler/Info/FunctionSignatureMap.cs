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
			signatures = new Hashtable();
		}

		public void MapSignature( int functionID, FunctionSignature signature )
		{
			lock (signatures.SyncRoot)
			{
				signatures[functionID] = signature;
			}
		}

		public string GetFunctionSignature( int functionID )
		{
			lock ( signatures.SyncRoot )
			{
				FunctionSignature signature = ( FunctionSignature )signatures[ functionID ];
				if (signature == null)
				{
					return "Unknown!";
				}

				return signature.Signature;
			}
		}

		private Hashtable signatures;
	}
}
