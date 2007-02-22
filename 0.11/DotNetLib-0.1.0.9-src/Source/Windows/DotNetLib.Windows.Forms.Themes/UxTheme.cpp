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

#include "UxTheme.h"

#include < vcclr.h >

using namespace DotNetLib::Windows::Forms::Themes;
using namespace System::Drawing;


#define MAX_PROPERTY_LENGTH 99

#define SAFE_GRAPHICS_HDC( g ) \
	g != NULL ? (HDC)graphics->GetHdc().ToInt32() : NULL

#define SAFE_RELEASEDC( g, hdc )\
	if ( g != NULL ) g->ReleaseHdc( hdc );

#define SAFE_CONTROL_HWND( c ) \
	c != NULL ? (HWND)c->Handle.ToInt32() : NULL

#define THROW_ON_FAIL( hr ) \
	if ( FAILED(hr) ) \
		throw new System::Runtime::InteropServices::ExternalException( GetLastErrorMessage(), hr );

UxTheme::UxTheme( HTHEME hTheme )
{	
	m_pXpStyle = new CVisualStylesXP();
	m_hTheme = hTheme;
}

UxTheme::~UxTheme()
{
	Dispose();
	delete m_pXpStyle;
}

void UxTheme::Dispose()
{
	if ( m_hTheme != NULL )
		m_pXpStyle->CloseThemeData( m_hTheme );

	m_hTheme = NULL;

	GC::SuppressFinalize(this);
}

UxTheme* UxTheme::OpenTheme( Control* control, String* ClassList )
{
	THROW_IF_NULL( ClassList );

	const WCHAR __pin*  str = PtrToStringChars( ClassList );

	UxTheme* t = NULL;
	CVisualStylesXP XpStyle;
	HTHEME hTheme = XpStyle.OpenThemeData( SAFE_CONTROL_HWND( control ), str );
	if ( hTheme != NULL )
		t = new UxTheme( hTheme );

	return t;
}

UxTheme* UxTheme::GetWindowTheme( Control* control )
{
	THROW_IF_NULL( control );

	UxTheme* t = NULL;
	CVisualStylesXP XpStyle;
	HTHEME hTheme = XpStyle.GetWindowTheme( (HWND)(control->Handle.ToInt32()) );
	if ( hTheme != NULL )
		t = new UxTheme( hTheme );

	return t;
}

System::Drawing::Font* UxTheme::GetSysFont( Int32 FontId )
{
	LOGFONT lf;
	m_pXpStyle->GetThemeSysFont( m_hTheme, FontId, &lf );
	return System::Drawing::Font::FromHfont( ::CreateFontIndirect( &lf ) );
}

bool UxTheme::IsThemeDialogTextureEnabled( Control* control )
{
	THROW_IF_NULL( control );
	CVisualStylesXP XpStyle;
	return XpStyle.IsThemeDialogTextureEnabled((HWND)control->Handle.ToInt32()) == TRUE ? true : false;
}

void UxTheme::DrawBackground( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect, System::Drawing::Rectangle ClipRect )
{
	THROW_IF_NULL( graphics );

	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	RECT clipRect;
	SetRect( &clipRect, ClipRect.Left, ClipRect.Top, ClipRect.Right, ClipRect.Bottom );
	
	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->DrawThemeBackground( m_hTheme, hdc, PartId, StateId, &rect, &clipRect );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );
}

void UxTheme::DrawBackground( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect )
{
	THROW_IF_NULL( graphics );

	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->DrawThemeBackground( m_hTheme, hdc, PartId, StateId, &rect, NULL );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );
}

void UxTheme::DrawText( Graphics* graphics, Int32 PartId, Int32 StateId, String* Text, Int32 CharCount, DrawTextFlags TextFlags, bool Grayed, System::Drawing::Rectangle Rect)
{
	THROW_IF_NULL( graphics );
	THROW_IF_NULL( Text );

	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	const WCHAR __pin*  str = PtrToStringChars( Text );

	DWORD dwTextFlags2 = Grayed ? DTT_GRAYED : NULL;

	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->DrawThemeText( m_hTheme, hdc, PartId, StateId, str, CharCount, (DWORD)TextFlags, dwTextFlags2, &rect );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );
}

