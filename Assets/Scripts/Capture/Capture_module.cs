using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Kinect2;
using Kinect2.Local;
using Kinect2.IO;

public class Capture_module : MonoBehaviour
{
	/***
	 * Description:
	 * Capture module
	 * Version: 1.0
	 * Author:
	 * Yvain Tisserand - MIRALab
	 * Nick Zioulis - CERTH (Edit,Additions Jan 2015)
	 *****/
	public bool IsRecording { get { return this.isRecording; } }
	public bool IsCalibrationDone { get { return this.calibrator != null && this.calibrator.CanCalibrate && !this.calibrator.IsCalibrating; } }
	public bool IsSynchronizationDone { get { return this.syncer != null && this.syncer.CanSync && !this.syncer.IsSyncing; } }

    public GameObject _realtime_avatar;

	void Start () 
	{
		recordedMotion = new Animation_data();
	}

	void LateUpdate () 
	{
		if(activeVisualization)
		{
			//Render skeleton
		}
		if(isRecording && skeletonCapturer.HasNewFrame)
		{
			this.recordedMotion.AddFrame(skeletonCapturer.CurrentFrame);
		}
	}
	
	public void Initialise_capture(Device device)
	{
		this.device = device;
		var capturers = this.GetComponents<Capturer>();	
		if(capturers.Any())
		{
			foreach(var capturer in capturers)
			{
				Destroy (capturer);
			}
		}
		switch(this.device)
		{
			case Device.MULTI_KINECT_WIMUS:				
				this.capturer = this.gameObject.AddComponent<Kinect2Managing>();				
				this.calibrator = this.capturer is ICalibrator ? this.capturer as ICalibrator : null;
				this.syncer = this.capturer is ISyncer ? this.capturer as ISyncer : null;
				this.reconstructor = this.capturer is IReconstructor ? this.capturer as IReconstructor : null;	
				this.recorder = CompositeRecorder.FromRecorders(this.capturer);//TODO: fusion capturer to be added here
				this.skeletonCapturer = new NoneSkeletonCapturer();//TODO: add fusion here
				break;
			case Device.SINGLE_KINECT_2:
				var k2controller = this.gameObject.AddComponent<Kinect2Controlling>();
				k2controller.Streams = KinectStreams.Skeleton;
				k2controller.enabled = false;// re-enable		
				k2controller.enabled = true;
				this.capturer = k2controller;
				this.exporter = GetComponent<SkeletonExporting>();
				this.recorder = CompositeRecorder.FromRecorders(this.capturer);
				this.skeletonCapturer = FindObjectsOfType<Capturer>().First((cap) => cap is ISkeletonGenerator<SkeletonFrame>) as ISkeletonGenerator<SkeletonFrame>;
				break;
		}
	}
	
	public void Activate_visualization(bool state)
	{
		this.activeVisualization = state;
        _realtime_avatar.SetActive(state);
	}
	
	public bool Start_capture()
	{
		recordedMotion.Reset();
		this.isRecording = this.recorder.CanRecord && this.recorder.StartRecording() && this.capturer.IsRecording;	
		return this.isRecording;
	}
	public void Prepare_new_capture()
	{//Function call to clean all recorded data and to be ready for a new recording
		this.recordedMotion.Reset();
	}
	
	public Animation_data Stop_capture()
	{
		this.isRecording =! (this.recorder.CanStop && this.recorder.Stop() && !this.recorder.IsRecording);
		this.recordedMotion.Filename = this.exporter != null ? this.exporter.Filename : string.Empty;
		return this.recordedMotion;
	}

	public bool StartCalibration()
	{
		return this.calibrator != null && this.calibrator.CanCalibrate
			&& !this.calibrator.IsCalibrating && this.calibrator.StartCalibration();		
	}

	public bool StartSynchronization()
	{
		return this.syncer != null && this.syncer.CanSync && !this.syncer.IsSyncing
			&& this.syncer.StartSynchronization();		
	}

	public bool StartReconstruction()
	{
		return this.reconstructor != null && this.reconstructor.CanReconstruct
			&& !this.reconstructor.IsReconstructing && this.reconstructor.StartReconstruction();
	}

	private Device device; //Type of device use
	private Animation_data recordedMotion; //Contains recorded motion (overwrite for new motion)
	private bool activeVisualization = false; //True if the obtain skeleton is rendered
	private bool isRecording;

	private CompositeRecorder recorder;
	private Capturer capturer;	
	private ICancellation stopper;
	private ICalibrator calibrator;
	private ISyncer syncer;
	private IReconstructor reconstructor;
	private SkeletonExporting exporter;
	private ISkeletonGenerator<SkeletonFrame> skeletonCapturer;
}
