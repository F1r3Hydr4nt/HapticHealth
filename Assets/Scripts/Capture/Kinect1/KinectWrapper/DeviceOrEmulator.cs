using UnityEngine;
using System.Collections;
using KinectOne;

public class DeviceOrEmulator : MonoBehaviour {
	
	public KinectOneSensor device;
	public KinectOneEmulator emulator;
	
	public bool useEmulator = false;
	
	// Use this for initialization
	void Start () {
		if(useEmulator){
			emulator.enabled = true;
		}
		else {
			device.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	public KinectOne.KinectOneInterface getKinect() {
		if(useEmulator){
			return emulator;
		}
		return device;
	}
}
