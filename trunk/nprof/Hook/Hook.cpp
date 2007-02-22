/***************************************************************************
                          profiler.cpp  -  description
                             -------------------
    begin                : Sat Jan 18 2003
    copyright            : (C) 2003, 2004, 2005, 2006 by Matthew Mastracci, Christian Staudenmeyer
    email                : mmastrac@canada.com
 ***************************************************************************/

/***************************************************************************
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the GNU General Public License as published by  *
 *   the Free Software Foundation; either version 2 of the License, or     *
 *   (at your option) any later version.                                   *
 *                                                                         *
 ***************************************************************************/

#pragma warning ( disable : 4530 )
#include <atlbase.h>
#include <atlcom.h>
#include <map>
#include <string>
#include <sstream>
#include <fstream>
#include <vector>
#include "cor.h"
#include "corprof.h"
#include "Dbghelp.h"
#include "resource.h"
#include <iostream>
#include <set>
#include "winreg.h"
#define Li2Double(x) ((double)((x).HighPart) * 4.294967296E9 + (double)((x).LowPart))

#include    <windows.h>
#include    <winperf.h>
#include    <stdio.h>
#include    <stdlib.h>
#include    <string.h>
#include    <tchar.h>

using namespace ATL;
using namespace std;

const int MAX_FUNCTION_LENGTH=2048;
#define guid "107F578A-E019-4BAF-86A1-7128A749DB05"

