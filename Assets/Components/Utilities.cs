#define DEBUGLOG

#region Description
/*	TODO Description
	*/
#endregion
#region Namespaces
using Vector = UnityEngine.Vector3;
using Matrix = UnityEngine.Matrix4x4;
using UnityEngine;

using System.Linq;
using System.Diagnostics;

using Logger = UnityEngine.Debug;

//using Skeleton;
using VclKinectBridge;
#endregion

namespace Utilities
{
/// <summary>
/// Debugging Helper Functions.
/// Optionally compiled.
/// </summary>
    public static class Debugging
    {
        [Conditional("DEBUGLOG")]
        public static void Log(string message)
        {
            Logger.Log(message);
        }

        [Conditional("DEBUGLOG")]
        public static void Warn(string message)
        {
            Logger.LogWarning(message);
        }

        [Conditional("DEBUGLOG")]
        public static void Error(string message)
        {
            Logger.LogError(message);
        }
    }
/// <summary>
/// Extension methods.
/// </summary>
    public static class Extensions
    {
        internal static float[] ToArray(this SkeletonJoint joint)
        {
            return new float[] 
					{
						(int)joint.TrackingState * 0.5f,joint.Position.x,joint.Position.y,joint.Position.z,
						joint.Orientation.x,joint.Orientation.y,joint.Orientation.z,joint.Orientation.w
					};
        }

        internal static float[] ToArray(this Matrix matrix)
        {
            return new float[]
						{
							matrix.m00,matrix.m01,matrix.m02,matrix.m03,
							matrix.m10,matrix.m11,matrix.m12,matrix.m13,
							matrix.m20,matrix.m21,matrix.m22,matrix.m23,
							matrix.m30,matrix.m31,matrix.m32,matrix.m33,
						};
        }

        internal static Matrix ToMatrix(this float[] data)
        {
            return new Matrix()//TODO: Check Translation Scaling
						{//TODO: allaxa y kai x ( 1i kai 3i stili )
							m00 = data[1], m01 = data[2], m02 = data[0], m03 = data[9]/1000,
							m10 = data[4], m11 = data[5], m12 = data[3], m13 =data[11]/1000,
							m20 = data[7], m21 = data[8], m22 = data[6], m23 = data[10]/1000,
							m30 = 0.0f, m31 = 0.0f, m32 = 0.0f, m33 = 1.0f
						};
        }

        public static void FromMatrix4x4(this Transform transform, Matrix matrix)
        {
            transform.localScale = matrix.GetScale();
            transform.rotation = matrix.GetRotation();
            transform.position = matrix.GetPosition();
        }
		
        public static Quaternion GetRotation(this Matrix matrix)
        {
            var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
            var w = 4 * qw;
            var qx = (matrix.m21 - matrix.m12) / w;
            var qy = (matrix.m02 - matrix.m20) / w;
            var qz = (matrix.m10 - matrix.m01) / w;
			
            return new Quaternion(qx, qy, qz, qw);
        }
		
        public static Vector GetPosition(this Matrix matrix)
        {
            var x = matrix.m03;
            var y = matrix.m13;
            var z = matrix.m23;
			
            return new Vector(x, y, z);
        }
		
        public static Vector GetScale(this Matrix m)
        {
            var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
            var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
            var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);
			
            return new Vector(x, y, z);
        }

        public static Quaternion GetUnityRotation(this Matrix matrix)
        {			
            return Quaternion.LookRotation(matrix.GetColumn(2), matrix.GetColumn(1));			
        }
    }
/// <summary>
/// Unity GUI helpers.
/// </summary>
    public static class GuiHelp
    {
        public static void AutoResize(int screenWidth, int screenHeight)
        {
            Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
            GUI.matrix = Matrix.TRS(Vector.zero, Quaternion.identity, new Vector(resizeRatio.x, resizeRatio.y, 1.0f));
        }
    }
