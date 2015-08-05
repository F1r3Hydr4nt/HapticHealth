#region Description
/* 		Kinect 2 Local Controller
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using Windows.Kinect;
#endregion
namespace Kinect2.Local
{
	[Flags]
	public enum KinectStreams 
	{
		Color = 0x01,
		Depth = 0x02,
		Skeleton = 0x04,
		Infrared = 0x08,
		LongExposure = 0x10,
		Audio = 0x20,
		ColorAndDepth = Color | Depth,
		ColorAndSkeleton = Color | Skeleton,
		DepthAndSkeleton = Depth | Skeleton		
	}

	internal sealed class Kinect2Controlling : Capturer
	{
		#region Properties
		public KinectStreams Streams;
		internal Configuration Config { get; private set; }
		#endregion
		#region IDevice implementation
		public override Device Type { get { return Device.SINGLE_KINECT_2; } }
		#endregion
		#region Unity
		void Awake()
		{
			this.Config = new Configuration();
			this.sensor = KinectSensor.GetDefault();			
		}

		void Start () 
		{
			if(!this.sensor.IsOpen)
			{
				this.sensor.Open();
			}
			this.enabled = this.sensor.IsOpen;
		}

		void OnEnable()
		{
			this.OpenAndAttachStreams();
		}

		void Update () 
		{
		
		}

		void OnDisable()
		{

		}

		void OnDestroy()
		{
			this.sensor.Close();
		}
		#endregion
		#region Helpers
		private void OpenAndAttachStreams()
		{
			var recorders = new List<IRecorder>();
			if((this.Streams & KinectStreams.Color) == KinectStreams.Color)
			{
				recorders.Add(this.gameObject.AddComponent<ColorCapturing>());
			}
			if((this.Streams & KinectStreams.Depth) == KinectStreams.Depth)
			{
				recorders.Add(this.gameObject.AddComponent<DepthCapturing>());
            }
			if((this.Streams & KinectStreams.Skeleton) == KinectStreams.Skeleton)
			{
				recorders.Add(this.gameObject.AddComponent<SkeletonCapturing>());
            }
			this.recorders = new CompositeRecorder(recorders);
        }
        #endregion
		#region IRecorder implementation
		public override bool StartRecording ()
		{
			return this.recorders.StartRecording();
		}

		public override bool CanRecord { get { return this.recorders.CanRecord; } }

		public override bool IsRecording { get { return this.recorders.IsRecording; } }

		public float RecordingConfidence { get { return this.recorders.RecordingConfidence; } }
		#endregion
		#region ICancellation implementation
		public override bool Stop ()
		{
			return this.recorders.Stop();
		}

		public override bool CanStop { get { return this.recorders.CanStop; } }
		#endregion
		#region Fields
		private KinectSensor sensor;
		private CompositeRecorder recorders;
		#endregion
	}
}
