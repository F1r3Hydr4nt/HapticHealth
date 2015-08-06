using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.IO;
using ShimmerConnect;
using Shimmer;
//using Utilities;
using AHRS;
using Windows.Kinect;
using Kinect2.Local;
using Kinect2.IO;
using KinectOne;

namespace Fusion
{
	// Relative to the Fused skeleton elements
	public class FusionJoint
	{
		public Vector3 position;
		public UnityEngine.Quaternion orientation;
		public JointType jointType;
		public void SetPosition( float x, float y, float z ) {
			position.x = x;
			position.y = y;
			position.z = z;
		}
		public void SetPosition( Vector3 pos ) {
			position = pos;
		}
		public void SetOrientation( float x, float y, float z, float w ) {
			orientation.x = x;
			orientation.y = y;
			orientation.z = z;
			orientation.w = w;
		}
		public void SetOrientation( UnityEngine.Quaternion q ) {
			orientation = q;
		}
		public void SetType(JointType t ) {
			jointType = t;
		}
		public Windows.Kinect.CameraSpacePoint ToKinectPosition()
		{
			CameraSpacePoint result = new CameraSpacePoint ();
			result.X = position.x;
			result.Y = position.y;
			result.Z = position.z;
			return result;
		}
		public Windows.Kinect.Vector4 ToKinectOrientation()
		{
			Windows.Kinect.Vector4 vec = new Windows.Kinect.Vector4 ();
			vec.X = orientation.x;
			vec.Y = orientation.y;
			vec.Z = orientation.z;
			vec.W = orientation.w;
			return vec;
		}
	} 
	
	public class FusedSkeleton_Main : MonoBehaviour {
		
		/*-----------------------------------------------------*/
		// Recorder
		public void Start_recording()
		{
			if (this.fusionCapturer.StartRecording ()) {
				
			}
		}
		
		public void Stop_recording()
		{
			this.fusionCapturer.StopRecording ();
		}
		/*-----------------------------------------------------*/
		
		// Debug
		int viewChoice = 0;
		
		// Kinect related flags
		public bool useKinectCalibration = true;
		public bool useKinectTracking = false;
		public bool fillWithKinect = true;
		public bool calibrateWimuFromKinect = true;

		// Kinect skeleton links
		public GameObject fusedViewPrefab;
		public BodySourceView sourceView;
		private BodyFusedView fusedView;
		public PostureEstimator postureEstimator;
		
		// Kinect 2 linkage
		public BodySourceManager kinectManager;
		
		// Fused skeleton Exporter
		public bool captureKinect = true;
		private SkeletonCapturing kinectCapturer;
		private FusionCapturing fusionCapturer;
		
		// Calibration time to get the Kinect skeleton
		//private float delay = 2.0f;
		private float delayCalibrationOnTheFly = 2.0f;
		private float delayOnTheFly;
		private float temporization = 2.0f;
		private float delayTemporization;
		private bool calibrated = false;
		
		// Motion blending
		public float blendWeight = 1.00f; // [0, 1] 1 == no blending
		
		// WIMUs datasets
		public static int MAX_WIMUS = 10;
		private bool WimusOnline = false;
		public ShimmerReceiving receiver;
		private SensorData[] WIMUsData = new SensorData[ MAX_WIMUS ];
		private UnityEngine.Quaternion[] WIMUsOrientation = new UnityEngine.Quaternion[ MAX_WIMUS ];
		private UnityEngine.Quaternion[] WIMUsRefPosition = new UnityEngine.Quaternion[ MAX_WIMUS ];
		private UnityEngine.Quaternion[] WIMUsRefOrientation = new UnityEngine.Quaternion[ MAX_WIMUS ];
		private UnityEngine.Quaternion[] firstOrientation = new UnityEngine.Quaternion[ MAX_WIMUS ];
		private bool[] isFirstOrientation = new bool[ MAX_WIMUS ];
		
		// Fused skeleton rendering
		private UnityEngine.Quaternion GlobalRotation = UnityEngine.Quaternion.AngleAxis( 0, new Vector3(0, 1, 0) );
		private Vector3 globalOffset = new Vector3( 1.5f, 0.0f, 0.0f );
		
		// Global rotation using 9 DoF Madgwick estimator
		public UnityEngine.Quaternion globalHeading = UnityEngine.Quaternion.identity;
		public bool runHeadingCalibration = false;
		public float headingEstimationDelay = 15;
		private float headingDelay;
		
		// Impact and trajectory
	//	BallTrajectory trajectoryHandler;
		
