#include "stdafx.h"
#include "profiler_helper.h"

ProfilerHelper::ProfilerHelper()
{
}

ProfilerHelper::~ProfilerHelper(void)
{
}

void ProfilerHelper::Initialize(  ICorProfilerInfo* pPrfInfo )
{
  _pPrfInfo = pPrfInfo;
}

ThreadID ProfilerHelper::GetCurrentThreadID()
{
  ThreadID tid;
  _pPrfInfo->GetCurrentThreadID( &tid );

  return tid;
}

string ProfilerHelper::GetCurrentThreadName()
{
  return "";
}

void ProfilerHelper::GetFunctionSignature( 
  FunctionID fid,
  UINT32& uiMethodAttributes,
  string& strRet, 
  string& strClassName,
  string& strFnName,
  string& strParameters )
{
    ULONG ulArgs;
    WCHAR wszRet[ MAX_FUNCTION_LENGTH ];
    WCHAR wszParams[ MAX_FUNCTION_LENGTH ];
    WCHAR wszFnName[ MAX_FUNCTION_LENGTH ];
    WCHAR wszClassName[ MAX_FUNCTION_LENGTH ];
    GetFunctionProperties( fid, &uiMethodAttributes, &ulArgs, wszRet, wszParams, wszClassName, wszFnName );
    
    strRet = CW2A( wszRet );
    strParameters = CW2A( wszParams );
    strClassName = CW2A( wszClassName );
    strFnName = CW2A( wszFnName );
}

/* static public */
HRESULT ProfilerHelper::GetFunctionProperties( 
                       FunctionID functionID,
										   UINT32* uiMethodAttr,
										   ULONG *argCount,
										   WCHAR *returnTypeStr, 
										   WCHAR *functionParameters,
										   WCHAR *className,
                       WCHAR *funName )
{
    HRESULT hr = E_FAIL; // assume success
	
		

	// init return values
	*argCount = 0;
	returnTypeStr[0] = NULL; 
	functionParameters[0] = NULL;
	funName[0] = NULL;
  className[0] = NULL;



    if ( functionID != NULL )
	{
	    mdToken	token;
		ClassID classID;
    ModuleID moduleID;
		IMetaDataImport *pMDImport = NULL;	
    mdToken moduleToken;
			    
			    
	    
	    //
		// Get the classID 
		//
		try
		{
			hr = _pPrfInfo->GetFunctionInfo( functionID,
											&classID,
						&moduleID,
						&moduleToken );
		}
		catch ( ... )
		{
			hr = E_FAIL;
		}

	    if ( FAILED( hr ) )
		{
			hr = S_OK;
			swprintf( funName, L"FAILED" );	
		}
		else
		{
		    //
			// Get the MetadataImport interface and the metadata token 
			//
			hr = _pPrfInfo->GetTokenAndMetaDataFromFunction( functionID, 
			           								 		IID_IMetaDataImport, 
															(IUnknown **)&pMDImport,
															&token );
			if ( SUCCEEDED( hr ) )
			{
				hr = pMDImport->GetMethodProps( token,
											    NULL,
											    funName,
											    MAX_FUNCTION_LENGTH,
											    0,
											    0,
											    NULL,
											    NULL,
											    NULL, 
											    NULL );
				if ( SUCCEEDED( hr ) )
				{
					mdTypeDef classToken = NULL;

					hr = _pPrfInfo->GetClassIDInfo( classID, 
						        				   NULL,  
						                           &classToken );
					
					if SUCCEEDED( hr )
					{
				      	if ( classToken != mdTypeDefNil )
						{
				          	hr = pMDImport->GetTypeDefProps( classToken, 
								                             className, 
								                             MAX_FUNCTION_LENGTH,
								                             NULL, 
								                             NULL, 
								                             NULL ); 
						}

					    DWORD methodAttr = 0;
						PCCOR_SIGNATURE sigBlob = NULL;


					    hr = pMDImport->GetMethodProps( (mdMethodDef) token,
					                                    NULL,
													    NULL,
													    0,
					                                    NULL,
					                                    &methodAttr,
					                                    &sigBlob,
					                                    NULL,
					                                    NULL,
					                                    NULL );
						if ( SUCCEEDED( hr ) )
						{
						    ULONG callConv;


							//
							// Is the method static ?
							//
              *uiMethodAttr = methodAttr;

					     	//
						    // Make sure we have a method signature.
						    //
							char buffer[2 * MAX_FUNCTION_LENGTH];
						    
						    
						    sigBlob += CorSigUncompressData( sigBlob, &callConv );
						    if ( callConv != IMAGE_CEE_CS_CALLCONV_FIELD )
							{
								static WCHAR* callConvNames[8] = 
								{	
									L"", 
									L"unmanaged cdecl ", 
									L"unmanaged stdcall ",	
									L"unmanaged thiscall ",	
									L"unmanaged fastcall ",	
									L"vararg ",	 
									L"<error> "	 
									L"<error> "	 
								};	
								buffer[0] = '\0';
								if ( (callConv & 7) != 0 )
									sprintf( buffer, "%s ", callConvNames[callConv & 7]);	
								
								//
								// Grab the argument count
								//
								sigBlob += CorSigUncompressData( sigBlob, argCount );

								//
							    // Get the return type
							    //
								sigBlob = ParseElementType( pMDImport, sigBlob, buffer );

								//
								// if the return typ returned back empty, write void
								//
								if ( buffer[0] == '\0' )
									sprintf( buffer, "void" );

								swprintf( returnTypeStr, L"%S",buffer );
								
								//
								// Get the parameters
								//								
								for ( ULONG i = 0; 
									  (SUCCEEDED( hr ) && (sigBlob != NULL) && (i < (*argCount))); 
									  i++ )
								{
									buffer[0] = '\0';

									sigBlob = ParseElementType( pMDImport, sigBlob, buffer );									
									if ( i == 0 )
										swprintf( functionParameters, L"%S", buffer );

									else if ( sigBlob != NULL )
										swprintf( functionParameters, L"%s, %S", functionParameters, buffer );
									
									else
										hr = E_FAIL;
								}								    								
							}
							else
							{
								//
							    // Get the return type
							    //
								buffer[0] = '\0';
								sigBlob = ParseElementType( pMDImport, sigBlob, buffer );
								swprintf( returnTypeStr, L"%s %S",returnTypeStr, buffer );
							}
						} 
					} 
				} 

				pMDImport->Release();
			} 		
		}
	}
	//
	// This corresponds to an unmanaged frame
	//
	else
	{
		//
		// Set up return parameters
		//
		hr = S_OK;
		swprintf( funName, L"UNMANAGED FRAME" );	
	}

	
	return hr;

} // BASEHELPER::GetFunctionProperties

