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
				StartRecording();
			}
			else{
				StopRecording();
			}
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			PlaybackRecording();
		}
	}

	void StartRecording(){
		recording = true;
		fusionSkeleton.StartRecording();
		videoRecorder.StartRecording();
	}

	void StopRecording(){
		recording = false;
		fusionSkeleton.StopRecording();
		videoRecorder.StopRecording();
	}

	void PlaybackRecording(){
		
		fusedSkeletonPlayback.gameObject.SetActive(true);
		fusedSkeletonPlayback.StartPlayback (fusionSkeleton.fusionCapturer.exporter.totalFramesWritten);
		videoPlayer.StartPlayback ();
	}
}
