using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System;

public class KinectVideoRecorder : MonoBehaviour {
	public GameObject videoView;
	public KinectVideoPlayer videoPlayer;
	List<byte[]> videoFrames;
	public List<Texture2D> videoFrameTextures;
	bool isRecording = false;
	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 60;
		//Invoke ("StartRecording",1f);
	}

	float videoRecordTime = 3f;
	
	public void StartRecording ()
	{
		Tick ();
		videoFrames = new List<byte[]> ();
		videoFrameTextures = new List<Texture2D> ();
		isRecording = true;
		videoPlayer.StopPlayback ();
		print ("Started Recording");
		startTime = Environment.TickCount;
		lastUpdateTime = startTime;
		elapsedTime = 0f;
		totalTime = 0f;
		//take first frame
		
		if(videoView.renderer.material.mainTexture!=null){
			Texture2D textureCopy = Instantiate((Texture2D)videoView.renderer.material.mainTexture)as Texture2D;
			videoFrameTextures.Add(textureCopy);
		}
		totalTime += fixedFrameTime;
	}
	float elapsedTime;
	int startTime;
	
	public void StopRecording ()
	{
		print (TimeSpan.FromMilliseconds(totalTime).ToString() + "s");
		Tock ();
		isRecording = false;
		//videoPlayer.SyncTime((int)(fps*(totalTime/stopWatchMilliseconds)));
		videoPlayer.PassFrames (videoFrameTextures);
		System.GC.Collect ();
	}

	void ExportVideo (string testVideo)
	{
		VideoExporter exporter = new VideoExporter (Application.dataPath + "/Videos/"+testVideo, videoFrames);

						// Create the thread object, passing in the Alpha.Beta method
						// via a ThreadStart delegate. This does not start the thread.
						Thread oThread = new Thread (new ThreadStart (exporter.ExportVideo));
			
						// Start the thread
						oThread.Start ();
	}
	public Texture2D testTexture;

	public static int fps = 20;
	public static float fixedFrameTime = 1000f / (float)fps;
	int lastUpdateTime = 0;
	float downSamplingFactor = 0.001f;
	void AddScaledTexture (Texture2D textureCopy)
	{
		TextureScale.Point (textureCopy,(int)(textureCopy.width*downSamplingFactor),(int)(textureCopy.height*downSamplingFactor));
		videoFrameTextures.Add (textureCopy);
	}
	float totalTime =0f;

	void Update(){
		
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			fps --;
			fixedFrameTime = 1000f / (float)fps;
				}
		
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			fps++;
			fixedFrameTime = 1000f / (float)fps;
				}
		if (isRecording) {
			/*elapsedTime += Time.deltaTime;
			//print (elapsedTime +" "+fixedFrameTime);
			if(elapsedTime>fixedFrameTime){
				totalTime+=fixedFrameTime;
				if(videoView.renderer.material.mainTexture!=null){
					Texture2D textureCopy = Instantiate((Texture2D)videoView.renderer.material.mainTexture)as Texture2D;
					videoFrameTextures.Add(textureCopy);
				}
				//how far past the required time have we gotten?
				float overflow = elapsedTime%fixedFrameTime;
				
				if(overflow>fixedFrameTime){
					print ("Skipping a frame here in recorder");
					Debug.Break ();
				}
				elapsedTime = overflow;
			}*/
			int currentTimeMilliseconds = Environment.TickCount;
			int timeElapsed = currentTimeMilliseconds - lastUpdateTime;
			//if we have gone over the required elapsed Time
			if(timeElapsed>=fixedFrameTime){
//				print (timeElapsed+" "+fixedFrameTime);
				totalTime+=fixedFrameTime;
				//Take a frame
			if(videoView.renderer.material.mainTexture!=null){
				Texture2D textureCopy = Instantiate((Texture2D)videoView.renderer.material.mainTexture)as Texture2D;
				videoFrameTextures.Add(textureCopy);
			}
				//how far past the required time have we gotten?
				int overflow = (int)(timeElapsed%fixedFrameTime);
				
				if(overflow>fixedFrameTime){
					print ("Skipping a frame here in recorder");
					//Debug.Break ();
				}

				int correctedLastUpdateTime = currentTimeMilliseconds - overflow;

				//set the last Update time as the time now minus the overlap of the delta
				lastUpdateTime = correctedLastUpdateTime;
				
			//	print ("Delta Recording: "+deltaTime+" currentTime - lastUpdateTime "+(currentTimeMilliseconds-lastUpdateTime)+" should be equal to overflow "+overflow);
			}
			 
		}
	}
	System.Diagnostics.Stopwatch stopwatch;

	void Tick(){
		stopwatch = System.Diagnostics.Stopwatch.StartNew ();
	}
		long stopWatchMilliseconds = 0;
	void Tock(){
			stopWatchMilliseconds = stopwatch.ElapsedMilliseconds;
		print ("VideoRecorder time: "+stopwatch.Elapsed+" RecordedFrames# "+videoFrameTextures.Count);
	}
}
