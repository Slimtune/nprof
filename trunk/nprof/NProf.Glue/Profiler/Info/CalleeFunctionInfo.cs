using System;
using System.Xml.Serialization;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionInfo.
	/// </summary>
	[Serializable]
	public class CalleeFunctionInfo
	{
		public CalleeFunctionInfo()
		{
			signatures = new FunctionSignatureMap();
		}
		public CalleeFunctionInfo( FunctionSignatureMap signatures, int id, int calls, long totalTime, long totalRecursiveTime)
		{
			this.id = id;
			this.calls = calls;
			this.totalTime = totalTime;
			this.totalRecursiveTime = totalRecursiveTime;
			this.signatures = signatures;
		}
		public int ID
		{
			get { return id; }
			set { id = value; }
		}
		public string Signature
		{
			get { return signatures.GetFunctionSignature( ID ); }
		}
		public int Calls
		{
			get { return calls; }
			set { calls = value; }
		}
		[XmlIgnore]
		public long TotalTime
		{
			get { return totalTime; }
			set { totalTime = value; }
		}
		[XmlIgnore]
		public long TotalRecursiveTime
		{
			get { return totalRecursiveTime; }
			set { totalRecursiveTime = value; }
		}
		[XmlIgnore]
		public double PercentOfTotalTimeInMethod
		{
			get { return ( double )totalTime / ( double )function.ThreadTotalTicks * 100; }
		}
		[XmlIgnore]
		public double PercentOfParentTimeInMethod
		{
			get { return ( double )totalTime / ( double )function.TotalTicks * 100; }
		}
		internal FunctionInfo FunctionInfo
		{
			set { function = value; }
		}
		private int id;
		private int calls;
		private FunctionInfo function;
		private FunctionSignatureMap signatures;
		private long totalTime;
		private long totalRecursiveTime;
	}
}
