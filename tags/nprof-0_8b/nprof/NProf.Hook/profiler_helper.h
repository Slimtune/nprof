#pragma once

class ProfilerHelper
{
public:
  ProfilerHelper();
  ~ProfilerHelper(void);
  void Initialize( ICorProfilerInfo* pPrfInfo ); 
  
  void GetFunctionSignature(   
    FunctionID fid, 
    UINT32& uiMethodAttributes, 
    string& strRet, 
    string& strClassName,
    string& strFnName,
    string& strParameters );
  ThreadID GetCurrentThreadID();
  string ProfilerHelper::GetCurrentThreadName();
private:
  HRESULT GetFunctionProperties( FunctionID functionID,
										    UINT32* uiMethodAttr,
										    ULONG *argCount,
										    WCHAR *returnTypeStr, 
										    WCHAR *functionParameters,
										    WCHAR *className,
                        WCHAR *funName );
  PCCOR_SIGNATURE ParseElementType( IMetaDataImport *pMDImport,
											    PCCOR_SIGNATURE signature, 
											    char *buffer );
  CComPtr< ICorProfilerInfo > _pPrfInfo;
};
