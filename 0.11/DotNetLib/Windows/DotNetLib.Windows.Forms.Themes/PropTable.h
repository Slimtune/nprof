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

//
// PropTable wraps the raw static data created by TmSchema.h
//
class PropTable
{
public:
	PropTable(void);
	~PropTable(void);

	const wchar_t* GetPropTableEntry( int index );
	int GetCount();

	int GetStartIndex(){ return s_PropTableStart; }

	const wchar_t* operator[]( int index ){ return GetPropTableEntry(index); }

protected:
	static int s_PropTableStart;

};
