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
internal sealed class FusionCapturing : Capturer,ISkeletonGenerator<SkeletonFrame>
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
			_fusionJoints[JointType.HandLeft],
			_fusionJoints[JointType.ShoulderRight],
			_fusionJoints[JointType.ElbowRight],
			_fusionJoints[JointType.WristRight],
			_fusionJoints[JointType.HandRight],
			_fusionJoints[JointType.HipLeft],
			_fusionJoints[JointType.KneeLeft],
			_fusionJoints[JointType.AnkleLeft],
			_fusionJoints[JointType.FootLeft],
			_fusionJoints[JointType.HipRight],
			_fusionJoints[JointType.KneeRight],
			_fusionJoints[JointType.AnkleRight],
			_fusionJoints[JointType.FootRight],
			_fusionJoints [JointType.SpineShoulder],
			_fusionJoints[JointType.HandLeft],
			_fusionJoints[JointType.HandLeft],
			_fusionJoints[JointType.HandRight],
			_fusionJoints[JointType.HandRight]
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
	
	void Update () 
	{
		// Here we update the current time in Ticks
		currentTime = new TimeSpan (DateTime.Now.Ticks);
		
		var dt = this.currentTime - this.latestTime;
		this.HasNewFrame = dt  > TimeSpan.Zero;
		if(this.HasNewFrame)
		{			
			this.latestTime = this.currentTime;
			++this.frameNumber;			
		}
		
		//Console.Log(" New Fusion Frame ? " + this.HasNewFrame + " " + new TimeSpan(dt.Ticks/10000).ToString());
		//Console.ImportantIf(new TimeSpan(dt.Ticks/10000).TotalMilliseconds > 40.0f,"SKIPPED FRAME");
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
		currentTime = _currentTime;
		currentFloor = _currentFloor;
		return StartRecording ();
	}
	
	public override bool CanRecord { get { return this.exporter != null; } }	
	public override bool IsRecording { get { return this.exporter.enabled; } }	
	//public float RecordingConfidence { get; private set; }
	#endregion
	
	#region ICancellation implementation
	public void StopRecording()
	{
		this.Stop();
		this.LatestCapture = this.exporter.Filename;
		GlobalVariables.LatestSkeletonCapture = this.exporter.Filename;
	}
	
	public override bool CanStop { get { return this.exporter.enabled; } }	
	public override bool Stop ()
	{
		this.exporter.enabled = false;
		//this.RecordingConfidence = this.exporter.Elapsed.FPS * 10000.0f / 33.0f;
		//Console.Important("Rec Conf = " + this.RecordingConfidence);
		return !this.exporter.enabled;
	}
	#endregion
	
	#region Fields
	private FusionExporting exporter;
	private BodyFrameReader reader;
	private Body[] bodies;
	private BodyFrame current;
	private TimeSpan currentTime,latestTime;
	private Floor currentFloor;
	private int count,frameNumber,currentBodyCount;
	#endregion
}

