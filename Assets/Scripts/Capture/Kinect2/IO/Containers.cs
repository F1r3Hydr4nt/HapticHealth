#region Description
/* 		Kinect 2 IO FileHelpers Containers
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

using FileHelpers;
#endregion
namespace Kinect2.IO
{
	[DelimitedRecord("\t")]
	[IgnoreCommentedLines("#")]
	[IgnoreFirst(37)]
	// lines in header count ignore
	internal sealed class FrameContainer
	{
		public FrameContainer()
		{
		}
		public string FrameString;
		public int Index;
		public long TimeStamp;
		public int UserCount;
		[FieldConverter(typeof(FloorConverter))]
		public float[] Floor;
		//internal const string CommentIdentifier = "#";
	}

	[DelimitedRecord("\t")]
	internal sealed class SkeletonContainer
	{
		public SkeletonContainer()
		{
		}
		[FieldNullValue((ulong)ulong.MaxValue)]
		public ulong ID;
		public int State;
		[FieldInNewLine()]
		[FieldConverter(typeof(LeanConverter))]
		public float[] LeanState;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] SpineBase;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] SpineMid;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] Neck;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] Head;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] ShoulderLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] ElbowLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] WristLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] HandLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] ShoulderRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] ElbowRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] WristRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] HandRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] HipLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] KneeLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] AnkleLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] FootLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] HipRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] KneeRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] AnkleRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] FootRight;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] SpineShoulder;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] HandTipLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] ThumbLeft;
		[FieldInNewLine()]
		[FieldConverter(typeof(JointConverter))]
		public float[] HandTipRight;
		[FieldConverter(typeof(JointConverter))]
		[FieldInNewLine()]
		public float[] ThumbRight;
		
		internal Skeleton ToSkeleton()
		{
			var joints = new SkeletonJoint[]
			{
				new SkeletonJoint(0,this.SpineBase),
				new SkeletonJoint(1,this.SpineMid),
				new SkeletonJoint(2,this.Neck),   
				new SkeletonJoint(3,this.Head),
				new SkeletonJoint(4,this.ShoulderLeft),
				new SkeletonJoint(5,this.ElbowLeft),
				new SkeletonJoint(6,this.WristLeft),
				new SkeletonJoint(7,this.HandLeft),                
				new SkeletonJoint(8,this.ShoulderRight),
				new SkeletonJoint(9,this.ElbowRight),
				new SkeletonJoint(10,this.WristRight),
				new SkeletonJoint(11,this.HandRight),
				new SkeletonJoint(12,this.HipLeft),
				new SkeletonJoint(13,this.KneeLeft),
				new SkeletonJoint(14,this.AnkleLeft),
				new SkeletonJoint(15,this.FootLeft),
				new SkeletonJoint(16,this.HipRight),
				new SkeletonJoint(17,this.KneeRight),
				new SkeletonJoint(18,this.AnkleRight),
				new SkeletonJoint(19,this.FootRight),
				new SkeletonJoint(20,this.SpineShoulder),
				new SkeletonJoint(21,this.HandTipLeft),
				new SkeletonJoint(22,this.ThumbLeft),
				new SkeletonJoint(23,this.HandTipRight),
				new SkeletonJoint(24,this.ThumbRight)
			};
			return new Skeleton(joints, this.ID, this.LeanState, this.State);
		}
	}
	
	#region Converters
	/// <summary>
	/// Float[] converter.
	/// Converts nine FLOAT type fields to and form an array[].
	/// Used to specify the number format and ignore globalization.
	/// Required by FileHelpers library.
	/// </summary>
	internal sealed class JointConverter : ConverterBase
	{
		public override object StringToField(string from)
		{
			float[] array = new float[FloatsPerJoint];
			string[] delimitedFloats = from.Split(delimiter, FloatsPerJoint);
			for (int i = 0; i < FloatsPerJoint; ++i)
			{
				array[i] = float.Parse(delimitedFloats[i], ni);
			}
			return array;
		}
		
		public override string FieldToString(object from)
		{
			float[] array = (float[])from;
			return array.Select((f) => f.ToString(ni)).Aggregate((s1, s2) => s1 + " " + s2);
		}
		
		private static char[] delimiter = new char[] { ' ' };
		private static NumberFormatInfo ni = new NumberFormatInfo()
		{
			NumberDecimalSeparator = ".",
			NumberDecimalDigits = 6
		};
		private const int FloatsPerJoint = 8;
	}
	
	internal sealed class LeanConverter : ConverterBase
	{
		public override object StringToField(string from)
		{
			float[] array = new float[FloatsPerLean];
			string[] delimitedFloats = from.Split(delimiter, FloatsPerLean);
			for (int i = 0; i < FloatsPerLean; ++i)
			{
				array[i] = float.Parse(delimitedFloats[i], ni);
			}
			return array;
		}
		
		public override string FieldToString(object from)
		{
			float[] array = (float[])from;
			return array.Select((f) => f.ToString(ni)).Aggregate((s1, s2) => s1 + " " + s2);
		}
		
		private static char[] delimiter = new char[] { ' ' };
		private static NumberFormatInfo ni = new NumberFormatInfo()
		{
			NumberDecimalSeparator = ".",
			NumberDecimalDigits = 6
		};
		private const int FloatsPerLean = 3;
	}
	
	internal sealed class FloorConverter : ConverterBase
	{
		public override object StringToField(string from)
		{
			float[] array = new float[FloatsPerFloor];
			string[] delimitedFloats = from.Split(delimiter, FloatsPerFloor);
			for (int i = 0; i < FloatsPerFloor; ++i)
			{
				array[i] = float.Parse(delimitedFloats[i], ni);
			}
			return array;
		}
		
		public override string FieldToString(object from)
		{
			float[] array = (float[])from;
			return array.Select((f) => f.ToString(ni)).Aggregate((s1, s2) => s1 + " " + s2);
		}
		
		private static char[] delimiter = new char[] { ' ' };
		private static NumberFormatInfo ni = new NumberFormatInfo()
		{
			NumberDecimalSeparator = ".",
			NumberDecimalDigits = 6
		};
		private const int FloatsPerFloor = 4;
	}
	#endregion
}
