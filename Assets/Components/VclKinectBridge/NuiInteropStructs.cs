/*	
Microsoft Kinect SDK v1.8 Interoperation Marshalled Structures

TODO:
    -   Finalize Vec4

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/

using UnityEngine;

using System;
using System.Runtime.InteropServices;

namespace VclKinectBridge
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KinectImageFrameFormat
    {
        public int StructSize;
        public int Height;
        public int Width;
        public int BytesPerPixel;
        public int BufferSize;
    }

    [StructLayout(LayoutKind.Sequential)]	
    public struct NuiTransformSmoothParameters
    {
        public float Smoothing;
        public float Correction;
        public float Prediction;
        public float JitterRadius;
        public float MaxDeviationRadius;
    }	

    [StructLayout(LayoutKind.Sequential)]
    public struct NuiSkeletonFrame
    {
        public Int64 TimeStamp;
        public uint FrameIndex;
        public uint Flags;
        public Vector4 FloorClipPlane;
        public Vector4 NormalToGravity;
		
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
        public NuiSkeletonData[]
            SkeletonData;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4
    {
        public Vec4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
        public float x;
        public float y;
        public float z;
        public float w;

        public static implicit operator Vector4(Vec4 vector)
        {//TODO: correct? check ToVector4
            //return new Vector4 (vector.x, vector.y, vector.z, vector.w);
            return new Vector4(vector.w, vector.x, vector.y, vector.z);
        }

        public static implicit operator Quaternion(Vec4 vector)
        {
            return new Quaternion(vector.x, vector.y, vector.z, vector.w);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NuiSkeletonData
    {
        public NuiSkeletonTrackingState TrackingState;
        public uint TrackingID;
        public uint EnrollmentIndex_NotUsed;
        public uint UserIndex;
        public Vec4 Position;
        public uint QualityFlags;
		
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
        public Vec4[]
            SkeletonPositions;
		
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 20, ArraySubType = UnmanagedType.Struct)]
        public NuiSkeletonPositionTrackingState[]
            SkeletonPositionTrackingStates;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NuiSkeletonBoneRotation
    {
        public Matrix4x4 RotationMatrix;
        public Vec4 RotationQuaternion;
        //public Quaternion RotationQuaternion;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NuiSkeletonBoneOrientation
    {
        public NuiSkeletonPositionIndex EndJoint;
        public NuiSkeletonPositionIndex StartJoint;
        public NuiSkeletonBoneRotation HierarchicalRotation;
        public NuiSkeletonBoneRotation AbsoluteRotation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NuiDepthImagePixel
    {
        public ushort PlayerIndex;
        public ushort Depth;
    }
}