/// <summary>
/// Skeleton helpers and common Constants.
/// </summary>
    public static class SkeletonHelp
    {
        public const int HipCenter = 0;
        public const int Spine = 1;
        public const int ShoulderCenter = 2;
        public const int Head = 3;
        public const int ShoulderLeft = 4;
        public const int ElbowLeft = 5;
        public const int WristLeft = 6;
        public const int HandLeft = 7;
        public const int ShoulderRight = 8;
        public const int ElbowRight = 9;
        public const int WristRight = 10;
        public const int HandRight = 11;
        public const int HipLeft = 12;
        public const int KneeLeft = 13;
        public const int AnkleLeft = 14;
        public const int FootLeft = 15;
        public const int HipRight = 16;
        public const int KneeRight = 17;
        public const int AnkleRight = 18;
        public const int FootRight = 19;

        public static int[] NuiSkeletonPosition = new int[]
			{
					(int)NuiSkeletonPositionIndex.HipCenter,(int)NuiSkeletonPositionIndex.Spine,(int)NuiSkeletonPositionIndex.ShoulderCenter,
					(int)NuiSkeletonPositionIndex.Head,(int)NuiSkeletonPositionIndex.ShoulderLeft,(int)NuiSkeletonPositionIndex.ElbowLeft,
					(int)NuiSkeletonPositionIndex.WristLeft,(int)NuiSkeletonPositionIndex.HandLeft,(int)NuiSkeletonPositionIndex.ShoulderRight,
					(int)NuiSkeletonPositionIndex.ElbowRight,(int)NuiSkeletonPositionIndex.WristRight,(int)NuiSkeletonPositionIndex.HandRight,
					(int)NuiSkeletonPositionIndex.HipLeft,(int)NuiSkeletonPositionIndex.KneeLeft,(int)NuiSkeletonPositionIndex.AnkleLeft,
					(int)NuiSkeletonPositionIndex.FootLeft,(int)NuiSkeletonPositionIndex.HipRight,(int)NuiSkeletonPositionIndex.KneeRight,
					(int)NuiSkeletonPositionIndex.AnkleRight,(int)NuiSkeletonPositionIndex.FootRight
			};

        private static Matrix SwitchAxesZwithY = Matrix.TRS(Vector.zero, Quaternion.AngleAxis(-90, Vector.right) * Quaternion.AngleAxis(-90, Vector.forward), Vector.one);

        [Conditional("DEBUGLOG")]
        public static void DebugSkeleton(SkeletonFrame skeleton, Matrix transform)
        {
            if (skeleton != null)
            {
                //transform *= SwitchAxesZwithY;
                var positions = skeleton.JointPositions.Select((position) => transform * position).ToArray();
                Logger.DrawLine(positions [Head], positions [ShoulderCenter]);
                Logger.DrawLine(positions [ShoulderCenter], positions [ShoulderLeft]);
                Logger.DrawLine(positions [ShoulderCenter], positions [ShoulderRight]);
                Logger.DrawLine(positions [ShoulderLeft], positions [ElbowLeft]);
                Logger.DrawLine(positions [ElbowLeft], positions [WristLeft]);
                //Logger.DrawLine (positions [WristLeft], positions [HandLeft]);
                Logger.DrawLine(positions [ShoulderRight], positions [ElbowRight]);
                Logger.DrawLine(positions [ElbowRight], positions [WristRight]);
                //Logger.DrawLine (positions [WristRight], positions [HandRight]);
                Logger.DrawLine(positions [ShoulderCenter], positions [Spine]);
                //Logger.DrawLine (positions [ShoulderLeft], positions [Spine]);
                //Logger.DrawLine (positions [ShoulderRight], positions [Spine]);
                Logger.DrawLine(positions [Spine], positions [HipCenter]);
                Logger.DrawLine(positions [HipCenter], positions [HipLeft]);
                Logger.DrawLine(positions [HipCenter], positions [HipRight]);
                Logger.DrawLine(positions [HipLeft], positions [KneeLeft]);
                Logger.DrawLine(positions [KneeLeft], positions [AnkleLeft]);
                //Logger.DrawLine (positions [AnkleLeft], positions [FootLeft]);
                Logger.DrawLine(positions [HipRight], positions [KneeRight]);
                Logger.DrawLine(positions [KneeRight], positions [AnkleRight]);
                //Logger.DrawLine (positions [AnkleRight], positions [FootRight]);
            }
        }

        public static Vector4 ToVector4(this Vec4 vector)
        {
            return new Vector4(vector.w, vector.x, vector.y, vector.z);
        }

        public static Quaternion ToQuaternion(this Vec4 vector)
        {
            return new Quaternion(vector.w, vector.x, vector.y, vector.z);
        }

        public static Vec4 ToVec4(this Quaternion quaternion)
        {
            return new Vec4(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
        }
    }
}

