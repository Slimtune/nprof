using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using NProf.GUI;
using NProf.Glue.Profiler.Project;

namespace NProf.Application
{
	/// <summary>
	/// Main NProf application.
	/// </summary>
	public class Application
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main( string[] args ) 
		{
			// Always print GPL notice
			Console.WriteLine( "NProf version 0.4.1 (C) 2003 by Matthew Mastracci" );
			Console.WriteLine( "http://nprof.sourceforge.net" );
			Console.WriteLine();
			Console.WriteLine( "This is free software, and you are welcome to redistribute it under certain" );
			Console.WriteLine( "conditions set out by the GNU General Public License.  A copy of the license" );
			Console.WriteLine( "is available in the distribution package and from the NProf web site." );
			Console.WriteLine();

			ProfilerForm pf = new ProfilerForm();
			if ( args.Length > 0 )
			{
				pf.InitialProject = CreateProjectInfo( args );
			
				// If we're in command-line mode and we couldn't create a project, return
				if ( pf.InitialProject == null )
					return;
			}

			System.Windows.Forms.Application.Run( pf );
		}

		/// <summary>
		/// Print the usage information for the program.
		/// </summary>
		static void PrintUsage()
		{
			Console.WriteLine( "Usage: nprof [/r:<file> [/w:<workingdir>] [/a:<args>]] | [/help]" );
			Console.WriteLine();
			Console.WriteLine( "Options (use quotes around any arguments containing spaces)");
			Console.WriteLine( "  /r:<file>        The application to profile" );
			Console.WriteLine( "  /w:<workingdir>  Specifies the working directory for the application" );
			Console.WriteLine( "  /a:<args>        Specifies command line arguments" );
			Console.WriteLine();
			Console.WriteLine( @"Example: Run testapp.exe in C:\Program Files\Test App with ""-i -q"" as arguments" );
			Console.WriteLine( @"  nprof /r:testapp.exe ""/w:C:\Program Files\Test App"" ""/a:-i -q""" );
		}

		/// <summary>
		/// Create a <see cref="ProjectInfo"/> from the given arguments.
		/// </summary>
		/// <param name="args">The arguments</param>
		/// <returns>A <see cref="ProjectInfo"/> structure, or null if it could not be created</returns>
		static ProjectInfo CreateProjectInfo( string[] args )
		{
			ProjectInfo pInfo = new ProjectInfo();
			pInfo.Arguments = pInfo.WorkingDirectory = pInfo.Name = pInfo.ApplicationName = String.Empty;
			foreach ( string arg in args )
			{
				string upperArg = arg.ToUpper();
				if ( upperArg.StartsWith( "/R:" ) )
				{
					pInfo.Name = pInfo.ApplicationName = arg.Substring( 3 );
				} 
				else if ( upperArg.StartsWith( "/W:" ) )
				{
					pInfo.WorkingDirectory = arg.Substring( 3 );
				} 
				else if ( upperArg.StartsWith( "/A:" ) )
				{
					pInfo.Arguments = arg.Substring( 3 );
				}
				else if ( upperArg.Equals( "/HELP" ) )
				{
					PrintUsage();
					return null;
				}
				else
				{
					Console.WriteLine( @"Error: Unrecognized argument ""{0}"".  Use /help for usage details.", arg );
					return null;
				}
			}
			
			// Check if the user has specified an application to run
			if ( pInfo.Name.Length == 0 )
			{
				Console.WriteLine("Error: You need to specify an application to run.");
				return null;
			}
			
			// Set the working directory, if not specified
			if ( pInfo.WorkingDirectory.Length == 0 )
			{
				// Note: if the pInfo.Name is rooted, it will override the app startup path
				pInfo.WorkingDirectory = Path.Combine( System.Windows.Forms.Application.StartupPath, Path.GetDirectoryName( pInfo.Name ) );
			}

			return pInfo;
		}	
	}
}
