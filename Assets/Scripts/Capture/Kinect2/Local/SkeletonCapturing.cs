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
		#endregion
		#region Unity
		void Awake()
		{
			this.exporter = this.gameObject.AddComponent<SkeletonExporting>();
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
				}
			};
		}
		
		void OnEnable()
		{
			this.frameNumber = 0;
		}
		
		void Update () 
		{
			var dt = this.currentTime - this.latestTime;
			this.HasNewFrame = dt  > TimeSpan.Zero;
			if(this.HasNewFrame)
			{			
				this.latestTime = this.currentTime;
				++this.frameNumber;			
			}
			//Console.Log("New Skeleton Frame ? " + this.HasNewFrame + " " + new TimeSpan(dt.Ticks/10000).ToString());
			//Console.ImportantIf(new TimeSpan(dt.Ticks/10000).TotalMilliseconds > 40.0f,"SKIPPED FRAME");
		}
		
		void OnDestroy()
		{
			if(reader != null)
				reader.Dispose();
		}
		#endregion
		#region IRecorder implementation
		public override bool StartRecording ()
		{
			return this.exporter.enabled = true;
		}
		
		public override bool CanRecord { get { return this.exporter != null; } }
		
		public override bool IsRecording { get { return this.exporter.enabled; } }
		
		//public float RecordingConfidence { get; private set; }
		#endregion
		#region ICancellation implementation
		public override bool CanStop { get { return this.exporter.enabled; } }
		
		public override bool Stop ()
		{
			this.exporter.enabled = false;
			//this.RecordingConfidence = this.exporter.Elapsed.FPS * 10000.0f / 33.0f;
			//Console.Important("Rec Conf = " + this.RecordingConfidence);
			return !this.exporter.enabled;
		}
		#endregion
		#region Fields
		private SkeletonExporting exporter;
		private BodyFrameReader reader;
		private Body[] bodies;
		private BodyFrame current;
		private TimeSpan currentTime,latestTime;
		private Floor currentFloor;
		private int count,frameNumber,currentBodyCount;
		#endregion
	}
}
