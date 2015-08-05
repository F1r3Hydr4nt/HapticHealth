using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using KinectOne;

public class KinectOneEmulator : MonoBehaviour, KinectOneInterface {
	
	public string inputFile = "CoachAndTrain/playback0";
	private string inputFileDefault = "CoachAndTrain/playbackDefault";
	private float playbackSpeed = 0.0333f;
	private float timer = 0;
	private bool isDefault = true;
	
	/// <summary>
	/// how high (in meters) off the ground is the sensor
	/// </summary>
	public float sensorHeight;
	/// <summary>
	/// where (relative to the ground directly under the sensor) should the kinect register as 0,0,0
	/// </summary>
	public Vector3 kinectCenter;
	/// <summary>
	/// what point (relative to kinectCenter) should the sensor look at
	/// </summary>
	public Vector4 lookAt;
	
	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	private bool newSkeleton = false;
	private int curFrame = 0;
	private NuiSkeletonFrame[] skeletonFrame;
	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	//private bool updatedColor = false;
	//private bool newColor = false;
	//private Color32[] colorImage;
	/// <summary>
	///variables used for updating and accessing depth data 
	/// </summary>
	//private bool updatedDepth = false;
	//private bool newDepth = false;
	//private short[] depthPlayerData;
	
	
	// Use this for initialization
	void Start () {
		LoadPlaybackFile(inputFile);
	}
	
	void Update () {
		timer += Time.deltaTime;
		if(Input.GetKeyUp(KeyCode.F12)) {
			if(isDefault) {
				isDefault = false;
				LoadPlaybackFile(inputFile);
			}
			else {
				isDefault = true;
				LoadPlaybackFile(inputFile);
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		newSkeleton = false;
	}
	
	void LoadPlaybackFile(string filePath)  {
		Debug.Log("Simulating "+@filePath);
		FileStream input = new FileStream(@filePath, FileMode.Open);
		BinaryFormatter bf = new BinaryFormatter();
		try {
			SerialSkeletonFrame[] serialSkeleton = (SerialSkeletonFrame[])bf.Deserialize(input);
			skeletonFrame = new NuiSkeletonFrame[serialSkeleton.Length];
			for(int ii = 0; ii < serialSkeleton.Length; ii++){
				skeletonFrame[ii] = serialSkeleton[ii].deserialize();
			}
		}
		catch( IOException e ) {
			Console.Log( e.ToString() );
		}
		input.Close();
		timer = 0;
	}
	
	float KinectOneInterface.getSensorHeight() {
		return sensorHeight;
	}
	Vector3 KinectOneInterface.getKinectCenter() {
		return kinectCenter;
	}
	Vector4 KinectOneInterface.getLookAt() {
		return lookAt;
	}
	
	bool KinectOneInterface.pollSkeleton() {
		int frame = Mathf.FloorToInt(Time.realtimeSinceStartup / playbackSpeed);
		if(frame > curFrame){
			curFrame = frame;
			newSkeleton = true;
			return newSkeleton;
		}
		return newSkeleton;
	}
	
	NuiSkeletonFrame KinectOneInterface.getSkeleton() {
		return skeletonFrame[curFrame % skeletonFrame.Length];
	}

	// Access
	NuiSkeletonFrame currentFrame;
	public NuiSkeletonFrame GetSkeleton() {
		int frame = Mathf.FloorToInt(Time.realtimeSinceStartup / playbackSpeed);
		if(frame > curFrame) {
			curFrame = frame;
			currentFrame = skeletonFrame[curFrame % skeletonFrame.Length];
		}
		return currentFrame;
	}
	public Vector4 [] GetJointPosition() {
		NuiSkeletonFrame frame = GetSkeleton ();
		return frame.SkeletonData [0].SkeletonPositions;
	}

	/*
	NuiSkeletonBoneOrientation[] KinectInterface.getBoneOrientations(NuiSkeletonFrame skeleton){
		return null;
	}
	*/
	NuiSkeletonBoneOrientation[] KinectOneInterface.getBoneOrientations(NuiSkeletonData skeletonData){
		NuiSkeletonBoneOrientation[] boneOrientations = new NuiSkeletonBoneOrientation[(int)(NuiSkeletonPositionIndex.Count)];
		NativeMethods.NuiSkeletonCalculateBoneOrientations(ref skeletonData, boneOrientations);
		return boneOrientations;
	}

	// Access
	public NuiSkeletonBoneOrientation[] GetBoneOrientations() {
		NuiSkeletonFrame frame = GetSkeleton ();
		NuiSkeletonBoneOrientation[] boneOrientations = new NuiSkeletonBoneOrientation[(int)(NuiSkeletonPositionIndex.Count)];
		NativeMethods.NuiSkeletonCalculateBoneOrientations(ref (frame.SkeletonData[0]), boneOrientations);
		return boneOrientations;
	}

	bool KinectOneInterface.pollColor() {
		return false;
	}
	
	Color32[] KinectOneInterface.getColor() {
		return null;
	}
	
	bool KinectOneInterface.pollDepth() {
		return false;
	}
	
	short[] KinectOneInterface.getDepth() {
		return null;
	}
}
