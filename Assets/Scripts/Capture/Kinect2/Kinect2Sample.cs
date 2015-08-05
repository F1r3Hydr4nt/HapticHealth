#define KEYBOARD_INPUT
#define GUI_INPUT

#region Description
/* 		Sample Script to test Kinect 2 Remoting
 * 		Use the above defined to include keyboard and/or gui inputs
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 *  @version: 1.0
 */
#endregion
#region Namespaces
using UnityEngine;

using Kinect2.Remote;
#endregion
namespace Kinect2
{
	[RequireComponent(typeof(Kinect2Managing),typeof(GUILayer))]
	internal class Kinect2Sample : MonoBehaviour 
	{
		void Awake()
		{
			this.manager = GetComponent<Kinect2Managing>();
			this.capturer = this.manager is Capturer ? this.manager as Capturer : null;
			this.calibrator = this.manager is ICalibrator ? this.manager as ICalibrator : null;
			this.syncer = this.manager is ISyncer ? this.manager as ISyncer : null;
			this.reconstructor = this.manager is IReconstructor ? this.manager as IReconstructor : null;		
			this.stopper = this.manager is ICancellation ? this.manager as ICancellation : null;
		}

		void Start () 
		{
			this.enabled = this.capturer.enabled;
		}

		void Update () 
		{
#if KEYBOARD_INPUT
			if(this.calibrator != null)
			{
				if(Input.GetKeyUp(KeyCode.C))
					Console.ImportantIf(this.calibrator.StartCalibration(),"<b><color=aqua>Calib !</color></b>");
			}
			if(this.syncer != null)
			{
				if(Input.GetKeyUp(KeyCode.A))
					Console.ImportantIf(this.syncer.StartSynchronization(),"<b><color=yellow>Sync !</color></b>");
			}
			if(Input.GetKeyUp(KeyCode.R))
				Console.ImportantIf(this.capturer.StartRecording(),"<b><color=red>Record !</color></b>");
			if(Input.GetKeyUp(KeyCode.S))
				Console.ImportantIf(this.stopper.Stop(),"<b><color=magenta>Stop !</color></b>");
			if(Input.GetKeyUp(KeyCode.Backspace))
				Console.ImportantIf(this.stopper.Stop(),"<b><color=magenta>Cancelled !</color></b>");
#endif
		}

		void OnGUI()
		{
#if GUI_INPUT
			GUI.Label(new Rect(0.0f,150.0f,50.0f,50.0f),"Process Conf");			
			GUI.Label(new Rect(150.0f,250.0f,50.0f,50.0f),"Capture");
			GUI.Label(new Rect(250.0f,250.0f,50.0f,50.0f),"Transfer");
			GUI.Label(new Rect(350.0f,250.0f,50.0f,50.0f),"Process");
			GUI.Label(new Rect(0.0f,50.0f,50.0f,50.0f),"Capture Conf");			
			//GUI.enabled = true;			
			GUI.enabled = this.stopper.CanStop;		
			if(GUI.Button(new Rect(400.0f,100.0f,100.0f,50.0f),"Cancel"))
				this.stopper.Stop();
			if(this.manager != null)
			{
				GUI.enabled = true;
				GUI.Label(new Rect(00.0f,250.0f,100.0f,150.0f),this.manager.State.ToString());		
				GUI.Label(new Rect(200.0f,250.0f,50.0f,50.0f),(this.manager.CurrentCaptureProgress * 100.0f).ToString("F0") + " %");			
				GUI.Label(new Rect(300.0f,250.0f,50.0f,50.0f),(this.manager.CurrentTransferProgress * 100.0f).ToString("F0") + " %");
				GUI.Label(new Rect(400.0f,250.0f,50.0f,50.0f),(this.manager.CurrentProcessProgress * 100.0f).ToString("F0") + " %");
				GUI.Label(new Rect(325.0f,50.0f,50.0f,50.0f),(this.capturer.RecordingConfidence * 100.0f).ToString("F0") + " %");		
				GUI.enabled = this.capturer.CanRecord;
				GUI.backgroundColor = this.capturer.IsRecording ? Color.green : Color.gray;			
				if(GUI.Button(new Rect(300.0f,100.0f,100.0f,50.0f),"Rec"))
					this.capturer.StartRecording();
			}
			if(this.calibrator != null)
			{
				GUI.enabled = this.calibrator.CanCalibrate;
				GUI.Label(new Rect(125.0f,150.0f,50.0f,50.0f),(this.calibrator.CalibrationConfidence * 100.0f).ToString("F0") + " %");							
				GUI.backgroundColor = this.calibrator.IsCalibrating ? Color.green : Color.gray;
				if(GUI.Button(new Rect(100.0f,100.0f,100.0f,50.0f),"Calib"))
					this.calibrator.StartCalibration();
			}
			if(this.syncer != null)
			{
				GUI.enabled = this.syncer.CanSync;
				GUI.backgroundColor = this.syncer.IsSyncing ? Color.green : Color.gray;							
				GUI.Label(new Rect(225.0f,50.0f,50.0f,50.0f),(this.syncer.SyncRecordingConfidence * 100.0f).ToString("F0") + " %");	
				GUI.Label(new Rect(225.0f,150.0f,50.0f,50.0f),(this.syncer.SyncConfidence * 100.0f).ToString("F0") + " %");									
				if(GUI.Button(new Rect(200.0f,100.0f,100.0f,50.0f),"Sync"))
					this.syncer.StartSynchronization();
			}
			if(this.reconstructor != null)
			{
				GUI.enabled = this.reconstructor.CanReconstruct;
				GUI.backgroundColor = this.reconstructor.IsReconstructing ? Color.green : Color.gray;							
				if(GUI.Button(new Rect(0.0f,100.0f,100.0f,50.0f),"Reconstruct"))
					this.reconstructor.StartReconstruction();
			}
#endif
		}
		private Kinect2Managing manager;
		private Capturer capturer;
		private ICancellation stopper;
		private ICalibrator calibrator;
		private ISyncer syncer;
		private IReconstructor reconstructor;
	}
}
