using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System.Threading;

using System;
//This class controls the main scene
public class HapticHealthController : MonoBehaviour {
	public static HapticHealthController Instance;
	public KinectVideoRecorder videoRecorder;
	public KinectVideoPlayer videoPlayer;
	public FusedSkeleton_FromFile fusedSkeletonPlayback;
	public FusedSkeleton_Main fusionSkeleton;
	public WiMuPlotter wiMuPlotter;
	public UIController uiController;
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
	public bool readingKeyboard = true;
	void Update () {
		if(readingKeyboard)ProcessKeyboardInput ();
		if (playing) {
			if(wiMuPlotter.isFinishedPlayback&&fusedSkeletonPlayback.isFinishedPlayback&&videoPlayer.isFinishedPlayback &&playing)StopPlayback();
		}

	}

	void Awake ()
	{
		Instance = this;
		//Test to show parallel threading
		/*VideoExporter exporter = new VideoExporter (FusedSkeleton_FromFile.recordDirectory + "/Videos/", "",new List<byte[]>());
		
		// Create the thread object, passing in the Alpha.Beta method
		// via a ThreadStart delegate. This does not start the thread.
		Thread oThread = new Thread (new ThreadStart (exporter.TestThread));
		
		// Start the thread
		oThread.Start ();*/
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
		if (Input.GetKeyDown (KeyCode.P))
			PlaybackPrerecordedMotion("firstMotion",false);
		
		if (Input.GetKeyDown (KeyCode.R)) {
			if(!recording)
				RecordFirstMotion();
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			if(!recording)
				RecordFirstMotion();
		}
	}

	public void PlaybackPrerecordedMotion (string s, bool l)
	{
		fusedSkeletonPlayback.SetMotionFilename(s);
		wiMuPlotter.SetMotionFilename(s);
		videoPlayer.SetMotionFilename(s);
		videoPlayer.ImportVideoFrames ();
		isLoopingPlayback = l;
		//videoPlayer.ImportVideoFramesAsyncronously ();
		StartCoroutine ("WaitForAsyncronousVideoImport");
	}

	IEnumerator WaitForAsyncronousVideoImport(){

		while (videoPlayer.importer.videoImported == false)
		{
			//print ("Running coroutine");
			yield return null;
		}
		print ("Finished");
		StartPlaybackVideoLoaded ();
	}

	void StartPlaybackVideoLoaded(){
		videoPlayer.PlaybackPrerecordedMotion ();
		fusedSkeletonPlayback.PlaybackPrerecordedMotion ();
		wiMuPlotter.PlaybackPrerecordedMotion ();
		playing = true;
	}

	bool isSavingVideoToFile = false;
	
	
	void RecordFirstMotion ()
	{
		print ("RecordFirstMotion");
		fusionSkeleton.SetMotionFilename("firstMotion");
		wiMuPlotter.SetMotionFilename ("firstMotion");
		videoRecorder.SetMotionFilename ("firstMotion");
		isSavingVideoToFile = true;
		StartRecording ();
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
		if (!isSavingVideoToFile)
						videoRecorder.StopRecording ();
				else
						videoRecorder.StopRecordingAndExportVideo ();
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
		if (isLoopingPlayback)
						StartPlayback ();
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
