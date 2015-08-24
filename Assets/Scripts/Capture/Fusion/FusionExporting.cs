#region Description
/* 		Kinect 2 Skeleton Exporting Behavior
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Kinect2;
using Kinect2.Local;
using Kinect2.IO;
using Fusion;
#endregion
[RequireComponent(typeof(FusionCapturing))]
public sealed class FusionExporting : MonoBehaviour 
{
	#region Properties
	internal FrameSpan Elapsed { get; private set; }
	internal string Filename { get; private set; }
	#endregion
	#region Unity
	void Awake()
	{
		this.tracker = GetComponent<FusionCapturing>();
		this.outputDirectory = Path.Combine(Environment.CurrentDirectory,OutputSubDirectory);								
		this.enabled = this.tracker != null && this.tracker.CanRecord;
	}

	void Start ()
	{
	}

	void OnEnable()
	{
		var path = Path.Combine(this.outputDirectory,
		                                     DateTime.UtcNow.ToString("yy-MM-dd-HH-mm-ss"));
		if(!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		this.Filename = Path.Combine(path,DefaultFileName);
		Console.Important("FUSION EXPORTING @ " + this.Filename);
		this.writer = SklxtWriter.Constructor.Start().New(this.Filename).Construct();
		this.enabled = this.writer.Start();		
		Tick ();
		this.writer.Write(this.tracker.CurrentFrame);
		totalFramesWritten++;
	}

	void LateUpdate () 
	{
		//if(this.tracker.HasNewFrame)
		//{
		//	this.writer.Write(this.tracker.CurrentFrame);
		//}
	}
	System.Diagnostics.Stopwatch stopwatch;
	void Tick(){
		stopwatch = System.Diagnostics.Stopwatch.StartNew ();
	}
	long stopWatchMilliseconds = 0;
	void Tock(){
		stopWatchMilliseconds = stopwatch.ElapsedMilliseconds;
		print ("FusionExporting time: "+stopwatch.Elapsed+" RecordedFrames# "+totalFramesWritten);
	}

	int lastUpdateTime = 0;
	float elapsedTime = 0f;
	float totalTime=0;

	void Update(){
		if (tracker.IsRecording) {
			
			int currentTimeMilliseconds = Environment.TickCount;
			int elapsedTime = currentTimeMilliseconds - lastUpdateTime;
			//if we have gone over the required elapsed Time
			//print ("deltatime " + deltaTime +" interval "+intervalTime);
			if (elapsedTime >= HapticHealthController.fixedFrameTimeRecording) {
				this.writer.Write(this.tracker.CurrentFrame);
				totalFramesWritten++;
				int overflow = (int)(elapsedTime %  HapticHealthController.fixedFrameTimeRecording);
				totalTime +=  HapticHealthController.fixedFrameTimeRecording;
				if (overflow >  HapticHealthController.fixedFrameTimeRecording) {
					print ("Skipping a frame here in player");
					//Debug.Break ();
				}
				int correctedLastUpdateTime = currentTimeMilliseconds - overflow;
				//set the last Update time as the time now minus the overlap of the delta
				lastUpdateTime = correctedLastUpdateTime;
			}
		}

	}
	public int totalFramesWritten = 0;
	void OnDisable()
	{
		if(this.writer != null && this.writer.CanWrite)
		{
			//print ("FusionExporting wrote "+totalFramesWritten);
			//Debug.Break();
			this.Elapsed = this.writer.Stop();
			Tock ();
		}
	}

	void OnDestroy()
	{

	}

	#endregion
	#region Fields
	private FusionCapturing tracker;
	public SklxtWriter writer;
	private string outputDirectory;
	private const string DefaultFileName = "tmp-fusion.sklxt";
	private const string OutputSubDirectory = "CoachAndTrain";
	#endregion
}

