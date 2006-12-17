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
#include ".\partscollection.h"

using namespace System;
using namespace DotNetLib::Windows::Forms::Themes;

PartsCollection::PartsCollection( int startIndex, IHasThemePartId* parent ) : ThemeItemCollection( startIndex )
{
	m_Parent = parent;
}

PartsCollection::~PartsCollection(void)
{
}

ThemePart* PartsCollection::get_Item( int index )
{
	return new ThemePart( GetItemTableIndex( index ), m_Parent );
}

ThemePart* PartsCollection::get_Item( String* partName )
{
	return new ThemePart( GetItemTableIndex( partName ), m_Parent );
}

