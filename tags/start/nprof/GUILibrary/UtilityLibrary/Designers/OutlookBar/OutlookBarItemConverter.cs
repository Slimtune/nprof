using System;
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;

namespace UtilityLibrary.Designers
{
	/// <summary>
	/// A custom TypeConvert for OutlookBarItem.  
	/// </summary>
	/// <remarks>
	/// We need this class in order to tell the VS.NET code generator how 
	/// to create OutlookBarItem objects, because we don't want them to be 
	/// created with a default constructor.
	/// </remarks>
	class OutlookBarItemConverter: TypeConverter 
	{
		
	}
}
