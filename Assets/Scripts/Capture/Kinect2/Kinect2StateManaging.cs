#region Description
/* 		Multiple Kinect for Windows version 2 Controller
 * 		Tcp/Ip Server , Kinect for Windows version 2 clients
 * 		Handles the messaging system
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 */
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

using Stateless;

using Kinect2.Remote;
#endregion
namespace Kinect2
{
	internal sealed partial class Kinect2Managing : Capturer,IReconstructor,ISyncer,ICalibrator
	{
		#region Properties
		public override bool CanRecord { get { return this.machine.CanFire(KinectAction.Record); } }		
		public override bool CanStop { get { return this.machine.CanFire(KinectAction.Stop); } }						
		public bool CanCalibrate { get { return this.machine.CanFire(KinectAction.Calibrate); } }
		public bool CanSync { get { return this.machine.CanFire(KinectAction.Sync); } }
		public bool CanReconstruct { get { return this.machine.CanFire(KinectAction.Reconstruct); } }		

		public override bool IsRecording { get { return (this.State & KinectState.Recording) != KinectState.None; } }			
		public bool IsCalibrating { get { return (this.State & KinectState.Calibrating) != KinectState.None; } }
		public bool IsSyncing { get { return (this.State & KinectState.Syncing) != KinectState.None; } }
		public bool IsReconstructing { get { return (this.State == KinectState.Reconstructing); } }
		#endregion
		#region Methods
		public  bool StartCalibration()
		{
			this.machine.Fire(KinectAction.Calibrate);
			return this.machine.State == KinectState.Calibrating;
		}
		
		public bool StartSynchronization()
		{
			this.machine.Fire(KinectAction.Sync);
			return this.machine.State == KinectState.Syncing;
		}
		
		public override bool StartRecording()
		{
			this.machine.Fire(KinectAction.Record);
			return this.machine.State == KinectState.Recording;
		}

		public bool StopRecording()
		{
			this.machine.Fire(KinectAction.Stop);
			return this.machine.State == KinectState.Idle;
		}

		public override bool Stop()
		{
			this.machine.Fire(KinectAction.Stop);
			return this.machine.State == KinectState.Idle;
		}
		
		public bool StartReconstruction()
		{
			this.machine.Fire(KinectAction.Reconstruct);
			return this.State == KinectState.Reconstructing;
		}
		
