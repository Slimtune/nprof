using System;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
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
	public class Profiler
	{
		private const string PROFILER_GUID = "{791DA9FE-05A0-495E-94BF-9AD875C4DF0F}";
		public Profiler()
		{
			_htFunctionMap = new Hashtable();
		}

		public static string Version
		{
			get { return "0.8-alpha"; }
		}

		public bool CheckSetup( out string strMessage )
		{
			strMessage = String.Empty;
			using ( RegistryKey rk = Registry.ClassesRoot.OpenSubKey( "CLSID\\" + PROFILER_GUID ) )
			{
				if ( rk == null )
				{
					strMessage = "Unable to find the registry key for the profiler hook.  Please register the NProf.Hook.dll file.";
					return false;
				}
			}

			return true;
		}

		public bool Start( ProjectInfo p, Run run, ProcessCompletedHandler pch )
		{
			_dtStart = DateTime.Now;
			_pch = pch;
			_run = run;
			_run.State = Run.RunState.Running;

			return Start( p.ApplicationName, p.Arguments, p.WorkingDirectory, p.Options );
		}

		public bool Start( string strApplication, string strArguments, string strWorkingDirectory )
		{
			Options po = new Options();

			return Start( strApplication, strArguments, strWorkingDirectory, po );
		}

		public bool EnableAndStartIIS( ProjectInfo pi, Run run, ProcessCompletedHandler pch )
		{
			_run = run;
			_pch = pch;

			_pss = new ProfilerSocketServer( pi.Options );
			_pss.Start();
			_pss.Exited += new EventHandler( OnProcessExited );
			_pss.Error += new ProfilerSocketServer.ErrorHandler( OnError );
			_pss.Message += new ProfilerSocketServer.MessageHandler( OnMessage );

			using ( RegistryKey rk = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Services\W3SVC" ) )
			{
				ArrayList alItems = new ArrayList( ( string[] )rk.GetValue( "Environment" ) );
				
				for ( int nIndex = 0; nIndex < alItems.Count; nIndex++ )
				{
					string[] astrItemComponents = ( ( string )alItems[ nIndex ] ).Split( '=' );
					string strItemName = astrItemComponents[ 0 ].Trim().ToUpper();
					
					// Remove any of the entries we're going to add back
					if ( strItemName == "COR_ENABLE_PROFILING" || strItemName == "COR_PROFILER" || strItemName == "NPROF_PROFILING_SOCKET" )
					{
						alItems.RemoveAt( nIndex );
						nIndex--;
					}
				}

				alItems.Add( "COR_ENABLE_PROFILING=0x1" );
				alItems.Add( "COR_PROFILER=" + PROFILER_GUID );
				alItems.Add( "NPROF_PROFILING_SOCKET=" + _pss.Port.ToString() );

				rk.SetValue( "Environment", alItems.ToArray( typeof( string ) ) );

				Process p = Process.Start( "iisreset" );
				p.WaitForExit();
				int nExitCode = p.ExitCode;

				if ( nExitCode != 0 )
					return false;
			}

			return true;
		}

		public bool EnableAndStart( ProjectInfo pi, Run run, ProcessCompletedHandler pch )
		{
			_run = run;
			_pch = pch;

			_pss = new ProfilerSocketServer( pi.Options );
			_pss.Start();
			_pss.Exited += new EventHandler( OnProcessExited );
			_pss.Error += new ProfilerSocketServer.ErrorHandler( OnError );
			_pss.Message += new ProfilerSocketServer.MessageHandler( OnMessage );

			SetEnvironmentVariable( "COR_ENABLE_PROFILING", "0x1" );
			SetEnvironmentVariable( "COR_PROFILER", PROFILER_GUID );
			SetEnvironmentVariable( "NPROF_PROFILING_SOCKET", _pss.Port.ToString() );

			return true;
		}

		public void Disable()
		{
			SetEnvironmentVariable( "COR_ENABLE_PROFILING", "0x0" );
		}

		public bool Start( string strApplication, string strArguments, string strWorkingDirectory, Project.Options po )
		{
			_pss = new ProfilerSocketServer( po );
			_pss.Start();
			_pss.Exited += new EventHandler( OnProcessExited );
			_pss.Error += new ProfilerSocketServer.ErrorHandler( OnError );
			_pss.Message += new ProfilerSocketServer.MessageHandler( OnMessage );

			_p = new Process();
			_p.StartInfo = new ProcessStartInfo( strApplication, strArguments );
			_p.StartInfo.EnvironmentVariables.Add( "COR_ENABLE_PROFILING", "0x1" );
			_p.StartInfo.EnvironmentVariables.Add( "COR_PROFILER", PROFILER_GUID );
			_p.StartInfo.EnvironmentVariables.Add( "NPROF_PROFILING_SOCKET", _pss.Port.ToString() );
			_p.StartInfo.UseShellExecute = false;
			_p.StartInfo.Arguments = strArguments;
			_p.StartInfo.WorkingDirectory = strWorkingDirectory;
			_p.EnableRaisingEvents = true;

			//_p.Exited += new EventHandler( OnProcessExited );

			return _p.Start();
		}

		public void Stop()
		{
			if ( _pss != null )
				_pss.Stop();
		}

		private void OnProcessExited( object oSender, EventArgs ea )
		{
			_dtEnd = DateTime.Now;
			_pss.Stop();
//			if ( ProcessCompleted != null )
//				ProcessCompleted( _pss.ThreadInfoCollection );

			_run.EndTime = _dtEnd;
			_run.ThreadInfoCollection = _pss.ThreadInfoCollection;
			_run.Messages = _pss.Messages;

			_pch( _run );
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
			return ( int[] )new ArrayList( _htFunctionMap.Keys ).ToArray( typeof( int ) );
		}

		public string GetFunctionSignature( int nFunctionID )
		{
			return ( string )_htFunctionMap[ nFunctionID ];
		}

		public delegate void ProcessCompletedHandler( Run run );
		public event ProcessCompletedHandler ProcessCompleted;
		public delegate void ErrorHandler( Exception e );
		public event ErrorHandler Error;
		public delegate void MessageHandler( string strMessage );
		public event MessageHandler Message;

		private ProcessCompletedHandler _pch;
		private DateTime _dtStart;
		private DateTime _dtEnd;
		private Run _run;

		[DllImport("Kernel32.dll", CharSet = CharSet.Auto )]
		private static extern bool SetEnvironmentVariable( string strVariable, string strNewValue );

		private Hashtable _htFunctionMap;
		private Process _p;
		private ProfilerSocketServer _pss;
	}
}
