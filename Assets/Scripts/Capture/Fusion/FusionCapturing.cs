#region Description
/* 		Kinect 2 Skeleton Capturing
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using FileHelpers;
using Kinect2.IO;
using Floor = Windows.Kinect.Vector4;
using Fusion;
#endregion
public class FusionCapturing : Capturer,ISkeletonGenerator<SkeletonFrame>
{
	#region Properties
	public string LatestCapture { get; private set; }
	public override Device Type { get { return Device.MULTI_KINECT_WIMUS; } }		
	public bool HasNewFrame { get; private set; }
	public int currentIndex = 0;

	// Fusion embedded into SkeletonFrame structure
	private SkeletonFrame currentFrame;
	public void UpdateCurrentFrame( Dictionary< JointType, FusionJoint > _fusionJoints ) 
	{
		const int fusionConfidence = 1;

		// Create Joints
		/*
		    0		SpineBase
			1		SpineMid	
			2		Neck
			3		Head
			4		ShoulderLeft
			5		ElbowLeft
			6		WristLeft
			7		HandLeft
			8		ShoulderRight
			9		ElbowRight
			10		WristRight
			11		HandRight
			12		HipLeft
			13		KneeLeft
			14		AnkleLeft
			15		FootLeft
			16		HipRight
			17		KneeRight
			18		AnkleRight
			19		FootRight
			20      SpineShoulder
			21      HandTipLeft
			22      ThumbLeft
			23      HandTipRight
			24      ThumbRight
        */
		const int numJoints = 25;
		FusionJoint [] fusionJoints = new FusionJoint[numJoints]
		{
			_fusionJoints [JointType.SpineBase],
			_fusionJoints [JointType.SpineMid],
			_fusionJoints [JointType.Neck],
			_fusionJoints [JointType.Head],
			_fusionJoints[JointType.ShoulderLeft],
			_fusionJoints[JointType.ElbowLeft],
			_fusionJoints[JointType.WristLeft],
			_fusionJoints[JointType.WristLeft],
			_fusionJoints[JointType.ShoulderRight],
			_fusionJoints[JointType.ElbowRight],
			_fusionJoints[JointType.WristRight],
			_fusionJoints[JointType.WristRight],
			_fusionJoints[JointType.HipLeft],
			_fusionJoints[JointType.KneeLeft],
			_fusionJoints[JointType.AnkleLeft],
			_fusionJoints[JointType.FootLeft],
			_fusionJoints[JointType.HipRight],
			_fusionJoints[JointType.KneeRight],
			_fusionJoints[JointType.AnkleRight],
			_fusionJoints[JointType.FootRight],
			_fusionJoints [JointType.SpineShoulder],
			_fusionJoints[JointType.WristLeft],
			_fusionJoints[JointType.WristLeft],
			_fusionJoints[JointType.WristRight],
			_fusionJoints[JointType.WristRight]
		};

		// To SkeletonJoints
		SkeletonJoint [] joints = new SkeletonJoint[ numJoints ];
		for (int i = 0; i < numJoints; ++i)
			joints[ i ]  = new SkeletonJoint (fusionJoints [i].jointType, fusionConfidence, fusionJoints [i].ToKinectPosition (), fusionJoints [i].ToKinectOrientation ());

		// To Skeletons
		Skeleton [] skeletons = new Skeleton[] { new Skeleton(joints, 0, new float[numJoints], 0) };

		// Update frame
		currentFrame = new SkeletonFrame (currentIndex++, skeletons, currentFloor.ToFloats(), currentTime.Ticks);
	}

	// Get a new Fusion frame
	public SkeletonFrame CurrentFrame { get { return currentFrame; } }
	#endregion
	#region Unity
	void Awake()
	{
		this.exporter = this.gameObject.AddComponent<FusionExporting>();
	}

	void Start () 
	{
	}

	void OnEnable()
	{
		this.frameNumber = 0;
	}
	float fixedFrameTime = 1000f / (float)KinectVideoRecorder.fps;
	int lastUpdateTime = 0;
	
	float totalTime =0f;

	void Update(){	

			/*if (isRecording) {
				
			int currentTimeMilliseconds = Environment.TickCount;
			int timeElapsed = currentTimeMilliseconds - lastUpdateTime;
			print ("Fusion Capturing, time elapsed "+timeElapsed+" >??? "+fixedFrameTime);
			//if we have gone over the required elapsed Time
			if (timeElapsed >= fixedFrameTime) {
				//				print (timeElapsed+" "+fixedFrameTime);
				//Take a frame
				this.HasNewFrame = true;
				// Update the renderer
				this.latestTime = this.currentTime;
				++this.frameNumber;	
				//this.exporter.writer.Write(currentFrame);
					//print("success"+currentFrame.Index);
				//else print ("LOCKED"+ this.exporter.writer.filename);
				//how far past the required time have we gotten?
				int overflow = (int)(timeElapsed % fixedFrameTime);
			
				if (overflow > fixedFrameTime) {
					print ("Skipping a frame here in fused skeleton");
					//Debug.Break ();
				}
			
				int correctedLastUpdateTime = currentTimeMilliseconds - overflow;
			
				//set the last Update time as the time now minus the overlap of the delta
				lastUpdateTime = correctedLastUpdateTime;
			}
		}*/
		if (isRecording) {
						currentTime = new TimeSpan (DateTime.Now.Ticks);
						var dt = currentTime - latestTime;
						this.HasNewFrame = dt > TimeSpan.Zero;
						if (this.HasNewFrame) {
								latestTime = currentTime;
								++frameNumber;
						}
				}
	}


	void OnDestroy()
	{
		if(this.reader != null)
			this.reader.Dispose();
	}
	#endregion
	#region IRecorder implementation
	public override bool StartRecording ()
	{
		return this.exporter.enabled = true;
	}
	public bool StartRecording( TimeSpan _currentTime, Windows.Kinect.Vector4 _currentFloor )
	{
		//Tick ();
		startTime = Time.time;
		currentTime = _currentTime;
		currentFloor = _currentFloor;
		isRecording = true;
		return StartRecording ();
	}

	public override bool CanRecord { get { return this.exporter != null; } }	
	public override bool IsRecording { get { return this.exporter.enabled; } }	
	//public float RecordingConfidence { get; private set; }
	#endregion
	System.Diagnostics.Stopwatch stopwatch;
	void Tick(){
		stopwatch = System.Diagnostics.Stopwatch.StartNew ();
	}
	long stopWatchMilliseconds = 0;
	void Tock(){
		stopWatchMilliseconds = stopwatch.ElapsedMilliseconds;
		print ("FusionCapturing time: "+stopwatch.Elapsed+" RecordedFrames# "+frameNumber);
	}
	#region ICancellation implementation

	bool isRecording=false;
	public void StopRecording()
	{
		//Tock ();
		isRecording = false;
		this.Stop();
		this.LatestCapture = this.exporter.Filename;
		print ("passing filename " + this.LatestCapture);
		FusedSkeleton_FromFile.recordFile = LatestCapture;
	}
	public KinectVideoPlayer videoPlayer;
	public override bool CanStop { get { return this.exporter.enabled; } }	
	public override bool Stop ()
	{
		this.exporter.enabled = false;
	
		//this.RecordingConfidence = this.exporter.Elapsed.FPS * 10000.0f / 33.0f;
		//Console.Important("Rec Conf = " + this.RecordingConfidence);
		return !this.exporter.enabled;
	}
	#endregion
	
	float startTime = 0f;
	#region Fields
	public FusionExporting exporter;
	private BodyFrameReader reader;
	private Body[] bodies;
	private BodyFrame current;
	private TimeSpan currentTime,latestTime;
	private Floor currentFloor;
	private int count,frameNumber,currentBodyCount;
	#endregion
}

