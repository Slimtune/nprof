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
#include ".\themepartstate.h"
#using <mscorlib.dll>
#include "UxTheme.h"

using namespace System;
using namespace DotNetLib::Windows::Forms::Themes;

ThemePartState::ThemePartState( int propTableIndex, IHasThemePartId* parent ) : ThemeItem( propTableIndex )
{
	m_Parent = parent;
	wchar_t szStates[] = L"STATES";
	m_ID = getID( szStates );
}

ThemePartState::~ThemePartState(void)
{
}

void ThemePartState::DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect )
{
	m_Parent->UxTheme->DrawBackground( graphics, m_Parent->ThemePartId, m_ID, Rect );
}

void ThemePartState::DrawBackground( Graphics* graphics, System::Drawing::Rectangle Rect, System::Drawing::Rectangle ClipRect )
{
	m_Parent->UxTheme->DrawBackground( graphics, m_Parent->ThemePartId, m_ID, Rect, ClipRect );
}


void ThemePartState::DrawText( Graphics* graphics, String* Text, Int32 CharCount, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect )
{
	m_Parent->UxTheme->DrawText( graphics, m_Parent->ThemePartId, m_ID, Text, CharCount, TextFlags, Grayed, Rect );
}

void ThemePartState::DrawText( Graphics* graphics, String* Text, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect )
{
	m_Parent->UxTheme->DrawText( graphics, m_Parent->ThemePartId, m_ID, Text, -1, TextFlags, Grayed, Rect );
}

System::Drawing::Rectangle ThemePartState::GetBackgroundContentRect( Graphics* graphics, System::Drawing::Rectangle BoundingRect )
{
	return m_Parent->UxTheme->GetBackgroundContentRect( graphics, m_Parent->ThemePartId, m_ID, BoundingRect );
}

System::Drawing::Color ThemePartState::get_TextColor()
{
	return m_Parent->UxTheme->GetColor( m_Parent->ThemePartId, m_ID, TMT_TEXTCOLOR ); 
}

bool ThemePartState::get_IsDefined()
{
	return m_Parent->UxTheme->IsPartDefined( m_Parent->ThemePartId, m_ID );
}

System::Drawing::Size ThemePartState::GetSize( Graphics* graphics, ThemeSize eSize )
{
	return m_Parent->UxTheme->GetPartSize( graphics, m_Parent->ThemePartId, m_ID, eSize );
}