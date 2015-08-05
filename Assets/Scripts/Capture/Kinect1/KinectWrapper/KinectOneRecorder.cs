using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Linq;
using KinectOne;

public class KinectOneRecorder : MonoBehaviour {
	
	public DeviceOrEmulator devOrEmu;
	private KinectOneInterface kinect;
	
	public string outputFile = "/playback";
	private bool isRecording = false;
	private ArrayList currentData = new ArrayList();
	
	
	//add by lxjk
	private int fileCount = 0;
	//end lxjk
	
	
	// Use this for initialization
	void Start () {
		kinect = devOrEmu.getKinect();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isRecording){
			if(Input.GetKeyDown(KeyCode.F10) || Input.GetKeyDown(KeyCode.R) ){
				StartRecord();
			}
		} else {
			if(Input.GetKeyDown(KeyCode.F10) || Input.GetKeyDown(KeyCode.S) ){
				StopRecord();
			}

			// DEBUG
			kinect.pollSkeleton();
			currentData.Add(kinect.getSkeleton());
		}
	}
	
	void StartRecord() {
		isRecording = true;
		Debug.Log("Start recording Kinect 1.");
	}
	
	void StopRecord() {
		isRecording = false;
		//edit by lxjk
		string filePath = outputFile+fileCount.ToString()+ "-" + DateTime.UtcNow.ToString("yy-MM-dd-HH-mm-ss");
		Console.Log ("Writing to " + @filePath);
		FileStream output = new FileStream(@filePath,FileMode.Create);
		//end lxjk
		BinaryFormatter bf = new BinaryFormatter();
		
		SerialSkeletonFrame[] data = new SerialSkeletonFrame[currentData.Count];
		for(int ii = 0; ii < currentData.Count; ii++){
			data[ii] = new SerialSkeletonFrame((NuiSkeletonFrame)currentData[ii]);
		}
		bf.Serialize(output, data);
		output.Close();
		fileCount++;

		// Empty the data
		currentData = new ArrayList();

		Debug.Log("Stop recording Kinect 1.");
	}
}