void    SetPerfIndexes (HWND hWnd);
class ProfilerHelper
{
public:
	void Initialize(ICorProfilerInfo2* profilerInfo) {
		this->profilerInfo = profilerInfo;
		SetPerfIndexes(NULL);
	}
	string GetClass(string className) {
		int index=className.length();
		for(;index>0 && className[index-1]!='.';index--);
		return className.substr(index);
	}
	string GetFunctionSignature(FunctionID functionId) {
		string text;
		ClassID classID;
		ModuleID moduleID;
		mdToken moduleToken;
		if(FAILED(profilerInfo->GetFunctionInfo(functionId,&classID,&moduleID,&moduleToken))) {
			DebugBreak();
		}

		IMetaDataImport *metaDataImport = 0;	
		mdToken	token;
		if(FAILED(profilerInfo->GetTokenAndMetaDataFromFunction(functionId, IID_IMetaDataImport, (IUnknown **)&metaDataImport,&token))) {
			DebugBreak();
		}

		WCHAR functionName[MAX_FUNCTION_LENGTH];
		if(FAILED(metaDataImport->GetMethodProps(token, 0, functionName, MAX_FUNCTION_LENGTH,0,0,0,0,0,0))) {
			DebugBreak();
		}

		mdTypeDef classToken = 0;
		profilerInfo->GetClassIDInfo(classID, 0, &classToken);
		if(classToken != mdTypeDefNil) {
			WCHAR className[MAX_FUNCTION_LENGTH];
			if(FAILED(metaDataImport->GetTypeDefProps(classToken, className, MAX_FUNCTION_LENGTH,0, 0, 0))) {
				DebugBreak();
			}
			text=text+GetClass((string)CW2A(className))+"."+(string)CW2A(functionName);
		}

		DWORD methodAttr = 0;
		PCCOR_SIGNATURE sigBlob = 0;
		if(FAILED(metaDataImport->GetMethodProps( (mdMethodDef) token,0,0,0,0,&methodAttr,&sigBlob,0,0,0 ))) {
			DebugBreak();
		}

		ULONG callConv;
		sigBlob += CorSigUncompressData(sigBlob, &callConv);
		text+="(";
		//if ( callConv != IMAGE_CEE_CS_CALLCONV_FIELD ) {

			ULONG argCount=0;
			sigBlob += CorSigUncompressData(sigBlob, &argCount);
			//DebugBreak();
			sigBlob--;
			string returnType=ParseElementType(metaDataImport, &sigBlob);
			//sigBlob--;
			if(sigBlob!=0) {
				for(ULONG i = 0; (sigBlob != 0) && (i < (argCount)); i++) {
					if(i!=0){
						text+=", ";
					}
					text+=ParseElementType(metaDataImport,&sigBlob);
					//sigBlob--;
				}
			}
		//}
		text+=")";
		metaDataImport->Release();
		return text;
	}
	template<class T> string ToString(T i) {
		string s;
		stringstream stream;
		stream<<i;
		stream>>s;
		return s;
	}
	string ParseElementType(IMetaDataImport *metaDataImport,PCCOR_SIGNATURE* ppSignature) {
		ULONG count=0;
		(*ppSignature)++;
		CorElementType type=(CorElementType)**ppSignature;
		switch((CorElementType)type) {
			case ELEMENT_TYPE_VOID:
				return "void";
			case ELEMENT_TYPE_BOOLEAN:
				return "bool";
			case ELEMENT_TYPE_CHAR:
				return "wchar";
			case ELEMENT_TYPE_I1:
				return "byte";
			case ELEMENT_TYPE_U1:
				return "ubyte";
			case ELEMENT_TYPE_I2:
				return "short";
			case ELEMENT_TYPE_U2:
				return "ushort";
			case ELEMENT_TYPE_I4:
				return "int";
			case ELEMENT_TYPE_U4:
				return "uint";
			case ELEMENT_TYPE_I8:
				return "long";
			case ELEMENT_TYPE_U8:
				return "ulong";
			case ELEMENT_TYPE_R4:
				return "float";
			case ELEMENT_TYPE_R8:
				return "double";
			case ELEMENT_TYPE_U:
				return "uint";
			case ELEMENT_TYPE_I:
				return "int";
			case ELEMENT_TYPE_OBJECT:
				return "object";
			case ELEMENT_TYPE_STRING:
				return "string";
			case ELEMENT_TYPE_VAR:
				return "var ";
			case ELEMENT_TYPE_TYPEDBYREF:
				return "refany";
			case ELEMENT_TYPE_CLASS:	
			case ELEMENT_TYPE_VALUETYPE:
			case ELEMENT_TYPE_CMOD_REQD:
			case ELEMENT_TYPE_CMOD_OPT:
				mdToken	token;
				(*ppSignature)++;
				(*ppSignature) += CorSigUncompressToken((PCCOR_SIGNATURE)(*ppSignature),&token);
				if(TypeFromToken(token)!=mdtTypeRef) {
					WCHAR zName[MAX_FUNCTION_LENGTH];
					metaDataImport->GetTypeDefProps(token,zName,MAX_FUNCTION_LENGTH,0,0,0);
					(*ppSignature)--;
					return GetClass((string)CW2A(zName));
				}
				return "mdtTypeRef";
			case ELEMENT_TYPE_SZARRAY:
				return ParseElementType(metaDataImport,ppSignature)+"[]";
			case ELEMENT_TYPE_ARRAY: {	
				ULONG rank;
				string text=ParseElementType(metaDataImport,ppSignature);
				rank = CorSigUncompressData((PCCOR_SIGNATURE&)*ppSignature);
				if ( rank == 0 ) {
					text+="[?]";
				}
				else {
					ULONG *lower;
					ULONG *sizes;
					ULONG numsizes;
					ULONG arraysize = (sizeof ( ULONG ) * 2 * rank);
	                
					lower=(ULONG*)_alloca(arraysize);
					memset(lower,0,arraysize);
					sizes = &lower[rank];

					numsizes = CorSigUncompressData((PCCOR_SIGNATURE&)*ppSignature);
					if ( numsizes <= rank ) {
						ULONG numlower;			                    
						for ( ULONG i = 0; i < numsizes; i++ ) {
							sizes[i] = CorSigUncompressData((PCCOR_SIGNATURE&)*ppSignature);
						}			                    
						numlower = CorSigUncompressData((PCCOR_SIGNATURE&)*ppSignature);	
						if ( numlower <= rank ) {
							for (ULONG i = 0; i < numlower; i++) {
								lower[i] = CorSigUncompressData((PCCOR_SIGNATURE&)*ppSignature); 
							}
							text+="[";
							for (ULONG i = 0; i < rank; i++ ) {	
								if ( (sizes[i] != 0) && (lower[i] != 0) ) {	
									if ( lower[i] == 0 ) {
										text+=ToString(sizes[i]);
									}
									else {
										text+=ToString(lower[i]);	
										text+="...";
										if ( sizes[i] != 0 ) {
											text+=ToString((lower[i] + sizes[i] + 1) );	
										}
									}	
								}			                            	
								if ( i < (rank - 1) ) {
									text+=",";
								}
							}		                        
							text+="]";
						}
					}
				}
				return text;
			}
			//case ELEMENT_TYPE_GENERICINST: {
			//	string text="generic type: ";
			//	text+=ParseElementType(metaDataImport,ppSignature);
			//	//ParseElementType(
			//	//(*ppSignature) += CorSigUncompressToken((PCCOR_SIGNATURE)(*ppSignature),&token);
			//	////(*ppSignature)--;
			//	////if(TypeFromToken(token)!=mdtTypeRef) {
			//	//	WCHAR zName[MAX_FUNCTION_LENGTH];
			//	//	metaDataImport->GetTypeDefProps(token,zName,MAX_FUNCTION_LENGTH,0,0,0);
			//	//	text+=(string)CW2A(zName);
			//	////}
			//	ULONG count=0;
			//	(*ppSignature) += CorSigUncompressData(*ppSignature, &count);
			//	text+="<";
			//	text+=ToString(count);
			//	return text;
			//}
			case ELEMENT_TYPE_PINNED:
				return ParseElementType(metaDataImport,ppSignature)+"pinned";
			case ELEMENT_TYPE_PTR:   
				return ParseElementType(metaDataImport,ppSignature)+"*";
			case ELEMENT_TYPE_BYREF:   
				return ParseElementType(metaDataImport,ppSignature)+"&";
			case ELEMENT_TYPE_END:
				//return "<end>";
			case ELEMENT_TYPE_GENERICINST:
			default:
				return "<UNKNOWN>";
		}
	}
	CComPtr< ICorProfilerInfo2 > profilerInfo;
};
HRESULT __stdcall __stdcall StackWalker(FunctionID funcId,UINT_PTR ip,COR_PRF_FRAME_INFO frameInfo,
	ULONG32 contextSize,BYTE context[  ],void *clientData
) {
	if(funcId!=0) {
		((vector<FunctionID>*)clientData)->push_back(funcId);
	}
	return S_OK;
}

typedef PERF_DATA_BLOCK             PERF_DATA,      *PPERF_DATA;
typedef PERF_OBJECT_TYPE            PERF_OBJECT,    *PPERF_OBJECT;
typedef PERF_INSTANCE_DEFINITION    PERF_INSTANCE,  *PPERF_INSTANCE;
typedef PERF_COUNTER_DEFINITION     PERF_COUNTER,   *PPERF_COUNTER;


