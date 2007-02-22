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

namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	//
	// This a managed translation of the windows TEXTMETRIC struct so that
	// it can be used from C#
	//
	__value public struct TextMetric
	{
		int        tmHeight;
		int        tmAscent;
		int        tmDescent;
		int        tmInternalLeading;
		int        tmExternalLeading;
		int        tmAveCharWidth;
		int        tmMaxCharWidth;
		int        tmWeight;
		int        tmOverhang;
		int        tmDigitizedAspectX;
		int        tmDigitizedAspectY;
		System::Char       tmFirstChar;
		System::Char       tmLastChar;
		System::Char       tmDefaultChar;
		System::Char       tmBreakChar;
		BYTE        tmItalic;
		BYTE        tmUnderlined;
		BYTE        tmStruckOut;
		BYTE        tmPitchAndFamily;
		BYTE        tmCharSet;

	public private:
		TextMetric( TEXTMETRIC* ptm )
		{
			tmHeight = ptm->tmHeight;
			tmAscent = ptm->tmAscent;
			tmDescent = ptm->tmDescent;
			tmInternalLeading = ptm->tmInternalLeading;
			tmExternalLeading = ptm->tmExternalLeading;
			tmAveCharWidth = ptm->tmAveCharWidth;
			tmMaxCharWidth = ptm->tmMaxCharWidth;
			tmWeight = ptm->tmWeight;
			tmOverhang = ptm->tmOverhang;
			tmDigitizedAspectX = ptm->tmDigitizedAspectX;
			tmDigitizedAspectY = ptm->tmDigitizedAspectY;
			tmFirstChar = ptm->tmFirstChar;
			tmLastChar = ptm->tmLastChar;
			tmDefaultChar = ptm->tmDefaultChar;
			tmBreakChar = ptm->tmBreakChar;
			tmItalic = ptm->tmItalic;
			tmUnderlined = ptm->tmUnderlined;
			tmStruckOut = ptm->tmStruckOut;
			tmPitchAndFamily = ptm->tmPitchAndFamily;
			tmCharSet = ptm->tmCharSet;
		}
	};
}
}
}
}