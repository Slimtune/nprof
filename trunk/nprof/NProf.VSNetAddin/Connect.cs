using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using EnvDTE;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;
using Extensibility;
using NProf.Glue.Profiler;
using NProf.Glue.Profiler.Project;
using NProf.GUI;

namespace NProf.VSNetAddin
{
	/// <summary>
	/// Summary description for Connect.
	/// </summary>
	[GuidAttribute( "8B414BB0-DDEE-472d-B0BC-6214EDBDBBA2" ), 
		ProgId( "NProf.Connect" )]
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		public Connect()
		{
			System.Diagnostics.Debug.WriteLine( "Connect()" );
			_bEnabled = false;
		}

		void IDTExtensibility2.OnAddInsUpdate( ref System.Array arrCustom )
		{
		}

		void IDTExtensibility2.OnBeginShutdown( ref System.Array arrCustom )
		{
		}

		void IDTExtensibility2.OnConnection( object oApplication, Extensibility.ext_ConnectMode cmConnectMode, object oAddInInstance, ref System.Array arrCustom )
		{
			_pApplication = ( _DTE )oApplication;
			_pAddInInstance = ( EnvDTE.AddIn )oAddInInstance;

			Command cmdDebug;

			_pf = new ProfilerForm();
			_pf.Owner = null;
			_pf.Closed += new EventHandler(_pf_Closed);

			// Hook up to run events
			cmdDebug = _pApplication.Commands.Item( "Debug.Start", 1 );
			_cmdevDebugStart = _pApplication.Events.get_CommandEvents( cmdDebug.Guid, cmdDebug.ID );
			_cmdevDebugStart.BeforeExecute += new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler( OnBeforeRun );
			_cmdevDebugStart.AfterExecute += new EnvDTE._dispCommandEvents_AfterExecuteEventHandler( OnAfterRun );

			cmdDebug = _pApplication.Commands.Item( "Debug.StartWithoutDebugging", 1 );			
			_cmdevNoDebugStart = _pApplication.Events.get_CommandEvents( cmdDebug.Guid, cmdDebug.ID );
			_cmdevNoDebugStart.BeforeExecute += new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler( OnBeforeRun );
			_cmdevNoDebugStart.AfterExecute += new EnvDTE._dispCommandEvents_AfterExecuteEventHandler( OnAfterRun );

			if ( cmConnectMode == Extensibility.ext_ConnectMode.ext_cm_UISetup )
			{
				object[] contextGUIDS = new object[] { };
				foreach ( Command cmd in _pApplication.Commands )
				{
					try
					{
						if ( cmd.Name != null && cmd.Name.StartsWith( "NProf.Connect" ) )
							cmd.Delete();
					}
					catch ( Exception )
					{
					}
				}

				try
				{
					CommandBar barNProf = _pApplication.CommandBars[ "nprof Profiling" ];
					if ( barNProf != null )
						barNProf.Delete();
				}
				catch ( Exception )
				{
				}

				Command command = _pApplication.Commands.AddNamedCommand( 
					_pAddInInstance, 
					"Enable", 
					"Enable nprof", 
					"Toggle nprof integration", 
					true, 
					0, 
					ref contextGUIDS, 
					(int)vsCommandStatus.vsCommandStatusUnsupported );

				CommandBar barTools = ( CommandBar )_pApplication.CommandBars[ "Tools" ];
				CommandBar barMenu = ( CommandBar )_pApplication.Commands.AddCommandBar( "nprof Profiling", vsCommandBarType.vsCommandBarTypeMenu, barTools, 1 );

				CommandBarControl cbc = command.AddControl( barMenu, 1 );
			}
		}

