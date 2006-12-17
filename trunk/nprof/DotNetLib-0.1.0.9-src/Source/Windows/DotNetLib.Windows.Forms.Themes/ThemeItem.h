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

#include "StdAfx.h"

using namespace System;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// prop table entry represents a single record in the prop table
	//
	__gc __abstract public class ThemeItem
	{
	public private:
		ThemeItem( int propTableIndex );
		int getID( wchar_t* name );

	private:
		~ThemeItem(void);

	public:
		__property virtual String* get_Name();

	protected private:
		const int m_propTableIndex;			// this is the record number in the schema property table
											// that the object is attached to

	};
}
}
}
}