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

#include "IThemeItemCollection.h"

using namespace System::Collections;
using namespace DotNetLib::Windows::Forms::Themes;

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	__gc __sealed public class ThemeItemEnumerator : public IEnumerator
	{
	public private:
		ThemeItemEnumerator( IThemeItemCollection* collection );

	private:
		~ThemeItemEnumerator(void);

	public:
		__property System::Object* get_Current();
		bool MoveNext();
		void Reset();

	private:
		 IThemeItemCollection* m_Collection;
		 int m_CurrentIndex;
		 int m_CountCache;		// becuase getting the count can entail a lot of string compares we cahce the count in the
								// constructor so that we can iterate faster
	};
}
}
}
}