		void IDTExtensibility2.OnDisconnection( Extensibility.ext_DisconnectMode dmDisconnectMode, ref System.Array arrCustom )
		{
			// Remove command handlers
			_cmdevDebugStart.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler( OnBeforeRun );
			_cmdevDebugStart.AfterExecute -= new EnvDTE._dispCommandEvents_AfterExecuteEventHandler( OnAfterRun );
			_cmdevDebugStart = null;

			_cmdevNoDebugStart.BeforeExecute -= new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler( OnBeforeRun );
			_cmdevNoDebugStart.AfterExecute -= new EnvDTE._dispCommandEvents_AfterExecuteEventHandler( OnAfterRun );
			_cmdevNoDebugStart = null;

			_pApplication = null;
			_pAddInInstance = null;

			_pf = null;
		}

		void IDTExtensibility2.OnStartupComplete( ref System.Array arrCustom )
		{
			System.Diagnostics.Debug.WriteLine( "OnStartupComplete!" );
//			_pApplication.Events.DTEEvents.OnMacrosRuntimeReset += new _dispDTEEvents_OnMacrosRuntimeResetEventHandler( OnRuntimeReset );
//			_pApplication.Events.SolutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler( OnSolutionOpened );
			//new EnvDTE._dispBuildEvents_OnBuildProjConfigBeginEventHandler( );
		}

		void IDTCommandTarget.Exec( 
			string strCommandName, 
			EnvDTE.vsCommandExecOption eoExecOption, 
			ref object oIn,
			ref object oOut,
			ref bool bHandled )
		{
			System.Diagnostics.Debug.WriteLine( String.Format( "Exec: {0}", strCommandName ) );

			_bEnabled = !_bEnabled;

			bHandled = true;
		}

		void IDTCommandTarget.QueryStatus( 
			string strCommandName, 
			EnvDTE.vsCommandStatusTextWanted cstwStatus, 
			ref EnvDTE.vsCommandStatus csCommandStatus, 
			ref object oCommandText )
		{
			//System.Diagnostics.Debug.WriteLine( String.Format( "QueryStatus: {0}", strCommandName ) );
			if( cstwStatus == EnvDTE.vsCommandStatusTextWanted.vsCommandStatusTextWantedNone )
			{
				oCommandText = "Enable Profiling";

				if( strCommandName == "NProf.Connect.Enable" )
				{
					csCommandStatus = vsCommandStatus.vsCommandStatusSupported;

					if ( _pApplication.Solution.IsOpen )
					{
						if ( _bEnabled )
							csCommandStatus |= vsCommandStatus.vsCommandStatusLatched;

						csCommandStatus |= vsCommandStatus.vsCommandStatusEnabled;
					}
				}
			}
		}

		private void OnRuntimeReset()
		{
			System.Diagnostics.Debug.WriteLine( "OnRuntimeReset!" );
		}

		private void OnSolutionOpened()
		{
			System.Diagnostics.Debug.WriteLine( "OnSolutionOpened!" );
		}

		private void OnBeforeRun( string strGUID, int ID, object oIn, object oOut, ref bool bCancelDefault )
		{
			bCancelDefault = false;
			_pf.Disable();

			if ( !_bEnabled )
				return;			

			_pf.Show();
			_pf.EnableAndStart();
		} 

		private void _pf_Closed(object sender, EventArgs e)
		{
			_pf = new ProfilerForm();
			_pf.Owner = null;
			_pf.Closed += new EventHandler( _pf_Closed );
		}

		private void OnAfterRun( string strGUID, int ID, object oIn, object oOut )
		{
			System.Diagnostics.Debug.WriteLine( "OnAfterRun!" );
		}

		private void OnBeforeAllEvents( string strGUID, int ID, object oIn, object oOut, ref bool bCancelDefault )
		{
			System.Diagnostics.Debug.WriteLine( String.Format( "{0} {1}", strGUID, ID ) );
		}

		private _DTE			_pApplication;
		private EnvDTE.AddIn	_pAddInInstance;
		private bool			_bEnabled;
		private CommandEvents	_cmdevDebugStart;
		private CommandEvents	_cmdevNoDebugStart;
		private ProfilerForm	_pf;
	}
}