System::Drawing::Rectangle UxTheme::GetBackgroundContentRect( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle BoundingRect )
{
	RECT boundingRect;
	SetRect( &boundingRect, BoundingRect.Left, BoundingRect.Top, BoundingRect.Right, BoundingRect.Bottom );
	
	RECT outRect;
	
	HDC hdc = SAFE_GRAPHICS_HDC( graphics );
	HRESULT hr = m_pXpStyle->GetThemeBackgroundContentRect( m_hTheme, hdc, PartId, StateId, &boundingRect, &outRect );
	SAFE_RELEASEDC( graphics, hdc );

	THROW_ON_FAIL( hr );

	return Rectangle::FromLTRB( outRect.left, outRect.top, outRect.right, outRect.bottom );
}

System::Drawing::Rectangle UxTheme::GetBackgroundExtent( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle ContentRect )
{
	RECT contentRect;
	SetRect( &contentRect, ContentRect.Left, ContentRect.Top, ContentRect.Right, ContentRect.Bottom );
	
	RECT outRect;
	
	HDC hdc = SAFE_GRAPHICS_HDC( graphics );
	HRESULT hr = m_pXpStyle->GetThemeBackgroundExtent( m_hTheme, hdc, PartId, StateId, &contentRect, &outRect );
	SAFE_RELEASEDC( graphics, hdc );

	THROW_ON_FAIL( hr );

	return Rectangle::FromLTRB( outRect.left, outRect.top, outRect.right, outRect.bottom );
}

System::Drawing::Size UxTheme::GetPartSize( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect, ThemeSize eSize )
{
	THROW_IF_NULL( graphics );

	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	SIZE size;

	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->GetThemePartSize( m_hTheme, hdc, PartId, StateId, &rect, (THEMESIZE)eSize, &size );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );

	Size outSize;
	outSize.set_Height( size.cy );
	outSize.set_Width( size.cx );

	return outSize;
}

System::Drawing::Size UxTheme::GetPartSize( Graphics* graphics, Int32 PartId, Int32 StateId, ThemeSize eSize )
{
	THROW_IF_NULL( graphics );

	SIZE size;

	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->GetThemePartSize( m_hTheme, hdc, PartId, StateId, NULL, (THEMESIZE)eSize, &size );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );

	Size outSize;
	outSize.set_Height( size.cy );
	outSize.set_Width( size.cx );

	return outSize;
}

System::Drawing::Rectangle UxTheme::GetTextExtent( Graphics* graphics, Int32 PartId, Int32 StateId, String* Text, Int32 CharCount, DrawTextFlags TextFlags, System::Drawing::Rectangle BoundingRect )
{
	THROW_IF_NULL( graphics );
	THROW_IF_NULL( Text );

	RECT boundingRect;
	SetRect( &boundingRect, BoundingRect.Left, BoundingRect.Top, BoundingRect.Right, BoundingRect.Bottom );
	
	const WCHAR __pin*  str = PtrToStringChars( Text );
	RECT outRect;
	
	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->GetThemeTextExtent( m_hTheme, hdc, PartId, StateId, str, CharCount, (DWORD)TextFlags, &boundingRect, &outRect );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );

	return Rectangle::FromLTRB( outRect.left, outRect.top, outRect.right, outRect.bottom );
}

TextMetric UxTheme::GetTextMetrics( Graphics* graphics, Int32 PartId, Int32 StateId )
{
	TEXTMETRIC tm;

	HDC hdc = SAFE_GRAPHICS_HDC( graphics );
	HRESULT hr = m_pXpStyle->GetThemeTextMetrics( m_hTheme, hdc, PartId, StateId, &tm );
	SAFE_RELEASEDC( graphics, hdc );

	THROW_ON_FAIL( hr );

	TextMetric metric( &tm );
	return metric;
}

