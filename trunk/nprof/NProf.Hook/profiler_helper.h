#pragma once

class ProfilerHelper
{
public:
  ProfilerHelper();
  ~ProfilerHelper(void);
  void Initialize( ICorProfilerInfo* profilerInfo ); 
  
  void GetFunctionSignature(   
    FunctionID functionId, 
    UINT32& methodAttributes, 
    string& returnType, 
    string& className,
    string& functionName,
    string& parameters );
  ThreadID GetCurrentThreadID();
  string ProfilerHelper::GetCurrentThreadName();
private:
  HRESULT GetFunctionProperties( FunctionID functionID,
										    UINT32* methodAttributes,
										    ULONG* argCount,
										    WCHAR* returnType, 
										    WCHAR* parameters,
										    WCHAR* className,
                        WCHAR* functionName );
  PCCOR_SIGNATURE ParseElementType( IMetaDataImport* metaDataImport,
											    PCCOR_SIGNATURE signature, 
											    char* buffer );
  CComPtr< ICorProfilerInfo > profilerInfo;
};
