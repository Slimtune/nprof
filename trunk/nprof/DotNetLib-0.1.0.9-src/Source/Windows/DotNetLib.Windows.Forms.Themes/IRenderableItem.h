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

#using <mscorlib.dll>

using namespace DotNetLib::Windows::Forms::Themes;
using namespace System::Drawing;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// both window classes and window parts have states
	// This interface is implemented by both WindowTheme and ThemePart
	// and is used by the ThemePArtStates class so it can get the correct
	// ID values to pass to UxTheme
	__gc public __interface IRenderableItem
	{
		__property bool get_IsDefined();
		void DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect, System::Drawing::Rectangle ClipRect );
		void DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect );

		void DrawText( Graphics* graphics, String* Text, Int32 CharCount, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect );
		void DrawText( Graphics* graphics, String* Text, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect );

		System::Drawing::Size GetSize( Graphics* graphics, ThemeSize eSize );

		__property System::Drawing::Color get_TextColor();
	};
}
}
}
}