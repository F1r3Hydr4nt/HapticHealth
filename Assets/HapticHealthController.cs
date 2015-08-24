using UnityEngine;
using System.Collections;
using Fusion;

public class HapticHealthController : MonoBehaviour {
	public KinectVideoRecorder videoRecorder;
	public KinectVideoPlayer videoPlayer;
	public FusedSkeleton_FromFile fusedSkeletonPlayback;
	public FusedSkeleton_Main fusionSkeleton;
	public WiMuPlotter wiMuPlotter;
	bool recording = false;
	bool playing = false;
	
	public static int recordingFPS = 30;
	public static int playbackFPS = 30;
	public static float fixedFrameTimeRecording = 1000f / (float)recordingFPS;
	public static float fixedFrameTimePlayback = 1000f / (float)playbackFPS;
	// Update is called once per frame
	public enum ControllerState {RECORDING, PLAYBACK, IDLE};
	public static ControllerState state = ControllerState.IDLE;
	bool isLoopingPlayback = true;
	void Update () {
		ProcessKeyboardInput ();
		if (playing) {
			if(wiMuPlotter.isFinishedPlayback&&fusedSkeletonPlayback.isFinishedPlayback&&videoPlayer.isFinishedPlayback &&playing)StopPlayback();
		}

	}

	void RecordUpdate ()
	{
	}

	void PlaybackUpdate ()
	{
	}

	void ProcessKeyboardInput ()
	{
		if (Input.GetKeyDown (KeyCode.A)) {
			if(!recording)
				StartRecording();
			else
				StopRecording();
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			if(!playing)
				StartPlayback();
			else
				StopPlayback();
		}
		if (Input.GetKeyDown (KeyCode.D))
			HalfPlayBackSpeed();
		if (Input.GetKeyDown (KeyCode.F))
			DoublePlayBackSpeed();
	}

	void StartRecording(){
		recording = true;
		fusionSkeleton.StartRecording();
		videoRecorder.StartRecording();
		wiMuPlotter.StartRecording ();
	}

	void StopRecording(){
		recording = false;
		fusionSkeleton.StopRecording();
		videoRecorder.StopRecording();
		wiMuPlotter.StopRecording ();
	}
	
	void StartPlayback(){
		playing = true;
		fusedSkeletonPlayback.StartPlayback ();
		//fusedSkeletonPlayback.StartPlaybackFromMemory (fusionSkeleton.fusionCapturer.exporter.totalFramesWritten);
		videoPlayer.StartPlayback ();
		wiMuPlotter.StartPlayback ();
	}

	void StopPlayback(){
		playing = false;
	}

	void HalfPlayBackSpeed ()
	{
		playbackFPS = (int)(playbackFPS * 0.5f);
		fixedFrameTimePlayback = 1000f / (float)playbackFPS;
		print ("FPS x 2");
	}

	void DoublePlayBackSpeed ()
	{
		playbackFPS = (int)(playbackFPS * 2f);
		fixedFrameTimePlayback = 1000f / (float)playbackFPS;
		print ("FPS x 0.5");
	}
}
