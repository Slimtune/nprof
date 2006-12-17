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
#include ".\themeitemenumerator.h"
#using <mscorlib.dll>

using namespace System;
using namespace DotNetLib::Windows::Forms::Themes;

ThemeItemEnumerator::ThemeItemEnumerator( IThemeItemCollection* collection )
{
	m_Collection = collection;
	m_CountCache = m_Collection->Count;
	Reset();
}

ThemeItemEnumerator::~ThemeItemEnumerator(void)
{
}


System::Object* ThemeItemEnumerator::get_Current()
{
	try
	{
		return m_Collection->GetEnumeratorItem( m_CurrentIndex );
	}
	catch ( System::IndexOutOfRangeException* )
	{
		throw new System::InvalidOperationException();
	}
}

bool ThemeItemEnumerator::MoveNext()
{
	m_CurrentIndex++;
	return m_CurrentIndex < m_CountCache;
}

void ThemeItemEnumerator::Reset()
{
	m_CurrentIndex = -1;
}