		// Fused skeleton joints / relative WIMUs
		Dictionary< JointType, FusionJoint > fusionJoints = new Dictionary< JointType, FusionJoint>()
		{
			{ JointType.SpineMid, new FusionJoint() } ,
			{ JointType.ShoulderLeft, new FusionJoint() } ,
			{ JointType.ElbowLeft, new FusionJoint() } ,
			{ JointType.WristLeft, new FusionJoint() } ,
			{ JointType.ShoulderRight, new FusionJoint() } ,
			{ JointType.ElbowRight, new FusionJoint() } ,
			{ JointType.WristRight, new FusionJoint() } ,
			{ JointType.HipLeft, new FusionJoint() } ,
			{ JointType.KneeLeft, new FusionJoint() } ,
			{ JointType.AnkleLeft, new FusionJoint() } ,
			{ JointType.HipRight, new FusionJoint() } ,
			{ JointType.KneeRight, new FusionJoint() } ,
			{ JointType.AnkleRight, new FusionJoint() } ,
			{ JointType.Head, new FusionJoint() } ,
			{ JointType.Neck, new FusionJoint() } ,
			{ JointType.SpineBase, new FusionJoint() } ,
			{ JointType.SpineShoulder, new FusionJoint() } ,
			
			// Additional ones
			{ JointType.FootLeft, new FusionJoint() } ,
			{ JointType.FootRight, new FusionJoint() } ,
			{ JointType.HandLeft, new FusionJoint() } ,
			{ JointType.HandRight, new FusionJoint() } 
		};
		Dictionary< JointType, FusionJoint > fusionReferenceJoints = new Dictionary< JointType, FusionJoint>()
		{
			{ JointType.SpineMid, new FusionJoint() } ,
			{ JointType.ShoulderLeft, new FusionJoint() } ,
			{ JointType.ElbowLeft, new FusionJoint() } ,
			{ JointType.WristLeft, new FusionJoint() } ,
			{ JointType.ShoulderRight, new FusionJoint() } ,
			{ JointType.ElbowRight, new FusionJoint() } ,
			{ JointType.WristRight, new FusionJoint() } ,
			{ JointType.HipLeft, new FusionJoint() } ,
			{ JointType.KneeLeft, new FusionJoint() } ,
			{ JointType.AnkleLeft, new FusionJoint() } ,
			{ JointType.HipRight, new FusionJoint() } ,
			{ JointType.KneeRight, new FusionJoint() } ,
			{ JointType.AnkleRight, new FusionJoint() } ,
			{ JointType.Head, new FusionJoint() } ,
			{ JointType.Neck, new FusionJoint() } ,
			{ JointType.SpineBase, new FusionJoint() } ,
			{ JointType.SpineShoulder, new FusionJoint() } ,
			
			// Additional ones
			{ JointType.FootLeft, new FusionJoint() } ,
			{ JointType.FootRight, new FusionJoint() } ,
			{ JointType.HandLeft, new FusionJoint() } ,
			{ JointType.HandRight, new FusionJoint() } 
		};
		
		// This WIMUs configuration array will change according to <<FusedConfiguration.CurrentFusionType>>
		int wimusJointCount = 21;
		Dictionary< int, JointType > fusionWIMUs = new Dictionary<int, JointType>()
		{
			{ 5, JointType.SpineMid } ,
			{ 6, JointType.ElbowRight } ,
			{ 7, JointType.WristRight } ,
			{ 8, JointType.ElbowLeft } ,
			{ 4, JointType.WristLeft } ,
			
			{ 0, JointType.KneeRight } ,
			{ 1, JointType.AnkleRight } ,
			{ 2, JointType.KneeLeft } ,
			{ 3, JointType.AnkleLeft } ,
			
			// Not useful ones
			{  9, JointType.ShoulderLeft } ,
			{ 10, JointType.ShoulderRight } ,
			{ 11, JointType.HipLeft } ,
			{ 12, JointType.HipRight } ,
			{ 13, JointType.Head } ,
			{ 14, JointType.Neck } ,
			{ 15, JointType.SpineBase } ,
			{ 16, JointType.SpineShoulder } ,
			
			// Additional ones
			{ 17, JointType.FootLeft } ,
			{ 18, JointType.FootRight } ,
			{ 19, JointType.HandLeft } ,
			{ 20, JointType.HandRight } 
		};
		
		Dictionary< int, JointType > fusionPrecedentWIMUs = new Dictionary<int, JointType>()
		{
			{ 5, JointType.SpineMid } ,
			{ 6, JointType.ShoulderRight } ,
			{ 7, JointType.ElbowRight } ,
			{ 8, JointType.ShoulderLeft } ,
			{ 4, JointType.ElbowLeft } ,
			
			{ 0, JointType.HipRight } ,
			{ 1, JointType.KneeRight } ,
			{ 2, JointType.HipLeft } ,
			{ 3, JointType.KneeLeft } ,
			
			// Not useful ones
			{  9, JointType.SpineMid } ,
			{ 10, JointType.SpineMid } ,
			{ 11, JointType.SpineMid } , 
			{ 12, JointType.SpineMid }, 
			{ 13, JointType.SpineMid } ,
			{ 14, JointType.SpineMid } ,
			{ 15, JointType.SpineMid } ,
			{ 16, JointType.SpineMid } ,
			
			// Additional ones
			{ 17, JointType.AnkleLeft } ,
			{ 18, JointType.AnkleRight } ,
			{ 19, JointType.WristLeft } ,
			{ 20, JointType.WristRight }
		};
		
