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
#include ".\proptable.h"
#using <mscorlib.dll>

#include <tmschema.h>

#define TMT_ENUMDEF 8
#define TMT_ENUMVAL TEXT('A')
#define TMT_ENUM	TEXT('B')

#define SCHEMA_STRINGS
#include "TmSchema.h" 

using namespace System;

int PropTable::s_PropTableStart = -1;

PropTable::PropTable(void)
{
	// the front part of the table contains a bunch of records that we don't use
	// The first time through this code finds the first window class record
	if ( s_PropTableStart == -1 )
	{
		const wchar_t szParts[] = L"PARTS";
		const TMPROPINFO* pPropTable = GetSchemaInfo()->pPropTable;
			
		int i = 0;

		//	Move past the items at the beginning of the file.
		while ( ( i < GetSchemaInfo()->iPropCount ) && ( !wcsstr( pPropTable[i].pszName, szParts ) ) )
			i++;
		
		// if we didn't spin through the whole table remmber the position
		if ( i < GetSchemaInfo()->iPropCount )
			s_PropTableStart = i;
	}

	MASSERT( s_PropTableStart != -1 );
}

PropTable::~PropTable(void)
{
}


int PropTable::GetCount()
{
	return GetSchemaInfo()->iPropCount; 
}

const wchar_t* PropTable::GetPropTableEntry( int index )
{
	MASSERT( index < GetSchemaInfo()->iPropCount );
	const TMPROPINFO* pPropTable = GetSchemaInfo()->pPropTable;

	return pPropTable[index].pszName;
}
