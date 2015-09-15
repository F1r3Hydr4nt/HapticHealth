using Fusion;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System;
public class WiMuPlotter : MonoBehaviour {
	public void StopComparing ()
	{
		print ("StopComparing");
		comparingSignals = false;
		//		comparator.CheckHandAccelerationValues ();
		comparator.CheckHandAccelerationCombinedValues ();
	}
	
	public FusedSkeleton_Main fusedSkeleton;
	
	string currentFilename = "";
	public void SetMotionFilename (string s)
	{
		currentFilename = s;
	}
	
	public void PlaybackPrerecordedMotion ()
	{
		ReadValuesInFromFile ();
		StartPlayback ();
	}
	bool comparingSignals = false;
	public void StartComparingSignals(){
		comparator = new AccelerationComparator (wiMuValues1, wiMuValues2);
		comparingSignals = true;
		print ("StartComparing");
	}
	
	public AccelerationComparator comparator;
	
	void ReadValuesInFromFile(){
		wiMuValues1 = new List<float> ();
		wiMuValues2 = new List<float> ();
		string text = System.IO.File.ReadAllText(FusedSkeleton_FromFile.recordDirectory+@currentFilename+".txt");
		string [] values = text.Split ('\n');
		foreach (string s in values) {
			if(s!=""){
				//			print (s);
				string []valueSplit = s.Split(' ');
				wiMuValues1.Add (float.Parse(valueSplit[0]));
				wiMuValues2.Add (float.Parse(valueSplit[1]));
			}
		}
	}

	// Use this for initialization
	void Start () {
		//  Create a new graph named "MouseX", with a range of 0 to 2000, colour green at position 100,100
		PlotManager.Instance.PlotCreate("0", -0.5f, 3, Color.cyan, new Vector2(10,	10));
		//  Create a new graph named "MouseX", with a range of 0 to 2000, colour green at position 100,100
		PlotManager.Instance.PlotCreate("1", Color.yellow, "0");
		
		//PlotManager.Instance.PlotCreate("2", Color.green, "0");
		//PlotManager.Instance.PlotCreate("3", Color.red, "0");
		PlotManager.Instance.PlotCreate("4", Color.green, "0");
		PlotManager.Instance.PlotCreate("5", Color.green, "0");
		//print ("---- " + i + " " + WIMUsOrientation[ i ].ToString() );
	}
	
	bool isRecording = false;
	public void StartRecording ()
	{
		Tick ();
		isRecording = true;
		startTime = Environment.TickCount;
		lastUpdateTime = startTime;
		elapsedTime = 0f;
		totalTime = 0f;
		wiMuValues1 = new List<float> ();
		wiMuValues2 = new List<float> ();
		//take first frame
		totalTime += HapticHealthController.fixedFrameTimeRecording;
	}
	float elapsedTime;
	int startTime;
	bool recordingToFile = true;
	public void StopRecording ()
	{
		isRecording = false;
		if (recordingToFile)
			WriteValuesToFile ();
		Tock ();
	}
	
	void WriteValuesToFile(){
		string valueFile = "";
		// WriteAllLines creates a file, writes a collection of strings to the file, 
		// and then closes the file.  You do NOT need to call Flush() or Close().
		for(int i=0;i<wiMuValues1.Count;i++){
			valueFile+=wiMuValues1[i].ToString()+" "+wiMuValues2[i].ToString()+'\n';
		}
		System.IO.File.WriteAllText(FusedSkeleton_FromFile.recordDirectory+@currentFilename+".txt",valueFile);
	}
	
	int currentFrame;
	bool isPlaying = false;
	public bool isFinishedPlayback = false;
	public void StartPlayback ()
	{
		if (comparingSignals)
			comparator.Reset ();
		startTime = Environment.TickCount;
		lastUpdateTime = startTime;
		currentFrame = 0;
		elapsedTime = 0f;
		isPlaying = true;
		totalTime += HapticHealthController.fixedFrameTimePlayback;
		isFinishedPlayback = false;
	}
	
	bool isFinishedPlaying = false;
	public void StopPlayback ()
	{
		isFinishedPlayback = true;
		isPlaying = false;
	}
	
	int lastUpdateTime = 0;
	float totalTime =0f;
	List<float> wiMuValues1, wiMuValues2;
	void Update(){
		//PlotManager.Instance.PlotAdd ("4", 0f);
		//PlotManager.Instance.PlotAdd ("4", 1f);
		//PlotManager.Instance.PlotAdd ("4", 2f);
		//PlotManager.Instance.PlotAdd ("4", 3f);
		PlotManager.Instance.PlotAdd ("4", 1.25f);
		PlotManager.Instance.PlotAdd ("5", 1.55f);
		int currentTimeMilliseconds = Environment.TickCount;
		int timeElapsed = currentTimeMilliseconds - lastUpdateTime;
		//if we have gone over the required elapsed Time
		if(timeElapsed>=HapticHealthController.fixedFrameTimeRecording){
			//				print (timeElapsed+" "+fixedFrameTime);
			totalTime+=HapticHealthController.fixedFrameTimeRecording;
			//Take a frame
			if (fusedSkeleton.WimusOnline) {
				//print ("online");
				float value1 = Mathf.Abs(fusedSkeleton.totalMagnitudes [0]);
				float value2 = Mathf.Abs(fusedSkeleton.totalMagnitudes [1]);
				
				PlotManager.Instance.PlotAdd ("0", (value1+value2)*0.5f);
				//PlotManager.Instance.PlotAdd ("1", value2);
				if(isRecording){
					wiMuValues1.Add (value1);
					wiMuValues2.Add (value2);
				}
				if(comparingSignals){
					//comparator.CompareValues(currentFrame,value1,value2);
					comparator.AddRealtimeValues(currentFrame,value1,value2);
				}
				if(isPlaying){
					//print ("isPlayingBack");
					if(currentFrame<wiMuValues1.Count){
						PlotManager.Instance.PlotAdd ("1", (wiMuValues1[currentFrame]+wiMuValues2[currentFrame])*0.5f);
						//PlotManager.Instance.PlotAdd ("3", wiMuValues2[currentFrame]);
						currentFrame++;
					}else StopPlayback();
				}
			} 
			//Record here
			
			//how far past the required time have we gotten?
			int overflow = (int)(timeElapsed%HapticHealthController.fixedFrameTimeRecording);
			
			if(overflow>HapticHealthController.fixedFrameTimeRecording){
				print ("Skipping a frame here in recorder");
				//Debug.Break ();
			}
			
			int correctedLastUpdateTime = currentTimeMilliseconds - overflow;
			
			//set the last Update time as the time now minus the overlap of the delta
			lastUpdateTime = correctedLastUpdateTime;
			
			//	print ("Delta Recording: "+deltaTime+" currentTime - lastUpdateTime "+(currentTimeMilliseconds-lastUpdateTime)+" should be equal to overflow "+overflow);
		}
		
	}
	System.Diagnostics.Stopwatch stopwatch;
	
	void Tick(){
		stopwatch = System.Diagnostics.Stopwatch.StartNew ();
	}
	long stopWatchMilliseconds = 0;
	void Tock(){
		stopWatchMilliseconds = stopwatch.ElapsedMilliseconds;
		print ("WiMu Plotter time: "+stopwatch.Elapsed+" RecordedFrames# "+wiMuValues1.Count);
	}
}
