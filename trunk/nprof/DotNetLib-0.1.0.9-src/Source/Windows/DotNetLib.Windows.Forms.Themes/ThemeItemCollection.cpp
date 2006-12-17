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
#include ".\ThemeItemCollection.h"
#using <mscorlib.dll>
#include < vcclr.h >
#include "PropTable.h"

using namespace System;
using namespace DotNetLib::Windows::Forms::Themes;

ThemeItemCollection::ThemeItemCollection( int startIndex ) :  m_StartIndex( startIndex )
{
}

ThemeItemCollection::~ThemeItemCollection(void)
{
}


int ThemeItemCollection::get_Count()
{
	if ( m_StartIndex == -1 )
		return 0;

	const wchar_t szParts[] = L"PARTS";
	const wchar_t szStates[] = L"STATES";

	int count = 0;

	PropTable table;

	for ( int i = m_StartIndex + 1; i < table.GetCount(); i++ )
	{
		if ( wcsstr( table[i], szParts ) )
			break;
		else if ( ( wcsstr( table[i], szStates ) ) )
			break;
		else
			count++;
	}

	return count;
}

int ThemeItemCollection::GetItemTableIndex( int localIndex )
{
	const wchar_t szParts[] = L"PARTS";
	const wchar_t szStates[] = L"STATES";

	int tableIndex = -1;

	if ( m_StartIndex != -1 )
	{
		PropTable table;

		int count = 0;
		for ( int i = m_StartIndex + 1; i < table.GetCount(); i++ )
		{
			if ( wcsstr( table[i], szParts ) )
				break;
			else if ( ( wcsstr( table[i], szStates ) ) )
				break;
			else if ( localIndex == count )
			{
				tableIndex = i;
				break;
			}
			
			count++;		
		}
	}

	if ( tableIndex == -1 )
		throw new System::IndexOutOfRangeException();

	return tableIndex;
}

int ThemeItemCollection::GetItemTableIndex( String* name )
{
	THROW_IF_NULL( name );

	const wchar_t szParts[] = L"PARTS";
	const wchar_t szStates[] = L"STATES";

	int tableIndex = -1;

	if ( m_StartIndex != -1 )
	{
		PropTable table;
		//__const_Char_ptr wcName = PtrToStringChars( name->ToUpper() );
		const wchar_t __pin* wcName = PtrToStringChars( name->ToUpper() );

		for ( int i = m_StartIndex + 1; i < table.GetCount(); i++ )
		{
			if ( wcsstr( table[i], szParts ) )
				break;
			else if ( ( wcsstr( table[i], szStates ) ) )
				break;
			else if ( wcscmp( table[i], wcName ) == 0 )
			{
				tableIndex = i;	
				break;
			}
		}
	}

	if ( tableIndex == -1 )
		throw new System::IndexOutOfRangeException();

	return tableIndex;
}

void ThemeItemCollection::CopyTo( System::Array* array, int startIndex )
{
	THROW_IF_NULL( array );

	if ( array->Rank != 1 )
		throw new System::ArgumentException( "array", "Array must be one dimensional" );

	if ( startIndex < 0 )
		throw new System::ArgumentOutOfRangeException( "startIndex", "startIndex must be greater or equal to 0" );

	if ( startIndex + Count > array->Length )
		throw new System::ArgumentException( "array", "The array is not large enough to copy the collection contents into" );

	for ( int i = 0; i < Count; i++ )
		array->SetValue( GetEnumeratorItem( i ), startIndex + i );
}