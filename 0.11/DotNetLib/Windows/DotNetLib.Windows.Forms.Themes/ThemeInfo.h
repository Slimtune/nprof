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

#include "WindowTheme.h"

#include "ThemeItemEnumerator.h"

using namespace System::Collections;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// ThemeInfo represents the collection of window themes
	// and some simple meta data bout the current theme, like name
	// 
	[System::Reflection::DefaultMember("Item")]
	__gc public __sealed class ThemeInfo  : public IThemeItemCollection
	{
	public:
		ThemeInfo();

	private:
		~ThemeInfo(void);

	public:
		__property String* get_Name();
		__property String* get_ColorName();
		__property String* get_SizeName();


		__property WindowTheme* get_Item( int index );
		__property WindowTheme* get_Item( String* partName );


		__property int get_Count();
		__property System::Object* get_SyncRoot(){ return this; }
		__property bool get_IsSynchronized(){ return false; }

		System::Collections::IEnumerator* GetEnumerator(){ return new ThemeItemEnumerator( this ); }
		void CopyTo( System::Array* array, int startIndex );

		[System::ComponentModel::EditorBrowsable(EditorBrowsableState::Never)]
		System::Object* GetEnumeratorItem( int index ){ return get_Item( index ); }

	};
}
}
}
}