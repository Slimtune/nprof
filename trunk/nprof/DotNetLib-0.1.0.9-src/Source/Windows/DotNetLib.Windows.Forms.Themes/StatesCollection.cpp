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
#include ".\statescollection.h"
#using <mscorlib.dll>
#include < vcclr.h >
#include "Proptable.h"

using namespace System;
using namespace DotNetLib::Windows::Forms::Themes;
using namespace System::Runtime::InteropServices;

StatesCollection::StatesCollection( int startIndex, String* parentPartName, IHasThemePartId* parent )
{
	m_Parent = parent;

	const wchar_t szParts[] = L"PARTS";
	const wchar_t szStates[] = L"STATES";

	wchar_t findName[35];

	const wchar_t __pin* name = PtrToStringChars( parentPartName );

	wcsncpy( findName, name, min( 29, parentPartName->Length + 1 ) );
	wcscat( findName, szStates );

	m_StartIndex = -1;

	PropTable table;

	for ( int i = startIndex + 1; i < table.GetCount(); i++ )
	{
		if ( wcsstr( table[i], szParts ) )
			break;
		else if ( wcscmp( findName, table[i] ) == 0 )
		{
			m_StartIndex = i;
			break;
		}
	}
}

StatesCollection::~StatesCollection(void)
{
}

ThemePartState* StatesCollection::get_Item( int index )
{
	return new ThemePartState( GetItemTableIndex( index ), m_Parent );
}

ThemePartState* StatesCollection::get_Item( String* partName )
{
	return new ThemePartState( GetItemTableIndex( partName ), m_Parent );
}

