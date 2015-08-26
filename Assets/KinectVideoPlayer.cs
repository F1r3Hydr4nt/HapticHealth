using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Threading;
public class KinectVideoPlayer : MonoBehaviour {
	public void PlaybackPrerecordedMotion ()
	{
		if (!hasLoadedFrames) {
			frameTextures=importer.ConvertFramesToTextures();
			hasLoadedFrames = true;
		}
		StartPlayback ();
	}

	public List<byte[]> frames = new List<byte[]>();
	public List<Texture2D> frameTextures = new List<Texture2D>();
	bool isPlaying = false;
	string currentFilename = "";
	public void SetMotionFilename (string s)
	{
		currentFilename = s;
	}

	// Use this for initialization
	void Awake(){
		frame = new Texture2D(2,2);
		gameObject.renderer.material.mainTexture = frame;
	}
	int startTime;
	public void PassFrames (List<Texture2D> newFrames) {
		frameTextures = newFrames;
	}

	bool hasLoadedFrames = false;
	public VideoImporter importer;

	public void ImportVideoFrames(){
		if (!hasLoadedFrames) {
			print ("Importing video via thread");
						importer = new VideoImporter (FusedSkeleton_FromFile.recordDirectory + "/Videos/", currentFilename);
			
						// Create the thread object, passing in the Alpha.Beta method
						// via a ThreadStart delegate. This does not start the thread.
						Thread oThread = new Thread (new ThreadStart (importer.ImportVideoData));
			
						// Start the thread
						oThread.Start ();
						print ("Started thread");
				} else
						hasLoadedFrames = true;
	}

	public void StartPlayback () {
		print ("StartPlayback");
//		print ("fixedFrameTime * Frame count should be equal at end: "+fixedFrameTime*frameTextures.Count);
		Tick ();
		startTime = Environment.TickCount;
		lastUpdateTime = startTime;
		currentFrame = 0;
		elapsedTime = 0f;
		isPlaying = true;
		totalTime += HapticHealthController.fixedFrameTimePlayback;
		isFinishedPlayback = false;
	}
	public Texture2D testTexture;
	public void StopPlayback () {
		Tock ();
		//foreach(Texture2D t in frameTextures)Destroy (t);
		isPlaying = false;
		isFinishedPlayback = true;
		currentFrame = 0;
	}
	int currentFrame = 0;
	
	public bool isFinishedPlayback = false;
	// Update is called once per frame
	Texture2D frame;
	public bool looping = false;

	int lastUpdateTime = 0;
	float elapsedTime = 0f;
	float totalTime=0;
	void Update(){
		if (isPlaying) {
			/*elapsedTime += Time.deltaTime;
			if(elapsedTime>fixedFrameTime){

				frame = frameTextures[currentFrame];
				gameObject.renderer.material.mainTexture = frame;
				//Old way
				//frame.LoadImage(frames[currentFrame]);
				currentFrame++;
				if(currentFrame>frameTextures.Count-1){
					if(looping){
						print (totalTime+"s");
						totalTime = 0f;
						Tock ();
						Tick ();
						currentFrame = 0;
					}
					else{
						StopPlayback();
						frame = new Texture2D(2,2);
						gameObject.renderer.material.mainTexture = frame;
					}
				}
				//how far past the required time have we gotten?
				float overflow = elapsedTime%fixedFrameTime;
				totalTime+=fixedFrameTime;
				if(overflow>fixedFrameTime){
					print ("Skipping a frame here in recorder");
					Debug.Break ();
				}
				elapsedTime = overflow;
			}*/
				
			int currentTimeMilliseconds = Environment.TickCount;
			int elapsedTime = currentTimeMilliseconds - lastUpdateTime;
			//if we have gone over the required elapsed Time
			//print ("deltatime " + deltaTime +" interval "+intervalTime);
			if(elapsedTime>=HapticHealthController.fixedFrameTimePlayback){
				frame = frameTextures[currentFrame];
				gameObject.renderer.material.mainTexture = frame;
				//Old way
				//frame.LoadImage(frames[currentFrame]);
				currentFrame++;
				if(currentFrame>frameTextures.Count-1){
					/*if(looping){
						print (TimeSpan.FromMilliseconds(totalTime).ToString()+"s");
						totalTime = 0f;
						Tock ();
						Tick ();
						currentFrame = 0;
					}
					else{*/
						StopPlayback();
						//frame = new Texture2D(2,2);
						//gameObject.renderer.material.mainTexture = frame;
					//}
				}
				int overflow = (int)(elapsedTime%HapticHealthController.fixedFrameTimePlayback);
				totalTime+=HapticHealthController.fixedFrameTimePlayback;
				if(overflow>HapticHealthController.fixedFrameTimePlayback){
					print ("Skipping a frame here in player");
					//Debug.Break ();
				}
				//set the last Update time as the time now minus the overlap of the delta
				lastUpdateTime = currentTimeMilliseconds-overflow;
//				print ("Delta Playback: "+deltaTime+" currentTime - lastUpdateTime "+(currentTimeMilliseconds-lastUpdateTime)+" should be equal to overflow "+overflow);
			
			}
		}
	}

	
	System.Diagnostics.Stopwatch stopwatch;
	
	void Tick(){
		stopwatch = System.Diagnostics.Stopwatch.StartNew ();
	}
	
	void Tock(){
		if(stopwatch!=null)
			print ("VideoPlayback time: "+stopwatch.Elapsed+" RecordedFrames# "+frameTextures.Count);
	}
}