Region* UxTheme::GetBackgroundRegion( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect )
{
	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	HRGN hRgn;

	HDC hdc = SAFE_GRAPHICS_HDC( graphics );
	HRESULT hr = m_pXpStyle->GetThemeBackgroundRegion( m_hTheme, hdc, PartId, StateId, &rect, &hRgn );
	SAFE_RELEASEDC( graphics, hdc );

	THROW_ON_FAIL( hr );

	return System::Drawing::Region::FromHrgn( hRgn );
}

HitTestReturnCodes UxTheme::HitTestBackground( Graphics* graphics, Int32 PartId, Int32 StateId, HitTestOptions Options, System::Drawing::Rectangle Rect, Region* region, System::Drawing::Point ptTest )
{
	THROW_IF_NULL( graphics );

	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	POINT point;
	point.x = ptTest.X;
	point.y = ptTest.Y;

	WORD HitTestCode;
	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->HitTestThemeBackground( m_hTheme, hdc, PartId, StateId, (DWORD)Options, &rect, (HRGN)region->GetHrgn( graphics ).ToInt32(), point, &HitTestCode );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );

	return (HitTestReturnCodes)HitTestCode;
}

void UxTheme::DrawEdge( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle DestRect, EdgeFlags Edge, BorderFlags Flags, System::Drawing::Rectangle ContentRect )
{
	THROW_IF_NULL( graphics );

	RECT rect;
	SetRect( &rect, DestRect.Left, DestRect.Top, DestRect.Right, DestRect.Bottom );

	RECT contentRect;
	SetRect( &contentRect, ContentRect.Left, ContentRect.Top, ContentRect.Right, ContentRect.Bottom );

	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->DrawThemeEdge( m_hTheme, hdc, PartId, StateId, &rect, (UINT)Edge, (UINT)Flags, &contentRect );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );
}

void UxTheme::DrawIcon( Graphics* graphics, Int32 PartId, Int32 StateId, System::Drawing::Rectangle Rect, ImageList* imageList, Int32 iImageIndex )
{
	THROW_IF_NULL( graphics );
	THROW_IF_NULL( imageList );

	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	HDC	hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = m_pXpStyle->DrawThemeIcon( m_hTheme, hdc, PartId, StateId, &rect, (HIMAGELIST)imageList->Handle.ToInt32(), iImageIndex );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );
}

bool UxTheme::IsPartDefined(Int32 PartId, Int32 StateId)
{
	return m_pXpStyle->IsThemePartDefined( m_hTheme, PartId, StateId ) == TRUE ? true : false;
}

bool UxTheme::IsBackgroundPartiallyTransparent( Int32 PartId, Int32 StateId)
{
	return m_pXpStyle->IsThemeBackgroundPartiallyTransparent( m_hTheme, PartId, StateId ) == TRUE ? true : false;
}

System::Drawing::Color UxTheme::GetColor( Int32 PartId, Int32 StateId, Int32 PropId )
{
	COLORREF c;
	HRESULT hr = m_pXpStyle->GetThemeColor( m_hTheme, PartId, StateId, PropId, &c );
	THROW_ON_FAIL( hr );

	return Color::FromArgb( GetRValue( c ), GetGValue( c ), GetBValue( c ) );
}

Int32 UxTheme::GetMetric( Graphics* graphics, Int32 PartId, Int32 StateId, Int32 PropId )
{
	int val;

	HDC hdc = SAFE_GRAPHICS_HDC( graphics );
	HRESULT hr = m_pXpStyle->GetThemeMetric( m_hTheme, hdc, PartId, StateId, PropId, &val );
	SAFE_RELEASEDC( graphics, hdc );

	THROW_ON_FAIL( hr );

	return val;
}

String* UxTheme::GetString( Int32 PartId, Int32 StateId, Int32 PropId )
{
	WCHAR str[MAX_PROPERTY_LENGTH];

	HRESULT hr = m_pXpStyle->GetThemeString( m_hTheme, PartId, StateId, PropId, str, MAX_PROPERTY_LENGTH );
	THROW_ON_FAIL( hr );

	return System::Runtime::InteropServices::Marshal::PtrToStringAuto( str );
}