/* static public */
PCCOR_SIGNATURE ProfilerHelper::ParseElementType( IMetaDataImport *pMDImport,
											  PCCOR_SIGNATURE signature, 
											  char *buffer )
{	
	switch ( *signature++ ) 
	{	
		case ELEMENT_TYPE_VOID:
        	strcat( buffer, "void" );	
			break;					
		
        
		case ELEMENT_TYPE_BOOLEAN:	
			strcat( buffer, "bool" );	
			break;	
		
        
		case ELEMENT_TYPE_CHAR:
        	strcat( buffer, "wchar" );	
			break;		
					
        
		case ELEMENT_TYPE_I1:
        	strcat( buffer, "int8" );	
			break;		
 		
        
		case ELEMENT_TYPE_U1:
        	strcat( buffer, "unsigned int8" );	
			break;		
		
        
		case ELEMENT_TYPE_I2:
        	strcat( buffer, "int16" );	
			break;		
		
        
		case ELEMENT_TYPE_U2:
        	strcat( buffer, "unsigned int16" );	
			break;			
		
        
		case ELEMENT_TYPE_I4:
        	strcat( buffer, "int32" );	
			break;
            
        
		case ELEMENT_TYPE_U4:
        	strcat( buffer, "unsigned int32" );	
			break;		
		
        
		case ELEMENT_TYPE_I8:
        	strcat( buffer, "int64" );	
			break;		
		
        
		case ELEMENT_TYPE_U8:
        	strcat( buffer, "unsigned int64" );	
			break;		
		
        
		case ELEMENT_TYPE_R4:
        	strcat( buffer, "float32" );	
			break;			
		
        
		case ELEMENT_TYPE_R8:
        	strcat( buffer, "float64" );	
			break;		
		
        
		case ELEMENT_TYPE_U:
        	strcat( buffer, "unsigned int" );	
			break;		 
		
        
		case ELEMENT_TYPE_I:
        	strcat( buffer, "int" );	
			break;			  
		
        
		case ELEMENT_TYPE_OBJECT:
        	strcat( buffer, "Object" );	
			break;		 
		
        
		case ELEMENT_TYPE_STRING:
        	strcat( buffer, "String" );	
			break;		 
		
        
		case ELEMENT_TYPE_TYPEDBYREF:
        	strcat( buffer, "refany" );	
			break;				       

		case ELEMENT_TYPE_CLASS:	
		case ELEMENT_TYPE_VALUETYPE:
        case ELEMENT_TYPE_CMOD_REQD:
        case ELEMENT_TYPE_CMOD_OPT:
        	{	
				mdToken	token;	
				char classname[MAX_FUNCTION_LENGTH];


				classname[0] = '\0';
			   	signature += CorSigUncompressToken( signature, &token ); 
				if ( TypeFromToken( token ) != mdtTypeRef )
				{
                	HRESULT	hr;
					WCHAR zName[MAX_FUNCTION_LENGTH];
					
					
					hr = pMDImport->GetTypeDefProps( token, 
													 zName,
													 MAX_FUNCTION_LENGTH,
													 NULL,
													 NULL,
													 NULL );
					if ( SUCCEEDED( hr ) )
						wcstombs( classname, zName, MAX_FUNCTION_LENGTH );
                }
                    
				strcat( buffer, classname );		
			}
            break;	
		
        
		case ELEMENT_TYPE_SZARRAY:	 
			signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "[]" );
			break;		
		
        
		case ELEMENT_TYPE_ARRAY:	
			{	
				ULONG rank;
                

				signature = ParseElementType( pMDImport, signature, buffer );                 
				rank = CorSigUncompressData( signature );													
				if ( rank == 0 ) 
					strcat( buffer, "[?]" );

				else 
				{
					ULONG *lower;	
					ULONG *sizes; 	
                    ULONG numsizes; 
					ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
                    
                                         
					lower = (ULONG *)_alloca( arraysize );                                                        
					memset( lower, 0, arraysize ); 
                    sizes = &lower[rank];

					numsizes = CorSigUncompressData( signature );	
					if ( numsizes <= rank )
					{
                    	ULONG numlower;
                        
                        
						for ( ULONG i = 0; i < numsizes; i++ )	
							sizes[i] = CorSigUncompressData( signature );	
						
                        
						numlower = CorSigUncompressData( signature );	
						if ( numlower <= rank )
						{
							for (ULONG i = 0; i < numlower; i++)	
								lower[i] = CorSigUncompressData( signature ); 
							
                            
							strcat( buffer, "[" );	
							for (ULONG i = 0; i < rank; i++ )	
							{	
								if ( (sizes[i] != 0) && (lower[i] != 0) )	
								{	
									if ( lower[i] == 0 )	
										sprintf ( buffer, "%d", sizes[i] );	

									else	
									{	
										sprintf( buffer, "%d", lower[i] );	
										strcat( buffer, "..." );	
										
										if ( sizes[i] != 0 )	
											sprintf( buffer, "%d", (lower[i] + sizes[i] + 1) );	
									}	
								}
                                	
								if ( i < (rank - 1) ) 
									strcat( buffer, "," );	
							}	
                            
							strcat( buffer, "]" );  
						}						
					}
				}
			} 
			break;	

		
		case ELEMENT_TYPE_PINNED:
			signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "pinned" );	
			break;	
         
        
        case ELEMENT_TYPE_PTR:   
            signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "*" );	
			break;   
        
        
        case ELEMENT_TYPE_BYREF:   
            signature = ParseElementType( pMDImport, signature, buffer ); 
			strcat( buffer, "&" );	
			break;  		    


		default:	
		case ELEMENT_TYPE_END:	
		case ELEMENT_TYPE_SENTINEL:	
			strcat( buffer, "<UNKNOWN>" );	
			break;				                      				            
                        	
	} // switch	
    
	
	return signature;

} // BASEHELPER::ParseElementType
