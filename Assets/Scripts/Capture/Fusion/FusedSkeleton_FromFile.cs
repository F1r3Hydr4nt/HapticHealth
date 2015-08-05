using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
//using Utilities;
using Windows.Kinect;
using Kinect2.Local;
using Kinect2.IO;
using Fusion;
using KinectOne;

public class FusedSkeleton_FromFile : MonoBehaviour {

	// Reading and Rendering
	public static string recordDirectory = @"CoachAndTrain\";
	public static string recordFile = recordDirectory + @"tmp.sklxt";//@"\tmp-fusion.sklxt";
	private FusionReader reader;// = new FusionReader( recordFile );
	public GameObject fusedViewPrefab;
	public BodySourceView sourceView;
	private BodyFusedView fusedView;
	private SkeletonFrame currentFrame;

	void OnDestroy() {
		
		// Put back the color
		fusedView.SetOffMaterial ();
	}
	
	// Use this for initialization
	void Start () {
	}

	void Awake() {
		Console.Important ("Start reading '" + recordFile + "'.");
		fusedView = ((GameObject)GameObject.Instantiate (fusedViewPrefab)).GetComponent<BodyFusedView>();
		// Color
		fusedView.sourceView = sourceView;
		fusedView.SetOffMaterial ();
		//fusedView.Awake ();
		fusedView.enableRendering ();
		reader = new FusionReader (recordFile);
		reader.Start_Reading ();
	}
	float fixedFrameTime = 1000f / (float)KinectVideoRecorder.fps;
	int lastUpdateTime = 0;
	
	float totalTime =0f;
	public void StartPlayback () {
			lastUpdateTime = Environment.TickCount;
	}
	void Update(){	
				int currentTimeMilliseconds = Environment.TickCount;
				int timeElapsed = currentTimeMilliseconds - lastUpdateTime;
				//if we have gone over the required elapsed Time
				if (timeElapsed >= fixedFrameTime) {
					//				print (timeElapsed+" "+fixedFrameTime);
					//Take a frame
					
					// Update the renderer
					UpdateJoints ();
		
					//how far past the required time have we gotten?
					int overflow = (int)(timeElapsed % fixedFrameTime);
					
					if (overflow > fixedFrameTime) {
						print ("Skipping a frame here in fused skeleton");
						Debug.Break ();
					}
					
					int correctedLastUpdateTime = currentTimeMilliseconds - overflow;
					
					//set the last Update time as the time now minus the overlap of the delta
					lastUpdateTime = correctedLastUpdateTime;
				}
	}

		
	// Kinect skeleton JointType / relative index
	Dictionary< int, JointType > KinectJoints = new Dictionary< int, JointType> ()	{
		{ 0, JointType.SpineBase } ,
		{ 1, JointType.SpineMid } ,
		{ 2, JointType.Neck } ,
		{ 3, JointType.Head } ,
		{ 4, JointType.ShoulderLeft } ,
		{ 5, JointType.ElbowLeft } ,
		{ 6, JointType.WristLeft } ,
		{ 7, JointType.HandLeft } ,
		{ 8, JointType.ShoulderRight } ,
		{ 9, JointType.ElbowRight } ,
		{ 10, JointType.WristRight } ,
		{ 11, JointType.HandRight } ,
		{ 12, JointType.HipLeft } ,
		{ 13, JointType.KneeLeft } ,
		{ 14, JointType.AnkleLeft } ,
		{ 15, JointType.FootLeft } ,
		{ 16, JointType.HipRight } ,
		{ 17, JointType.KneeRight } ,
		{ 18, JointType.AnkleRight } ,
		{ 19, JointType.FootRight } ,
		{ 20, JointType.SpineShoulder } ,
		{ 21, JointType.HandTipLeft } ,
		{ 22, JointType.ThumbLeft } ,
		{ 23, JointType.HandTipRight } ,
		{ 24, JointType.ThumbRight } 
	};

	// Kinect 1
	//public DeviceOrEmulator emulator;
	//NuiSkeletonFrame frame;
	//------------------------------------