		private IEnumerator WaitStandalone(object state)
		{
			var standalone = state as Standalone;
			try
			{
				standalone.Start();
			}
			catch(Exception e)
			{// error => done
				this.machine.Fire(KinectAction.Done);							
				Console.Standalone.Error("Error starting standalone <b>" + Path.GetFileNameWithoutExtension(standalone.FileName).ToUpper() + "</b> Info = \n" + e.ToString());
				yield break;
			}
			Console.Standalone.Start( standalone.FileName);				
			while(!standalone.Done)
			{// wait till done
				this.CurrentProcessProgress = standalone.Progress;
				Console.Standalone.Progress(standalone.Progress);
				yield return new WaitForSeconds(this.config.ProcessWaitSeconds);
			}
			Console.Standalone.Exit(standalone.FileName, standalone.ExitCode,standalone.Elapsed.TotalSeconds);
			this.machine.Fire(KinectAction.Done);
			yield break;
		}
		#endregion
		#region State Machine Config
		private void InitStateMachine ()
		{
			this.State = KinectState.None;
			this.machine = new StateMachine<KinectState,KinectAction>(() => this.State,(state) => this.State = state);
			// No State Config
			this.machine.Configure(KinectState.None)			
				.OnEntry(() => 
				         {
					StartCoroutine("AcceptConnections");
					StopCoroutine("AcceptMessages");
				})
					.Permit(KinectAction.Init,KinectState.Idle);
			// Idle State Config
			this.machine.Configure(KinectState.Idle)
				.OnEntryFrom(KinectAction.Init,() => StartCoroutine("AcceptMessages"))							
					.Permit(KinectAction.Reinit,KinectState.None)
					.Permit(KinectAction.Reconstruct,KinectState.Reconstructing)
					.Permit(KinectAction.Calibrate,KinectState.Calibrating)
					.Permit(KinectAction.Sync,KinectState.Syncing)
					.Permit(KinectAction.Record,KinectState.Recording);		
			// Calibrating State Config
			this.machine.Configure(KinectState.Calibrating)
				.OnEntryFrom(KinectAction.Calibrate,() => 
				             {
					StartCoroutine("WaitCapture",KinectAction.Calibrate);
					this.SendToAll(new Message(MessageType.Start,MessageParameter.Calibrate));
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.OnExit(() => 
					        {
						
					})
					.PermitDynamic(KinectAction.Stop,() => 
					               {
						this.SendToAll(new Message(MessageType.Stop,MessageParameter.Calibrate));
						return KinectState.StopCalibrating;
					})
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Done,() => --this.doneReceivedNeeded == 0 ? KinectState.WaitingCalibratingTransfer : KinectState.Calibrating);
			// Stop Calibrating State Config
			this.machine.Configure(KinectState.StopCalibrating)
				.OnEntryFrom(KinectAction.Stop,() =>
				             {
					this.doneReceivedNeeded = this.config.ClientCount;		
					StopCoroutine("WaitCapture");
					this.CurrentCaptureProgress = 0.0f;
				})
					.PermitDynamic(KinectAction.Done,() => --this.doneReceivedNeeded == 0 ? KinectState.Idle : KinectState.StopCalibrating);
			// Waiting Calibration Transfer State Config
			this.machine.Configure(KinectState.WaitingCalibratingTransfer)					
				.OnEntryFrom(KinectAction.Done,() =>
				             {
					this.SendToAll(new Message(MessageType.Merge,MessageParameter.Calibrate));
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.Permit(KinectAction.Reinit,KinectState.None)
					.Permit(KinectAction.Stop,KinectState.Idle)
					.PermitDynamic(KinectAction.Merge,() => --this.doneReceivedNeeded == 0 ? KinectState.CalibratingTransfer : KinectState.WaitingCalibratingTransfer);
			// Transfering Calibration State Config						
			this.machine.Configure(KinectState.CalibratingTransfer)
				.OnEntryFrom(KinectAction.Merge,() =>
				             {
					if(this.fileReceivers.All((receiver) => receiver.TryNext()))
					{
						StartCoroutine("WaitTransfer",KinectAction.Calibrate);							
					}//TODO:  missing case			
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.OnExit(() =>
					        {
						if(this.fileReceivers.Any((receiver) => receiver.Pending))
						{
							foreach(var receiver in this.fileReceivers.Where((receiver) => receiver.Pending))
							{
								receiver.Abort();
							}
							this.SendToAll(new Message(MessageType.Stop,MessageParameter.Merge));
							this.CurrentTransferProgress = 0.0f;							
						}
					})
					.Permit(KinectAction.Stop,KinectState.Idle)
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Done,() =>--this.doneReceivedNeeded == 0 ? KinectState.ProcessingCalibration : KinectState.CalibratingTransfer);				
			// Processing Calibration State Config						
			this.machine.Configure(KinectState.ProcessingCalibration)														
				.OnEntry(() => 
				         {
					this.calibrationExe = Standalone.Factory.Create(
						StandaloneType.Calibration,this.FindMostRecent(this.config.CalibrationOutputDirectory),true);
					StartCoroutine("WaitStandalone",this.calibrationExe);											
					Console.Important("<b><color=lightblue>Calibration Process Started !</color></b>");
				})
					.OnExit(() =>
					        {
						StopCoroutine("WaitStandalone");
						this.calibrationExe.Stop();
						Console.Important("<b><color=orange>Calibration Process Ended !</color></b>");						
					})
					.Permit(KinectAction.Reinit,KinectState.None)
					.Permit(KinectAction.Done,KinectState.Idle);
			// Syncing State Config			
			this.machine.Configure(KinectState.Syncing)									
				.OnEntryFrom(KinectAction.Sync,() => 
				             {
					StartCoroutine("WaitCapture",KinectAction.Sync);			
					this.SendToAll(new Message(MessageType.Start,MessageParameter.Sync));
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.PermitDynamic(KinectAction.Stop,() => 
					               {
						this.SendToAll(new Message(MessageType.Stop,MessageParameter.Sync));
						return KinectState.StopSyncing;
					})
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Done,() =>--this.doneReceivedNeeded == 0 ? KinectState.WaitingSyncingTransfer : KinectState.Syncing);
			// Stop Syncing State Config
			this.machine.Configure(KinectState.StopSyncing)
				.OnEntryFrom(KinectAction.Stop,() =>
				             {
					this.doneReceivedNeeded = this.config.ClientCount;		
					StopCoroutine("WaitCapture");					
					this.CurrentCaptureProgress = 0.0f;
				})
					.PermitDynamic(KinectAction.Done,() => --this.doneReceivedNeeded == 0 ? KinectState.Idle : KinectState.StopSyncing);
			// Waiting Syncing Transfer State Config
			this.machine.Configure(KinectState.WaitingSyncingTransfer)								
				.OnEntryFrom(KinectAction.Done,() =>
				             {
					this.SyncRecordingConfidence = this.kinects.Aggregate(0.0f,(seed,kinect) => seed += kinect.Elapsed.FPS / AudioNominalFPS) / this.kinects.Count;						
					this.SendToAll(new Message(MessageType.Merge,MessageParameter.Sync));
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Merge,() =>  --this.doneReceivedNeeded == 0 ? KinectState.SyncingTransfer : KinectState.WaitingSyncingTransfer);
			// Transfering Syncing State Config						
			this.machine.Configure(KinectState.SyncingTransfer)		
				.OnEntryFrom(KinectAction.Merge,() =>
				             {
					if(this.fileReceivers.All((receiver) => receiver.TryNext()))
					{
						StartCoroutine("WaitTransfer",KinectAction.Sync);							
					}				
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.OnExit(() =>
					        {
						if(this.fileReceivers.Any((receiver) => receiver.Pending))
						{
							foreach(var receiver in this.fileReceivers.Where((receiver) => receiver.Pending))
							{
								receiver.Abort();
							}
							this.SendToAll(new Message(MessageType.Stop,MessageParameter.Merge));
							this.CurrentTransferProgress = 0.0f;							
						}
					})
					.Permit(KinectAction.Stop,KinectState.Idle)
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Done,() => --this.doneReceivedNeeded == 0 ? KinectState.ProcessingSynchronization : KinectState.SyncingTransfer);
			// Syncing Calibration State Config						
			this.machine.Configure(KinectState.ProcessingSynchronization)								
				.OnEntry(() => 
				         {
					this.synchronizationExe = Standalone.Factory.Create(
						StandaloneType.Synchronization,this.FindMostRecent(this.config.AudioOutputDirectory),true);
					StartCoroutine("WaitStandalone",this.synchronizationExe);										
					Console.Important("<b><color=lightblue>Synchronization Process Started !</color></b>");
				})
					.OnExit(() =>
					        {
						StopCoroutine("WaitStandalone");						
						this.synchronizationExe.Stop();
						Console.Important("<b><color=orange>Synchronization Process Ended !</color></b>");						
					})			
					.Permit(KinectAction.Reinit,KinectState.None)
					.Permit(KinectAction.Done,KinectState.Idle);
			// Recording State Config						
			this.machine.Configure(KinectState.Recording)								
				.OnEntryFrom(KinectAction.Record,() => 
				             {		
					StartCoroutine("WaitCapture",KinectAction.Record);				
					this.SendToAll(new Message(MessageType.Start,MessageParameter.Record));
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.Permit(KinectAction.Reinit,KinectState.None)
					.Permit(KinectAction.Stop,KinectState.StopRecording);
			// Stop Recording Transfer State Config
			this.machine.Configure(KinectState.StopRecording)
				.OnEntryFrom(KinectAction.Stop,() =>
				             {
					this.SendToAll(new Message(MessageType.Stop,MessageParameter.Record));					
					this.doneReceivedNeeded = this.config.ClientCount;	
					StopCoroutine("WaitCapture");					
					this.CurrentCaptureProgress = 0.0f;
				})
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Done,() =>  --this.doneReceivedNeeded == 0 ? KinectState.WaitingRecordingTransfer : KinectState.StopRecording);
			// Waiting Recording Transfer State Config
			this.machine.Configure(KinectState.WaitingRecordingTransfer)										
				.OnEntryFrom(KinectAction.Done,() =>
	             {
					this.RecordingConfidence = this.kinects.Aggregate(0.0f,(seed,kinect) => seed += kinect.Elapsed.FPS / DepthNominalFPS) / this.kinects.Count;						
					this.SendToAll(new Message(MessageType.Merge,MessageParameter.Record));										
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Merge,() => --this.doneReceivedNeeded == 0 ? KinectState.RecordingTransfer : KinectState.WaitingRecordingTransfer);
			// Transfering Recording State Config						
			this.machine.Configure(KinectState.RecordingTransfer)									
				.OnEntryFrom(KinectAction.Merge,() => 
				             {
					if(this.fileReceivers.All((receiver) => receiver.TryNext()))
					{
						StartCoroutine("WaitTransfer",KinectAction.Record);							
					}														
					this.doneReceivedNeeded = this.config.ClientCount;
				})
					.OnExit(() =>
					        {
						if(this.fileReceivers.Any((receiver) => receiver.Pending))
						{
							foreach(var receiver in this.fileReceivers.Where((receiver) => receiver.Pending))
							{
								receiver.Abort();
							}
							this.SendToAll(new Message(MessageType.Stop,MessageParameter.Merge));		
							this.CurrentTransferProgress = 0.0f;							
						}
					})
					.Permit(KinectAction.Stop,KinectState.Idle)
					.Permit(KinectAction.Reinit,KinectState.None)
					.PermitDynamic(KinectAction.Done,() => --this.doneReceivedNeeded == 0 ? KinectState.Idle : KinectState.RecordingTransfer);
			// Recording Calibration State Config						
			this.machine.Configure(KinectState.Reconstructing)														
				.OnEntry(() => 
		         {
					var latestRecordings = this.FindMostRecent(this.config.RecordingOutputDirectory);
					var latestRecordingsTime = Path.GetFileNameWithoutExtension(latestRecordings);
					var meshfolder =Path.Combine(this.config.MeshOutputDirectory,latestRecordingsTime);						
					if(!Directory.Exists(meshfolder))
					{
						Directory.CreateDirectory(meshfolder);
					}
					var meshfile = Path.Combine(meshfolder, "raw.mesh");					
					this.reconstructionExe = Standalone.Factory.Create(
						StandaloneType.Reconstruction,"/recordings " + latestRecordings + " /mesh " + meshfile + " /out " + meshfile + " /calibration "
						+ this.FindMostRecent(this.config.CalibrationOutputDirectory) + " /sync " + this.FindMostRecent(this.config.AudioOutputDirectory),true);
					StartCoroutine("WaitStandalone",this.reconstructionExe);											
					Console.Important("<b><color=lightblue>Recording Process Started !</color></b>");
				})
					.OnExit(() =>
					        {
						StopCoroutine("WaitStandalone");						
						this.reconstructionExe.Stop();
						Console.Important("<b><color=orange>Recording Process Ended !</color></b>");						
					})				
					.Permit(KinectAction.Reinit,KinectState.None)
					.Permit(KinectAction.Done,KinectState.Idle);
			// Unhandled State Config									
			this.machine.OnUnhandledTrigger((state,action) =>
			                                {
				Console.StateMachine.Unhandled(state.ToString(), action.ToString());
			});
			this.machine.OnTransitioned((trans) => 
			                            {
				Console.StateMachine.Transition(!trans.IsReentry,trans.Source.ToString(), trans.Destination.ToString());
			});
		}
		#endregion
		#region Fields
		private StateMachine<KinectState,KinectAction> machine;
		private Standalone calibrationExe,synchronizationExe,reconstructionExe;
		private int doneReceivedNeeded;
		
		private const float DepthNominalFPS = 30.0f;
		private const float AudioNominalFPS = 60.0f;
		#endregion
	}
}