		// Special not useful joints
		bool isUsedJoint( JointType jt ) {
			return (jt != JointType.ShoulderLeft && jt != JointType.ShoulderRight && 
			        jt != JointType.HipLeft && jt != JointType.HipRight && 
			        jt != JointType.Neck && jt != JointType.Head && 
			        jt != JointType.SpineBase && jt != JointType.SpineShoulder );
		}
		
		// First time: init the automatic calibration step
		void Awake() {
			
			// Kinect Red color
			fusedView = ((GameObject)GameObject.Instantiate (fusedViewPrefab)).GetComponent<BodyFusedView>();
			// Color
			fusedView.sourceView = sourceView;
			fusedView.SetOffMaterial ();
			
			delayOnTheFly = delayCalibrationOnTheFly;
			delayTemporization = temporization;
		}
		
		void OnDestroy() {
			
			// Put back the color
			fusedView.SetOffMaterial ();
		}
		
		// Use this for initialization
		void Start () {
			
			// Get the WIMUs manager
			receiver = GameObject.Find("WIMUs Server").GetComponent<ShimmerReceiving>();
			
			// Our ball trajectory handler
			//trajectoryHandler = this.gameObject.AddComponent<BallTrajectory>();
			
			// Get our exporters
			fusionCapturer = this.GetComponent<FusionCapturing>();
			kinectCapturer = this.GetComponent<SkeletonCapturing>();
			
			// Init rotational reference + Reset orientations
			for( int i = 0; i < receiver.SensorsCount; ++i ) {
				isFirstOrientation[ i ] = true;
			}
			
			headingDelay = headingEstimationDelay;
			
			// Standalone code
			fusedView.enableRendering ();
			
			// Here we adapt our WIMUs configuration
			switch( FusedConfiguration.CurrentFusionType ) {
			case FusedConfiguration.FusionType.FUSION_LOWERBODY:
				fusionWIMUs[ 0 ] = JointType.SpineMid;
				fusionWIMUs[ 1 ] = JointType.SpineBase;
				fusionWIMUs[ 2 ] = JointType.KneeRight;
				fusionWIMUs[ 3 ] = JointType.AnkleRight;
				fusionWIMUs[ 4 ] = JointType.KneeLeft;
				fusionWIMUs[ 5 ] = JointType.AnkleLeft;
				fusionWIMUs[ 6 ] = JointType.FootRight;
				fusionWIMUs[ 7 ] = JointType.FootLeft;
				fusionPrecedentWIMUs[ 0 ] = JointType.SpineMid;
				fusionPrecedentWIMUs[ 1 ] = JointType.SpineBase;
				fusionPrecedentWIMUs[ 2 ] = JointType.HipRight;
				fusionPrecedentWIMUs[ 3 ] = JointType.KneeRight;
				fusionPrecedentWIMUs[ 4 ] = JointType.HipLeft;
				fusionPrecedentWIMUs[ 5 ] = JointType.KneeLeft;
				fusionPrecedentWIMUs[ 6 ] = JointType.AnkleRight;
				fusionPrecedentWIMUs[ 7 ] = JointType.AnkleLeft;
				
				// TODO Fill with missing joints
				wimusJointCount = 8;
				
				// Fill the other ones with unused joint
				for(int i = 8; i < wimusJointCount; ++i) {
					fusionWIMUs[ i ] = (JointType) (-1);
				}
				break;
			default: // Keep it the way it was initialized.
				break;
			};
		}
		
		// Get the current 
		private int CurrentKinectId()
		{
			int id = 0;
			if( kinectManager != null && kinectManager.GetData() != null )
			{
				foreach( Body body in kinectManager.GetData() )	{
					if( body != null && body.IsTracked )
						break;
					else
						id++;
				}
				// Not tracked
				if (id >= kinectManager.GetData ().Length)
					id = 0;
			}
			return id;
		}
		
		// Each frame
		int recordedFrameNumber = 0;
		void Update () {
			
			// Debug view choice
			if (Input.GetKeyUp (KeyCode.A)) {
				if( viewChoice == 0 )
					viewChoice = 1;
				else
					viewChoice = 0;
			}
			
			//--------------------------------------------
			// Special for heading estimation (use of magnetometers)
			if (runHeadingCalibration) {
				
				if( !calibrated ) {
					ReinitializeOrientations();
					calibrated = true;
				}
				
				if( UpdateWIMUs() && WIMUsData[ 0 ] != null ) {
					
					// Timer until get our heading
					headingDelay -= Time.deltaTime;
					print ( headingDelay + "--- " + WIMUsOrientation[ 0 ].ToString());
					
					if( headingDelay <= 0 ) {
						headingDelay = headingEstimationDelay;
						
						// Put our value
						globalHeading = WIMUsOrientation[ 0 ];
						print ( "Global heading value: " + WIMUsOrientation[ 0 ].ToString());
					}
				}
				
				return;
			}
			
			//--------------------------------------------
			// Get the Kinect
			if (kinectManager != null && kinectManager.GetData () != null && kinectManager.GetData () [CurrentKinectId ()] != null ) {
				if (WimusOnline == false || receiver.SensorsCount == 0) {
					
					// Copy the Kinect skeleton into our fusion joints
					for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++) {
						if (fusionJoints.ContainsKey (jt)) {
							Windows.Kinect.Joint j = kinectManager.GetData () [CurrentKinectId ()].Joints [jt];
							fusionJoints[jt].SetType(jt);
							fusionJoints[jt].SetPosition (j.Position.X, j.Position.Y, j.Position.Z);
							
							JointOrientation jo = kinectManager.GetData () [CurrentKinectId ()].JointOrientations [jt];
							fusionJoints[ jt ].SetOrientation( jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W );
						}
					}
				}
			}
			
