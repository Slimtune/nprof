using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.ServiceProcess;
using NProf.Glue.Profiler.Core;
using NProf.Glue.Profiler.Info;
using NProf.Glue.Profiler.Project;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace NProf.Glue.Profiler
{
	/// <summary>
	/// Summary description for Profiler.
	/// </summary>
	[Serializable]
	public class Profiler
	{
		private const string PROFILER_GUID = "{791DA9FE-05A0-495E-94BF-9AD875C4DF0F}";
		public Profiler()
		{
			functionMap = new Hashtable();
		}

		public static string Version
		{
			get { return "0.9-alpha"; }
		}

		public bool CheckSetup( out string message )
		{
			message = String.Empty;
			using ( RegistryKey rk = Registry.ClassesRoot.OpenSubKey( "CLSID\\" + PROFILER_GUID ) )
			{
				if ( rk == null )
				{
					message = "Unable to find the registry key for the profiler hook.  Please register the NProf.Hook.dll file.";
					return false;
				}
			}

			return true;
		}

		public bool Start( ProjectInfo pi, Run run, ProcessCompletedHandler pch )
		{
			this.start = DateTime.Now;
			this.project = pi;
			this.completed = pch;
			this.run = run;
			this.run.State = Run.RunState.Initializing;

			socketServer = new ProfilerSocketServer( pi.Options, run );
			socketServer.Start();
			socketServer.Exited += new EventHandler( OnProcessExited );
			socketServer.Error += new ProfilerSocketServer.ErrorHandler( OnError );
			socketServer.Message += new ProfilerSocketServer.MessageHandler( OnMessage );

			switch ( pi.ProjectType )
			{
				case ProjectType.File:
				{
					process = new Process();
					process.StartInfo = new ProcessStartInfo( pi.ApplicationName, pi.Arguments );
					process.StartInfo.EnvironmentVariables[ "COR_ENABLE_PROFILING" ] = "0x1";
					process.StartInfo.EnvironmentVariables[ "COR_PROFILER" ] = PROFILER_GUID;
					process.StartInfo.EnvironmentVariables[ "NPROF_PROFILING_SOCKET" ] = socketServer.Port.ToString();
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.Arguments = pi.Arguments;
					process.StartInfo.WorkingDirectory = pi.WorkingDirectory;
					process.EnableRaisingEvents = true;
					//_p.Exited += new EventHandler( OnProcessExited );

					return process.Start();
				}

				case ProjectType.AspNet:
				{
					using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\W3SVC", true ) )
					{
						if ( rk != null )
							SetRegistryKeys( rk, true );
					}

					using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\IISADMIN", true ) )
					{
						if ( rk != null )
							SetRegistryKeys( rk, true );
					}

					Process p = Process.Start( "iisreset.exe", "" );
					p.WaitForExit();
					this.run.Messages.AddMessage( "Navigate to your project and ASP.NET will connect to the profiler" );
					this.run.Messages.AddMessage( "NOTE: ASP.NET must be set to run under the SYSTEM account in machine.config" );
					this.run.Messages.AddMessage( @"If ASP.NET cannot be profiled, ensure that the userName=""SYSTEM"" in the <processModel> section of machine.config." );

					return true;
				}

				case ProjectType.VSNet:
				{
					SetEnvironmentVariable( "COR_ENABLE_PROFILING", "0x1" );
					SetEnvironmentVariable( "COR_PROFILER", PROFILER_GUID );
					SetEnvironmentVariable( "NPROF_PROFILING_SOCKET", socketServer.Port.ToString() );

					return true;
				}

				default:
					throw new InvalidOperationException( "Unknown project type: " + pi.ProjectType );
			}
		}

		public void Disable()
		{
			SetEnvironmentVariable( "COR_ENABLE_PROFILING", "0x0" );
		}

		public void Stop()
		{
			Run r;

			lock ( runLock )
			{
				r = run;

				// Is there anything to stop?
				if ( run == null )
					return;

				run = null;
			}

			// Stop the profiler socket server if profilee hasn't connected
			if ( r.State == Run.RunState.Initializing )
			{
				r.Messages.AddMessage( "Shutting down profiler..." );
				socketServer.Stop();
				r.State = Run.RunState.Finished;
				r.Success = false;
			}

			if ( project.ProjectType == ProjectType.AspNet )
			{
				using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\W3SVC", true ) )
				{
					if ( rk != null )
						SetRegistryKeys( rk, false );
				}

				using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\IISADMIN", true ) )
				{
					if ( rk != null )
						SetRegistryKeys( rk, false );
				}

				r.Messages.AddMessage( "Terminating ASP.NET..." );
				Process.Start( "iisreset.exe", "/stop" ).WaitForExit();
			}
		}

		private void OnProcessExited( object oSender, EventArgs ea )
		{
			Run r;

			lock ( runLock )
			{
				r = run;

				// This gets called twice for file projects - FIXME
				if ( run == null )
					return;

				run = null;
			}

			if ( !socketServer.HasStoppedGracefully )
			{
				if ( r.State == Run.RunState.Initializing )
				{
					r.Messages.AddMessage( "No connection made with profiled application." );
					r.Messages.AddMessage( "Application might not support .NET." );
				}
				else
				{
					r.Messages.AddMessage( "Application stopped before profiler information could be retrieved." );
				}

				r.Success = false;
				r.State = Run.RunState.Finished;
				r.Messages.AddMessage( "Profiler run did not complete successfully." );
			}
			else
			{
				r.Success = true;
			}

			end = DateTime.Now;
			r.Messages.AddMessage( "Stopping profiler listener..." );
			socketServer.Stop();
//			if ( ProcessCompleted != null )
//				ProcessCompleted( _pss.ThreadInfoCollection );

			r.EndTime = end;

			completed( r );
		}

		private void OnError( Exception e )
		{
			if ( Error != null )
				Error( e );
		}

		private void OnMessage( string strMessage )
		{
			if ( Message != null )
				Message( strMessage );
		}

		public int[] GetFunctionIDs()
		{
			return ( int[] )new ArrayList( functionMap.Keys ).ToArray( typeof( int ) );
		}

		public string GetFunctionSignature( int nFunctionID )
		{
			return ( string )functionMap[ nFunctionID ];
		}

		private void SetRegistryKeys( RegistryKey key, bool isSet )
		{
			if ( key == null )
				return;
			
			if ( !isSet )
			{
				// Get rid of the environment
				key.DeleteValue( "Environment", false );
				return;
			}

			object oKeys = key.GetValue( "Environment" );
			
			// If it's not something we expected, fix it
			if ( oKeys == null || !( oKeys is string[] ) )
				oKeys = new string[ 0 ]; 

			// Save the environment the first time through
			if ( key.GetValue( "nprof Saved Environment" ) == null && ( ( string[] )oKeys ).Length > 0 )
				key.SetValue( "nprof Saved Environment", oKeys );

			Hashtable items = new Hashtable( Environment.GetEnvironmentVariables() );

			// Set the environment to be the default system environment
			using ( RegistryKey rkEnv = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment" ) )
			{
				if ( rkEnv == null )
					throw new InvalidOperationException( "Unable to locate machine environment key" );

				foreach ( string strValueName in rkEnv.GetValueNames() )
					items[ strValueName ] = rkEnv.GetValue( strValueName );
				
			}

			items.Remove( "COR_ENABLE_PROFILING" );
			items.Remove( "COR_PROFILER" );
			items.Remove( "NPROF_PROFILING_SOCKET" );

			items.Add( "COR_ENABLE_PROFILING", "0x1" );
			items.Add( "COR_PROFILER", PROFILER_GUID );
			items.Add( "NPROF_PROFILING_SOCKET", socketServer.Port.ToString() );

			ArrayList itemList = new ArrayList();
			foreach ( DictionaryEntry de in items )
				itemList.Add( String.Format( "{0}={1}", de.Key, de.Value ) );

			key.SetValue( "Environment", ( string[] )itemList.ToArray( typeof( string ) ) );
		}

		public delegate void ProcessCompletedHandler( Run run );
		[field:NonSerialized]
		public event ProcessCompletedHandler ProcessCompleted;
		public delegate void ErrorHandler( Exception e );
		[field:NonSerialized]
		public event ErrorHandler Error;
		public delegate void MessageHandler( string strMessage );
		[field:NonSerialized]
		public event MessageHandler Message;

		[NonSerialized]
		private ProcessCompletedHandler completed;
		private DateTime start;
		private DateTime end;
		private Run run;
		private object runLock = 0;

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto )]
		private static extern bool SetEnvironmentVariable( string strVariable, string strNewValue );

		private Hashtable functionMap;
		[NonSerialized]
		private Process process;
		private ProjectInfo project;
		[NonSerialized]
		private ProfilerSocketServer socketServer;
	}
}
