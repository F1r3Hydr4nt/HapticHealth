/*
Kinect Frames

    ~KinectImageFrame is a Kinect scan frame ,
             i.e. the color frame and depth frame produced at a point in time from the device
    ~SkeletonFrame is a custom Kinect skeleton frame 
            containing only a single body with its timestamped tracked skeletal position+orientation info
            with a SkeletonJoint indexed property and enumerator
            The orientations of the indexed property are absolute while the enumerated ones are hierarchical
    Both are immutable
	TODO:
        -   More than one body per SkeletonFrame?
        -   Decide on absolute/hierarchical orientation

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#region Namespaces
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Utilities;
#endregion

namespace VclKinectBridge
{
    public struct KinectImageFrame
    {
        public KinectImageFrame(ulong colorTimestamp, ulong depthTimestamp, byte[] colorBuffer, byte[] depthBuffer)
        {
            this.ColorTimestamp = colorTimestamp;
            this.DepthTimestamp = depthTimestamp;
            this.ColorBuffer = colorBuffer;
            this.DepthBuffer = depthBuffer;
        }
        public readonly ulong ColorTimestamp;
        public readonly ulong DepthTimestamp;
        public readonly byte[] ColorBuffer;
        public readonly byte[] DepthBuffer;
    }

    public class SkeletonFrame : IEnumerable<SkeletonJoint>
    {
        public SkeletonFrame(NuiSkeletonData data, ulong timestamp)
        {
            this.IsEmpty = data.Position == Vector4.zero;
            this.Positions = new Vector4[JointsNumber];
            this.Orientations = new NuiSkeletonBoneOrientation[JointsNumber];
            this.TrackingStates = new NuiSkeletonPositionTrackingState[JointsNumber];
            //data.SkeletonPositions.CopyTo (this.Positions, 0);
            this.Positions = data.SkeletonPositions.Select((vec4) => vec4.ToVector4()).ToArray();
            data.SkeletonPositionTrackingStates.CopyTo(this.TrackingStates, 0);
            this.TrackingInformation = new TrackingInfo(data.TrackingID, data.UserIndex, data.TrackingState, data.Position);
            this.Timestamp = timestamp;
            NuiSkeletonCalculateBoneOrientations(ref data, this.Orientations);
        }

        internal SkeletonFrame(SkeletonJoint[] joints, ulong timestamp)
        {
            this.Positions = joints.Select((joint) => joint.Position).ToArray();
            this.Orientations = joints.Select((joint) => new NuiSkeletonBoneOrientation()
                { 
                    AbsoluteRotation = new NuiSkeletonBoneRotation() { RotationMatrix = Matrix4x4.identity, RotationQuaternion =  joint.Orientation.ToVec4() },
                    HierarchicalRotation = new NuiSkeletonBoneRotation() { RotationMatrix = Matrix4x4.identity, RotationQuaternion = Quaternion.identity.ToVec4() }
                }).ToArray();
            this.TrackingStates = joints.Select((joint) => joint.TrackingState).ToArray();
            this.Timestamp = timestamp;
            this.TrackingInformation = new TrackingInfo(1, 1, NuiSkeletonTrackingState.SkeletonTracked, this.Positions [SkeletonHelp.Spine]);
            this.IsEmpty = this.Positions [SkeletonHelp.Spine] == Vector4.zero;
        }

        public SkeletonJoint this [int index]
        {
            get
            {
                return new SkeletonJoint(this.Positions [index], this.Orientations [index].AbsoluteRotation.RotationQuaternion, this.TrackingStates [index]);
            }
        }

        public IEnumerable<Vector4> JointPositions
        {
            get
            {
                for (int index = 0; index < JointsNumber; ++index)
                {
                    yield return this.Positions [index];
                }
            }
        }

        public IEnumerable<Quaternion> JointOrientations
        {
            get
            {
                for (int index = 0; index < JointsNumber; ++index)
                {
                    yield return this.Orientations [index].HierarchicalRotation.RotationQuaternion;
                }
            }
        }

		#region IEnumerable implementation
        public IEnumerator<SkeletonJoint> GetEnumerator()
        {
            for (int index = 0; index < JointsNumber; ++index)
            {
                yield return new SkeletonJoint(this.Positions [index], this.Orientations [index].HierarchicalRotation.RotationQuaternion, this.TrackingStates [index]);
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
		#endregion
		
        public readonly ulong Timestamp;			
        public readonly TrackingInfo TrackingInformation;
        public readonly bool IsEmpty;
				
        [DllImport(@"Kinect10.dll", EntryPoint = "NuiSkeletonCalculateBoneOrientations")]
        private static extern int NuiSkeletonCalculateBoneOrientations(ref NuiSkeletonData pSkeletonData, NuiSkeletonBoneOrientation[] pBoneOrientations);

        private readonly Vector4[] Positions;
        private readonly NuiSkeletonBoneOrientation[] Orientations;
        private readonly NuiSkeletonPositionTrackingState[] TrackingStates;
        private const int JointsNumber = 20;
    }
    public struct SkeletonJoint
    {
        public SkeletonJoint(Vector4 position, Quaternion orientation, NuiSkeletonPositionTrackingState trackingState)
        {
            this.Position = position;
            this.Orientation = orientation;
            this.TrackingState = trackingState;
        }
        internal SkeletonJoint(float[] data)
        {
            this.Position = new Vector4(data [1], data [2], data [3], 1.0f);
            this.Orientation = new Quaternion(data [4], data [5], data [6], data [7]);
            this.TrackingState = (NuiSkeletonPositionTrackingState)(int)(data [0] / 0.5f);
        }

        public readonly NuiSkeletonPositionTrackingState TrackingState;
        public readonly Vector4 Position;
        public readonly Quaternion Orientation;
    }

    public struct TrackingInfo
    {
        public TrackingInfo(uint trackingID, uint userID, NuiSkeletonTrackingState trackingState, Vector4 position)
        {
            this.TrackingID = trackingID;
            this.UserID = userID;
            this.TrackingState = trackingState;
            this.Position = position;
        }
        public readonly Vector4 Position;
        public readonly uint TrackingID;
        public readonly uint UserID;
        public readonly NuiSkeletonTrackingState TrackingState;
    }
}

