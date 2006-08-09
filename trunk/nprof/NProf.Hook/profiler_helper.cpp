#include "stdafx.h"
#include "profiler_helper.h"

ProfilerHelper::ProfilerHelper()
{
}

ProfilerHelper::~ProfilerHelper(void)
{
}

void ProfilerHelper::Initialize(  ICorProfilerInfo* profilerInfo )
{
  this->profilerInfo = profilerInfo;
}

ThreadID ProfilerHelper::GetCurrentThreadID()
{
  ThreadID threadId;
  profilerInfo->GetCurrentThreadID( &threadId );

  return threadId;
}

string ProfilerHelper::GetCurrentThreadName()
{
  return "";
}

void ProfilerHelper::GetFunctionSignature( 
  FunctionID functionId,
  UINT32& methodAttributes,
  string& returnType, 
  string& className,
  string& functionName,
  string& parameters )
{
    ULONG args;
    WCHAR returnTypeString[ MAX_FUNCTION_LENGTH ];
    WCHAR parametersString[ MAX_FUNCTION_LENGTH ];
    WCHAR functionNameString[ MAX_FUNCTION_LENGTH ];
    WCHAR classNameString[ MAX_FUNCTION_LENGTH ];
    GetFunctionProperties( functionId, &methodAttributes, &args, returnTypeString, parametersString, classNameString, functionNameString );
    
    returnType = CW2A( returnTypeString );
    parameters = CW2A( parametersString );
    className = CW2A( classNameString );
    functionName = CW2A( functionNameString );
}

/* static public */
HRESULT ProfilerHelper::GetFunctionProperties( 
                       FunctionID functionID,
										   UINT32* methodAttributes,
										   ULONG *argCount,
										   WCHAR *returnType, 
										   WCHAR *parameters,
										   WCHAR *className,
                       WCHAR *funName )
{
    HRESULT hr = E_FAIL; // assume success
	
		

	// init return values
	*argCount = 0;
	returnType[0] = NULL; 
	parameters[0] = NULL;
	funName[0] = NULL;
	className[0] = NULL;



    if ( functionID != NULL )
	{
	    mdToken	token;
		ClassID classID;
		ModuleID moduleID;
		IMetaDataImport *metaDataImport = NULL;	
		mdToken moduleToken;
			    
			    
	    
	    //
		// Get the classID 
		//
		try
		{
			hr = profilerInfo->GetFunctionInfo(
						functionID,
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
			hr = profilerInfo->GetTokenAndMetaDataFromFunction( functionID, 
			           								 		IID_IMetaDataImport, 
															(IUnknown **)&metaDataImport,
															&token );
			if ( SUCCEEDED( hr ) )
			{
				hr = metaDataImport->GetMethodProps( token,
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

					hr = profilerInfo->GetClassIDInfo( classID, 
						        				   NULL,  
						                           &classToken );
					
					if SUCCEEDED( hr )
					{
				      	if ( classToken != mdTypeDefNil )
						{
				          	hr = metaDataImport->GetTypeDefProps( classToken, 
								                             className, 
								                             MAX_FUNCTION_LENGTH,
								                             NULL, 
								                             NULL, 
								                             NULL ); 
						}

					    DWORD methodAttr = 0;
						PCCOR_SIGNATURE sigBlob = NULL;


					    hr = metaDataImport->GetMethodProps( (mdMethodDef) token,
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
              *methodAttributes = methodAttr;

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
								sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );

								//
								// if the return typ returned back empty, write void
								//
								if ( buffer[0] == '\0' )
									sprintf( buffer, "void" );

								swprintf( returnType, L"%S",buffer );
								
								//
								// Get the parameters
								//								
								for ( ULONG i = 0; 
									  (SUCCEEDED( hr ) && (sigBlob != NULL) && (i < (*argCount))); 
									  i++ )
								{
									buffer[0] = '\0';

									sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );									
									if ( i == 0 )
										swprintf( parameters, L"%S", buffer );

									else if ( sigBlob != NULL )
										swprintf( parameters, L"%s, %S", parameters, buffer );
									
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
								sigBlob = ParseElementType( metaDataImport, sigBlob, buffer );
								swprintf( returnType, L"%s %S",returnType, buffer );
							}
						} 
					} 
				} 

				metaDataImport->Release();
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
PCCOR_SIGNATURE ProfilerHelper::ParseElementType( IMetaDataImport *metaDataImport,
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
					
					
					hr = metaDataImport->GetTypeDefProps( token, 
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
			signature = ParseElementType( metaDataImport, signature, buffer ); 
			strcat( buffer, "[]" );
			break;		
		
        
		case ELEMENT_TYPE_ARRAY:	
			{	
				ULONG rank;
                

				signature = ParseElementType( metaDataImport, signature, buffer );                 
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
			signature = ParseElementType( metaDataImport, signature, buffer ); 
			strcat( buffer, "pinned" );	
			break;	
         
        
        case ELEMENT_TYPE_PTR:   
            signature = ParseElementType( metaDataImport, signature, buffer ); 
			strcat( buffer, "*" );	
			break;   
        
        
        case ELEMENT_TYPE_BYREF:   
            signature = ParseElementType( metaDataImport, signature, buffer ); 
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