			// Wait "delay" until calibration with Kinect		
			/*
			if(delay > 0) {
				delay -= Time.deltaTime;			
				if(delay <= 0) {
					delay = 0;
					
					if( GetCalibration( useKinectCalibration ) ) {
						WimusOnline = true;
						ReinitializeOrientations();
					}
					else {
						print( "Wrong frame..." );
						delay = 0.01f;
					}
				}
			}
			*/
			
			// Temporization after a calibration
			if( calibrated ) {
				delayTemporization -= Time.deltaTime;
				if( delayTemporization <= 0 ) {
					delayTemporization = temporization;
					calibrated = false;
				}
			}
			else {
				
				// Check for a new calibration (T-Pose)
				postureEstimator.setActiveKinect(CurrentKinectId());
				if( postureEstimator.pose == PostureEstimator.Poses.TPose ) {
					
					fusedView.SetOnMaterial();
					
					// Wait some time
					if( delayOnTheFly > 0 ) {
						delayOnTheFly -= Time.deltaTime;			
						if( delayOnTheFly <= 0 ) {
							delayOnTheFly = delayCalibrationOnTheFly;
							
							// New Calibration
							if( GetCalibration( useKinectCalibration ) ) {
								WimusOnline = true;
								calibrated = true;
								ReinitializeOrientations();
								fusedView.SetDefaultMaterial();	
							}
						}
					}
				}
				else
					fusedView.SetOffMaterial();				
			}
			
			//--------------------------------------------
			// Get the WIMUs datasets and animate
			if( WimusOnline )
			{
				if( UpdateWIMUs() ) {
				}
				
				// Animate the fused skeleton
				AnimateFusedSkeleton();		
			}
			
			//--------------------------------------------
			// Update rotations / position to the viewer
			UpdateJoints();
			
			// Start / Stop recorder
			if( WimusOnline )
			{
				// Update the fused skeleton recorder
				if(fusionCapturer.IsRecording)
				{
					fusionCapturer.UpdateCurrentFrame (fusionJoints);
					recordedFrameNumber++;
					
					// Stop
					if(Input.GetKeyUp(KeyCode.S))
					{
						// Record from Ticks time
						if(fusionCapturer.CanStop)
							fusionCapturer.StopRecording();
						Console.Important("<b><color=red>Fusion recording has stopped.</color></b>");
						
						// Kinect capture
						if(captureKinect && kinectCapturer.CanStop)
						{
							kinectCapturer.Stop();
							Console.Important("<b><color=red>Kinect recording has stopped.</color></b>");
						}
					}
				}
				else
					// Start
					if(Input.GetKeyUp(KeyCode.R))
				{
					recordedFrameNumber = 1;
					
					// Record from Ticks time
					fusionCapturer.StartRecording( kinectManager.CurrentTime, kinectManager.CurrentFloor );
					Console.Important("<b><color=aqua>Fusion recording started.</color></b>");
					
					// Kinect capture
					if(captureKinect && kinectCapturer.CanRecord)
					{
						kinectCapturer.StartRecording();
						Console.Important("<b><color=aqua>Kinect recording started.</color></b>");
					}
					
				}
			}
			
			// Global Torso joint fusion tracking On/Off
			if(Input.GetKeyUp(KeyCode.C))
			{
				useKinectTracking = !useKinectTracking;
			}
		}	
		
