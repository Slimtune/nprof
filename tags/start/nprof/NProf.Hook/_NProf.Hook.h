
#pragma warning( disable: 4049 )  /* more than 64k source lines */

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0347 */
/* at Sat Mar 08 13:41:32 2003
 */
/* Compiler settings for _NProf.Hook.idl:
    Os, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef ___NProf2EHook_h__
#define ___NProf2EHook_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __INProfCORHook_FWD_DEFINED__
#define __INProfCORHook_FWD_DEFINED__
typedef interface INProfCORHook INProfCORHook;
#endif 	/* __INProfCORHook_FWD_DEFINED__ */


#ifndef __CNProfCORHook_FWD_DEFINED__
#define __CNProfCORHook_FWD_DEFINED__

#ifdef __cplusplus
typedef class CNProfCORHook CNProfCORHook;
#else
typedef struct CNProfCORHook CNProfCORHook;
#endif /* __cplusplus */

#endif 	/* __CNProfCORHook_FWD_DEFINED__ */


/* header files for imported files */
#include "prsht.h"
#include "mshtml.h"
#include "mshtmhst.h"
#include "exdisp.h"
#include "objsafe.h"
#include "corsym.h"
#include "corpub.h"
#include "corsvc.h"
#include "corprof.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

#ifndef __INProfCORHook_INTERFACE_DEFINED__
#define __INProfCORHook_INTERFACE_DEFINED__

/* interface INProfCORHook */
/* [unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_INProfCORHook;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5B94DF43-780B-42FD-AC4C-ABAB35D4A274")
    INProfCORHook : public IDispatch
    {
    public:
    };
    
#else 	/* C style interface */

    typedef struct INProfCORHookVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            INProfCORHook * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            INProfCORHook * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            INProfCORHook * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            INProfCORHook * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            INProfCORHook * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            INProfCORHook * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            INProfCORHook * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        END_INTERFACE
    } INProfCORHookVtbl;

    interface INProfCORHook
    {
        CONST_VTBL struct INProfCORHookVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define INProfCORHook_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define INProfCORHook_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define INProfCORHook_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define INProfCORHook_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define INProfCORHook_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define INProfCORHook_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define INProfCORHook_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __INProfCORHook_INTERFACE_DEFINED__ */



#ifndef __NProfHook_LIBRARY_DEFINED__
#define __NProfHook_LIBRARY_DEFINED__

/* library NProfHook */
/* [helpstring][uuid][version] */ 


EXTERN_C const IID LIBID_NProfHook;

EXTERN_C const CLSID CLSID_CNProfCORHook;

#ifdef __cplusplus

class DECLSPEC_UUID("791DA9FE-05A0-495E-94BF-9AD875C4DF0F")
CNProfCORHook;
#endif
#endif /* __NProfHook_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