bool UxTheme::GetBool( Int32 PartId, Int32 StateId, Int32 PropId )
{
	BOOL val;
	HRESULT hr = m_pXpStyle->GetThemeBool( m_hTheme, PartId, StateId, PropId, &val );
	THROW_ON_FAIL( hr );

	return val == TRUE ? true : false;
}

PropertyOrigin UxTheme::GetPropertyOrigin( Int32 PartId, Int32 StateId, Int32 PropId )
{
	PROPERTYORIGIN origin;
	HRESULT hr = m_pXpStyle->GetThemePropertyOrigin( m_hTheme, PartId, StateId, PropId, &origin );
	THROW_ON_FAIL( hr );

	return (PropertyOrigin)origin;
}

Int32 UxTheme::GetInt( Int32 PartId, Int32 StateId, Int32 PropId )
{
	int val;
	HRESULT hr = m_pXpStyle->GetThemeInt( m_hTheme, PartId, StateId, PropId, &val );
	THROW_ON_FAIL( hr );

	return val;
}

Int32 UxTheme::GetEnumValue( Int32 PartId, Int32 StateId, Int32 PropId )
{
	int val;
	HRESULT hr = m_pXpStyle->GetThemeEnumValue( m_hTheme, PartId, StateId, PropId, &val );
	THROW_ON_FAIL( hr );

	return val;
}

Color UxTheme::GetSysColor(Int32 ColorId)
{
	COLORREF c = m_pXpStyle->GetThemeSysColor( m_hTheme, ColorId );

	return Color::FromArgb( GetRValue( c ), GetGValue( c ), GetBValue( c ) );
}

System::Drawing::Point UxTheme::GetPosition( Int32 PartId, Int32 StateId, Int32 PropId )
{
	POINT pt;
	HRESULT hr = m_pXpStyle->GetThemePosition( m_hTheme, PartId, StateId, PropId, &pt );
	THROW_ON_FAIL( hr );

	Point point( pt.x, pt.y );
	return point;
}

System::Drawing::Font* UxTheme::GetFont( Graphics* graphics, Int32 PartId, Int32 StateId, Int32 PropId )
{
	LOGFONT lf;

	HDC hdc = SAFE_GRAPHICS_HDC( graphics );
	HRESULT hr = m_pXpStyle->GetThemeFont( m_hTheme, hdc, PartId, StateId, PropId, &lf );
	SAFE_RELEASEDC( graphics, hdc );

	THROW_ON_FAIL( hr );

	return System::Drawing::Font::FromHfont( ::CreateFontIndirect( &lf ) ); 
}

System::Drawing::Rectangle UxTheme::GetRect( Int32 PartId, Int32 StateId, Int32 PropId )
{
	RECT rect;

	HRESULT hr = m_pXpStyle->GetThemeRect( m_hTheme, PartId, StateId, PropId, &rect );
	THROW_ON_FAIL( hr );

	return Rectangle::FromLTRB( rect.left, rect.top, rect.right, rect.bottom );
}

Margins UxTheme::GetMargins( Graphics* graphics, Int32 PartId, Int32 StateId, Int32 PropId, System::Drawing::Rectangle Rect )
{
	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	MARGINS margins;

	HDC hdc = SAFE_GRAPHICS_HDC( graphics );
	HRESULT hr = m_pXpStyle->GetThemeMargins( m_hTheme, hdc, PartId, StateId, PropId, &rect, &margins );
	SAFE_RELEASEDC( graphics, hdc );

	THROW_ON_FAIL( hr );

	Margins ret;
	ret.cxLeftWidth = margins.cxLeftWidth;
	ret.cxRightWidth = margins.cxRightWidth;
	ret.cyBottomHeight = margins.cyBottomHeight;
	ret.cyTopHeight = margins.cyTopHeight;

	return ret;
}