		// Get WIMUs calibration from the Kinect skeleton
		private bool GetCalibration( bool kinect ) {
			
			Console.Log( "Calibration..." );
			
			// Calibration of the initial position of each bone of the hierarchy
			// (not working without Kinect yet)		
			if (kinectManager != null && kinectManager.GetData() != null) {
				
				// Copy the Kinect skeleton into our fusion joints
				for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++) {
					if( fusionJoints.ContainsKey(jt )) {
						fusionJoints[ jt ].SetType(jt);
						Windows.Kinect.Joint j = kinectManager.GetData()[CurrentKinectId()].Joints [jt];
						fusionJoints[ jt ].SetPosition( j.Position.X, j.Position.Y, j.Position.Z );
						fusionReferenceJoints[ jt ].SetPosition( j.Position.X, j.Position.Y, j.Position.Z );
						
						// New orientations if needed
						JointOrientation jo = kinectManager.GetData()[CurrentKinectId()].JointOrientations [jt];
						fusionJoints[ jt ].SetOrientation( jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W );
						fusionReferenceJoints[ jt ].SetOrientation( jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W );
						if( isUsedJoint( jt ) ) {
							UnityEngine.Quaternion q = new UnityEngine.Quaternion( jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W );
							UnityEngine.Quaternion dir;
							
							// Special case for the legs
							if( jt == JointType.SpineMid || jt == JointType.SpineBase || 
							   jt == JointType.AnkleRight || 
							   jt == JointType.KneeRight ||
							   jt == JointType.FootRight ) {
								dir = UnityEngine.Quaternion.LookRotation( q * new Vector3( 0, 1, 0 ), Vector3.right );
								//dir *= UnityEngine.Quaternion.AngleAxis( 90, dir * new Vector3( 1, 0, 0 ) );
								/*UnityEngine.Quaternion _dir = dir;
								dir.x = _dir.z;
								dir.y = _dir.y;
								dir.z = _dir.x;*/
								Vector3 X = dir * new Vector3 (1, 0, 0);
								Vector3 Y = dir * new Vector3 (0, 1, 0);
								Vector3 Z = dir * new Vector3 (0, 0, 1);
								UnityEngine.Quaternion _q = UnityEngine.Quaternion.AngleAxis (-90, X);
								dir = _q * dir;	
							}
							else
							if( jt == JointType.AnkleLeft || jt == JointType.KneeLeft || jt == JointType.FootLeft ) {
								dir = UnityEngine.Quaternion.LookRotation( q * new Vector3( 0, 1, 0 ), Vector3.left );
								//dir *= UnityEngine.Quaternion.AngleAxis( 90, dir * new Vector3( 1, 0, 0 ) );
								/*UnityEngine.Quaternion _dir = dir;
								dir.x = -_dir.z;
								dir.y = _dir.y;
								dir.z = _dir.x;*/
								Vector3 X = dir * new Vector3 (1, 0, 0);
								Vector3 Y = dir * new Vector3 (0, 1, 0);
								Vector3 Z = dir * new Vector3 (0, 0, 1);
								UnityEngine.Quaternion _q = UnityEngine.Quaternion.AngleAxis (-90, X);
								dir = _q * dir;	
								
								
							}
							else
								dir = UnityEngine.Quaternion.LookRotation( q * new Vector3( 0, 1, 0 ) );
							
							
							// Apply them
							fusionJoints[ jt ].SetOrientation( dir );
							fusionReferenceJoints[ jt ].SetOrientation( dir );
						}
					}
				}
			}
			
			// Set a new color
			fusedView.SetOnMaterial ();
			Console.Log( "Done." );
			
