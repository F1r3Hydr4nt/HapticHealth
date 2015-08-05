#region Description
/*	
Skeleton Exporters
    ~ NullExporter No Operation Exporter
    ~ SkeletonTextExporter Exports Kinect generated Skeleton Frames to Text format ( .skelext )
             Initially the formated output is streamed into memory and when done dumped to the disk
            This results in less file i/o operations which may impact the heavier Kinect recording to the disk
            The frames are first kept in a list and written in chunks to the memory

    The Master-Detail record type was selected to allow for
        more than one skeleton ( Detail ) per frame ( Master )

    The axes X Y Z are according to the Kinect specification

TODO:
    -   switch to HUM format
    -   Refactor Dispose into Export? IDisposable OBSOLETE?
    -   Add support for multiple user exporting?
    -   Check float array conversion from text ( 9 split to 8 split ? )
    -   Check culture invariancy at float array conversion from object ( 1st ToString() ? )

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FileHelpers;
using FileHelpers.MasterDetail;

using VclKinectBridge;

using Utilities;
#endregion

namespace SkeletonOne
{
	#region Exporters

	internal sealed class NullExporter : SkeletonOneFrameExporter
    {
        public NullExporter()
        {
        }
		public override SkeletonOneExporterType Type { get { return SkeletonOneExporterType.None; } }
        public override void AddFrame(SkeletonFrame frame)
        {
        }
        public override void Export(string directory, string filename)
        {
        }
    }
    /// <summary>
    /// Skeleton text exporter.
    /// Adds skeleton frames first in a list then written to memory in chunks
    /// and lastly when finished dumps them to the disk
    /// </summary>
	internal sealed class SkeletonTextExporter : SkeletonOneFrameExporter,IDisposable
    {
		#region Constructors
        public SkeletonTextExporter()
        {
            engine = new MasterDetailEngine(
								typeof(FrameContainer),
								typeof(SkeletonContainer),
								CommonSelector.MasterIfBegins, "Frame");
            writer = new StreamWriter(new MemoryStream());
            writer.AutoFlush = true;
            writeableBlock = new List<MasterDetails>(BlockBufferSize);
            writer.WriteLine(TextHeader);
        }
		#endregion
		
		#region Properties
		public override SkeletonOneExporterType Type { get { return SkeletonOneExporterType.Text; } }
		#endregion
		
		#region Methods
        public override void AddFrame(SkeletonFrame frame)
        {
            if (frame != null && !frame.IsEmpty)
            {
                writeableBlock.Add(this.FrameToMasterDetail(frame));
                if (writeableBlock.Count == BlockBufferSize)
                {
                    engine.WriteStream(writer, writeableBlock.ToArray());
                    writeableBlock.Clear();
                }
            }
        }
		
        private MasterDetails FrameToMasterDetail(SkeletonFrame frame)
        {
            if (this.initialTimestamp == 0)
            {
                this.initialTimestamp = frame.Timestamp;
            }
            return new MasterDetails()
						{
							Master = new FrameContainer ()
							{
								FrameString = FrameSpecifier,
								Index = ++this.totalFrames,
								UserCount = 1,
								TimeStamp = frame.Timestamp - this.initialTimestamp	
							},
							Details = new SkeletonContainer[]{ this.ToContainer(frame) }
						};
        }
	
        private SkeletonContainer ToContainer(SkeletonFrame frame)
        {
            return new SkeletonContainer()
						{
							ID = (short)frame.TrackingInformation.TrackingID,
							Confidence = (int)frame.TrackingInformation.TrackingState * 0.5f,
							HipCenter = frame[SkeletonHelp.HipCenter].ToArray(),
							Spine = frame[SkeletonHelp.Spine].ToArray(),
							ShoulderCenter = frame [SkeletonHelp.ShoulderCenter].ToArray (),
							Head = frame[SkeletonHelp.Head].ToArray(),
							ShoulderLeft = frame [SkeletonHelp.ShoulderLeft].ToArray (),
							ElbowLeft = frame [SkeletonHelp.ElbowLeft].ToArray (),
							WristLeft = frame [SkeletonHelp.WristLeft].ToArray (),
							HandLeft = frame [SkeletonHelp.HandLeft].ToArray (),			
							ShoulderRight = frame [SkeletonHelp.ShoulderRight].ToArray (),
							ElbowRight = frame [SkeletonHelp.ElbowRight].ToArray (),			
							WristRight = frame [SkeletonHelp.WristRight].ToArray (),
							HandRight = frame [SkeletonHelp.HandRight].ToArray (),
							HipLeft = frame [SkeletonHelp.HipLeft].ToArray (),
							KneeLeft = frame [SkeletonHelp.KneeLeft].ToArray (),
							AnkleLeft = frame [SkeletonHelp.AnkleLeft].ToArray (),
							FootLeft = frame [SkeletonHelp.FootLeft].ToArray (),				
							HipRight = frame [SkeletonHelp.HipRight].ToArray (),
							KneeRight = frame [SkeletonHelp.KneeRight].ToArray (),
							AnkleRight = frame [SkeletonHelp.AnkleRight].ToArray (),		
							FootRight = frame [SkeletonHelp.FootRight].ToArray ()					
						};
        }
        /// <summary>
        /// Export the added data from memory into the file specified by directory and filename.
        /// Auto disposes resources , should be the last function called .
        /// After calling Export the exporter is in undefined/unusable state and no more adding operations must be performed
        /// </summary>
        /// <param name="directory">Directory.</param>
        /// <param name="filename">Filename.</param>
        public override void Export(string directory, string filename)
        {
            if (writeableBlock.Count > 0)
            {
                engine.WriteStream(writer, writeableBlock.ToArray());
                writeableBlock.Clear();
            }
            using (FileStream fs = new FileStream(Path.Combine(directory,filename + TextExtension),FileMode.Create,FileAccess.Write))
            {
                MemoryStream m = writer.BaseStream as MemoryStream;
                m.WriteTo(fs);
                m.Close();
            }
            this.Dispose();
        }
		#endregion
		
		#region IDisposable Members
        public void Dispose()
        {
            this.writeableBlock.Clear();
            this.writeableBlock = null;
            this.writer.Close();
            this.writer = null;
            this.engine = null;
        }
		#endregion

		#region Fields
        private MasterDetailEngine engine;
        private List<MasterDetails> writeableBlock;
        private StreamWriter writer;
        private ulong initialTimestamp;
        //private readonly static string frameDelimiter = "\t";
        //private readonly static string skeletonDelimiter = "\t";
        private const  string FrameSpecifier = "Frame";
        private const int BlockBufferSize = 8;
        private const string TextExtension = ".skelxt";
        internal const string TextHeader =
			@"					********* SKELEXT DATA FORMAT *********
		Frame Info ( Master Node - One Line - Tab Delimited )
		[Frame \t FrameIndex \t TimeStamp \t UserCount]
		Skeleton Info[UserCount] ( Child Nodes - Multiple Joints a.k.a Lines per Child - SPACE Delimited )
		[UserID \n
		Confidence SPACE Position.X SPACE Position.Y SPACE Position.Z SPACE Orientation.X SPACE Orientation.Y SPACE Orientation.Z SPACE Orientation.W \n]
		Joint Order = 
					{ 
						0		HipCenter
						1		Spine	
						2		ShoulderCenter
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
					}
		************************************************************************
					";
		#endregion
    }
	#endregion

    [DelimitedRecord("\t")]
    internal sealed class SkeletonContainer
    {
        public SkeletonContainer()
        {
        }
        //private const short NOID = -1;
        [FieldNullValue((short)-1)]
        public short
            ID;
        [FieldInNewLine()]
        public float
            Confidence;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        [FieldInNewLine()]
        //[FieldDelimiter(" ")]
				[FieldConverter(typeof(FloatArrayConverter))]
				public float[]
            HipCenter;

        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        [FieldInNewLine()]
        //[FieldDelimiter(" ")]
				[FieldConverter(typeof(FloatArrayConverter))]
				public float[]
            Spine;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            ShoulderCenter;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            Head;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            ShoulderLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            ElbowLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            WristLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            HandLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            ShoulderRight;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            ElbowRight;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            WristRight;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            HandRight;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            HipLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            KneeLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            AnkleLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            FootLeft;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            HipRight;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            KneeRight;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            AnkleRight;
        //[FieldQuoted]
        //[FieldTrim(TrimMode.Both)]
        //[FieldOptional]
        //[FieldDelimiter(" ")]
        [FieldInNewLine()]
        [FieldConverter(typeof(FloatArrayConverter))]
        public float[]
            FootRight;
    }
	
    [DelimitedRecord("\t")]
    [IgnoreFirst(31)]
    // lines in header count ignore
		internal sealed class FrameContainer
		{
				public FrameContainer ()
				{
				}
				public string FrameString;
				public int Index;
				public ulong TimeStamp;
				public int UserCount;
		}
/// <summary>
/// Float[] converter.
/// Converts nine FLOAT type fields to and form an array[].
/// Used to specify the number format and ignore globalization.
/// Required by FileHelpers library.
/// </summary>
		internal sealed class FloatArrayConverter : ConverterBase
		{
				public override object StringToField (string from)
				{
						float[] array = new float[8];
						string[] delimitedFloats = from.Split (delimiter, 8);
						for (int i = 0; i < 8; ++i)
						{
								array [i] = float.Parse (delimitedFloats [i],
			                         System.Globalization.NumberStyles.Float,
			                         System.Globalization.NumberFormatInfo.CurrentInfo);
						}
						return array;
				}
	
				public override string FieldToString (object from)
				{
						float[] array = (float[])from;
						return array.Select ((f) => f.ToString ()).Aggregate ((s1, s2) => s1 + " " + s2);
				}
	
				private static char[] delimiter = new char[] { ' ' };
		}
}

