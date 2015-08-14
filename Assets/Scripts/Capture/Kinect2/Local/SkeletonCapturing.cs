#region Description
/* 		Kinect 2 Skeleton Capturing
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Linq;

using UnityEngine;

using Windows.Kinect;

using FileHelpers;

using Kinect2.IO;
using System.Collections.Generic;

using Floor = Windows.Kinect.Vector4;
#endregion
namespace Kinect2.Local
{
	internal sealed class SkeletonCapturing : Capturer,ISkeletonGenerator<SkeletonFrame>
	{
		#region Properties
		public override Device Type { get { return Device.SINGLE_KINECT_2; } }		
		public bool HasNewFrame { get; private set; }
		public SkeletonFrame CurrentFrame { get { return new SkeletonFrame(this.frameNumber,
				                                                                   this.bodies.Where((body) => body.IsTracked).ToArray(),this.currentFloor,this.currentTime); } }
		public List<SkeletonFrame> RecordedFrames = new List<SkeletonFrame> ();// { get { return RecordedFrames; } set; }// new List<SkeletonFrame>();	
		
		//		public override List<SkeletonFrame> RecordedFrames{ get;}
		#endregion
		#region Unity
		void Awake()
		{
			this.exporter = this.gameObject.AddComponent<SkeletonExporting>();
			RecordedFrames = new List<SkeletonFrame> ();
		}
		
		void Start () 
		{
			var sensor = KinectSensor.GetDefault();
			this.reader = sensor.BodyFrameSource.OpenReader();
			this.count = sensor.BodyFrameSource.BodyCount;
			this.bodies = new Body[this.count];
			this.reader.FrameArrived += (object sender, BodyFrameArrivedEventArgs e) =>
			{
				using(var frame = e.FrameReference.AcquireFrame())
				{
					this.currentTime = frame.RelativeTime;
					this.currentFloor = frame.FloorClipPlane;
					this.currentBodyCount = frame.BodyCount;
					frame.GetAndRefreshBodyData(this.bodies);	
					this.HasNewFrame = true;	
				}
			};
		}
		
		void OnEnable()
		{
			this.frameNumber = 0;
		}
		
		float fixedFrameTime = 1000f / (float)KinectVideoRecorder.fps;
		int lastUpdateTime = 0;
		float elapsedTime = 0f;
		float totalTime=0;
		void Update () 
		{
			/*
			if (IsRecording) {
				
				int currentTimeMilliseconds = Environment.TickCount;
				int elapsedTime = currentTimeMilliseconds - lastUpdateTime;
				//if we have gone over the required elapsed Time
				//print ("deltatime " + deltaTime +" interval "+intervalTime);
				if (elapsedTime >= fixedFrameTime) {
					//if (this.HasNewFrame) {			
					if(this.HasNewFrame){
					RecordedFrames.Add (CurrentFrame);
						this.latestTime = this.currentTime;
						++this.frameNumber;	
						this.HasNewFrame = false;
					}else {
						if (RecordedFrames.Count > 0) {
							print ("Copying a frame here in skeleton capturing");
							this.latestTime = this.currentTime;
							//Debug.Break ();
							RecordedFrames.Add (RecordedFrames.Last ());
							++this.frameNumber;	
						}
					}
					int overflow = (int)(elapsedTime % fixedFrameTime);
					totalTime += fixedFrameTime;
					if (overflow > fixedFrameTime) {
						print ("Skipping a frame here in player");
						Debug.Break ();
					}
					//set the last Update time as the time now minus the overlap of the delta
				}
			}*/
			
			//currentTime = new TimeSpan (DateTime.Now.Ticks);
			var dt = currentTime - latestTime;
			this.HasNewFrame = dt > TimeSpan.Zero;
			if (this.HasNewFrame) {
				latestTime = currentTime;
				++frameNumber;
			}
		}
		
		void OnDestroy()
		{
			this.reader.Dispose();
		}
		#endregion
		#region IRecorder implementation
		public override bool StartRecording ()
		{
			Tick ();
			frameNumber = 0;
			RecordedFrames.Clear ();
			startTime = Time.time;
			return this.exporter.enabled = true;
		}
		
		float startTime = 0f;
		float endTime = 0f;
		
		public override bool CanRecord { get { return this.exporter != null; } }
		
		public override bool IsRecording { get { return this.exporter.enabled; } }
		
		//public float RecordingConfidence { get; private set; }
		#endregion
		#region ICancellation implementation
		public override bool CanStop { get { return this.exporter.enabled; } }
		public override bool Stop ()
		{
			Tock ();
			this.exporter.enabled = false;
			this.RecordingConfidence = this.exporter.Elapsed.FPS * 10000.0f / 33.0f;
			Console.Important("Rec Conf = " + this.RecordingConfidence);
			return !this.exporter.enabled;
		}
		System.Diagnostics.Stopwatch stopwatch;
		void Tick(){
			stopwatch = System.Diagnostics.Stopwatch.StartNew ();
		}
		long stopWatchMilliseconds = 0;
		void Tock(){
			stopWatchMilliseconds = stopwatch.ElapsedMilliseconds;
//			print ("KinectCapturing time: "+stopwatch.Elapsed+" RecordedFrames# "+RecordedFrames.Count);
		}
		#endregion
		#region Fields
		public SkeletonExporting exporter;
		private BodyFrameReader reader;
		private Body[] bodies;
		private BodyFrame current;
		private TimeSpan currentTime,latestTime;
		private Floor currentFloor;
		private int count,frameNumber,currentBodyCount;
		#endregion
	}
}
