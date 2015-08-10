using UnityEngine;
using System.Collections;
using Fusion;

public class HapticHealthController : MonoBehaviour {
	public KinectVideoRecorder videoRecorder;
	public KinectVideoPlayer videoPlayer;
	public FusedSkeleton_FromFile fusedSkeletonPlayback;
	public FusedSkeleton_Main fusionSkeleton;
	bool recording = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			if(!recording){
				recording = true;
				fusionSkeleton.StartRecording();
				videoRecorder.StartRecording();
			}
			else{
				recording = false;
				fusionSkeleton.StopRecording();
				videoRecorder.StopRecording();
			}
		}
	}
}
