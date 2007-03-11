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

#include "StdAfx.h"
#include ".\ThemeItem.h"
#using <mscorlib.dll>

#include "ThemeInfo.h"
#include "PropTable.h"

using namespace System;
using namespace DotNetLib::Windows::Forms::Themes;

ThemeItem::ThemeItem( int propTableIndex ) : m_propTableIndex( propTableIndex )
{
}

ThemeItem::~ThemeItem(void)
{
}

String* ThemeItem::get_Name()
{
	PropTable table;
	return new String( table[ m_propTableIndex ] );
}

int ThemeItem::getID( wchar_t* name )
{
	PropTable table;

	int id = 1;

	// spin backward in the table until we find the record
	// with name
	for ( int i = m_propTableIndex - 1; i > 0; i-- )
	{
		if ( wcsstr( table[i], name ) )
			break;
		
		id++;
	}

	return id;
}