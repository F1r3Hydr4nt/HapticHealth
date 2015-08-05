#region Description
/*	
Kinect Scan Image Recording Behaviour
    Records Kinect frames in Scan Image Zip files
    There is no Update function , the recording happens in a different thread
    The recording thread continually polls for new frames and records them
    The Acquire function used is different from ScanImageCapturing , 
            this one extracts the raw depth from the packed depth frame ( containing the user map too )
            but also updates the ScanImageCapturing buffers as they are Attached

Switch Tracking On/Off by enabling/disabling

TODO:
    -   Investigate better performance polling
    -   Choice to record packed format?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.IO;
using System.Threading;

//using ScanImageZipFile;
//using ScanImageZipFile.Record;
using VclKinectBridge;

using KinectHandle = System.Int32;

using Utilities;
#endregion

namespace VclUnityKinect
{
    public sealed  class ScanImageRecording : WithKinectHandle
    {
			#region Properties
        public string OutputDirectory
        {
            get { return this.outputDirectory;}
            set
            {
                if (Path.IsPathRooted(value))
                {
                    this.outputDirectory = value;
                }
            }
        }
        public string Filename
        {
            get{ return this.filename;}
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.filename = value;
                }
            }
        }
        //public KinectInfo KinectInformation { private get; set; }
        public bool IsRecording
        {
            get
            {
                bool retValue;
                this.recordingLock.EnterReadLock();
                retValue = this.isRecording;
                this.recordingLock.ExitReadLock();
                return retValue;
            }
            private set
            {
                this.recordingLock.EnterWriteLock();
                this.isRecording = value;
                this.recordingLock.ExitWriteLock();
            }
        }
        internal  byte[] ColorBuffer
        {
            set
            {
                this.colorBuffer = value;
                this.colorBufferSize = (uint)value.Length;
            }
        }
        internal  byte[] DepthBuffer
        {
            set
            {
                this.depthBuffer = value;
                this.depthBufferSize = (uint)value.Length;
            }
        }
			#endregion
			#region Unity
        void Awake()
        {
            this.enabled = false;
            this.recordingLock = new ReaderWriterLockSlim();
        }

        void OnEnable()
        {
            //this.recorder = new JpegLz4Recorder(Path.Combine(this.OutputDirectory, Path.ChangeExtension(this.Filename, JpegLz4Recorder.Extension)));
            this.enabled = this.IsValid && NativeBindings.KinectStartCompressor(this.Handle, ScanImageCompression.Default,
							ref this.colorWorstCaseSize, ref this.depthWorstCaseSize);

            if (this.enabled)
            {
                this.Initialize();					
                //this.recorder.Begin(this.KinectInformation);
                this.recordingThread = new Thread(new ThreadStart(this.Record))
				{
					Name = "RecordingThread " + this.Handle,
					Priority = System.Threading.ThreadPriority.AboveNormal
				};
                this.recordingThread.Start();
                Console.Log("Recording Kinect " + this.Handle + " Started");
            } else
            {//TODO:Handle Error ? Stop Compress/Rec?

            }
        }

        internal void Initialize()
        {
            this.colorCompressedBuffer = new byte[this.colorWorstCaseSize];
            this.depthCompressedBuffer = new byte[this.depthWorstCaseSize];
            if (this.depthPixels == null)
            {
                this.depthPixelsSize = NativeBindings.DefaultImageSize;
                this.depthPixels = new NuiDepthImagePixel[this.depthPixelsSize];
            }
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
            this.recordedFrames = 0;			
            this.IsRecording = true;
            this.initialTimestamp = 0;	
        }
	
        void Record()
        {
            while (this.IsRecording)
            {
                Thread.Sleep(SleepTime);//TODO:Needed?
                DateTime now = DateTime.UtcNow;
                if (
										//NativeBindings.KinectAllFramesReady (this.Handle) &&
										NativeBindings.KinectIsColorFrameReady(this.Handle) && NativeBindings.KinectIsDepthFrameReady(this.Handle) &&
                    NativeBindings.KinectGetCompressedScanImagePixels(this.Handle,
										this.colorBuffer, this.colorBufferSize, ref this.colorTimestamp, this.depthBuffer, this.depthBufferSize, ref this.depthTimestamp,
										this.depthPixelsSize, this.depthPixels,
										this.colorCompressedBuffer, this.depthCompressedBuffer, ref this.colorCompressedBufferSize, ref this.depthCompressedBufferSize))
                {
                    Console.Log("Rec " + (DateTime.UtcNow - now).Milliseconds.ToString());
                    if (this.initialTimestamp == 0)
                    {
                        this.initialTimestamp = this.depthTimestamp;
                    }
                    //this.recorder.Record(new ScanFrame(++this.recordedFrames, this.colorTimestamp, this.colorCompressedBuffer, this.depthTimestamp, this.depthCompressedBuffer),
					//						this.colorCompressedBufferSize, this.depthCompressedBufferSize);				
                }
            }
        }

        new void OnDisable()
        {
            if (this.IsRecording)
            {
                this.IsRecording = false;
                this.recordingThread.Join();
                //this.recorder.End();
                NativeBindings.KinectStopCompressor(this.Handle);
                Console.Log("Recording Kinect " + this.Handle + " Ended (written " 
                    + this.recordedFrames + " frames in " + (this.depthTimestamp - this.initialTimestamp) / 1000.0f + " secs  at " + 
                    (this.depthTimestamp - this.initialTimestamp) / (ulong)this.recordedFrames + " ms )");
            }
        }
			#endregion
			#region Fields
        //private JpegLz4Recorder recorder;
        private Thread recordingThread;
        private byte[] colorCompressedBuffer, depthCompressedBuffer, colorBuffer, depthBuffer;
        private NuiDepthImagePixel[] depthPixels;
        private ulong colorTimestamp, depthTimestamp, initialTimestamp, currentTimestamp;
        private string outputDirectory, filename;
        private uint colorBufferSize, depthBufferSize, depthPixelsSize;
        private int colorCompressedBufferSize, depthCompressedBufferSize;
        private int colorWorstCaseSize, depthWorstCaseSize;
        private uint recordedFrames;
        //NOTE: sleep > 15ms => lose frames , sleep < 15ms => gain nothing
        private const int SleepTime = 15;
        //TODO: Refactor?
        private bool isRecording;
        private ReaderWriterLockSlim recordingLock;
			#endregion
    }

}