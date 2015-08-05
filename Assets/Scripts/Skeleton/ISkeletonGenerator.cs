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

interface ISkeletonGenerator<SkeletonFrameType> 
{
	bool HasNewFrame { get; }
	SkeletonFrameType CurrentFrame{ get; }
}

internal sealed class NoneSkeletonCapturer : ISkeletonGenerator<SkeletonFrame>
{
	public bool HasNewFrame { get { return false; } }
	public SkeletonFrame CurrentFrame { get { return NullFrame; } }

	private static SkeletonFrame NullFrame = new SkeletonFrame(0,new Body[0],new Floor(),TimeSpan.Zero);
}
