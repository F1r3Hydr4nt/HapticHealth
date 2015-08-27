using UnityEngine;
using System.Collections;
using System;
using Kinect = Windows.Kinect;

using Windows.Kinect;

public class PostureEstimator : MonoBehaviour {
	
	// Different poses
	public enum Poses 
	{
		None = 0,
		TPose = 1
	}

	// Estimator sensivity
	public double threshold = 0.85;

	// Current estimated pose
	public Poses pose = Poses.None;

	// Kinect skeleton links
	public BodySourceManager kinectManager;
	private int kinectID = -1;

	// Use this for initialization
	void Start () {

	}
	public bool hasTPosed = false;
	// Update is called once per frame
	void Update () {
	
		// Update skeleton pose
		if (kinectManager != null && kinectManager.GetData() != null) 
		{
			pose = EstimatePose();
		}
	}

	// Get the correct skeleton
	public void setActiveKinect( int id ) {
		kinectID = id;
	}

	// Kinect -> Quaternion
	private Quaternion kinectToQuaternion( JointOrientation j ) {
		return new Quaternion (j.Orientation.X, j.Orientation.Y, j.Orientation.Z, j.Orientation.W);
	}

	// Somewhere else maybe
	private Vector3 toVector( CameraSpacePoint p ) {
		return new Vector3 (p.X, p.Y, p.Z);
	}

	// Estimator
	public Poses EstimatePose() {
		Poses result = Poses.None;
		if (kinectID > 0) {
			Body body = kinectManager.GetData () [kinectID];

			// Check for a T-Pose (new version, simple one)
			Vector3 wr = toVector( body.Joints[ JointType.WristRight    ].Position );
			Vector3 er = toVector( body.Joints[ JointType.ElbowRight    ].Position );
			Vector3 sr = toVector( body.Joints[ JointType.ShoulderRight ].Position );
			Vector3 ar = toVector( body.Joints[ JointType.AnkleRight    ].Position );
			Vector3 kr = toVector( body.Joints[ JointType.KneeRight     ].Position );
			Vector3 hr = toVector( body.Joints[ JointType.HipRight      ].Position );
			Vector3 wl = toVector( body.Joints[ JointType.WristLeft     ].Position );
			Vector3 el = toVector( body.Joints[ JointType.ElbowLeft     ].Position );
			Vector3 sl = toVector( body.Joints[ JointType.ShoulderLeft  ].Position );
			Vector3 al = toVector( body.Joints[ JointType.AnkleLeft     ].Position );
			Vector3 kl = toVector( body.Joints[ JointType.KneeLeft      ].Position );
			Vector3 hl = toVector( body.Joints[ JointType.HipLeft       ].Position );
			Vector3 forearmr = (er - wr).normalized, armr   = (sr - er).normalized;
			Vector3 forelegr = (kr - ar).normalized, thighr = (hr - kr).normalized;
			Vector3 forearml = (wl - el).normalized, arml   = (el - sl).normalized;
			Vector3 forelegl = (kl - al).normalized, thighl = (hl - kl).normalized;
			result = ((Vector3.Dot( forearmr, forearml ) >= threshold) && 
			          (Vector3.Dot( armr    , arml     ) >= threshold) &&
			          (Vector3.Dot( forelegr, forelegl ) >= threshold) && 
			          (Vector3.Dot( thighr  , thighl   ) >= threshold) ) ? Poses.TPose : Poses.None;
		}
		return result;
	}

	// Diatance between 0 and 1
	public double QuatDiff( Quaternion q1, Quaternion q2 ) {
		//return 1.0 - (q1 * Quaternion.Inverse( q2 ) ).w ;
		double temp = (q1.x * q2.x + q1.y * q2.y + q1.z * q2.z + q1.w * q2.w );
		return 1 - temp * temp;
	}
}
