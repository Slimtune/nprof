using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using NProf.GUI;
using NProf.Glue.Profiler;
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
			EnableVisualStyles();

			// Always print GPL notice
			Console.WriteLine( "NProf version {0} (C) 2003 by Matthew Mastracci", Profiler.Version );
			Console.WriteLine( "http://nprof.sourceforge.net" );
			Console.WriteLine();
			Console.WriteLine( "This is free software, and you are welcome to redistribute it under certain" );
			Console.WriteLine( "conditions set out by the GNU General Public License.  A copy of the license" );
			Console.WriteLine( "is available in the distribution package and from the NProf web site." );
			Console.WriteLine();

			ProfilerForm form = new ProfilerForm();
			if ( args.Length > 0 )
			{
				form.InitialProject = CreateProjectInfo( args );
			
				// If we're in command-line mode and we couldn't create a project, return
				if ( form.InitialProject == null )
					return;
			}

			Console.Out.Flush();
			System.Threading.Thread.CurrentThread.Name = "GUI Thread";
			form.Show();
			if (form.InitialProject == null)
			{
				ProfilerProjectOptionsForm options=new ProfilerProjectOptionsForm();
				options.ShowDialog();
				form.Project=options.Project;
				//form.Controls.Add(new ProfilerControl());
			}
			System.Windows.Forms.Application.Run();
			//System.Windows.Forms.Application.Run( form );
		}

		/// <summary>
		/// Enables XP Visual Styles for those users running a framework that supports Application.EnableVisualStyles
		/// </summary>
		private static void EnableVisualStyles()
		{
			Type applicationType = typeof(System.Windows.Forms.Application);
			Type[] tArray = new Type[0];
			MethodInfo styleMethod = applicationType.GetMethod("EnableVisualStyles", tArray);

			// using .Net version 1.1 or higher
			if(styleMethod != null)
			{
				styleMethod.Invoke(null, null);

				System.Windows.Forms.Application.DoEvents(); // bug fix: http://www.codeproject.com/buglist/EnableVisualStylesBug.asp
			}
		}

		/// <summary>
		/// Print the usage information for the program.
		/// </summary>
		static void PrintUsage()
		{
			Console.WriteLine( "Usage: nprof [/r:<file> [/w:<workingdir>] [/a:<args>]] | [/help] | [/v]" );
			Console.WriteLine();
			Console.WriteLine( "Options (use quotes around any arguments containing spaces)");
			Console.WriteLine( "  /r:<file>        The application to profile" );
			Console.WriteLine( "  /w:<workingdir>  Specifies the working directory for the application" );
			Console.WriteLine( "  /a:<args>        Specifies command line arguments" );
			Console.WriteLine( "  /v               Returns the version of nprof and exists" );
			Console.WriteLine();
			Console.WriteLine( @"Example: Run testapp.exe in C:\Program Files\Test App with ""-i -q"" as arguments" );
			Console.WriteLine( @"  nprof /r:testapp.exe ""/w:C:\Program Files\Test App"" ""/a:-i -q""" );
		}

		/// <summary>
		/// Print the usage information for the program.
		/// </summary>
		static void PrintVersion()
		{
			Console.WriteLine( Profiler.Version );
		}

		/// <summary>
		/// Create a <see cref="ProjectInfo"/> from the given arguments.
		/// </summary>
		/// <param name="args">The arguments</param>
		/// <returns>A <see cref="ProjectInfo"/> structure, or null if it could not be created</returns>
		static ProjectInfo CreateProjectInfo( string[] args )
		{
			ProjectInfo project = new ProjectInfo( ProjectType.File );
			project.Arguments = project.WorkingDirectory = project.ApplicationName = String.Empty;
			foreach ( string arg in args )
			{
				string upperArg = arg.ToUpper();
				if ( upperArg.StartsWith( "/R:" ) )
				{
					project.ApplicationName = Path.GetFullPath( arg.Substring( 3 ) );
					Console.WriteLine( "Application: " + project.ApplicationName );
				} 
				else if ( upperArg.StartsWith( "/W:" ) )
				{
					project.WorkingDirectory = arg.Substring( 3 );
					Console.WriteLine( "Working Directory: " + project.WorkingDirectory );
				} 
				else if ( upperArg.StartsWith( "/A:" ) )
				{
					project.Arguments = arg.Substring( 3 );
					Console.WriteLine( "Arguments: " + project.Arguments );
				}
				else if ( upperArg.Equals( "/V" ) )
				{
					PrintVersion();
					return null;
				}
				else if ( upperArg.Equals( "/HELP" ) || upperArg.Equals( "/H" ) || upperArg.Equals( "/?" ) || upperArg.Equals( "-H" ) || upperArg.Equals( "-HELP" ) || upperArg.Equals( "--HELP" ) )
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
			if ( project.ApplicationName.Length == 0 )
			{
				Console.WriteLine( "Error: You need to specify an application to run." );
				return null;
			}
			
			// Set the working directory, if not specified
			if ( project.WorkingDirectory.Length == 0 )
			{
				// Note: if the pInfo.Name is rooted, it will override the app startup path
				project.WorkingDirectory = Path.Combine( Directory.GetCurrentDirectory(), Path.GetDirectoryName( project.ApplicationName ) );
			}

			return project;
		}	
	}
}