Int32 UxTheme::GetIntList( Int32 PartId, Int32 StateId, Int32 PropId )[]
{
	INTLIST intList;
	HRESULT hr = m_pXpStyle->GetThemeIntList( m_hTheme, PartId, StateId, PropId, &intList );
	THROW_ON_FAIL( hr );

	Int32 ia[] = __gc new Int32[intList.iValueCount];

	for ( int i = 0; i < intList.iValueCount; i++ )
		ia[i] = intList.iValues[i];

	return ia;
}

void UxTheme::SetWindowTheme( Control* control, String* SubAppName, String* SubIdList )
{
	THROW_IF_NULL( control );

	const WCHAR __pin*  szAppName = SubAppName == NULL ? NULL : PtrToStringChars( SubAppName );
	const WCHAR __pin*  szSubIdList = SubIdList == NULL ? NULL : PtrToStringChars( SubIdList );

	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.SetWindowTheme( (HWND)control->Handle.ToInt32(), szAppName, szSubIdList );
	THROW_ON_FAIL( hr );
}

String* UxTheme::GetFilename( Int32 PartId, Int32 StateId, Int32 PropId )
{
	WCHAR str[_MAX_PATH];
	HRESULT hr = m_pXpStyle->GetThemeFilename( m_hTheme, PartId, StateId, PropId, str, _MAX_PATH );
	THROW_ON_FAIL( hr );

	return System::Runtime::InteropServices::Marshal::PtrToStringAuto( str );
}

System::Drawing::Brush* UxTheme::GetSysColorBrush( Int32 ColorId )
{
	return new System::Drawing::SolidBrush( this->GetSysColor( ColorId ) );
}
	
bool UxTheme::GetSysBool( Int32 BoolId )
{
	return m_pXpStyle->GetThemeSysBool( m_hTheme, BoolId ) == TRUE ? true : false;
}

Int32 UxTheme::GetSysSize( Int32 SizeId )
{
	return m_pXpStyle->GetThemeSysSize( m_hTheme, SizeId );
}

String* UxTheme::GetSysString( Int32 StringId )
{
	WCHAR str[MAX_PROPERTY_LENGTH];

	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.GetThemeSysString( m_hTheme, StringId, str, MAX_PROPERTY_LENGTH );
	THROW_ON_FAIL( hr );

	return System::Runtime::InteropServices::Marshal::PtrToStringAuto( str );
}

Int32 UxTheme::GetSysInt( Int32 iIntId )
{
	int val;
	HRESULT hr = m_pXpStyle->GetThemeSysInt( m_hTheme, iIntId, &val );
	THROW_ON_FAIL( hr );

	return val;
}

void UxTheme::EnableThemeDialogTexture( Control* control, EnableThemeDialogTextureFlags Flags )
{
	THROW_IF_NULL( control );

	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.EnableThemeDialogTexture( (HWND)control->Handle.ToInt32(), (DWORD)Flags );
	THROW_ON_FAIL( hr );
}

void UxTheme::DrawThemeParentBackground( Control* control, Graphics* graphics, System::Drawing::Rectangle Rect )
{
	THROW_IF_NULL( control );
	THROW_IF_NULL( graphics );

	RECT rect;
	SetRect( &rect, Rect.Left, Rect.Top, Rect.Right, Rect.Bottom );

	CVisualStylesXP XpStyle;

	HDC hdc = (HDC)graphics->GetHdc().ToInt32();
	HRESULT hr = XpStyle.DrawThemeParentBackground( (HWND)control->Handle.ToInt32(), hdc, &rect );
	graphics->ReleaseHdc( hdc );

	THROW_ON_FAIL( hr );
}

String* UxTheme::get_CurrentThemeName()
{
	WCHAR name[MAX_PROPERTY_LENGTH];

	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.GetCurrentThemeName( name, MAX_PROPERTY_LENGTH, NULL, 0, NULL, 0 );
	THROW_ON_FAIL( hr );

	return System::Runtime::InteropServices::Marshal::PtrToStringAuto( name );
}

String* UxTheme::get_CurrentThemeColorName()
{
	WCHAR name[MAX_PROPERTY_LENGTH];
	WCHAR str[MAX_PROPERTY_LENGTH];

	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.GetCurrentThemeName( name, MAX_PROPERTY_LENGTH, str, MAX_PROPERTY_LENGTH, NULL, 0 );
	THROW_ON_FAIL( hr );

	return System::Runtime::InteropServices::Marshal::PtrToStringAuto( str );
}