PPERF_COUNTER FirstCounter (PPERF_OBJECT pObject)
{
    if (pObject)
        return (PPERF_COUNTER)((PCHAR) pObject + pObject->HeaderLength);
    else
        return NULL;
}
PPERF_COUNTER NextCounter (PPERF_COUNTER pCounter)
{
    if (pCounter)
        return (PPERF_COUNTER)((PCHAR) pCounter + pCounter->ByteLength);
    else
        return NULL;
}
PPERF_COUNTER FindCounter (PPERF_OBJECT pObject, DWORD TitleIndex)
{
PPERF_COUNTER pCounter;
DWORD         i = 0;

    if (pCounter = FirstCounter (pObject))
        while (i < pObject->NumCounters)
            {
            if (pCounter->CounterNameTitleIndex == TitleIndex)
                return pCounter;

            pCounter = NextCounter (pCounter);
            i++;
            }

    return NULL;

}
DWORD   GetPerfData    (HKEY        hPerfKey,
                        LPTSTR      szObjectIndex,
                        PPERF_DATA  *ppData,
                        DWORD       *pDataSize)
{
DWORD   DataSize;
DWORD   dwR;
DWORD   Type;


	//DebugBreak();
	if (!*ppData) {
        *ppData = (PPERF_DATA) LocalAlloc (LMEM_FIXED, *pDataSize);
	}

    do  {
        DataSize = *pDataSize;
        //dwR = RegQueryValueEx (HKEY_LOCAL_MACHINE,
        ////dwR = RegQueryValueEx (HKEY_LOCAL_MACHINE,
        //                       "230 232",
        //                       NULL,
        //                       &Type,
        //                       (BYTE *)*ppData,
        //                       &DataSize);

        dwR = RegQueryValueEx (hPerfKey,
                               szObjectIndex,
                               NULL,
                               &Type,
                               (BYTE *)*ppData,
                               &DataSize);

        if (dwR == ERROR_MORE_DATA)
            {
            LocalFree (*ppData);
            *pDataSize += 1024;
            *ppData = (PPERF_DATA) LocalAlloc (LMEM_FIXED, *pDataSize);
            }

        if (!*ppData)
            {
            LocalFree (*ppData);
            return ERROR_NOT_ENOUGH_MEMORY;
            }

        } while (dwR == ERROR_MORE_DATA);

    return dwR;
}
PPERF_DATA RefreshPerfData (HKEY        hPerfKey,
                            LPTSTR      szObjectIndex,
                            PPERF_DATA  pData,
                            DWORD       *pDataSize)
{
    if (GetPerfData (hPerfKey, szObjectIndex, &pData, pDataSize) == ERROR_SUCCESS)
        return pData;
    else
        return NULL;
}
HKEY            ghPerfKey = HKEY_PERFORMANCE_DATA;  // get perf data from this key
HKEY            ghMachineKey = HKEY_LOCAL_MACHINE;  // get title index from this key

