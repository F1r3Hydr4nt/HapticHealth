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
using Lean = Windows.Kinect.PointF;
using KinectJoint = Windows.Kinect.Joint;
#endregion
namespace Kinect2.IO
{
	public class Skeleton
	{
		public Skeleton(Body body)
		{
			this.ID = body.TrackingId;
			this.Lean = body.Lean;
			this.LeanTrackingState = (TrackingState)body.LeanTrackingState;
			this.State |= body.IsRestricted ? 0x01 : 0x00;
			this.State |= body.HandLeftState == HandState.Closed ? 0x02 : body.HandLeftState == HandState.Open ? 0x04 : body.HandLeftState == HandState.Lasso ? 0x08 : 0x00;
			this.State |= body.HandRightState == HandState.Closed ? 0x20 : body.HandRightState == HandState.Open ? 0x40 : body.HandRightState == HandState.Lasso ? 0x80 : 0x00;
			this.joints = new SkeletonJoint[Body.JointCount];
			for (int i = 0; i < Body.JointCount; ++i)
			{
				joints[i] = new SkeletonJoint(body.Joints[(JointType)i], body.JointOrientations[(JointType)i]);
			}
		}
		
		public Skeleton(SkeletonJoint[] joints, ulong id, float[] lean, int state)
		{
			this.joints = joints;
			this.ID = id;
			var leanp = new PointF();
			leanp.X = lean[1];
			leanp.Y = lean[2];
			this.Lean = leanp;
			this.LeanTrackingState = (TrackingState)(int)lean[0];
			this.State = state;
		}

		public TrackingState LeanTrackingState { get; private set; }
		public PointF Lean { get; private set; }
		public ulong ID { get; private set; }
		public int State { get; private set; }
		
		public float[] this[int index]
		{
			get { return this.joints[index].ToFloats(); }
		}
		
		public CameraSpacePoint this[JointType joint]
		{
			get { return this.joints[(int)joint].ToCamera(); }
		}
		
		private SkeletonJoint[] joints;
	}
}