			return true;
		}
		
		// Get our lastest data
		private bool UpdateWIMUs() {
			
			if( receiver != null ) {
				
				// Get them from the Bluetooth receiver
				WIMUsData = receiver.LatestData;
				
				// Update rotations
				if( WIMUsData != null ) {
					for( int i = 0; i < receiver.SensorsCount; ++i ) {
						if( WIMUsData[ i ] != null ) {
							
							// The reset message take time to be transmitted so we override the First orientation received
							if( !resetSended ) {						
								WIMUsOrientation[ i ].Set( (float)WIMUsData[ i ].GetQuaternion()[ 1 ], (float)WIMUsData[ i ].GetQuaternion()[ 2 ] ,
								                          (float)WIMUsData[ i ].GetQuaternion()[ 3 ], (float)WIMUsData[ i ].GetQuaternion()[ 0 ] );
								//print ("---- " + i + " " + WIMUsOrientation[ i ].ToString() );
								
							}
							else {
								resetSended = false;
								//Thread.Sleep( 1000 );							
							}
							
						}
					}
					return true;
				}
			}		
			return false;
		}
		
		// Nullify the rotation -- recalibration
		private bool resetSended = false;
		private void ReinitializeOrientations() {
			if( WimusOnline && receiver != null ) {
				print ("Reset orientations.");
				
				// Reset the sensor orientations
				for(int i = 0; i < receiver.SensorsCount; ++i)
				{
					// Special: Vertical or not
					if( fusionWIMUs[i] == JointType.SpineMid   || fusionWIMUs[i] == JointType.SpineBase || 
					   fusionWIMUs[i] == JointType.AnkleRight || fusionWIMUs[i] == JointType.AnkleLeft ||
					   fusionWIMUs[i] == JointType.KneeRight  || fusionWIMUs[i] == JointType.KneeLeft  ||
					   fusionWIMUs[i] == JointType.FootRight  || fusionWIMUs[i] == JointType.FootLeft  )
						WIMUsOrientation[ i ].Set( 0.0f, 0.7f, 0.0f, 0.7f );
					else
						WIMUsOrientation[ i ].Set( 0.0f, 0.0f, 0.0f, 1.0f );
					
					firstOrientation  [ i ] = UnityEngine.Quaternion.Inverse( WIMUsOrientation[ i ] );
					isFirstOrientation[ i ] = false;
					
					// Send the message to re-init
					if( fusionWIMUs[i] == JointType.SpineMid   || fusionWIMUs[i] == JointType.SpineBase || 
					   fusionWIMUs[i] == JointType.AnkleRight || fusionWIMUs[i] == JointType.AnkleLeft ||
					   fusionWIMUs[i] == JointType.KneeRight  || fusionWIMUs[i] == JointType.KneeLeft  ||
					   fusionWIMUs[i] == JointType.FootRight  || fusionWIMUs[i] == JointType.FootLeft  )
						receiver.SendToSensor( "RESET-VERTICAL", i);
					else
						receiver.SendToSensor( "RESET", i);
				}		
				
				// Temporization
				//
				// TODO When we have time, check the usefulness of this timer sleep.
				//
				Thread.Sleep( 200 );
				
				resetSended = true;
			}
		}
		
		// Apply the new rotations
		private void AnimateFusedSkeleton() {
			
			if( WIMUsData != null && WIMUsData[ 0 ] != null ) {
				
				// Get the orientation from the WIMUs
				// Substract also the global heading
				for( int i = 0; i < receiver.SensorsCount; ++i ) 
				if( fusionWIMUs[ i ] >= 0 ) {
					fusionJoints[ fusionWIMUs[ i ] ].SetOrientation( globalHeading * fusionReferenceJoints[ fusionWIMUs[ i ] ].orientation * GetCalibratedWIMU( i ) );
				}
				
				// Update the skeleton relative to the WIMUs and hierarchical orientations
				for( int i = 0; i < wimusJointCount; ++i ) 
				if( fusionWIMUs[ i ] >= 0 ) {
					
					// Update orientations of unused joints
					if( !isUsedJoint( fusionWIMUs[ i ] ) ) {
						UnityEngine.Quaternion diff = fusionJoints[ fusionWIMUs[ 0 ] ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ fusionWIMUs[ 0 ] ].orientation );
						fusionJoints[ fusionWIMUs[ i ] ].SetOrientation( diff * fusionReferenceJoints[ fusionWIMUs[ i ] ].orientation );
					}
					
					// Special for hands / feet
					if( fusionWIMUs[ i ] == JointType.HandLeft ) {
						UnityEngine.Quaternion diff = fusionJoints[ JointType.WristLeft ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.WristLeft ].orientation );
						fusionJoints[ fusionWIMUs[ i ] ].SetOrientation( diff * fusionReferenceJoints[ fusionWIMUs[ i ] ].orientation );
					}
					else
					if( fusionWIMUs[ i ] == JointType.HandRight ) {
						UnityEngine.Quaternion diff = fusionJoints[ JointType.WristRight ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.WristRight ].orientation );
						fusionJoints[ fusionWIMUs[ i ] ].SetOrientation( diff * fusionReferenceJoints[ fusionWIMUs[ i ] ].orientation );
					}
					else
					if( fusionWIMUs[ i ] == JointType.FootLeft ) {
						UnityEngine.Quaternion diff = fusionJoints[ JointType.AnkleLeft ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.AnkleLeft ].orientation );
						fusionJoints[ fusionWIMUs[ i ] ].SetOrientation( diff * fusionReferenceJoints[ fusionWIMUs[ i ] ].orientation );
					}
					else
					if( fusionWIMUs[ i ] == JointType.FootRight ) {
						UnityEngine.Quaternion diff = fusionJoints[ JointType.AnkleRight ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.AnkleRight ].orientation );
						fusionJoints[ fusionWIMUs[ i ] ].SetOrientation( diff * fusionReferenceJoints[ fusionWIMUs[ i ] ].orientation );
					}
					
					// Hierarchy
					fusionJoints[ fusionWIMUs[ i ] ].position = fusionReferenceJoints[ fusionWIMUs[ i ] ].position +
						fusionJoints[ fusionPrecedentWIMUs[ i ] ].position - fusionReferenceJoints[ fusionPrecedentWIMUs[ i ] ].position;
					Vector3 pos = fusionJoints[ fusionWIMUs[ i ] ].position - fusionJoints[ fusionPrecedentWIMUs[ i ] ].position;
					
					// Special for unused joints
					if( !isUsedJoint( fusionWIMUs[ i ] ) ) {
						pos = fusionJoints[ JointType.SpineMid ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.SpineMid ].orientation ) * pos;
					}
					else {
						pos = fusionJoints[ fusionWIMUs[ i ] ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ fusionWIMUs[ i ] ].orientation ) * pos;
					}
					// Special for hands / feet
					/*
						if( fusionWIMUs[ i ] == JointType.HandLeft )
						else
							pos = fusionJoints[ JointType.WristLeft  ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.WristLeft  ].orientation ) * pos;
						if( fusionWIMUs[ i ] == JointType.HandRight )
							pos = fusionJoints[ JointType.WristRight ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.WristRight ].orientation ) * pos;
						else
						*/
					if( fusionWIMUs[ i ] == JointType.FootLeft )
						pos = fusionJoints[ JointType.AnkleLeft  ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.AnkleLeft  ].orientation ) * pos;
					else
						if( fusionWIMUs[ i ] == JointType.FootRight )
							pos = fusionJoints[ JointType.AnkleRight ].orientation * UnityEngine.Quaternion.Inverse( fusionReferenceJoints[ JointType.AnkleRight ].orientation ) * pos;
					
					fusionJoints[ fusionWIMUs[ i ] ].position = pos + fusionJoints[ fusionPrecedentWIMUs[ i ] ].position;
				}
				
				// Update the global positioning tracking
				if( useKinectTracking ) {
					if (kinectManager != null && kinectManager.GetData() != null && kinectManager.GetData()[CurrentKinectId()].IsTracked) {
						CameraSpacePoint _kinectPos = kinectManager.GetData()[CurrentKinectId()].Joints [ JointType.SpineMid ].Position;
						
						Vector3 kinectPos = new Vector3( _kinectPos.X, _kinectPos.Y, _kinectPos.Z );
						Vector3 lastFusedPos = fusionJoints[ JointType.SpineMid ].position;
						fusionJoints[ JointType.SpineMid ].position = kinectPos;
						
						// Update the other ones
						for( int i = 1; i < wimusJointCount; ++i ) 
						if( fusionWIMUs[ i ] >= 0 ) {							
							fusionJoints[ fusionWIMUs[ i ] ].position += kinectPos - lastFusedPos;
						}
					}
				}
			}
			
			// Fill the missing joints with Kinect joints
			if (fillWithKinect && receiver.SensorsCount < 9) {
				if (kinectManager != null && kinectManager.GetData () != null && kinectManager.GetData () [CurrentKinectId ()].IsTracked) {
					for (int i = receiver.SensorsCount; i < wimusJointCount/*9*/; ++i) {				
						
						fusionJoints [fusionWIMUs [i]].SetType (fusionWIMUs [i]);
						Windows.Kinect.Joint j = kinectManager.GetData () [CurrentKinectId ()].Joints [fusionWIMUs [i]];
						JointOrientation jo = kinectManager.GetData () [CurrentKinectId ()].JointOrientations [fusionWIMUs [i]];
						
						// Special
						if (receiver.SensorsCount == 0 || fusionPrecedentWIMUs [i] != JointType.SpineMid)
							fusionJoints [fusionWIMUs [i]].SetPosition (j.Position.X, j.Position.Y, j.Position.Z);					
						
						fusionJoints [fusionWIMUs [i]].SetOrientation (jo.Orientation.X, jo.Orientation.Y, jo.Orientation.Z, jo.Orientation.W);
					}
				}
			}
		}
		
		// Calibration Kinect <-> WIMUs
		private UnityEngine.Quaternion GetCalibratedWIMU( int index ) {
			
			// Problem, happened with Bluetooth
			if (Double.IsNaN (WIMUsOrientation [index].x))
				return UnityEngine.Quaternion.identity;
			
			// NOT USEFUL
			//reduce the effect of the rotation using the blend parameter
			//WIMUsOrientation[ index ] = UnityEngine.Quaternion.Lerp(Quaternion.identity, WIMUsOrientation[ index ], blendWeight);
			
			UnityEngine.Quaternion _result = WIMUsOrientation[ index ];
			UnityEngine.Quaternion result = _result;
			UnityEngine.Quaternion _refo = firstOrientation[ index ];
			UnityEngine.Quaternion refo = _refo;
			
			
			// TEST
			//return result * refo;
			
			
			// For each WIMU: same transformation
			refo.x = _refo.y;
			refo.y = _refo.z;
			refo.z = _refo.x;
			
			result.x = _result.y;
			result.y = _result.z;
			result.z = _result.x;
			
			return result * refo;
		}
		
		//------------------------------------
		// Kinect 1
		public DeviceOrEmulator emulator;
		//------------------------------------
		
		// Update the rendering
		Vector3 lastPosition;
		void UpdateJoints() {
			
			if (viewChoice == 1) {
				//--------------------------
				// Kinect 1
				// Get new frame from Kinect 1
				UnityEngine.Vector4 [] k1JointsPos = emulator.getKinect ().GetJointPosition ();
				NuiSkeletonBoneOrientation [] k1JointsOri = emulator.getKinect ().GetBoneOrientations ();
				if (k1JointsPos == null || k1JointsOri == null)
					return;
				
				// Update the skeleton
				fusedView.SetJointPosition (JointType.SpineBase, (Vector3)k1JointsPos [0]);
				fusedView.SetJointPosition (JointType.SpineMid, (Vector3)k1JointsPos [1]);
				fusedView.SetJointPosition (JointType.SpineShoulder, (Vector3)k1JointsPos [2]);
				fusedView.SetJointPosition (JointType.Head, (Vector3)k1JointsPos [3]);
				fusedView.SetJointPosition (JointType.Neck, (Vector3)k1JointsPos [3]);
				fusedView.SetJointPosition (JointType.ShoulderLeft, (Vector3)k1JointsPos [4]);
				fusedView.SetJointPosition (JointType.ElbowLeft, (Vector3)k1JointsPos [5]);
				fusedView.SetJointPosition (JointType.WristLeft, (Vector3)k1JointsPos [6]);
				fusedView.SetJointPosition (JointType.HandLeft, (Vector3)k1JointsPos [7]);
				fusedView.SetJointPosition (JointType.ShoulderRight, (Vector3)k1JointsPos [8]);
				fusedView.SetJointPosition (JointType.ElbowRight, (Vector3)k1JointsPos [9]);
				fusedView.SetJointPosition (JointType.WristRight, (Vector3)k1JointsPos [10]);
				fusedView.SetJointPosition (JointType.HandRight, (Vector3)k1JointsPos [11]);
				fusedView.SetJointPosition (JointType.HipLeft, (Vector3)k1JointsPos [12]);
				fusedView.SetJointPosition (JointType.KneeLeft, (Vector3)k1JointsPos [13]);
				fusedView.SetJointPosition (JointType.AnkleLeft, (Vector3)k1JointsPos [14]);
				fusedView.SetJointPosition (JointType.FootLeft, (Vector3)k1JointsPos [15]);
				fusedView.SetJointPosition (JointType.HipRight, (Vector3)k1JointsPos [16]);
				fusedView.SetJointPosition (JointType.KneeRight, (Vector3)k1JointsPos [17]);
				fusedView.SetJointPosition (JointType.AnkleRight, (Vector3)k1JointsPos [18]);
				fusedView.SetJointPosition (JointType.FootRight, (Vector3)k1JointsPos [19]);		
				
				for (int i = 0; i < 20; ++i) {
					//fusedView.SetJointPosition( KinectJoints[ i ], (Vector3) k1JointsPos[ i ]);
					//fusedView.SetJointOrientation( KinectJoints[ i ], k1JointsOri[i].absoluteRotation.rotationQuaternion.GetQuaternion() );
				}
				return;
				//--------------------------
			}
			
			// Update the skeleton
			if( WimusOnline ) {
				for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++) {
					if (fusionJoints.ContainsKey (jt)) {
						fusedView.SetJointPosition (jt, fusionJoints [jt].position + globalOffset);
						fusedView.SetJointOrientation (jt, GetKinectCalibratedOrientation( fusionJoints [jt] ) );								
						
						// Special *HACK* because of the hands...
						if( jt == JointType.HandLeft )
							fusedView.SetJointOrientation( jt, GetKinectCalibratedOrientation( fusionJoints[ JointType.WristLeft  ] ) );
						else
							if( jt == JointType.HandRight )
								fusedView.SetJointOrientation( jt, GetKinectCalibratedOrientation( fusionJoints[ JointType.WristRight ] ) );
					}
				}
			}
			else {
				for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++) {
					if (fusionJoints.ContainsKey (jt)) {
						fusedView.SetJointPosition (jt, fusionJoints [jt].position + globalOffset);
						fusedView.SetJointOrientation (jt, fusionJoints [jt].orientation );								
					}
				}
			}
			
			// Check for impact
			/*
			ImpactData impact;
			if( (impact = receiver.GetHandler().PollImpact ()) != null )
			{
				Console.Important(">> Get an impact frame " + recordedFrameNumber + " " + impact.ToString() );
				trajectoryHandler.HandleNewImpact( impact, 
				         BodyFusedView.GetVector3FromJoint( fusionJoints [ JointType.WristRight ].position + globalOffset ),
				         GetKinectCalibratedOrientation( fusionJoints[ JointType.WristRight ] ) ,
				         BodyFusedView.GetVector3FromJoint( fusionJoints [ JointType.WristRight ].position + globalOffset ) - lastPosition );
			}
			else
				lastPosition = BodyFusedView.GetVector3FromJoint( fusionJoints [ JointType.WristRight ].position + globalOffset );
				*/
		}
		
		// Get orientations consistent with Kinect 2
		UnityEngine.Quaternion GetKinectCalibratedOrientation( FusionJoint joint ) {
			UnityEngine.Quaternion result = joint.orientation;
			
			// We can desactivate it
			if(!calibrateWimuFromKinect)
				return result;
			
			// Arms 
			if (joint.jointType == JointType.ElbowLeft || joint.jointType == JointType.WristLeft) {
				Vector3 X = joint.orientation * new Vector3 (1, 0, 0);
				Vector3 Y = joint.orientation * new Vector3 (0, 1, 0);
				UnityEngine.Quaternion q = UnityEngine.Quaternion.AngleAxis (90, X) * UnityEngine.Quaternion.AngleAxis (90, Y);
				result = q * result;			
			} 
			else
			if (joint.jointType == JointType.WristRight || joint.jointType == JointType.ElbowRight) {
				Vector3 X = joint.orientation * new Vector3 (1, 0, 0);
				Vector3 Y = joint.orientation * new Vector3 (0, 1, 0);
				UnityEngine.Quaternion q = UnityEngine.Quaternion.AngleAxis (90, X) * UnityEngine.Quaternion.AngleAxis (-90, Y);
				result = q * result;			
			}
			else
				// Legs
				if (joint.jointType == JointType.KneeLeft || joint.jointType == JointType.KneeRight ||
				   joint.jointType == JointType.AnkleLeft || joint.jointType == JointType.AnkleRight) {
				Vector3 Z = joint.orientation * new Vector3( 0, 0, 1 );
				UnityEngine.Quaternion q = UnityEngine.Quaternion.AngleAxis( 180, Z );
				result = q * result;
			}
			return result;
		}
	}
	
}