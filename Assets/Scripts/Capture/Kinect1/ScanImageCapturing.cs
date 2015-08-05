#region Description
/*	
Kinect Scan Image Capturing Behaviour
    Each Update Polls for New Scan Image Frame and Aquires it
    Exposes the Scan Image Frame through Frame Property
    Color and Depth Image are by default enabled
    Also attaches the already allocated buffers to the Recording Behaviour to auto-update while recording

Switch Tracking On/Off by enabling/disabling

TODO:
    -   Check stream start/stop

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Runtime.InteropServices;

using VclKinectBridge;

using KinectHandle = System.Int32;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed  class ScanImageCapturing : WithKinectHandle
    {
		#region Properties
        internal bool HasNewFrame{ get; private set; }

        public KinectImageFrame Frame
        {
            get
            {
                return new KinectImageFrame(this.colorTimestamp, this.depthTimestamp, this.colorBuffer, this.depthBuffer);
            }
        }
		#endregion
		#region Unity
        void Awake()
        {
            this.enabled = false;
            this.recorder = GetComponent<ScanImageRecording>();
        }

        //Disabled by Default
        void OnEnable()
        {
            // color and depth are enabled by default
            //NativeBindings.KinectEnableColorStream (this.Handle, NuiImageResolution.NUI_IMAGE_RESOLUTION_640x480, IntPtr.Zero);
            //NativeBindings.KinectEnableDepthStream (this.Handle, NuiImageResolution.NUI_IMAGE_RESOLUTION_640x480, IntPtr.Zero);
            //KinectImageFrameFormat format = new KinectImageFrameFormat();
            //format.StructSize = Marshal.SizeOf(typeof(KinectImageFrameFormat));
            //NativeBindings.KinectEnableDepthStream(this.Handle,false,NuiImageResolution.NUI_IMAGE_RESOLUTION_640x480,ref format);

            //TODO: Check format change?
            if (this.colorBuffer == null)
            {
                this.colorBufferSize = NativeBindings.DefaultColorBufferSize;
                this.colorBuffer = new byte[this.colorBufferSize];
            }
            if (this.depthBuffer == null)
            {
                this.depthBufferSize = NativeBindings.DefaultDepthBufferSize;
                this.depthBuffer = new byte[this.depthBufferSize];
            }
            Debugging.Log("Playing Kinect " + this.Handle + " Started");
        }

        void Start()
        {
            this.started = this.IsValid && (NativeBindings.KinectStartStreams(this.Handle) == SuccessCheck);

            //TODO: check future use?
            //NativeBindings.KinectStartColorStream(this.Handle);
            //NativeBindings.KinectStartDepthStream(this.Handle);
        }
		
        void Update()
        {
            DateTime now = DateTime.UtcNow;
            this.HasNewFrame =
            //(NativeBindings.KinectAllFramesReady (this.Handle) &&
								(NativeBindings.KinectIsColorFrameReady(this.Handle) && NativeBindings.KinectIsDepthFrameReady(this.Handle) &&
                (NativeBindings.KinectGetColorFrame(this.Handle, this.colorBufferSize, this.colorBuffer, ref this.colorTimestamp) == SuccessCheck) &&
                (NativeBindings.KinectGetDepthFrame(this.Handle, this.depthBufferSize, this.depthBuffer, ref this.depthTimestamp) == SuccessCheck));
            Debugging.Log(this.Handle + " New SCAN Frame? " + this.HasNewFrame + " Time " + this.depthTimestamp + " elapsed " + (DateTime.UtcNow - now).Milliseconds.ToString());
        }

        new void OnDisable()
        {
            if (this.IsValid)//TODO: Atm we disable when recording => need streams playing MAYBE PAUSE?
            {
                //NativeBindings.KinectStopColorStream (this.Handle);
                //NativeBindings.KinectStopDepthStream (this.Handle);
                //NativeBindings.KinectStopStreams (this.Handle);
            }
            Debugging.Log("Kinect " + this.Handle + " Stopped");
            //TODO Check if needed
        }

        void OnDestroy()
        {
            //sensors get disposed by the manager script
        }	
		#endregion
		#region Helpers
        internal void AttachToRecorder()
        {
            if (this.recorder != null)
            {
                this.recorder.ColorBuffer = this.colorBuffer;
                this.recorder.DepthBuffer = this.depthBuffer;
            }
        }
		#endregion	
		#region Fields
        private ScanImageRecording recorder;
        private uint colorBufferSize, depthBufferSize;
        private ulong colorTimestamp, depthTimestamp;
        private byte[] colorBuffer ; //TODO check Color32[]
        private byte[] depthBuffer ; //TODO check ushort/short[]
        private bool started;
		#endregion
    }
}
