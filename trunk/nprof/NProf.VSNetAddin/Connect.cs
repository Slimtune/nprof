using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using EnvDTE;
using Microsoft.Office.Core;
using System.Runtime.InteropServices;
using Extensibility;
using NProf.Glue.Profiler;
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

			if ( cmConnectMode == Extensibility.ext_ConnectMode.ext_cm_AfterStartup )
			{
				object[] contextGUIDS = new object[] { };
				Command command = _pApplication.Commands.AddNamedCommand( _pAddInInstance, "Enable", "Enable nprof", "Toggle nprof integration", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled );
				CommandBar commandBar = (CommandBar)_pApplication.CommandBars["Standard"];
				int nCount = commandBar.Controls.Count;
				CommandBarControl cbc = command.AddControl( commandBar, nCount );
			}
		}

		void IDTExtensibility2.OnDisconnection( Extensibility.ext_DisconnectMode dmDisconnectMode, ref System.Array arrCustom )
		{
		}

		void IDTExtensibility2.OnStartupComplete( ref System.Array arrCustom )
		{
			System.Diagnostics.Debug.WriteLine( "OnStartupComplete!" );
			_pApplication.Events.DTEEvents.OnMacrosRuntimeReset += new _dispDTEEvents_OnMacrosRuntimeResetEventHandler( OnRuntimeReset );
			_pApplication.Events.SolutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler( OnSolutionOpened );
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
			Command cmd;
			CommandEvents cmdev;

			_bEnabled = !_bEnabled;

			cmd = _pApplication.Commands.Item( "Debug.Start", 1 );
			cmdev = _pApplication.Events.get_CommandEvents( cmd.Guid, cmd.ID );
			cmdev.BeforeExecute += new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler( OnBeforeRun );
			cmdev.AfterExecute += new EnvDTE._dispCommandEvents_AfterExecuteEventHandler( OnAfterRun );

			cmd = _pApplication.Commands.Item( "Debug.StartWithoutDebugging", 1 );			
			cmdev = _pApplication.Events.get_CommandEvents( cmd.Guid, cmd.ID );
			cmdev.BeforeExecute += new EnvDTE._dispCommandEvents_BeforeExecuteEventHandler( OnBeforeRun );
			cmdev.AfterExecute += new EnvDTE._dispCommandEvents_AfterExecuteEventHandler( OnAfterRun );

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
				if( strCommandName == "NProf.Connect.Enable" )
				{
					csCommandStatus = vsCommandStatus.vsCommandStatusEnabled;
					if ( !_pApplication.Solution.IsOpen )
					{
						csCommandStatus |= vsCommandStatus.vsCommandStatusUnsupported;
					}
					else
					{
						if ( _bEnabled )
							csCommandStatus |= vsCommandStatus.vsCommandStatusLatched;
						csCommandStatus |= vsCommandStatus.vsCommandStatusSupported;
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
			if ( !_bEnabled )
				return;

			ProfilerForm pf = new ProfilerForm();
			pf.Owner = null;
			pf.Show();
			pf.EnableAndStart();

			//System.Diagnostics.Debug.WriteLine( "OnBeforeRun!" );
			//_p = new Profiler();
			//_p.EnableAndStart( new ProfilerOptions() );
			//object oTemp = null;
//			EnvDTE.Window winToolWindow = _pApplication.Windows.CreateToolWindow( _pAddInInstance, 
//				"NProf.ProfilerControl", ".NET Profiler", "{D34B8507-C286-4d40-83BC-0852E19DEC89}", ref oTemp );
			//_pApplication.MainWindow.LinkedWindows.Add( winToolWindow );
			//winToolWindow.Visible = true;
//			Solution s;
//			Project p;
//			p.Pr
		} 

		private void OnAfterRun( string strGUID, int ID, object oIn, object oOut )
		{
			System.Diagnostics.Debug.WriteLine( "OnAfterRun!" );
			//_p.Disable();
		}

		private void OnBeforeAllEvents( string strGUID, int ID, object oIn, object oOut, ref bool bCancelDefault )
		{
			System.Diagnostics.Debug.WriteLine( String.Format( "{0} {1}", strGUID, ID ) );
		}

		private _DTE			_pApplication;
		private EnvDTE.AddIn	_pAddInInstance;
		private bool			_bEnabled;
	}
}
