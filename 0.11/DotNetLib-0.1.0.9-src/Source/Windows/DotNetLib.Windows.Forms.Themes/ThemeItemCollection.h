/***************************************************************************\
|  Author:  Josh Carlson                                                    |
|                                                                           |
|  This work builds on code posted to CodeProject                           |
|  Don Kackman http://codeproject.com/managedcpp/ManagedUxTheme.asp         |
|  with some work from                                                      |
|  David Y Zhao http://codeproject.com/w2k/xpvisualstyle.asp                |
|                                                                           |
|  This code is provided "as is" and no warranty about                      |
|  it fitness for any specific task is expressed or                         |
|  implied.  If you choose to use this code, you do so                      |
|  at your own risk.                                                        |
\***************************************************************************/

#pragma once

#include "stdafx.h"

#include "ThemeItemEnumerator.h"

using namespace System;
using namespace System::Collections;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{

	//
	// PropTableEntryCollection and its derived class encapsulate the
	// organization of the property table defined in tmschema.h and schemadef.h.
	//
	// For a description of the organization of this data see the Themes.txt file
	//
	__gc __abstract public class ThemeItemCollection : public IThemeItemCollection
	{
	private protected:
		ThemeItemCollection( int startIndex );
		ThemeItemCollection(){}

	private:
		~ThemeItemCollection(void);

	public:
		__property int get_Count();
		__property System::Object* get_SyncRoot(){ return this; }
		__property bool get_IsSynchronized(){ return false; }

		void CopyTo( System::Array*, int );

		System::Collections::IEnumerator* GetEnumerator(){ return new ThemeItemEnumerator( this ); }

	private protected:

		int m_StartIndex;

		int GetItemTableIndex( int localIndex );
		int GetItemTableIndex( String* name );
	};
}
}
}
}