DWORD   GetPerfTitleSz (HKEY    hKeyMachine,
                        HKEY    hKeyPerf,
                        LPTSTR  *TitleBuffer,
                        LPTSTR  *TitleSz[],
                        DWORD   *TitleLastIdx)
{
HKEY	  hKey1;
HKEY    hKey2;
DWORD   Type;
DWORD   DataSize;
DWORD   dwR;
DWORD   Len;
DWORD   Index;
DWORD   dwTemp;
BOOL    bNT10;
LPTSTR  szCounterValueName;
LPTSTR  szTitle;




    // Initialize
    //
    hKey1        = NULL;
    hKey2        = NULL;
    *TitleBuffer = NULL;
    *TitleSz     = NULL;




    // Open the perflib key to find out the last counter's index and system version.
    //
    dwR = RegOpenKeyEx (hKeyMachine,
                        TEXT("software\\microsoft\\windows nt\\currentversion\\perflib"),
                        0,
                        KEY_READ,
                        &hKey1);
    if (dwR != ERROR_SUCCESS)
        goto done;



    // Get the last counter's index so we know how much memory to allocate for TitleSz
    //
    DataSize = sizeof (DWORD);
    dwR = RegQueryValueEx (hKey1, TEXT("Last Counter"), 0, &Type, (LPBYTE)TitleLastIdx, &DataSize);
    if (dwR != ERROR_SUCCESS)
        goto done;



    // Find system version, for system earlier than 1.0a, there's no version value.
    //
    dwR = RegQueryValueEx (hKey1, TEXT("Version"), 0, &Type, (LPBYTE)&dwTemp, &DataSize);

    if (dwR != ERROR_SUCCESS)
        // unable to read the value, assume NT 1.0
        bNT10 = TRUE;
    else
        // found the value, so, NT 1.0a or later
        bNT10 = FALSE;









    // Now, get ready for the counter names and indexes.
    //
    if (bNT10)
        {
        // NT 1.0, so make hKey2 point to ...\perflib\009 and get
        //  the counters from value "Counters"
        //
        szCounterValueName = TEXT("Counters");
        dwR = RegOpenKeyEx (hKeyMachine,
                            TEXT("software\\microsoft\\windows nt\\currentversion\\perflib\\009"),
                            0,
                            KEY_READ,
                            &hKey2);
        if (dwR != ERROR_SUCCESS)
            goto done;
        }
    else
        {
        // NT 1.0a or later.  Get the counters in key HKEY_PERFORMANCE_KEY
        //  and from value "Counter 009"
        //
        szCounterValueName = TEXT("Counter 009");
        hKey2 = hKeyPerf;
        }





    // Find out the size of the data.
    //
    dwR = RegQueryValueEx (hKey2, szCounterValueName, 0, &Type, 0, &DataSize);
    if (dwR != ERROR_SUCCESS)
        goto done;



    // Allocate memory
    //
    *TitleBuffer = (LPTSTR)LocalAlloc (LMEM_FIXED, DataSize);
    if (!*TitleBuffer)
        {
        dwR = ERROR_NOT_ENOUGH_MEMORY;
        goto done;
        }

    *TitleSz = (LPTSTR *)LocalAlloc (LPTR, (*TitleLastIdx+1) * sizeof (LPTSTR));
    if (!*TitleSz)
        {
        dwR = ERROR_NOT_ENOUGH_MEMORY;
        goto done;
        }





    // Query the data
    //
    dwR = RegQueryValueEx (hKey2, szCounterValueName, 0, &Type, (BYTE *)*TitleBuffer, &DataSize);
    if (dwR != ERROR_SUCCESS)
        goto done;




    // Setup the TitleSz array of pointers to point to beginning of each title string.
    // TitleBuffer is type REG_MULTI_SZ.
    //
    szTitle = *TitleBuffer;

    while (Len = lstrlen (szTitle))
        {
        Index = atoi (szTitle);

        szTitle = szTitle + Len +1;

        if (Index <= *TitleLastIdx)
            (*TitleSz)[Index] = szTitle;

        szTitle = szTitle + lstrlen (szTitle) +1;
        }



done:

    // Done. Now cleanup!
    //
    if (dwR != ERROR_SUCCESS)
        {
        // There was an error, free the allocated memory
        //
        if (*TitleBuffer) LocalFree (*TitleBuffer);
        if (*TitleSz)     LocalFree (*TitleSz);
        }

    // Close the hKeys.
    //
    if (hKey1) RegCloseKey (hKey1);
    if (hKey2 && hKey2 != hKeyPerf) RegCloseKey (hKey2);



    return dwR;

}
DWORD   PX_THREAD_SWITCHES;
DWORD	PX_THREAD_CPU;
DWORD   PX_PROCESS;
DWORD PX_THREAD_PRIV;
#define PN_THREAD_SWITCHES                  TEXT("Context Switches/sec")
#define PN_THREAD_ID						TEXT("ID Thread")
DWORD   GetTitleIdx (HWND hWnd, LPTSTR Title[], DWORD LastIndex, LPTSTR Name)
{
DWORD   Index;

    for (Index = 0; Index <= LastIndex; Index++)
        if (Title[Index])
            if (!lstrcmpi (Title[Index], Name))
                return Index;

    MessageBox (hWnd, Name, TEXT("Pviewer cannot find index"), MB_OK);
    return 0;
}
#define PN_THREAD_CPU                       TEXT("% Processor Time")
#define PN_THREAD_PRIV                      TEXT("% Privileged Time")


