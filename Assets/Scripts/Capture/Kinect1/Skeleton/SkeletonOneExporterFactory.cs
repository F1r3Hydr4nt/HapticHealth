#region Description
/*	
A Simple Factory Pattern creating different Skeleton Exports

TODO:
    -   HUM format exporter?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using System;
using System.IO;

using VclKinectBridge;
#endregion

namespace SkeletonOne
{
    public enum SkeletonOneExporterType : int
    {
        None,
        Text,
        Gif,
        Xml,
        Bin,
        Hum
    }

    public abstract class SkeletonOneFrameExporter
    {
		public SkeletonOneFrameExporter()
        {

        }
		
		public abstract SkeletonOneExporterType Type { get; }
		
        public abstract void AddFrame(SkeletonFrame frame);
		
        public abstract void Export(string directory, string filename);
		
        protected int totalFrames;
        protected string filename, directory;
    }

    public sealed class SkeletonOneExporterFactory
    {
		public static SkeletonOneFrameExporter CreateExporter(SkeletonOneExporterType type)
        {
			SkeletonOneFrameExporter exporter;
            switch (type)
            {
			case SkeletonOneExporterType.Text:
                    exporter = new SkeletonTextExporter();
                    break;
			case SkeletonOneExporterType.Xml:
			case SkeletonOneExporterType.Gif:
			case SkeletonOneExporterType.Bin:
			case SkeletonOneExporterType.None:
            default:
            	exporter = new NullExporter();
                break;
            }
            return exporter;
        }
    }
		
}