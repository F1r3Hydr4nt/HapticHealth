#region Description
/*	
Kinect Skeleton Exporting Behaviour
    Each LateUpdate polls the tracker for a new frame and adds it to the exporter
        LateUpdate is used to enforce ordering
        
Switch Tracking On/Off by enabling/disabling

TODO:
    -   Other Formats?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;

using SkeletonOne;
using VclKinectBridge;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed  class SkeletonOneExporting : MonoBehaviour
    {
		#region Properties
        public string OutputDirectory { get; set; }
        public string Filename { get; set; }
		#endregion
		#region Unity
        void Awake()
        {
            this.tracker = GetComponent<BodyTracking>();
        }

        void OnEnable()
        {
            if (this.tracker == null)
            {
                this.enabled = false;
                Debugging.Error("No Tracker Attached ! Cant Export , Disabling ....");
            } else
            {
                this.exporter = SkeletonOneExporterFactory.CreateExporter(SkeletonOneExporterType.Text);
                this.totalFrames = 0;
                Console.Log("Skeleton Exporting Started !");
            }

        }

        void LateUpdate()
        {
            if (this.tracker.HasNewFrame)
            {
                DateTime now = DateTime.UtcNow;
                this.exporter.AddFrame(this.tracker.Skeleton);
                Console.Log("Skeleton Frame Added in " + (DateTime.UtcNow - now).Milliseconds.ToString() + " ms.");
                ++this.totalFrames;
            }
        }

        void OnDisable()
        {
            if (this.exporter != null)
            {
                this.exporter.Export(this.OutputDirectory, this.Filename);
                this.exporter = null;
                Console.Log("Skeleton Exporting Stopped ! ( " + this.totalFrames + "  total frames recoded )");
            }
        }
		#endregion
		#region Fields
        private BodyTracking tracker;

        private SkeletonOneFrameExporter exporter;

        private int totalFrames	;
        private ulong initialTimestamp;		
		#endregion
    }
}