	// Update the rendering with the latest frame read
	int skipFrame = 0;
	const int skipNum = 0;
	long currentTime = 0, lastTime = 0, elapsed = 0;
	private void UpdateJoints() {

		// Slow motion
		if (skipFrame < skipNum)
		{
			skipFrame++;
			return;
		}
		else
			skipFrame = 0;

		//--------------------------
		// Kinect 1
		// Get new frame from Kinect 1
		/*
		if(emulator != null)
			frame = emulator.getKinect().GetSkeleton();
		else 
			return;

		UnityEngine.Vector4 [] k1JointsPos = emulator.emulator.GetJointPosition ();
		NuiSkeletonBoneOrientation [] k1JointsOri = emulator.emulator.GetBoneOrientations ();

		// Update the skeleton
		fusedView.SetJointPosition( JointType.SpineBase, (Vector3) k1JointsPos[ 0 ]);
		fusedView.SetJointPosition( JointType.SpineMid, (Vector3) k1JointsPos[ 1 ]);
		fusedView.SetJointPosition( JointType.SpineShoulder, (Vector3) k1JointsPos[ 2 ]);
		fusedView.SetJointPosition( JointType.Head, (Vector3) k1JointsPos[ 3 ]);
		fusedView.SetJointPosition( JointType.Neck, (Vector3) k1JointsPos[ 3 ]);
		fusedView.SetJointPosition( JointType.ShoulderLeft, (Vector3) k1JointsPos[ 4 ]);
		fusedView.SetJointPosition( JointType.ElbowLeft, (Vector3) k1JointsPos[ 5 ]);
		fusedView.SetJointPosition( JointType.WristLeft, (Vector3) k1JointsPos[ 6 ]);
		fusedView.SetJointPosition( JointType.HandLeft, (Vector3) k1JointsPos[ 7 ]);
		fusedView.SetJointPosition( JointType.ShoulderRight, (Vector3) k1JointsPos[ 8]);
		fusedView.SetJointPosition( JointType.ElbowRight, (Vector3) k1JointsPos[ 9 ]);
		fusedView.SetJointPosition( JointType.WristRight, (Vector3) k1JointsPos[ 10 ]);
		fusedView.SetJointPosition( JointType.HandRight, (Vector3) k1JointsPos[ 11 ]);
		fusedView.SetJointPosition( JointType.HipLeft, (Vector3) k1JointsPos[ 12 ]);
		fusedView.SetJointPosition( JointType.KneeLeft, (Vector3) k1JointsPos[ 13 ]);
		fusedView.SetJointPosition( JointType.AnkleLeft, (Vector3) k1JointsPos[ 14 ]);
		fusedView.SetJointPosition( JointType.FootLeft, (Vector3) k1JointsPos[ 15 ]);
		fusedView.SetJointPosition( JointType.HipRight, (Vector3) k1JointsPos[ 16 ]);
		fusedView.SetJointPosition( JointType.KneeRight, (Vector3) k1JointsPos[ 17 ]);
		fusedView.SetJointPosition( JointType.AnkleRight, (Vector3) k1JointsPos[ 18 ]);
		fusedView.SetJointPosition( JointType.FootRight, (Vector3) k1JointsPos[ 19 ]);


		for (int i = 0; i < 20; ++i ) {
		//	fusedView.SetJointPosition( KinectJoints[ i ], (Vector3) k1JointsPos[ i ]);
			fusedView.SetJointOrientation( KinectJoints[ i ], k1JointsOri[i].absoluteRotation.rotationQuaternion.GetQuaternion() );
		}
		return;
		*/
		//--------------------------
		// Timestamp based timing		
		double time = (reader.currentTimeStamp - reader.lastTimeStamp);		
		lastTime = currentTime;		
		currentTime = DateTime.Now.Ticks;		
		elapsed += currentTime - lastTime; //Time.deltaTime * 1000;		
		if (reader.lastTimeStamp > 0 && elapsed < time) 		
			return;		
		elapsed = 0;

		// Get the next frame
		reader.UpdateNextFrame ();

		// Update the skeleton
		for (int i = 0; i < 25; ++i ) {
			fusedView.SetJointPosition( KinectJoints[ i ], reader.jointPositions[ i ] );
			fusedView.SetJointOrientation( KinectJoints[ i ], reader.jointOrientations[ i ] );
		}
	}
}