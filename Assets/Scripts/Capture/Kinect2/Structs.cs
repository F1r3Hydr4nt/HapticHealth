#region Description
/* 		FrameSpan encapsulates time period information for an operation
 * 		Serialized & Transmitted
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 */
#endregion
#region Namespaces
using System;
using System.Runtime.InteropServices;

using ProtoBuf;
#endregion

[StructLayout(LayoutKind.Sequential)]
[ProtoContract]
public struct FrameSpan
{
	public FrameSpan(int frames, long start, long end)
	{
		this.Frames = frames;
		this.StartTime = start;
		this.EndTime = end;
	}
	
	[ProtoMember(1,DataFormat=DataFormat.FixedSize)]
	public readonly long StartTime;
	
	[ProtoMember(2,DataFormat=DataFormat.FixedSize)]
	public readonly long EndTime;		
	
	[ProtoMember(3,DataFormat=DataFormat.FixedSize)]
	public readonly int Frames;
	
	public float FPS { get { return this.Frames == 0 ? 0.0f : 1.0f / ((long)(this.EndTime - this.StartTime) / 10000000.0f / (float)this.Frames); } }
	
	public override string ToString()
	{
		return string.Format(" {0} in {1} @ {2}", this.Frames,
		                     TimeSpan.FromTicks(this.EndTime - this.StartTime).ToString(),
		                     this.Frames == 0 ? 0.0f : 1.0f / ((long)(this.EndTime - this.StartTime) / 10000000.0f / (float)this.Frames));
	}
}
