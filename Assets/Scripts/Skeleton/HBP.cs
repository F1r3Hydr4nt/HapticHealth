using UnityEngine;

using System;
using System.Runtime.InteropServices;

public class HBP
{
    // Structure of the library response
    public struct SkeletonBone
    {
        public float posx;
        public float posy;
        public float posz;
        public float m11;
        public float m12;
        public float m13;
        public float m21;
        public float m22;
        public float m23;
        public float m31;
        public float m32;
        public float m33;
        public IntPtr id;
    }
	
	public static SkeletonBone[] cloneSkeleton( ref SkeletonBone[] bone )
	{
		int size = bone.Length;
		SkeletonBone[] b = new HBP.SkeletonBone[size];
		
		for(int i=0 ; i<size ; ++i)
		{
			b[i].posx = bone[i].posx;
	        b[i].posy = bone[i].posy;
	        b[i].posz = bone[i].posz;
	        b[i].m11 = bone[i].m11;
	        b[i].m12 = bone[i].m12;
	        b[i].m13 = bone[i].m13;
	        b[i].m21 = bone[i].m21;
	        b[i].m22 = bone[i].m22;
	        b[i].m23 = bone[i].m23;
	        b[i].m31 = bone[i].m31;
	        b[i].m32 = bone[i].m32;
	        b[i].m33 = bone[i].m33;
	        b[i].id = bone[i].id;
		}
		return b;
	}
	
	public static Quaternion getQuaternionFromBoneData( ref SkeletonBone bone, bool flip )
	{
		Vector3 vZ;
        Vector3 vY;
		
		vZ.x = bone.m13;
		vZ.y = bone.m23;
		vZ.z = bone.m33;
		
		vY.x = bone.m12;
		vY.y = bone.m22;
		vY.z = bone.m32;

        if (!flip)
        {
            vZ.y = -vZ.y;
            vY.x = -vY.x;
            vY.z = -vY.z;
        }
        else
        {
            vZ.x = -vZ.x;
            vZ.y = -vZ.y;
            vY.z = -vY.z;
        }

        if (vZ.x != 0.0f || vZ.y != 0.0f || vZ.z != 0.0f)
            return Quaternion.LookRotation(vZ, vY);
        else
            return Quaternion.identity;
	}

    // Library functions
	/*
    [DllImport("sample_hbpUnity3DPlugin")]
    public static extern int initBodyPartDetector();

    [DllImport("sample_hbpUnity3DPlugin")]
    public static extern bool detectBodyParts(ref KinectWrapper.NuiSkeletonFrame skeleton);

    [DllImport("sample_hbpUnity3DPlugin")]
    public static extern bool getSkeleton([In, Out] SkeletonBone[] hAnimSkeleton, int userId);

    [DllImport("sample_hbpUnity3DPlugin")]
    public static extern void destroyBodyPartDetector();*/
}
