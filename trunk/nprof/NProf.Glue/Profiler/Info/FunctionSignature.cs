using System;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionSignature.
	/// </summary>
	[Serializable]
	public class FunctionSignature
	{
		public FunctionSignature()
		{
		}

		public FunctionSignature( UInt32 methodAttributes, string returnType, string className, string functionName, string parameters )
		{
			CorMethodAttr cma = ( CorMethodAttr )methodAttributes;
			this.isPInvoke = ( cma & CorMethodAttr.mdPinvokeImpl ) != 0;
			this.isStatic = ( cma & CorMethodAttr.mdStatic ) != 0;
			this.isExtern = ( cma & CorMethodAttr.mdUnmanagedExport ) != 0;

			this.returnType = returnType;
			this.className = className;
			this.functionName = functionName;
			this.parameters = parameters;
		}

		public bool IsPInvoke
		{
			get { return isPInvoke; }
			set { isPInvoke = value; }
		}

		public bool IsStatic
		{
			get { return isStatic; }
			set { isStatic = value; }
		}

		public bool IsExtern
		{
			get { return isExtern; }
			set { isExtern = value; }
		}

		public string ReturnType
		{
			get { return returnType; }
			set { returnType = value; }
		}

		public string ClassName
		{
			get { return className; }
			set { className = value; }
		}

		public string FunctionName
		{
			get { return functionName; }
			set { functionName = value; }
		}

		public string Parameters
		{
			get { return parameters; }
			set { parameters = value; }
		}

		public string[] Namespace
		{
			get
			{
				string[] astrPieces = className.Split( '.' );
				string[] astrNamespace = new String[ astrPieces.Length - 1 ];
				Array.Copy( astrPieces, 0, astrNamespace, 0, astrPieces.Length - 1 );

				return astrNamespace;
			}
		}

		public string NamespaceString
		{
			get { return String.Join( ".", Namespace ); }
		}

		/// <summary>
		/// Gets the human-readable form of the function signature.
		/// </summary>
		public string Signature
		{
			get
			{
				return String.Format( "{0}{1}{2}{3} {4}.{5}({6})", 
					isExtern ? "extern " : String.Empty,
					isPInvoke ? "pinvoke " : String.Empty,
					isStatic ? "static " : String.Empty,
					returnType,
					className,
					functionName,
					parameters );
			}
		}

		[Flags]
		enum CorMethodAttr
		{
			// member access mask - Use this mask to retrieve accessibility information.
			mdMemberAccessMask          =   0x0007,
			mdPrivateScope              =   0x0000,     // Member not referenceable.
			mdPrivate                   =   0x0001,     // Accessible only by the parent type.  
			mdFamANDAssem               =   0x0002,     // Accessible by sub-types only in this Assembly.
			mdAssem                     =   0x0003,     // Accessibly by anyone in the Assembly.
			mdFamily                    =   0x0004,     // Accessible only by type and sub-types.    
			mdFamORAssem                =   0x0005,     // Accessibly by sub-types anywhere, plus anyone in assembly.
			mdPublic                    =   0x0006,     // Accessibly by anyone who has visibility to this scope.    
			// end member access mask

			// method contract attributes.
			mdStatic                    =   0x0010,     // Defined on type, else per instance.
			mdFinal                     =   0x0020,     // Method may not be overridden.
			mdVirtual                   =   0x0040,     // Method virtual.
			mdHideBySig                 =   0x0080,     // Method hides by name+sig, else just by name.

			// vtable layout mask - Use this mask to retrieve vtable attributes.
			mdVtableLayoutMask          =   0x0100,
			mdReuseSlot                 =   0x0000,     // The default.
			mdNewSlot                   =   0x0100,     // Method always gets a new slot in the vtable.
			// end vtable layout mask

			// method implementation attributes.
			mdAbstract                  =   0x0400,     // Method does not provide an implementation.
			mdSpecialName               =   0x0800,     // Method is special.  Name describes how.

			// interop attributes
			mdPinvokeImpl               =   0x2000,     // Implementation is forwarded through pinvoke.
			mdUnmanagedExport           =   0x0008,     // Managed method exported via thunk to unmanaged code.

			// Reserved flags for runtime use only.
			mdReservedMask              =   0xd000,
			mdRTSpecialName             =   0x1000,     // Runtime should check name encoding.
			mdHasSecurity               =   0x4000,     // Method has security associate with it.
			mdRequireSecObject          =   0x8000,     // Method calls another method containing security code.

		} ;

		private bool isPInvoke;
		private bool isStatic;
		private bool isExtern;
		private string returnType;
		private string className;
		private string functionName;
		private string parameters;
	}
}
