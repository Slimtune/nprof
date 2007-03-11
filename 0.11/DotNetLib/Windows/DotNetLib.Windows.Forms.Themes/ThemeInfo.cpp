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
#include ".\themeinfo.h"
#using <mscorlib.dll>
#include < vcclr.h >

#include "UxTheme.h"
#include "PropTable.h"

using namespace DotNetLib::Windows::Forms::Themes;

ThemeInfo::ThemeInfo() 
{	

}

ThemeInfo::~ThemeInfo(void)
{
}

String* ThemeInfo::get_Name()
{
	return UxTheme::CurrentThemeName;
}

String* ThemeInfo::get_ColorName()
{
	return UxTheme::CurrentThemeColorName;
}

String* ThemeInfo::get_SizeName()
{
	return UxTheme::CurrentThemeSizeName;
}



int ThemeInfo::get_Count()
{
	int count = 0;

	if ( UxTheme::IsAppThemed )	
	{
		PropTable table;

		const wchar_t szParts[] = L"PARTS";

		for ( int i = table.GetStartIndex(); i < table.GetCount(); i++ )
		{
			// if the record has "PARTS" and we can get a handle for it then we count it
			if ( wcsstr( table[i], szParts ) && UxTheme::ThemeIsPresent( table[i] ) )
				count++;
		}
	}

	return count;
}


WindowTheme* ThemeInfo::get_Item( int index )
{
	WindowTheme* t = NULL;

	PropTable table;
	if ( UxTheme::IsAppThemed && table.GetStartIndex() != -1 )	
	{
		const wchar_t szParts[] = L"PARTS";

		int Count = 0;

		for ( int i = table.GetStartIndex(); i < table.GetCount(); i++ )
		{
			if ( wcsstr( table[i], szParts ) && UxTheme::ThemeIsPresent( table[i] ) )			
			{
				if ( index == Count )
				{
					t = new WindowTheme( i );
					break;
				}

				Count++;		
			}		
		}
	}

	if ( t == NULL )
		throw new System::IndexOutOfRangeException();

	return t;
}

WindowTheme* ThemeInfo::get_Item( String* className )
{
	THROW_IF_NULL( className );

	WindowTheme* t = NULL;

	PropTable table;
	if ( UxTheme::IsAppThemed && table.GetStartIndex() != -1 )	
	{
		const wchar_t szParts[] = L"PARTS";		
		wchar_t findClassName[35];
		const wchar_t __pin*  class_name = PtrToStringChars( className );

		wcsncpy( findClassName, class_name, min( 30, className->Length + 1 ) );
		wcscat( findClassName, szParts );

		for ( int i = table.GetStartIndex(); i < table.GetCount(); i++ )
		{
			if ( wcscmp( findClassName, table[i] ) == 0  && UxTheme::ThemeIsPresent( table[i] ) )
			{
				t = new WindowTheme( i );
				break;
			}
		}
	}

	if ( t == NULL )
		throw new System::IndexOutOfRangeException();

	return t;
}

void ThemeInfo::CopyTo( System::Array* array, int startIndex )
{
	THROW_IF_NULL( array );

	if ( array->Rank != 1 )
		throw new System::ArgumentException( "array", "Array must be one dimensional" );

	if ( startIndex < 0 )
		throw new System::ArgumentOutOfRangeException( "startIndex", "startIndex must be greater or equal to 0" );

	if ( startIndex + Count > array->Length )
		throw new System::ArgumentException( "array", "The array is not large enough to copy the collection contents into" );

	for ( int i = 0; i < Count; i++ )
		array->SetValue( Item[i], startIndex + i );
}
