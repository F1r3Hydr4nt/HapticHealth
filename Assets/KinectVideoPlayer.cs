using UnityEngine;
using System.Collections.Generic;
using System;

public class KinectVideoPlayer : MonoBehaviour {
	public List<byte[]> frames = new List<byte[]>();
	public List<Texture2D> frameTextures = new List<Texture2D>();
	bool isPlaying = false;
	// Use this for initialization
	void Awake(){
		frame = new Texture2D(2,2);
		gameObject.renderer.material.mainTexture = frame;
	}
	int startTime;
	public void PassFrames (List<Texture2D> newFrames) {
		frameTextures = newFrames;
	}
	public void StartPlayback () {
		print ("StartPlayback");
//		print ("fixedFrameTime * Frame count should be equal at end: "+fixedFrameTime*frameTextures.Count);
		Tick ();
		startTime = Environment.TickCount;
		lastUpdateTime = startTime;
		currentFrame = 0;
		elapsedTime = 0f;
		//UpdateFrame ();
		isPlaying = true;
		totalTime += KinectVideoRecorder.fixedFrameTime;
	}
	public Texture2D testTexture;
	public void StopPlayback () {
		Tock ();
		foreach(Texture2D t in frameTextures)Destroy (t);
		isPlaying = false;
		currentFrame = 0;
	}
	int currentFrame = 0;
	public void UpdateFrame(){
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
		totalTime += KinectVideoRecorder.fixedFrameTime;
		//print ("UpdateFrame");
		frame = frameTextures[currentFrame];
		gameObject.renderer.material.mainTexture = frame;
		//Old way
		//frame.LoadImage(frames[currentFrame]);
		currentFrame++;
		Invoke ("UpdateFrame", KinectVideoRecorder.fixedFrameTime * 0.001f);
	}
	// Update is called once per frame
	Texture2D frame;
	public bool looping = true;

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
			if(elapsedTime>=KinectVideoRecorder.fixedFrameTime){
				frame = frameTextures[currentFrame];
				gameObject.renderer.material.mainTexture = frame;
				//Old way
				//frame.LoadImage(frames[currentFrame]);
				currentFrame++;
				if(currentFrame>frameTextures.Count-1){
					if(looping){
						print (TimeSpan.FromMilliseconds(totalTime).ToString()+"s");
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
				int overflow = (int)(elapsedTime%KinectVideoRecorder.fixedFrameTime);
				totalTime+=KinectVideoRecorder.fixedFrameTime;
				if(overflow>KinectVideoRecorder.fixedFrameTime){
					print ("Skipping a frame here in player");
					//Debug.Break ();
				}
				//set the last Update time as the time now minus the overlap of the delta
				lastUpdateTime = currentTimeMilliseconds-overflow;
//				print ("Delta Playback: "+deltaTime+" currentTime - lastUpdateTime "+(currentTimeMilliseconds-lastUpdateTime)+" should be equal to overflow "+overflow);
			
			}
		}
		if (Input.GetKeyDown (KeyCode.S))
			StartPlayback ();
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
