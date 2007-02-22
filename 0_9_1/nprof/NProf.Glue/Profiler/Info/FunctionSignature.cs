using System;

namespace NProf.Glue.Profiler.Info
{
	/// <summary>
	/// Summary description for FunctionSignature.
	/// </summary>
	public class FunctionSignature
	{
		public FunctionSignature()
		{
		}

		public FunctionSignature( UInt32 uiMethodAttributes, string strReturnType, string strClassName, string strFunctionName, string strParameters )
		{
			CorMethodAttr cma = ( CorMethodAttr )uiMethodAttributes;
			_bIsPInvoke = ( cma & CorMethodAttr.mdPinvokeImpl ) != 0;
			_bIsStatic = ( cma & CorMethodAttr.mdStatic ) != 0;
			_bIsExtern = ( cma & CorMethodAttr.mdUnmanagedExport ) != 0;

			_strReturnType = strReturnType;
			_strClassName = strClassName;
			_strFunctionName = strFunctionName;
			_strParameters = strParameters;
		}

		public bool IsPInvoke
		{
			get { return _bIsPInvoke; }
			set { _bIsPInvoke = value; }
		}

		public bool IsStatic
		{
			get { return _bIsStatic; }
			set { _bIsStatic = value; }
		}

		public bool IsExtern
		{
			get { return _bIsExtern; }
			set { _bIsExtern = value; }
		}

		public string ReturnType
		{
			get { return _strReturnType; }
			set { _strReturnType = value; }
		}

		public string ClassName
		{
			get { return _strClassName; }
			set { _strClassName = value; }
		}

		public string FunctionName
		{
			get { return _strFunctionName; }
			set { _strFunctionName = value; }
		}

		public string Parameters
		{
			get { return _strParameters; }
			set { _strParameters = value; }
		}

		public string[] Namespace
		{
			get
			{
				string[] astrPieces = _strClassName.Split( '.' );
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
				return String.Format( "{0}{1}{2}{3} {4}::{5}({6})", 
					_bIsExtern ? "extern " : String.Empty,
					_bIsPInvoke ? "pinvoke " : String.Empty,
					_bIsStatic ? "static " : String.Empty,
					_strReturnType,
					_strClassName,
					_strFunctionName,
					_strParameters );
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

		private bool _bIsPInvoke, _bIsStatic, _bIsExtern;
		private string _strReturnType, _strClassName, _strFunctionName, _strParameters;
	}
}
