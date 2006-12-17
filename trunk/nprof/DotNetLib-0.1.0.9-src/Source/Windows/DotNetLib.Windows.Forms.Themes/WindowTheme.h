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

#include "PartsCollection.h"
#include "ThemeItem.h"
#include "UxTheme.h"
#include "IHasThemePartId.h"

using namespace DotNetLib::Windows::Forms::Themes;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// WindowTheme represent a single window class and its parts and states
	//
	__gc public __sealed class WindowTheme : public ThemeItem, public IHasThemePartId
	{
	public private:
		WindowTheme( int propTableIndex );

	private:
		~WindowTheme(void);

	public:
		__property PartsCollection* get_Parts() { return m_Parts; }
		__property StatesCollection* get_States(){ return m_States; }
		__property virtual String* get_Name();

		// IHasThemePartId
		__property int get_ThemePartId(){ return 0; }
		__property DotNetLib::Windows::Forms::Themes::UxTheme* get_UxTheme(){ return m_UxTheme; }

	private:
		PartsCollection* m_Parts;
		StatesCollection* m_States;

		DotNetLib::Windows::Forms::Themes::UxTheme* m_UxTheme;
	};
}
}
}
}