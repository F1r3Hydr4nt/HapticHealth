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
using System.Diagnostics;
using System.IO;
using System.Linq;


using FileHelpers.MasterDetail;
#endregion
namespace Kinect2.IO
{
	public sealed class SklxtWriter
	{
		#region Constructors
		private SklxtWriter()
		{
			this.stopped = true;
			this.engine = new MasterDetailEngine(
				typeof(FrameContainer),
				typeof(SkeletonContainer),
				CommonSelector.MasterIfBegins, "Frame");
			this.writer = new StreamWriter(new MemoryStream());
			this.writer.AutoFlush = true;
			this.writeableBlock = new List<MasterDetails>(BlockBufferSize);
			this.writer.WriteLine(TextHeader);			
		}
		
		private SklxtWriter(string filename)
			: this()
		{
			this.filename = filename;
		}
		#endregion
		#region Builder
		internal sealed class Constructor
		{
			private SklxtWriter writer;
			
			private Constructor()
			{
				
			}
			
			internal static Constructor Start()
			{
				return new Constructor();
			}
			
			internal Constructor New(string filename)
			{
				this.writer = new SklxtWriter(filename);
				return this;
			}
			
			internal Constructor New(Stream stream)
			{
				//TODO: future
				return this;
			}
			
			internal SklxtWriter Construct()
			{
				return this.writer;
			}
		}
		#endregion
		#region Methods
		private MasterDetails FrameToMasterDetail(SkeletonFrame frame)
		{
			return new MasterDetails()
			{
				Master = frame.ToContainer(),
				Details = this.ToContainer(frame)
			};
		}
		
		private SkeletonContainer[] ToContainer(SkeletonFrame frame)
		{
			return frame.Skeletons.Select((skeleton) => skeleton.ToContainer()).ToArray();
		}
		#endregion
		#region IWriter<SkeletonFrame,SkeletonHeader> Members
		public bool CanWrite
		{
			//get { return this.writer.BaseStream.CanWrite; }
			get { return true; }
		}

		public bool Start()
		{
			this.stopped = false;
			return !this.stopped;
		}

		public bool Write(SkeletonFrame frame)
		{
			bool success = false;
			lock (this.writer)
			{
				if (!this.stopped && (success = !frame.IsEmpty) 
				    && frame.RelativeTime.Ticks != this.previous.RelativeTime.Ticks)
				{
//					Console.Log (frame);
					this.writeableBlock.Add(this.FrameToMasterDetail(frame));
					this.initialTimestamp = this.initialTimestamp == 0 ? frame.RelativeTime.Ticks : this.initialTimestamp;
					this.previous = frame;
					if (this.writeableBlock.Count == BlockBufferSize)
					{
						//Console.Log("Writing frame");
						this.totalWrittenFrames += this.writeableBlock.Count;
						this.engine.WriteStream(this.writer, this.writeableBlock.ToArray());
						this.writeableBlock.Clear();
					}
				}
			}
			//if(success)Console.Log("Wrote frame successfully "+totalWrittenFrames);
		//	else Console.Log("Frame locked");
			return success;
		}
		
		public FrameSpan Stop()
		{
			lock (this.writer)
			{
				this.stopped = true;
			}
			if (this.unsubscriber != null)
			{
				this.unsubscriber.Dispose();
			}
			if (this.writeableBlock.Count > 0)
			{
				this.totalWrittenFrames += this.writeableBlock.Count;
				this.engine.WriteStream(this.writer, this.writeableBlock.ToArray());
				this.writeableBlock.Clear();
			}
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				MemoryStream m = this.writer.BaseStream as MemoryStream;
				m.WriteTo(fs);
				m.Close();
			}
			this.Dispose();
			return new FrameSpan(this.totalWrittenFrames, this.initialTimestamp, this.previous.RelativeTime.Ticks);
		}
		#endregion
		#region IDisposable Members
		public void Dispose()
		{
			this.writeableBlock.Clear();
			this.writer.Close();
		}
		#endregion
		#region Fields
		private MasterDetailEngine engine;
		private List<MasterDetails> writeableBlock;
		private StreamWriter writer;
		private SkeletonFrame previous;		
		private IDisposable unsubscriber;
		
		private long initialTimestamp;
		public int totalWrittenFrames;
		private bool stopped;
		public string filename;
		//private readonly static string frameDelimiter = "\t";
		//private readonly static string skeletonDelimiter = "\t";
		private const string FrameSpecifier = "Frame";
		private const int BlockBufferSize = 8;
		private const string TextExtension = ".sklxt";
		internal const string TextHeader =
			@"					********* SKELEXT DATA FORMAT *********
		Frame Info ( Master Node - One Line - Tab Delimited )
		[Frame \t FrameIndex \t TimeStamp \t UserCount]
		Skeleton Info[UserCount] ( Child Nodes - Multiple Joints a.k.a Lines per Child - SPACE Delimited )
		[UserID \t State \n
         LeanTrackingState LeanX LeanY
		Confidence SPACE Position.X SPACE Position.Y SPACE Position.Z SPACE Orientation.X SPACE Orientation.Y SPACE Orientation.Z SPACE Orientation.W \n]
		Joint Order = 
					{ 
						0		SpineBase
						1		SpineMid	
						2		Neck
						3		Head
						4		ShoulderLeft
						5		ElbowLeft
						6		WristLeft
						7		HandLeft
						8		ShoulderRight
						9		ElbowRight
						10		WristRight
						11		HandRight
						12		HipLeft
						13		KneeLeft
						14		AnkleLeft
						15		FootLeft
						16		HipRight
						17		KneeRight
						18		AnkleRight
						19		FootRight
                        20      SpineShoulder
                        21      HandTipLeft
                        22      ThumbLeft
                        23      HandTipRight
                        24      ThumbRight
					}
		************************************************************************
					";	
		#endregion
	}
}