#define PN_THREAD                           TEXT("Thread")
#define PN_PROCESS_ID                       TEXT("ID Process")
#define INDEX_STR_LEN       10
#define PN_PROCESS                          TEXT("Process")
TCHAR           INDEX_PROCTHRD_OBJ[2*INDEX_STR_LEN];
DWORD   PX_THREAD;
DWORD   PX_PROCESS_ID;
DWORD	PX_THREAD_ID;
//TCHAR           INDEX_COSTLY_OBJ[3*INDEX_STR_LEN];
DWORD   PX_PROCESS_ADDRESS_SPACE;
#define PN_PROCESS_ADDRESS_SPACE            TEXT("Process Address Space")
void    SetPerfIndexes (HWND hWnd)
{
	LPTSTR  TitleBuffer;
	LPTSTR  *Title;
	DWORD   Last;
	TCHAR   szTemp[50];
	DWORD   dwR;

	//DebugBreak();

    dwR = GetPerfTitleSz (ghMachineKey, ghPerfKey, &TitleBuffer, &Title, &Last);

    if (dwR != ERROR_SUCCESS)
        {
        wsprintf (szTemp, TEXT("Unable to retrieve counter indexes, ERROR -> %#x"), dwR);
        MessageBox (hWnd, szTemp, TEXT("Pviewer"), MB_OK|MB_ICONEXCLAMATION);
        return;
        }


    PX_PROCESS                       = GetTitleIdx (hWnd, Title, Last, PN_PROCESS);
    //PX_PROCESS_CPU                   = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_CPU);
    //PX_PROCESS_PRIV                  = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIV);
    //PX_PROCESS_USER                  = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_USER);
    //PX_PROCESS_WORKING_SET           = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_WORKING_SET);
    //PX_PROCESS_PEAK_WS               = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PEAK_WS);
    //PX_PROCESS_PRIO                  = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIO);
    //PX_PROCESS_ELAPSE                = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_ELAPSE);
    PX_PROCESS_ID                    = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_ID);
    //PX_PROCESS_PRIVATE_PAGE          = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_PAGE);
    //PX_PROCESS_VIRTUAL_SIZE          = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_VIRTUAL_SIZE);
    //PX_PROCESS_PEAK_VS               = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PEAK_VS);
    //PX_PROCESS_FAULT_COUNT           = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_FAULT_COUNT);

    PX_THREAD                        = GetTitleIdx (hWnd, Title, Last, PN_THREAD);
	PX_THREAD_ID					 = GetTitleIdx (hWnd, Title, Last, PN_THREAD_ID);
    PX_THREAD_CPU                    = GetTitleIdx (hWnd, Title, Last, PN_THREAD_CPU);
    PX_THREAD_PRIV                   = GetTitleIdx (hWnd, Title, Last, PN_THREAD_PRIV);
    //PX_THREAD_USER                   = GetTitleIdx (hWnd, Title, Last, PN_THREAD_USER);
    //PX_THREAD_START                = GetTitleIdx (hWnd, Title, Last, PN_THREAD_START); */ 
    PX_THREAD_SWITCHES               = GetTitleIdx (hWnd, Title, Last, PN_THREAD_SWITCHES);
 //   PX_THREAD_PRIO                   = GetTitleIdx (hWnd, Title, Last, PN_THREAD_PRIO);
 //   PX_THREAD_BASE_PRIO              = GetTitleIdx (hWnd, Title, Last, PN_THREAD_BASE_PRIO);
 //   PX_THREAD_ELAPSE                 = GetTitleIdx (hWnd, Title, Last, PN_THREAD_ELAPSE);

 //   PX_THREAD_DETAILS                = GetTitleIdx (hWnd, Title, Last, PN_THREAD_DETAILS);
 //   PX_THREAD_PC                     = GetTitleIdx (hWnd, Title, Last, PN_THREAD_PC);

 //   PX_IMAGE                         = GetTitleIdx (hWnd, Title, Last, PN_IMAGE);
 //   PX_IMAGE_NOACCESS                = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_NOACCESS);
 //   PX_IMAGE_READONLY                = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_READONLY);
 //   PX_IMAGE_READWRITE               = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_READWRITE);
 //   PX_IMAGE_WRITECOPY               = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_WRITECOPY);
 //   PX_IMAGE_EXECUTABLE              = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_EXECUTABLE);
 //   PX_IMAGE_EXE_READONLY            = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_EXE_READONLY);
 //   PX_IMAGE_EXE_READWRITE           = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_EXE_READWRITE);
 //   PX_IMAGE_EXE_WRITECOPY           = GetTitleIdx (hWnd, Title, Last, PN_IMAGE_EXE_WRITECOPY);

    PX_PROCESS_ADDRESS_SPACE         = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_ADDRESS_SPACE);
 //   PX_PROCESS_PRIVATE_NOACCESS      = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_NOACCESS);
 //   PX_PROCESS_PRIVATE_READONLY      = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_READONLY);
 //   PX_PROCESS_PRIVATE_READWRITE     = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_READWRITE);
 //   PX_PROCESS_PRIVATE_WRITECOPY     = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_WRITECOPY);
 //   PX_PROCESS_PRIVATE_EXECUTABLE    = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_EXECUTABLE);
 //   PX_PROCESS_PRIVATE_EXE_READONLY  = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_EXE_READONLY);
 //   PX_PROCESS_PRIVATE_EXE_READWRITE = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_EXE_READWRITE);
 //   PX_PROCESS_PRIVATE_EXE_WRITECOPY = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_PRIVATE_EXE_WRITECOPY);

 //   PX_PROCESS_MAPPED_NOACCESS       = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_NOACCESS);
 //   PX_PROCESS_MAPPED_READONLY       = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_READONLY);
 //   PX_PROCESS_MAPPED_READWRITE      = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_READWRITE);
 //   PX_PROCESS_MAPPED_WRITECOPY      = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_WRITECOPY);
 //   PX_PROCESS_MAPPED_EXECUTABLE     = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_EXECUTABLE);
 //   PX_PROCESS_MAPPED_EXE_READONLY   = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_EXE_READONLY);
 //   PX_PROCESS_MAPPED_EXE_READWRITE  = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_EXE_READWRITE);
 //   PX_PROCESS_MAPPED_EXE_WRITECOPY  = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_MAPPED_EXE_WRITECOPY);

 //   PX_PROCESS_IMAGE_NOACCESS        = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_NOACCESS);
 //   PX_PROCESS_IMAGE_READONLY        = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_READONLY);
 //   PX_PROCESS_IMAGE_READWRITE       = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_READWRITE);
 //   PX_PROCESS_IMAGE_WRITECOPY       = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_WRITECOPY);
 //   PX_PROCESS_IMAGE_EXECUTABLE      = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_EXECUTABLE);
 //   PX_PROCESS_IMAGE_EXE_READONLY    = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_EXE_READONLY);
 //   PX_PROCESS_IMAGE_EXE_READWRITE   = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_EXE_READWRITE);
 //   PX_PROCESS_IMAGE_EXE_WRITECOPY   = GetTitleIdx (hWnd, Title, Last, PN_PROCESS_IMAGE_EXE_WRITECOPY);*/


    wsprintf (INDEX_PROCTHRD_OBJ, TEXT("%ld %ld"), PX_PROCESS, PX_THREAD);
    //wsprintf (INDEX_COSTLY_OBJ, TEXT("%ld %ld %ld"),
    //          PX_PROCESS_ADDRESS_SPACE, PX_IMAGE, PX_THREAD_DETAILS);


    LocalFree (TitleBuffer);
    LocalFree (Title);

}
PPERF_OBJECT FirstObject (PPERF_DATA pData)
{
    if (pData)
        return ((PPERF_OBJECT) ((PBYTE) pData + pData->HeaderLength));
    else
        return NULL;
}
PPERF_OBJECT NextObject (PPERF_OBJECT pObject)
{
    if (pObject)
        return ((PPERF_OBJECT) ((PBYTE) pObject + pObject->TotalByteLength));
    else
        return NULL;
}
PPERF_OBJECT FindObject (PPERF_DATA pData, DWORD TitleIndex)
{
	PPERF_OBJECT pObject;
	DWORD        i = 0;

    if (pObject = FirstObject (pData))
        while (i < pData->NumObjectTypes)
            {
            if (pObject->ObjectNameTitleIndex == TitleIndex)
                return pObject;

            pObject = NextObject (pObject);
            i++;
            }

    return NULL;
}
PPERF_INSTANCE   FirstInstance (PPERF_OBJECT pObject)
{
    if (pObject)
        return (PPERF_INSTANCE)((PCHAR) pObject + pObject->DefinitionLength);
    else
        return NULL;
}
PPERF_INSTANCE   NextInstance (PPERF_INSTANCE pInst)
{
PERF_COUNTER_BLOCK *pCounterBlock;

    if (pInst)
        {
        pCounterBlock = (PERF_COUNTER_BLOCK *)((PCHAR) pInst + pInst->ByteLength);
        return (PPERF_INSTANCE)((PCHAR) pCounterBlock + pCounterBlock->ByteLength);
        }
    else
        return NULL;
}
PPERF_INSTANCE FindInstanceN (PPERF_OBJECT pObject, DWORD N)
{
PPERF_INSTANCE pInst;
DWORD          i = 0;

    if (!pObject)
        return NULL;
    else if (N >= (DWORD)(pObject->NumInstances))
        return NULL;
    else
        {
        pInst = FirstInstance (pObject);

        while (i != N)
            {
            pInst = NextInstance (pInst);
            i++;
            }

        return pInst;
        }
}
PVOID CounterData (PPERF_INSTANCE pInst, PPERF_COUNTER pCount)
{
PPERF_COUNTER_BLOCK pCounterBlock;

    if (pCount && pInst)
        {
        pCounterBlock = (PPERF_COUNTER_BLOCK)((PCHAR)pInst + pInst->ByteLength);
        return (PVOID)((PCHAR)pCounterBlock + pCount->CounterOffset);
        }
    else
        return NULL;
}

