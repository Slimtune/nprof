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

#include "ThemePart.h"
#include "ThemeItemCollection.h"

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// PartsCollection represent the records the represent the parts of a window class
	//
	[System::Reflection::DefaultMember("Item")]
	__gc public __sealed class PartsCollection : public ThemeItemCollection
	{
	public private:
		PartsCollection( int parentIndex, IHasThemePartId* parent );

	private:
		~PartsCollection(void);

	public:
		__property ThemePart* get_Item( int index );
		__property ThemePart* get_Item( String* partName );

		[System::ComponentModel::EditorBrowsable(EditorBrowsableState::Never)]
		System::Object* GetEnumeratorItem( int index ){ return get_Item( index ); }

	private:
		IHasThemePartId* m_Parent;		// the containing WindowTheme
	};
}
}
}
}