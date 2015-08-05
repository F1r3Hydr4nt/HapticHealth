#region Description
/* 		Kinect 2 IO Extension Methods
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System;

using UnityEngine;

using Windows.Kinect;
using Floor = Windows.Kinect.Vector4;
using KinectJoint = Windows.Kinect.Joint;
#endregion
namespace Kinect2.IO
{
	internal static class IOxtensions
	{
		internal static FrameContainer ToContainer(this SkeletonFrame frame)
		{
			return new FrameContainer()
			{
				FrameString = "Frame",
				Index = frame.Index,
				TimeStamp = frame.RelativeTime.Ticks,
				UserCount = frame.UserCount,
				Floor = new float[] { frame.Floor.X, frame.Floor.Y, frame.Floor.Z, frame.Floor.W }
			};
		}
		
		internal static float[] ToFloats(this SkeletonJoint joint)
		{
			return new float[]
			{
				(float)joint.Confidence,joint.Position.x,joint.Position.y,joint.Position.z,
				joint.Rotation.x,joint.Rotation.y,joint.Rotation.z,joint.Rotation.w
			};
		}
		
		internal static float[] ToFloats(this CameraSpacePoint joint)
		{
			return new float[]
			{
				joint.X,joint.Y,joint.Z                
			};
		}
		
		internal static float[] ToFloats(this KinectJoint joint)
		{
			return new float[]
			{
				joint.Position.X,joint.Position.Y,joint.Position.Z,(int)joint.TrackingState * 0.5f
			};
		}
		
		internal static float[] ToFloats(this Windows.Kinect.Vector4 joint)
		{
			return new float[]
			{
				joint.X,joint.Y,joint.Z,joint.W                
			};
		}
		
		internal static SkeletonContainer ToContainer(this Skeleton skeleton)
		{
			return new SkeletonContainer()
			{
				ID = skeleton.ID,
				State = skeleton.State,
				LeanState = new float[] { (float)skeleton.LeanTrackingState, skeleton.Lean.X, skeleton.Lean.Y },
				SpineBase = skeleton[0], // SpineBase
				SpineMid = skeleton[1], // SpineMid
				Neck = skeleton[2], // Neck
				Head = skeleton[3], // Head
				ShoulderLeft = skeleton[4], // ShoulderLeft
				ElbowLeft = skeleton[5], // ElbowLeft
				WristLeft = skeleton[6], // WristLeft
				HandLeft = skeleton[7], // HandLeft
				ShoulderRight = skeleton[8], // ShoulderRight
				ElbowRight = skeleton[9], // ElbowRight
				WristRight = skeleton[10], // WristRight
				HandRight = skeleton[11], // HandRight
				HipLeft = skeleton[12], // HipLeft
				KneeLeft = skeleton[13], // KneeLeft
				AnkleLeft = skeleton[14], // AnkleLeft
				FootLeft = skeleton[15], // FootLeft
				HipRight = skeleton[16], // HipRight
				KneeRight = skeleton[17], // KneeRight
				AnkleRight = skeleton[18], // AnkleRight
				FootRight = skeleton[19], // FootRight
				SpineShoulder = skeleton[20], // SpineShoulder
				HandTipLeft = skeleton[21], // HandTipLeft
				ThumbLeft = skeleton[22], // ThumbLeft
				HandTipRight = skeleton[23], // HandTipRight
				ThumbRight = skeleton[24] // ThumbRight
			};
		}

		public static CameraSpacePoint ToCamera(this SkeletonJoint joint)
		{
			return new CameraSpacePoint()
			{
				X = joint.Position.x,
				Y = joint.Position.y,
				Z = joint.Position.z
			};
		}
		
		public static Vector2 ToPoint(this DepthSpacePoint point)
		{
			return new Vector2(point.X, point.Y);
		}
	}
}
