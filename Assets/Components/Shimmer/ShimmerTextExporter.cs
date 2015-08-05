#region Description
/*
ShimmerTextExporter serializes and exports Shimmer3 generated data in text format.
    Initially the formated output is streamed into memory and when done dumped to the disk
    This results in less file i/o operations which may impact the heavier Kinect recording to the disk
    The data size even when formatted into text is small enough to allow for memory storage before dumping to the disk.

WimuRecord is a Plain Old Class Object containing
    -   Calibrated Shimmer3 Timestamp
    -   XYZ Accelerometer Values
    -   XYZ Gyroscope Values
    -   XYZ Magnetometer Values
    -   XYZW Orientation represented by  a Quaternion

The axes X Y Z are according to the Shimmer specification

TODO:
    -   Finalize extension and format?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
    */
#endregion
#region Namespaces
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;

using FileHelpers;

using UnityEngine;
using UQuaternion = UnityEngine.Quaternion;

using ShimmerConnect;
#endregion

namespace Shimmer
{
    internal sealed class ShimmerTextExporter
    {
        internal ShimmerTextExporter()
        {
            this.engine = new FileHelperEngine(typeof(WimuRecord));
            this.writer = new StreamWriter(new MemoryStream());
            this.writer.AutoFlush = true;
            this.writeableBlock = new List<WimuRecord>(BlockBufferSize);
            this.writer.WriteLine(Header);
        }

        internal void AddData(SensorData data)
        {//TODO: Bounded Write?
            this.writeableBlock.Add(new WimuRecord(data));
            if (this.writeableBlock.Count >= BlockBufferSize)
            {
                this.engine.WriteStream(this.writer, this.writeableBlock);
                this.writeableBlock.Clear();
            }
        }

        internal void AddMultipleData(SensorData[] data)
        {
            engine.WriteStream(this.writer, data.Select((sensorData) => new WimuRecord(sensorData)));
        }

/// <summary>
/// Export the added data from memory into the file specified by directory and filename.
/// Auto disposes resources , should be the last function called .
/// After calling Export the exporter is in undefined/unusable state and no more adding operations must be performed
/// </summary>
/// <param name="directory">Directory.</param>
/// <param name="filename">Filename.</param>
        internal void Export(string directory, string filename)
        {
            using (FileStream file = new FileStream(Path.Combine(directory, filename + Extension), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var stream = this.writer.BaseStream as MemoryStream;
                stream.WriteTo(file);
                stream.Close();
            }
            this.writeableBlock.Clear();
            this.writer.Close();
        }

        private FileHelperEngine engine;
        private StreamWriter writer;
        private List<WimuRecord> writeableBlock;
        private const string Header = @"WIMU File";
        private const int BlockBufferSize = 100;
        private const string Extension = @".wimu";
        internal const string Commenter = @"%";
        internal const string Delimiter = "\t";
        internal const int HeaderLinesCount = 1;
    }

    [DelimitedRecord(ShimmerTextExporter.Delimiter)]
    [IgnoreEmptyLines()]
    [IgnoreCommentedLines(ShimmerTextExporter.Commenter)]
    [IgnoreFirst(ShimmerTextExporter.HeaderLinesCount)]
    /// <summary>
/// Wimu record.
/// Accelerometer,Gyroscope and Magnetometer Data in X Y Z axis order
/// Orientation in X Y Z W ( Quaternion ) order
/// </summary>
    internal sealed class WimuRecord
    {
/// <summary>
/// Wimu record , empty constuctor , required for FileHelpers library.
/// </summary>
        public WimuRecord()
        {
        
        }
/// <summary>
/// Initializes a new instance of the <see cref="Shimmer.WimuRecord" from a SensorData object/> class.
/// </summary>
/// <param name="data">Data.</param>
        public WimuRecord(SensorData data)
        {
            this.Timestamp = data.GetTimeStamp();
            this.Accelerometer = data.GetAccel();
            this.Gyroscope = data.GetGyro();
            this.Magnetometer = data.GetMag();
            this.Orientation = data.GetQuaternion();
        }

        [FieldConverter(typeof(DoubleConverter))]
        internal double
            Timestamp;

        [FieldConverter(typeof(ThreeDoublesConverter))]
        internal double[]
            Accelerometer;

        [FieldConverter(typeof(ThreeDoublesConverter))]
        internal double[]
            Gyroscope;

        [FieldConverter(typeof(ThreeDoublesConverter))]
        internal double[]
            Magnetometer;

        [FieldConverter(typeof(FourDoublesConverter))]
        internal double[]
            Orientation;
    }
/// <summary>
/// Double converter.
/// Converts a Single DOUBLE type field to and form.
/// Used to specify the number format and ignore globalization.
/// Required by FileHelpers library.
/// </summary>
    internal sealed class DoubleConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            return double.Parse(from, NumberStyles.Float, ni);                 
        }
        
        public override string FieldToString(object from)
        {
            return ((double)from).ToString(ni);
        }
        
        private static char[] delimiter = new char[] { ' ' };
        private static NumberFormatInfo ni = new NumberFormatInfo(){  NumberDecimalSeparator = ","};
    }
/// <summary>
/// Double[] converter.
/// Converts three DOUBLE type fields to and form an array[].
/// Used to specify the number format and ignore globalization.
/// Required by FileHelpers library.
/// </summary>
    internal sealed class ThreeDoublesConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            double[] array = new double[3];
            string[] delimitedFloats = from.Split(delimiter, 3);
            for (int i = 0; i < delimitedFloats.Length; ++i)
            {
                array [i] = double.Parse(delimitedFloats [i], NumberStyles.Float, ni);                                   
            }
            return array;
        }
        
        public override string FieldToString(object from)
        {
            double[] array = (double[])from;
            return array.Select((d) => d.ToString(ni)).Aggregate((s1, s2) => s1 + " " + s2);
        }
        
        private static char[] delimiter = new char[] { ' ' };
        private static NumberFormatInfo ni = new NumberFormatInfo(){  NumberDecimalSeparator = "," };
    }
    /// <summary>
    /// Double[] converter.
    /// Converts four DOUBLE type fields to and form an array[].
    /// Used to specify the number format and ignore globalization.
    /// Required by FileHelpers library.
    /// </summary>
    internal sealed class FourDoublesConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            double[] array = new double[4];
            string[] delimitedFloats = from.Split(delimiter, 4);            
            for (int i = 0; i < delimitedFloats.Length; ++i)
            {
                array [i] = double.Parse(delimitedFloats [i], NumberStyles.Float, ni);
            }
            return array;
        }
        
        public override string FieldToString(object from)
        {
            double[] array = (double[])from;
            return array.Select((d) => d.ToString(ni)).Aggregate((s1,s2) => s1 + " " + s2);
        }
        
        private static char[] delimiter = new char[] { ' ' };
        private static NumberFormatInfo ni = new NumberFormatInfo(){  NumberDecimalSeparator = ","};
    }
}