LPTSTR  InstanceName (PPERF_INSTANCE pInst)
{
    if (pInst)
        return (LPTSTR) ((PCHAR) pInst + pInst->NameOffset);
    else
        return NULL;
}
const int interval=2;
PPERF_DATA      gpPerfData=NULL;
class Profiler {
public: 
	vector<vector<FunctionID>*> stackWalks;
	set<FunctionID> signatures;
	ofstream* file;
	string GetTemporaryFileName() {
		char path[MAX_PATH];
		path[0]=0;
		GetTempPath(MAX_PATH-1,path);
		return (string)path+guid+".nprof";
	}
	void EndAll(ProfilerHelper& profilerHelper) {
		file=new ofstream(GetTemporaryFileName().c_str(), ios::binary);
		WriteInteger(lastTime);
		for(set<FunctionID>::iterator i=signatures.begin();i!=signatures.end();i++) {
			WriteInteger(*i);
			WriteString(profilerHelper.GetFunctionSignature(*i));
		}
		WriteInteger(-1);
		for(vector<vector<FunctionID>*>::iterator stackWalk = stackWalks.begin(); stackWalk != stackWalks.end(); stackWalk++ ) {
			for(vector<FunctionID>::iterator stackFrame=(*stackWalk)->begin();stackFrame!=(*stackWalk)->end();stackFrame++) {
				WriteInteger(*stackFrame);
			}
			WriteInteger(-1);
		}
		WriteInteger(-1);
		WriteInteger(-2);
		file->close();
		delete file;
	}
	void WriteString(const string& signature) {
		WriteInteger((UINT32)signature.length());
		file->write(signature.c_str(),(int)signature.length());
	}
	void WriteInteger(FunctionID id) {
		file->write((char*)&id,sizeof(FunctionID));
	}
	Profiler::Profiler( ICorProfilerInfo2* profilerInfo ) {
		//lastTime=(__int64)0;
		this->profilerInfo = profilerInfo;
		this->profilerHelper.Initialize(profilerInfo);
		profilerInfo->SetEventMask(COR_PRF_ENABLE_STACK_SNAPSHOT|COR_PRF_MONITOR_THREADS|COR_PRF_DISABLE_INLINING);
		SetTimer();
	}
	void KillTimer() {
		timeKillEvent(timer);
	}
	void SetTimer() {
		TIMECAPS timeCaps;
		timeGetDevCaps(&timeCaps, sizeof(TIMECAPS));
		cout << timeCaps.wPeriodMin;
		timer = timeSetEvent(interval,10,TimerFunction,	(DWORD_PTR)this,TIME_PERIODIC);
		//timer = timeSetEvent(interval,1,TimerFunction,	(DWORD_PTR)this,TIME_PERIODIC);
	}
	void ThreadMap(ThreadID threadId, DWORD dwOSThread) {
		threadMap[dwOSThread] = threadId;
	}
	virtual void End() {
		KillTimer();
		EndAll(profilerHelper);
	}

