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
#include ".\windowtheme.h"
#using <mscorlib.dll>

using namespace System;
using namespace DotNetLib::Windows::Forms::Themes;

WindowTheme::WindowTheme( int propTableIndex ) : ThemeItem( propTableIndex )
{
	m_UxTheme = UxTheme::OpenTheme( NULL, get_Name() );
	MASSERT( m_UxTheme != NULL );

	m_Parts = new PartsCollection( propTableIndex, this );
	m_States = new StatesCollection( propTableIndex, get_Name(), this );
}

WindowTheme::~WindowTheme(void)
{
}

String* WindowTheme::get_Name()
{
	// the name of a window theme is the record name without the PARTS part
	String* name = ThemeItem::get_Name();
	int index = name->LastIndexOf( "PARTS" );

	MASSERT( index > -1 );

	return name->Substring( 0, index );
}
