#region Description
/* 		Kinect 2 IO Structs
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

using UnityEngine;

using Windows.Kinect;
using Floor = Windows.Kinect.Vector4;
using Orientation = Windows.Kinect.Vector4;
#endregion
namespace Kinect2.IO
{
	public struct SkeletonFrame
	{
		public Floor Floor { get { return this.floor; } }
		public TimeSpan RelativeTime { get { return this.relativeTime; } }
		public int UserCount { get { return this.skeletons.Length; } }
		public int Index { get { return this.index; } }
		public bool IsEmpty { get { return this.skeletons == null || this.skeletons.Length == 0; } }
		
		private int index;
		private TimeSpan relativeTime;
		private Floor floor;
		
		private Skeleton[] skeletons;
		public SkeletonFrame(int index, Body[] bodies, Floor floor, TimeSpan timeSpan)
		{
			this.index = index;
			this.floor = floor;
			this.relativeTime = timeSpan;
			this.skeletons = bodies.Select((body) => new Skeleton(body)).ToArray();			
		}
		public SkeletonFrame(int index, Skeleton[] skeletons, float[] floor, long timestamp)
		{
			this.skeletons = skeletons;
			this.index = index;
			this.relativeTime = new TimeSpan(timestamp);
			Floor v = new Floor();
			v.W = floor[3];
			v.Z = floor[2];
			v.Y = floor[1];
			v.X = floor[0];
			this.floor = v;
		}
		
		public IEnumerable<Skeleton> Skeletons
		{
			get
			{
				if (this.skeletons == null) yield break;
				foreach (var skeleton in this.skeletons)
				{
					yield return skeleton;
				}
			}
		}
		public Skeleton this[int index] { get { return index < this.UserCount ? this.skeletons[index] : null; } }
		
	}

	public struct SkeletonJoint
	{
		public SkeletonJoint(JointType type, int confidence, CameraSpacePoint position, Orientation orientation)
		{
			this.Type = type;
			this.Confidence = confidence;
			this.Position = new Vector3(position.X, position.Y, position.Z);
			this.Rotation = new Quaternion(orientation.X, orientation.Y, orientation.Z, orientation.W);
			this.State = 0;
		}
		public SkeletonJoint(Windows.Kinect.Joint joint, JointOrientation orientation)
		{
			this.Type = joint.JointType;
			this.Confidence = (int)joint.TrackingState;
			this.State = 0;
			this.Position = new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
			this.Rotation = new Quaternion(orientation.Orientation.X, orientation.Orientation.Y, orientation.Orientation.Z, orientation.Orientation.W);
		}
		public SkeletonJoint(int type, float[] data)
		{
			this.Type = (JointType)type;
			this.State = 0;
			this.Confidence = (int)data[0];
			this.Position = new Vector3(data[1], data[2], data[3]);
			this.Rotation = new Quaternion(data[4], data[5], data[6], data[7]);
		}
		public readonly JointType Type;
		public readonly int Confidence;
		public readonly int State;
		public readonly Vector3 Position;
		public readonly Quaternion Rotation;
	}
}