String* UxTheme::get_CurrentThemeSizeName()
{
	WCHAR name[MAX_PROPERTY_LENGTH];
	WCHAR str[MAX_PROPERTY_LENGTH];

	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.GetCurrentThemeName( name, MAX_PROPERTY_LENGTH, NULL, 0, str, MAX_PROPERTY_LENGTH );
	THROW_ON_FAIL( hr );

	return System::Runtime::InteropServices::Marshal::PtrToStringAuto( str );
}

String* UxTheme::GetThemeDocumentationProperty( String* pszThemeName,	String* pszPropertyName )
{
	WCHAR str[MAX_PROPERTY_LENGTH];

	const WCHAR __pin*  strName = PtrToStringChars( pszThemeName );
	const WCHAR __pin*  strProperty = PtrToStringChars( pszPropertyName );
		
	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.GetThemeDocumentationProperty( strName, strProperty, str, MAX_PROPERTY_LENGTH );
	THROW_ON_FAIL( hr );

	return System::Runtime::InteropServices::Marshal::PtrToStringAuto( str );
}

System::Drawing::Font* UxTheme::GetThemeSysFont( Int32 FontId )
{
	LOGFONT lf;

	CVisualStylesXP XpStyle;
	XpStyle.GetThemeSysFont( NULL, FontId, &lf );
	return System::Drawing::Font::FromHfont( ::CreateFontIndirect( &lf ) );
}

bool UxTheme::get_IsAppThemed()
{
	CVisualStylesXP XpStyle;
	return XpStyle.IsAppThemed() == TRUE ? true : false;
}

bool UxTheme::get_IsThemeActive()
{
	CVisualStylesXP XpStyle;
	return XpStyle.IsThemeActive() == TRUE ? true : false;
}

void UxTheme::EnableThemeing( bool enable )
{
	CVisualStylesXP XpStyle;
	HRESULT hr = XpStyle.EnableTheming(enable == true ? TRUE : FALSE);
	
	THROW_ON_FAIL( hr );
}

ThemeAppProperties UxTheme::get_ThemeAppProperties()
{
	CVisualStylesXP XpStyle;
	return (DotNetLib::Windows::Forms::Themes::ThemeAppProperties)XpStyle.GetThemeAppProperties();
}

void UxTheme::set_ThemeAppProperties( DotNetLib::Windows::Forms::Themes::ThemeAppProperties Flags )
{
	CVisualStylesXP XpStyle;
	XpStyle.SetThemeAppProperties( (DWORD)Flags );
}

bool UxTheme::ThemeIsPresent( LPCWSTR name )
{
	wchar_t szClassName[35];

	//strip off the PARTS part
	int n = wcslen( name ) - 5;
	wcsncpy(szClassName, name, n);
	szClassName[n] = TEXT('\0');    // Add the null terminator.

	CVisualStylesXP XpStyle;
	HTHEME hTheme = XpStyle.OpenThemeData( NULL, szClassName );
	bool ret = hTheme != NULL;
	if ( ret )
		XpStyle.CloseThemeData( hTheme );

	return ret;
}

String* UxTheme::GetLastErrorMessage()
{
	LPVOID lpMsgBuf;
	if (!FormatMessage( 
		FORMAT_MESSAGE_ALLOCATE_BUFFER | 
		FORMAT_MESSAGE_FROM_SYSTEM | 
		FORMAT_MESSAGE_IGNORE_INSERTS,
		NULL,
		GetLastError(),
		MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
		(LPTSTR) &lpMsgBuf,
		0,
		NULL ))
	{
		// Handle the error.
		return S"UxTheme.dll returned an unkown error";
	}

	String* s = System::Runtime::InteropServices::Marshal::PtrToStringAnsi( lpMsgBuf );

	// Free the buffer.
	LocalFree( lpMsgBuf );

	return s;
}
