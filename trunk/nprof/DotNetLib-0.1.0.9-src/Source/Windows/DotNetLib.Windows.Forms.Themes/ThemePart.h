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
#include "StatesCollection.h"
#include "IHasThemePartId.h"
#include "IRenderableItem.h"

using namespace DotNetLib::Windows::Forms::Themes;
using namespace System::Drawing;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// ThemePart represents a single part of a window class
	//
	__gc public __sealed class ThemePart : public ThemeItem, public IHasThemePartId, public IRenderableItem
	{
	public private:
		ThemePart( int propTableIndex, IHasThemePartId* parent );

	private:
		~ThemePart(void);

	public:
		__property StatesCollection* get_States(){ return m_States; }

		// IHasThemePartId
		__property int get_ThemePartId(){ return m_ID; }
		__property DotNetLib::Windows::Forms::Themes::UxTheme* get_UxTheme(){ return m_Parent->UxTheme; }

		__property bool get_IsDefined();
		void DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect, System::Drawing::Rectangle ClipRect );
		void DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect );

		void DrawText( Graphics* graphics, String* Text, Int32 CharCount, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect );
		void DrawText( Graphics* graphics, String* Text, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect );

		System::Drawing::Size GetSize( Graphics* graphics, ThemeSize eSize );

		__property System::Drawing::Color get_TextColor();

	private:
		StatesCollection* m_States;
		int m_ID;
		IHasThemePartId* m_Parent;
	};
}
}
}
}