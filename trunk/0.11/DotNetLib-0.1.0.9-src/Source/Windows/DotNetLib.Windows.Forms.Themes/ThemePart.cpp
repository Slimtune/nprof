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
#include ".\themepart.h"
#using <mscorlib.dll>

using namespace DotNetLib::Windows::Forms::Themes;

#define DEFAULT_STATEID 0

ThemePart::ThemePart( int propTableIndex, IHasThemePartId* parent ) : ThemeItem( propTableIndex )
{
	m_Parent = parent;
	m_States = new StatesCollection( propTableIndex, get_Name(), this );
	wchar_t szStates[] = L"PARTS";
	m_ID = getID( szStates );
}

ThemePart::~ThemePart(void)
{
}

void ThemePart::DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect )
{
	m_Parent->UxTheme->DrawBackground( graphics, m_ID, DEFAULT_STATEID, Rect );
}

void ThemePart::DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect, System::Drawing::Rectangle ClipRect )
{
	m_Parent->UxTheme->DrawBackground( graphics, m_ID, DEFAULT_STATEID, Rect, ClipRect );
}


void ThemePart::DrawText( Graphics* graphics, String* Text, Int32 CharCount, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect )
{
	m_Parent->UxTheme->DrawText( graphics, m_ID, DEFAULT_STATEID, Text, CharCount, TextFlags, Grayed, Rect );
}

void ThemePart::DrawText( Graphics* graphics, String* Text, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect )
{
	m_Parent->UxTheme->DrawText( graphics, m_ID, DEFAULT_STATEID, Text, -1, TextFlags, Grayed, Rect );
}


System::Drawing::Color ThemePart::get_TextColor()
{
	return m_Parent->UxTheme->GetColor( m_ID, DEFAULT_STATEID, TMT_TEXTCOLOR ); 
}

bool ThemePart::get_IsDefined()
{
	return m_Parent->UxTheme->IsPartDefined( m_ID, DEFAULT_STATEID );
}

System::Drawing::Size ThemePart::GetSize( Graphics* graphics, ThemeSize eSize )
{
	return m_Parent->UxTheme->GetPartSize( graphics, m_ID, DEFAULT_STATEID, eSize );
}
