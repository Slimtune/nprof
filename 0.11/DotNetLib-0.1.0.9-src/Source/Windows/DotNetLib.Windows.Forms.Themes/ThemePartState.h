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

#include "ThemeItem.h"
#include "IHasThemePartId.h"
#include "IRenderableItem.h"

using namespace System::Drawing;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// A ThemePArtState is a discrete state that a part of window class can be in
	// and that may render differently than the default state
	//
	// Examples are normal, hot, disabled.
	__gc public __sealed class ThemePartState : public ThemeItem, public IRenderableItem
	{
	public private:
		ThemePartState( int propTableIndex, IHasThemePartId* parent );

	public:
		__property int get_StateId(){ return m_ID; }

		__property bool get_IsDefined();
		void DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect, System::Drawing::Rectangle ClipRect );
		void DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect );

		void DrawText( Graphics* graphics, String* Text, Int32 CharCount, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect );
		void DrawText( Graphics* graphics, String* Text, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect );

		System::Drawing::Size GetSize( Graphics* graphics, ThemeSize eSize );

		System::Drawing::Rectangle GetBackgroundContentRect( Graphics* graphics, System::Drawing::Rectangle BoundingRect );

		__property System::Drawing::Color get_TextColor();

	private:
		~ThemePartState(void);

		int m_ID;		// part and state id's are the 1 based index of the row that defines the state
						// i.e. for the record GROUPBOXSTATES, the record immediately following has an id of 1

		IHasThemePartId* m_Parent;
	};
}
}
}
}
