using System;

namespace TestProfilee
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class TestClass
	{
		public static int Main( string[] astrArguments )
		{
			TestClass tc = new TestClass();
			int nType = 1;
			if ( astrArguments.Length > 0 )
				nType = Convert.ToInt32( astrArguments[ 0 ] );

			switch ( nType )
			{
				case 1:
					new TestClass1().Run();
					break;
				case 2:
					new TestClass2().Run();
					break;
				case 3:
					new TestClass3().Run();
					break;
				default:
					Console.WriteLine( "Unknown test: " + nType );
					break;
			}

			return 0;
		}
	}
}
