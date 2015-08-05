#region Description
/* 	 Remote Kinect 2 Message
 *	Serialized by Protocol Buffers ( .NET )
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 * @version: 1.0
 */
#endregion
#region Namespaces
using System;

using ProtoBuf;
#endregion
[Flags]
internal enum MessageType : byte
{
	Ok = 0x00,
	NotOk = 0x01,
	Hello = 0x02,
	Start = 0x04,
	Stop = 0x08,
	Merge = 0x10,
	Done = 0x20,
	Bye = 0x40,
	Ack = 0x80
}

internal enum MessageParameter : byte
{
	None = 0x00,
	Play = 0x01,
	Record = 0x02,
	Calibrate = 0x03,
	Sync = 0x04,
	Track = 0x05,
	Merge = 0x06
}

[ProtoContract(EnumPassthru=true)]
internal sealed class Message
{
	#region Constructors
	internal Message()
	{
		// Required for Protobuf-net
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter)
	{
		this.ID = id;
		this.Action = action;
		this.Parameter = parameter;
		this.Time = DateTime.UtcNow;
	}
	
	internal Message(MessageType action, MessageParameter parameter)
		: this(NoID,action,parameter)
	{
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter, DateTime time)
	{
		this.ID = id;
		this.Action = action;
		this.Parameter = parameter;
		this.Time = time;
	}
	
	internal Message(MessageType action, MessageParameter parameter, DateTime time)
	{
		this.ID = NoID;
		this.Action = action;
		this.Parameter = parameter;
		this.Time = time;
	}
	
	internal Message(MessageType action, MessageParameter parameter, FrameSpan elapsed)
		: this(NoID,action, parameter)
	{
		this.Elapsed = elapsed;
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter, FrameSpan elapsed)
		: this(id, action, parameter)
	{
		this.Elapsed = elapsed;
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter, DateTime time, FrameSpan elapsed)
		: this(id, action, parameter, time)
	{
		this.Elapsed = elapsed;
	}		
	
	internal Message(MessageType action, MessageParameter parameter, DateTime time, FrameSpan elapsed)
		: this(NoID, action, parameter, time)
	{
		this.Elapsed = elapsed;
	}		
	
	internal Message(MessageType action, MessageParameter parameter, string info)
		: this(NoID, action, parameter)
	{
		this.Info = info;
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter, string info)
		: this(id, action, parameter)
	{
		this.Info = info;
	}
	
	internal Message(MessageType action, MessageParameter parameter, DateTime time, string info)
		: this(NoID, action, parameter, time)
	{
		this.Info = info;
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter, DateTime time, string info)
		: this(id, action, parameter, time)
	{
		this.Info = info;
	}
	
	internal Message(MessageType action, MessageParameter parameter, FrameSpan elapsed, string info)
		: this(NoID, action, parameter)
	{
		this.Elapsed = elapsed;
		this.Info = info;
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter, FrameSpan elapsed, string info)
		: this(id, action, parameter)
	{
		this.Elapsed = elapsed;
		this.Info = info;
	}
	
	internal Message(MessageType action, MessageParameter parameter, DateTime time, FrameSpan elapsed, string info)
		: this(NoID, action, parameter, time)
	{
		this.Elapsed = elapsed;
		this.Info = info;
	}
	
	internal Message(int id, MessageType action, MessageParameter parameter, DateTime time, FrameSpan elapsed, string info)
		: this(id, action, parameter, time)
	{
		this.Elapsed = elapsed;
		this.Info = info;
	}
	#endregion
	
	[ProtoMember(1,DataFormat=DataFormat.FixedSize)]
	internal int ID;
	
	[ProtoMember(2,DataFormat=DataFormat.FixedSize)]
	internal readonly MessageType Action;
	
	[ProtoMember(3,DataFormat=DataFormat.FixedSize)]
	internal readonly MessageParameter Parameter;
	
	[ProtoMember(4,DataFormat=DataFormat.FixedSize)]
	internal readonly DateTime Time;
	
	[ProtoMember(5,IsRequired=false,DataFormat=DataFormat.FixedSize)]
	internal readonly FrameSpan Elapsed;
	
	[ProtoIgnore]
	internal bool ElapsedSpecified { get { return this.Elapsed.Frames > 0; } }
	
	[ProtoMember(6,IsRequired=false)]
	internal readonly string Info;
	
	[ProtoIgnore]
	internal bool InfoSpecified { get { return !string.IsNullOrEmpty(this.Info); } }
	
	public override string ToString()
	{
		return string.Format("Device<{0}> {1} # {2} @ {3} for {4}, [ {5} ]",
		                     this.ID, this.Action, this.Parameter, this.Time,
		                     this.ElapsedSpecified ? this.Elapsed.ToString() : "<notime>",
		                     this.InfoSpecified ? this.Info : "<noinfo>");
	}
	
	internal const int NoID = -1;
}
