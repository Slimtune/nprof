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
#using <System.dll>
#using <System.Drawing.dll>
#using <System.Windows.Forms.dll>

using namespace System;
using namespace System::Drawing;
using namespace System::ComponentModel;
using namespace System::Collections;
using namespace System::Diagnostics;

using namespace System::Windows::Forms;

#if !defined(DT_HIDEPREFIX)
#define DT_HIDEPREFIX               0x00100000
#define DT_PREFIXONLY               0x00200000
#endif

#include "VisualStylesXP.h"

#include "TextMetric.h"


namespace DotNetLib
{ namespace Windows
{ namespace Forms 
{ namespace Themes
{
	__value public struct Margins
	{
		Int32 cxLeftWidth;      // width of left border that retains its size
		Int32 cxRightWidth;     // width of right border that retains its size
		Int32 cyTopHeight;      // height of top border that retains its size
		Int32 cyBottomHeight;   // height of bottom border that retains its size
	};

		__value public enum ThemeSize
	{
		TS_MIN,             // minimum size
		TS_TRUE,            // size without stretching
		TS_DRAW,            // size that theme mgr will use to draw part
	};

	__value public enum PropertyOrigin
	{
		PO_STATE,           // property was found in the state section
		PO_PART,            // property was found in the part section
		PO_CLASS,           // property was found in the class section
		PO_GLOBAL,          // property was found in [globals] section
		PO_NOTFOUND         // property was not found
	};

	__value public enum DrawTextFlags
	{
		dt_TOP                     = DT_TOP,
		dt_LEFT                    = DT_LEFT,
		dt_CENTER                  = DT_CENTER,
		dt_RIGHT                   = DT_RIGHT,
		dt_VCENTER                 = DT_VCENTER,
		dt_BOTTOM                  = DT_BOTTOM,
		dt_WORDBREAK               = DT_WORDBREAK,
		dt_SINGLELINE              = DT_SINGLELINE,
		dt_EXPANDTABS              = DT_EXPANDTABS,
		dt_TABSTOP                 = DT_TABSTOP,
		dt_NOCLIP                  = DT_NOCLIP,
		dt_EXTERNALLEADING         = DT_EXTERNALLEADING,
		dt_CALCRECT                = DT_CALCRECT,
		dt_NOPREFIX                = DT_NOPREFIX,
		dt_INTERNAL                = DT_INTERNAL,
		dt_EDITCONTROL             = DT_EDITCONTROL,
		dt_PATH_ELLIPSIS           = DT_PATH_ELLIPSIS,
		dt_END_ELLIPSIS            = DT_END_ELLIPSIS,
		dt_MODIFYSTRING            = DT_MODIFYSTRING,
		dt_RTLREADING              = DT_RTLREADING,
		dt_WORD_ELLIPSIS           = DT_WORD_ELLIPSIS,
		dt_NOFULLWIDTHCHARBREAK    = DT_NOFULLWIDTHCHARBREAK,
		dt_HIDEPREFIX              = DT_HIDEPREFIX		
	};

	__value public enum HitTestReturnCodes
	{
		htERROR          =  HTERROR       ,
		htTRANSPARENT    =  HTTRANSPARENT ,
		htNOWHERE        =  HTNOWHERE     ,
		htCLIENT         =  HTCLIENT      ,
		htCAPTION        =  HTCAPTION     ,
		htSYSMENU        =  HTSYSMENU     ,
		htGROWBOX        =  HTGROWBOX     ,
		htSIZE           =  HTSIZE        ,
		htMENU           =  HTMENU        ,
		htHSCROLL        =  HTHSCROLL     ,
		htVSCROLL        =  HTVSCROLL     ,
		htMINBUTTON      =  HTMINBUTTON   ,
		htMAXBUTTON      =  HTMAXBUTTON   ,
		htLEFT           =  HTLEFT        ,
		htRIGHT          =  HTRIGHT       ,
		htTOP            =  HTTOP         ,
		htTOPLEFT        =  HTTOPLEFT     ,
		htTOPRIGHT       =  HTTOPRIGHT    ,
		htBOTTOM         =  HTBOTTOM      ,
		htBOTTOMLEFT     =  HTBOTTOMLEFT  ,
		htBOTTOMRIGHT    =  HTBOTTOMRIGHT ,
		htBORDER         =  HTBORDER      ,
		htREDUCE         =  HTREDUCE      ,
		htZOOM           =  HTZOOM        ,
		htSIZEFIRST      =  HTSIZEFIRST   ,
		htSIZELAST       =  HTSIZELAST    ,
		htOBJECT         =  HTOBJECT      ,
		htCLOSE          =  HTCLOSE       ,
		htHELP           =  HTHELP        ,
	};

	__value public enum HitTestOptions
	{
		httb_BACKGROUNDSEG = HTTB_BACKGROUNDSEG,	//  Theme background segment hit test option. 
		httb_FIXEDBORDER = HTTB_FIXEDBORDER,	//  Fixed border hit test option. 
		httb_CAPTION = HTTB_CAPTION,	//  Caption hit test option. 
		httb_RESIZINGBORDER = HTTB_RESIZINGBORDER,	// Resizing border hit test options. 
		httb_RESIZINGBORDER_LEFT = HTTB_RESIZINGBORDER_LEFT,	//  Resizing left border hit test option. 
		httb_RESIZINGBORDER_TOP = HTTB_RESIZINGBORDER_TOP,	//  Resizing top border hit test option. 
		httb_RESIZINGBORDER_RIGHT = HTTB_RESIZINGBORDER_RIGHT,	//  Resizing right border hit test option. 
		httb_RESIZINGBORDER_BOTTOM = HTTB_RESIZINGBORDER_BOTTOM,	//  Resizing bottom border hit test option. 
		httb_SIZINGTEMPLATE = HTTB_SIZINGTEMPLATE,	//  Resizing border is specified as a template, not just window edges. This option is mutually exclusive with HTTB_SYSTEMSIZINGMARGINS; HTTB_SIZINGTEMPLATE takes precedence. 
		httb_SYSTEMSIZINGMARGINS = HTTB_SYSTEMSIZINGMARGINS,	// Uses the system resizing border width rather than visual style content margins. This option is mutually exclusive with HTTB_SIZINGTEMPLATE; HTTB_SIZINGTEMPLATE takes precedence.
	};

	__value public enum EdgeFlags
	{
		bdr_RAISEDINNER = BDR_RAISEDINNER,
		bdr_SUNKENINNER = BDR_SUNKENINNER,
		bdr_RAISEDOUTER = BDR_RAISEDOUTER,
		bdr_SUNKENOUTER = BDR_SUNKENOUTER,
		edge_BUMP = EDGE_BUMP,
		edge_ETCHED = EDGE_ETCHED,
		edge_RAISED = EDGE_RAISED,
		edge_SUNKEN = EDGE_SUNKEN,
	};

	__value public enum BorderFlags
	{
		bf_ADJUST = BF_ADJUST,
		bf_BOTTOM = BF_BOTTOM,
		bf_BOTTOMLEFT = BF_BOTTOMLEFT,
		bf_BOTTOMRIGHT = BF_BOTTOMRIGHT,
		bf_DIAGONAL = BF_DIAGONAL ,
		bf_DIAGONAL_ENDBOTTOMLEFT = BF_DIAGONAL_ENDBOTTOMLEFT,
		bf_DIAGONAL_ENDBOTTOMRIGHT = BF_DIAGONAL_ENDBOTTOMRIGHT,
		bf_DIAGONAL_ENDTOPLEFT = BF_DIAGONAL_ENDTOPLEFT,
		bf_DIAGONAL_ENDTOPRIGHT = BF_DIAGONAL_ENDTOPRIGHT,
		bf_FLAT = BF_FLAT,
		bf_LEFT = BF_LEFT,
		bf_MIDDLE = BF_MIDDLE,
		bf_MONO = BF_MONO,
		bf_RECT = BF_RECT,
		bf_RIGHT = BF_RIGHT,
		bf_SOFT = BF_SOFT,
		bf_TOP = BF_TOP,
		bf_TOPLEFT = BF_TOPLEFT,
		bf_TOPRIGHT = BF_TOPRIGHT,
	};

	__value public enum EnableThemeDialogTextureFlags
	{
		etdt_ENABLE = ETDT_ENABLE,
		etdt_ENABLETAB = ETDT_ENABLETAB,
		etdt_DISABLE = ETDT_DISABLE,
		etdt_USETABTEXTURE = ETDT_USETABTEXTURE,
	};

	__value public enum ThemeAppProperties
	{
		stap_ALLOW_NONCLIENT = STAP_ALLOW_NONCLIENT, // Specifies that the nonclient areas of application windows have visual styles applied. 
		stap_ALLOW_CONTROLS = STAP_ALLOW_CONTROLS, // Specifies that controls in application windows have visual styles applied. 
		stap_ALLOW_WEBCONTENT = STAP_ALLOW_WEBCONTENT, // Specifies that all web content displayed in an application is rendered using visual styles.
	};

	/// <summary> 
	/// UxTheme is a managed wrapeer arounf the UxTheme API
	/// It indlues static functions that directly wrap method that do not take
	/// an HTHEME as an argument.
	/// Instance method wrap the HTHEME
	/// </summary>
	__gc public class UxTheme : public IDisposable
	{

	public private:	
		UxTheme( HTHEME hTheme );

	private:
		~UxTheme();

	public:

		void DrawBackground( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect, System::Drawing::Rectangle ClipRect );
		void DrawBackground( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect );

		void DrawText( Graphics* graphics, Int32 PartId, Int32 StateId, String* Text, Int32 CharCount, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect );

		void DrawEdge( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle DestRect, EdgeFlags Edge, BorderFlags Flags,  System::Drawing::Rectangle ContentRect );
		void DrawIcon( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect, ImageList* imageList, Int32 ImageIndex );

		System::Drawing::Rectangle GetBackgroundContentRect( Graphics* Graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle BoundingRect );
		System::Drawing::Rectangle GetBackgroundExtent( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle ContentRect );
		System::Drawing::Size GetPartSize( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect, ThemeSize eSize );
		System::Drawing::Size GetPartSize( Graphics* graphics, Int32 PartId, Int32 StateId, ThemeSize eSize );

		System::Drawing::Rectangle GetTextExtent( Graphics* graphics, Int32 PartId, Int32 StateId, String* Text, Int32 CharCount, DrawTextFlags TextFlags,  System::Drawing::Rectangle BoundingRect );
		TextMetric GetTextMetrics( Graphics* graphics, Int32 PartId, Int32 StateId );
		Region* GetBackgroundRegion( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect );

		HitTestReturnCodes HitTestBackground( Graphics* graphics, Int32 PartId, Int32 StateId, HitTestOptions Options, System::Drawing::Rectangle pRect, Region* region, System::Drawing::Point ptTest );

		bool IsPartDefined( Int32 PartId, Int32 StateId );
		bool IsBackgroundPartiallyTransparent( Int32 PartId, Int32 StateId );

		System::Drawing::Color GetColor( Int32 PartId, Int32 StateId, Int32 PropId );
		Int32 GetMetric( Graphics* graphics, Int32 PartId, Int32 StateId, Int32 PropId );
		String* GetString( Int32 PartId, Int32 StateId, Int32 PropId );
		bool GetBool( Int32 PartId, Int32 StateId, Int32 PropId );
		Int32 GetInt( Int32 PartId, Int32 StateId, Int32 PropId );
		Int32 GetEnumValue( Int32 PartId, Int32 StateId, Int32 PropId );
		Int32 GetIntList( Int32 PartId, Int32 StateId, Int32 PropId ) [];
		PropertyOrigin GetPropertyOrigin( Int32 PartId, Int32 StateId, Int32 PropId );

		System::Drawing::Point GetPosition( Int32 PartId, Int32 StateId, Int32 PropId );
		System::Drawing::Font* GetFont( Graphics* graphics, Int32 PartId, Int32 StateId, Int32 PropId );
		System::Drawing::Rectangle GetRect( Int32 PartId, Int32 StateId, Int32 PropId );
		Margins GetMargins( Graphics* graphics, Int32 PartId, Int32 StateId, Int32 PropId, System::Drawing::Rectangle rect );

		String* GetFilename( Int32 PartId, Int32 StateId, Int32 PropId );

		System::Drawing::Brush* GetSysColorBrush( Int32 ColorId );	
		bool GetSysBool( Int32 BoolId );
		Int32 GetSysSize( Int32 SizeId );
		String* GetSysString( Int32 StringId );
		Int32 GetSysInt( Int32 IntId );

		Color GetSysColor( Int32 ColorId );
		System::Drawing::Font* GetSysFont( Int32 FontId );

		void Dispose();

		__property IntPtr get_Handle(){ return m_hTheme; }

		static UxTheme* OpenTheme( Control* control, String* ClassList ) ;
		static UxTheme* GetWindowTheme( Control* control );
		
		static void EnableThemeing( bool enable );
		static void SetWindowTheme( Control* control, String* SubAppName, String* SubIdList );
		static void EnableThemeDialogTexture( Control* control, EnableThemeDialogTextureFlags Flags );
		static void DrawThemeParentBackground( Control* control, Graphics* graphics, System::Drawing::Rectangle Rect );

		__property static String* get_CurrentThemeName();
		__property static String* get_CurrentThemeColorName();
		__property static String* get_CurrentThemeSizeName();
		__property static bool get_IsAppThemed();
		__property static bool get_IsThemeActive();
		__property static ThemeAppProperties get_ThemeAppProperties();
		__property static void set_ThemeAppProperties( DotNetLib::Windows::Forms::Themes::ThemeAppProperties Flags );

		static System::Drawing::Font* GetThemeSysFont( Int32 FontId ) ;

		static bool IsThemeDialogTextureEnabled( Control* control );

		static String* GetThemeDocumentationProperty( String* ThemeName, String* PropertyName );

	public private:
		static bool ThemeIsPresent( LPCWSTR name );
		static String* GetLastErrorMessage();

	private:
		HTHEME m_hTheme;
		CVisualStylesXP* m_pXpStyle;
	};
}
}
}
}