	static void CALLBACK TimerFunction(UINT wTimerID, UINT msg, DWORD dwUser, DWORD dw1, DWORD dw2) {
		Profiler* profiler=(Profiler*)dwUser;
		profiler->WalkStack();
	}
	static __int64 lastTime;
	static UINT timer;
	map<DWORD,DWORD> switchMap;
	void WalkStack() {
		bool anyFound=false;

		for(map<DWORD,ThreadID>::iterator i=threadMap.begin();i!=threadMap.end();i++) {
			DWORD threadId=i->first;

			HANDLE threadHandle=OpenThread(THREAD_SUSPEND_RESUME|THREAD_QUERY_INFORMATION|THREAD_GET_CONTEXT,false,threadId);
			int suspendCount=SuspendThread(threadHandle);
			vector<FunctionID>* functions=new vector<FunctionID>();
			ThreadID id=threadMap[threadId];

			CONTEXT context;
			context.ContextFlags=CONTEXT_FULL;
			GetThreadContext(threadHandle,&context);
			FunctionID newID;

			//bool found;
			//if(profilerInfo->GetFunctionFromIP((BYTE*)context.Eip,&newID)!=S_OK && newID!=0) {
			//	found=true;
			//}
			//else {
			//	found=false;
			//	STACKFRAME64 stackFrame;
			//	memset(&stackFrame, 0, sizeof(stackFrame));
			//	stackFrame.AddrPC.Offset = context.Eip;
			//	stackFrame.AddrPC.Mode = AddrModeFlat;
			//	stackFrame.AddrFrame.Offset = context.Ebp;
			//	stackFrame.AddrFrame.Mode = AddrModeFlat;
			//	stackFrame.AddrStack.Offset = context.Esp;
			//	stackFrame.AddrStack.Mode = AddrModeFlat;
			//	while(true) {
			//		if(!StackWalk64(IMAGE_FILE_MACHINE_I386,GetCurrentProcess(),threadHandle,
			//			 &stackFrame,0,0,SymFunctionTableAccess64,SymGetModuleBase64,0)) {
			//			break;
			//		}
			//		if (stackFrame.AddrPC.Offset == stackFrame.AddrReturn.Offset) {
			//			break;
			//		}
			//		FunctionID id;
			//		if(profilerInfo->GetFunctionFromIP((BYTE*)stackFrame.AddrPC.Offset,&id)==S_OK && id!=0) {
			//			memset(&context,0,sizeof(context));
			//			context.Eip=stackFrame.AddrPC.Offset;
			//			context.Ebp=stackFrame.AddrFrame.Offset;
			//			context.Esp=stackFrame.AddrStack.Offset;
			//			found=true;
			//			break;
			//		}
			//	}
			//}
			//if(found) {
				//anyFound=true;

				//profilerInfo->DoStackSnapshot(
				//	id,StackWalker,COR_PRF_SNAPSHOT_DEFAULT,functions,NULL,NULL);//(BYTE*)&context,sizeof(context));


				profilerInfo->DoStackSnapshot(
					id,StackWalker,COR_PRF_SNAPSHOT_DEFAULT,functions,(BYTE*)&context,sizeof(context));

				for(int index=0;index<functions->size();index++) {
					FunctionID id=functions->at(index);
					const set<FunctionID>::iterator found = signatures.find(id);
					if(found == signatures.end()){
						signatures.insert(id);
					}
				}
			//}
			stackWalks.push_back(functions);
			if(functions->size()!=0) {
				anyFound=true;
			}
			ResumeThread(threadHandle);
			CloseHandle(threadHandle);
		}
		//SetTimer();
		//return anyFound;
	}
	map< DWORD, ThreadID > threadMap;
protected:
	CComPtr< ICorProfilerInfo2 > profilerInfo;
	ProfilerHelper profilerHelper;
	bool statistical;
};
UINT Profiler::timer;
__int64 Profiler::lastTime;
[
	object,
	uuid("FDEDE932-9F80-4CE5-891E-3B24768CFBCB"),
	dual,helpstring("INProfCORHook Interface"),
	pointer_default(unique)
]
__interface INProfCORHook : IDispatch {
};
[
  coclass,
  threading("apartment"),
  vi_progid("NProf.NProfCORHook"),
  progid("NProf.NProfCORHook.1"),
  version(1.0),
  uuid(guid),
  helpstring("nprof COR Profiling Hook Class")
]
class ATL_NO_VTABLE CNProfCORHook : public INProfCORHook,public ICorProfilerCallback2 {
public:
	CNProfCORHook() {
		this->profiler = 0;
	}
	DECLARE_PROTECT_FINAL_CONSTRUCT()
	HRESULT FinalConstruct() {
		return S_OK;
	}
	void FinalRelease() {
		delete profiler;
	}
public:
	static Profiler* profiler;
	CRITICAL_SECTION criticalSection;
public:
	static Profiler* GetProfiler() {
		return profiler;
	}
	STDMETHOD(Initialize)(LPUNKNOWN pICorProfilerInfoUnk) {
		CComQIPtr< ICorProfilerInfo2 > profilerInfo = pICorProfilerInfoUnk;
		InitializeCriticalSection(&criticalSection);
		profiler = new Profiler( profilerInfo );
		return S_OK;
	}
	STDMETHOD(Shutdown)() {
		EnterCriticalSection(&criticalSection);
		profiler->End();
		delete profiler;
		profiler = 0;
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(AppDomainCreationStarted)(AppDomainID appDomainId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AppDomainCreationFinished)(AppDomainID appDomainId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(AppDomainShutdownStarted)(AppDomainID appDomainId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AppDomainShutdownFinished)(AppDomainID appDomainId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyLoadStarted)(AssemblyID assemblyId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyLoadFinished)(AssemblyID assemblyId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyUnloadStarted)(AssemblyID assemblyId) {
		return E_NOTIMPL;
	}
	STDMETHOD(AssemblyUnloadFinished)(AssemblyID assemblyId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleLoadStarted)(ModuleID moduleId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleLoadFinished)(ModuleID moduleId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleUnloadStarted)(ModuleID moduleId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleUnloadFinished)(ModuleID moduleId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ModuleAttachedToAssembly)(ModuleID moduleId, AssemblyID assemblyId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassLoadStarted)(ClassID classId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassLoadFinished)(ClassID classId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassUnloadStarted)(ClassID classId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ClassUnloadFinished)(ClassID classId, HRESULT hrStatus) {
		return E_NOTIMPL;
	}
	STDMETHOD(FunctionUnloadStarted)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCompilationStarted)(FunctionID functionId, BOOL fIsSafeToBlock) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCompilationFinished)(FunctionID functionId, HRESULT hrStatus, BOOL fIsSafeToBlock) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCachedFunctionSearchStarted)(FunctionID functionId, BOOL* pbUseCachedFunction) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITCachedFunctionSearchFinished)(FunctionID functionId, COR_PRF_JIT_CACHE result) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITFunctionPitched)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(JITInlining)(FunctionID callerId, FunctionID calleeId, BOOL* pfShouldInline) {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadCreated)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadDestroyed)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadAssignedToOSThread)(ThreadID managedThreadId, DWORD osThreadId) {
		EnterCriticalSection(&criticalSection);
		profiler->ThreadMap(managedThreadId, osThreadId);
		LeaveCriticalSection(&criticalSection);
		return S_OK;
	}
	STDMETHOD(RemotingClientInvocationStarted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientSendingMessage)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientReceivingReply)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingClientInvocationFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerReceivingMessage)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerInvocationStarted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerInvocationReturned)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RemotingServerSendingReply)(GUID * pCookie, BOOL fIsAsync) {
		return E_NOTIMPL;
	}
	STDMETHOD(UnmanagedToManagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) {
		return E_NOTIMPL;
	}
	STDMETHOD(ManagedToUnmanagedTransition)(FunctionID functionId, COR_PRF_TRANSITION_REASON reason) {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeSuspendStarted)(COR_PRF_SUSPEND_REASON suspendReason) {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeSuspendFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeSuspendAborted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeResumeStarted)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeResumeFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeThreadSuspended)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(RuntimeThreadResumed)(ThreadID threadId) {
		return E_NOTIMPL;
	}
	STDMETHOD(MovedReferences)(unsigned long cMovedObjectIDRanges, ObjectID* oldObjectIDRangeStart, ObjectID* newObjectIDRangeStart, unsigned long * cObjectIDRangeLength) {
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectAllocated)(ObjectID objectId, ClassID classId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectsAllocatedByClass)(unsigned long cClassCount, ClassID* classIds, unsigned long* cObjects) {
		return E_NOTIMPL;
	}
	STDMETHOD(ObjectReferences)(ObjectID objectId, ClassID classId, unsigned long cObjectRefs, ObjectID* objectRefIds) {
		return E_NOTIMPL;
	}
	STDMETHOD(RootReferences)(unsigned long cRootRefs, ObjectID* rootRefIds) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionThrown)(ThreadID thrownObjectId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFunctionEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFunctionLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFilterEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchFilterLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionSearchCatcherFound)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionOSHandlerEnter)(UINT_PTR __unused) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionOSHandlerLeave)(UINT_PTR __unused) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFunctionEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFunctionLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFinallyEnter)(FunctionID functionId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionUnwindFinallyLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCatcherEnter)(FunctionID functionId, ObjectID objectId) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCatcherLeave)() {
		return E_NOTIMPL;
	}
	STDMETHOD(COMClassicVTableCreated)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable, unsigned long cSlots) {
		return E_NOTIMPL;
	}
	STDMETHOD(COMClassicVTableDestroyed)(ClassID wrappedClassId, const GUID& implementedIID, void * pVTable) {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCLRCatcherFound)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ExceptionCLRCatcherExecute)() {
		return E_NOTIMPL;
	}
	STDMETHOD(ThreadNameChanged)(ThreadID threadId, ULONG cchName, WCHAR name[]) {
		return E_NOTIMPL;
	}
	STDMETHOD(GarbageCollectionStarted)(int cGenerations, BOOL generationCollected[], COR_PRF_GC_REASON reason) {
		return E_NOTIMPL;
	}
	STDMETHOD(SurvivingReferences) (ULONG cSurvivingObjectIDRanges,ObjectID objectIDRangeStart[],ULONG cObjectIDRangeLength[]) {
		return E_NOTIMPL;
	}
	STDMETHOD(GarbageCollectionFinished)() {
		return E_NOTIMPL;
	}
	STDMETHOD(FinalizeableObjectQueued)(DWORD finalizerFlags,ObjectID objectID) {
		return E_NOTIMPL;
	}
	STDMETHOD(RootReferences2)(ULONG cRootRefs, ObjectID rootRefIds[], COR_PRF_GC_ROOT_KIND rootKinds[],COR_PRF_GC_ROOT_FLAGS rootFlags[], UINT_PTR rootIds[]) {
		return E_NOTIMPL;
	}
	STDMETHOD(HandleCreated)(GCHandleID handleId,ObjectID initialObjectId) {
		return E_NOTIMPL;
	}
	STDMETHOD(HandleDestroyed)(GCHandleID handleId) {
		return E_NOTIMPL;
	}
};
[ 
	module(dll, uuid = "{A461E20A-C7DC-4A89-A24E-87B5E975A96B}", 
	name = "NProfHook", 
	helpstring = "NProf.Hook 1.0 Type Library",
	resource_name = "IDR_NPROFHOOK")
];
Profiler* CNProfCORHook::profiler;
