using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace NProf.Glue.Profiler.Info
{
	[Serializable]
	public class FunctionInfoCollection : Dictionary<int,FunctionInfo>
	{
		public FunctionInfoCollection()
		{
		}
		public FunctionInfoCollection(SerializationInfo info,StreamingContext context):base(info,context)
		{
		}
	}
}
