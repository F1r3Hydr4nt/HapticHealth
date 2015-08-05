#region Description
/*	
VclKinectBridge Native Interop Function Declarations
and common Constants

TODO:
    -   Check Constants , maybe Refactor?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Runtime.InteropServices;

using KinectHandle = System.Int32;
#endregion
namespace VclKinectBridge
{
    internal class NativeBindings
    {
	#region Device
        [DllImport(VclKinectBridge, EntryPoint = "KinectGetPortIDCount")]
        public static extern uint KinectGetPortIDCount();

        [DllImport(VclKinectBridge, EntryPoint = "KinectOpenDefaultSensor")]
        public static extern KinectHandle KinectOpenDefaultSensor();
	
        [DllImport(VclKinectBridge, EntryPoint = "KinectOpenConnectedDevices")]
        public static extern uint KinectOpenConnectedDevices(ref IntPtr handles);	
	
        [DllImport(VclKinectBridge, EntryPoint = "KinectIsHandleValid")]
        public static extern bool KinectIsHandleValid(KinectHandle handle);

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectCloseSensor")]
        public static extern void KinectCloseSensor(KinectHandle handle);

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetKinectSensorStatus")]
        public static extern KinectSensorStatus KinectGetKinectSensorStatus(KinectHandle handle);

        
#endregion
		
#region Stream
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetColorStreamStatus")]
        public static extern KinectStreamStatus KinectGetColorStreamStatus(KinectHandle handle);


        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetIRStreamStatus")]
        public static extern KinectStreamStatus KinectGetIRStreamStatus(KinectHandle handle);

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetDepthStreamStatus")]
        public static extern KinectStreamStatus KinectGetDepthStreamStatus(KinectHandle handle);

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetSkeletonStreamStatus")]
        public static extern KinectStreamStatus KinectGetSkeletonStreamStatus(KinectHandle handle);
        //Enable

//				[DllImportAttribute(KinectCommonBridgeLibraryFile, EntryPoint = "KinectEnableColorStream")]
//				public static extern void KinectEnableColorStream (KinectHandle handle,
//	                                                   NuiImageResolution resolution, ref IntPtr frameFormat);

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectEnableColorStream")]
        public static extern void KinectEnableColorStream(KinectHandle handle,
		                                                   NuiImageResolution resolution, ref KinectImageFrameFormat frameFormat);
	
//				[DllImportAttribute(KinectCommonBridgeLibraryFile, EntryPoint = "KinectEnableSkeletonStream")]
//				public static extern void KinectEnableSkeletonStream (KinectHandle handle,
//	                                                      bool nearMode, KinectSkeletonSelectionMode mode, ref IntPtr smoothParams);

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectEnableSkeletonStream")]
        public static extern void KinectEnableSkeletonStream(KinectHandle handle,
		                                                      bool nearMode, KinectSkeletonSelectionMode mode, ref NuiTransformSmoothParameters smoothParams);
	
//				[DllImportAttribute(KinectCommonBridgeLibraryFile, EntryPoint = "KinectEnableIRStream")]
//				public static extern void KinectEnableIRStream (KinectHandle handle,
//	                                                KinectSkeletonSelectionMode mode, ref IntPtr frameFormat);	

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectEnableIRStream")]
        public static extern void KinectEnableIRStream(KinectHandle handle, NuiImageResolution resolution,
		                                                ref KinectImageFrameFormat frameFormat);	
	
//				[DllImportAttribute(KinectCommonBridgeLibraryFile, EntryPoint = "KinectEnableDepthStream")]
//				public static extern void KinectEnableDepthStream (KinectHandle handle,
//	                                                   KinectSkeletonSelectionMode mode, ref IntPtr frameFormat);	

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectEnableDepthStream")]
        public static extern void KinectEnableDepthStream(KinectHandle handle, bool nearMode, NuiImageResolution resolution,
		                                                   ref KinectImageFrameFormat frameFormat);	
	
        //Start
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStartSkeletonStream")]
        public static extern int KinectStartSkeletonStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStartIRStream")]
        public static extern int KinectStartIRStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStartColorStream")]
        public static extern int KinectStartColorStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStartDepthStream")]
        public static extern int KinectStartDepthStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStartStreams")]
        public static extern int KinectStartStreams(KinectHandle handle);
	
        //Pause
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectPauseStreams")]
        public static extern void KinectPauseStreams(KinectHandle handle, bool pause);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectPauseIRStream")]
        public static extern void KinectPauseIRStream(KinectHandle handle, bool pause);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectPauseColorStream")]
        public static extern void KinectPauseColorStream(KinectHandle handle, bool pause);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectPauseDepthStream")]
        public static extern void KinectPauseDepthStream(KinectHandle handle, bool pause);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectPauseSkeletonStream")]
        public static extern void KinectPauseSkeletonStream(KinectHandle handle, bool pause);
	
        //Stop
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStopSkeletonStream")]
        public static extern void KinectStopSkeletonStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStopDepthStream")]
        public static extern void KinectStopDepthStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStopColorStream")]
        public static extern void KinectStopColorStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStopIRStream")]
        public static extern void KinectStopIRStream(KinectHandle handle);
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStopStreams")]
        public static extern void KinectStopStreams(KinectHandle handle);
#endregion

#region Frames
        //Query
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectIsColorFrameReady")]
        public static extern bool KinectIsColorFrameReady(KinectHandle handle) ;		
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectIsDepthFrameReady")]
        public static extern bool KinectIsDepthFrameReady(KinectHandle handle) ;
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectIsSkeletonFrameReady")]
        public static extern bool KinectIsSkeletonFrameReady(KinectHandle handle) ;
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectAnyFrameReady")]
        public static extern bool KinectAnyFrameReady(KinectHandle handle) ;
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectAllFramesReady")]
        public static extern bool KinectAllFramesReady(KinectHandle handle) ;
		
        //Get
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetIRFrame")]
        public static extern int KinectGetIRFrame(KinectHandle handle, uint bufferSize, byte[] buffer, ref ulong timestamp) ;
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetColorFrame")]
        public static extern int KinectGetColorFrame(KinectHandle handle, uint bufferSize, byte[] buffer, ref ulong timestamp) ;
	
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetDepthFrame")]
        public static extern int KinectGetDepthFrame(KinectHandle handle, uint bufferSize, byte[] buffer, ref ulong timestamp) ;

//				[DllImportAttribute(KinectCommonBridgeLibraryFile, EntryPoint = "KinectGetDepthFrame")]
//				public static extern int KinectGetDepthFrame (KinectHandle handle, uint bufferSize, [MarshalAs(UnmanagedType.LPArray)]ushort[] buffer, ref ulong timestamp) ;
    
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetSkeletonFrame")]		
        public static extern int KinectGetSkeletonFrame(KinectHandle handle, ref NuiSkeletonFrame skeletonFrame) ;
#endregion

#region Record & Play
        // Compress
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStartCompressor")]
        public static extern bool KinectStartCompressor(KinectHandle handle, ScanImageCompression type, ref int colorWorstCaseSize, ref int depthWorstCaseSize) ;		

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStopCompressor")]
        public static extern void KinectStopCompressor(KinectHandle handle);

		
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetCompressedScanImage")]
        public static extern bool KinectGetCompressedScanImage(KinectHandle handle, byte[] colorBuffer, uint colorBufferSize, ref ulong colorTimestamp,
																byte[] depthBuffer, uint depthBufferSize, ref ulong depthTimestamp,
																 byte[] colorBufferOut, byte[] depthBufferOut,
																 ref int colorOutBufferSize, ref int depthOutBufferSize) ;	

		
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectGetCompressedScanImagePixels")]
        public static extern bool KinectGetCompressedScanImagePixels(KinectHandle handle, byte[] colorBuffer, uint colorBufferSize, ref ulong colorTimestamp,
		                                                        byte[] depthBuffer, uint depthBufferSize, ref ulong depthTimestamp,
                                                              	uint depthPixelsSize, NuiDepthImagePixel[] pDepthPixels,
																byte[] colorBufferOut, byte[] depthBufferOut,
		                                                        ref int colorOutBufferSize, ref int depthOutBufferSize) ;	

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectCompressScanImage")]
        public static extern bool KinectCompressScanImage(KinectHandle handle, byte[] colorBuffer, uint colorBufferSize, byte[] depthBuffer, uint depthBufferSize,
																byte[] colorBufferOut, byte[] depthBufferOut, ref int colorOutBufferSize, ref int depthOutBufferSize);

        // Decompress
        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStartDecompressor")]
        public static extern bool KinectStartDecompressor(KinectHandle handle, ScanImageCompression type) ;		

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectStopDecompressor")]
        public static extern void KinectStopDecompressor(KinectHandle handle);

        [DllImportAttribute(VclKinectBridge, EntryPoint = "KinectDecompressScanImage")]
        public static extern bool KinectDecompressScanImage(KinectHandle handle, byte[] colorBufferCompressed, uint colorBufferSize,
	                                                            byte[] depthBufferCompressed, uint depthBufferSize, byte[] colorBufferOut, byte[] depthBufferOut,
		                                                        ref int colorOutBufferSize, ref int depthOutBufferSize) ;		

#endregion
    
#region Constants
        private const string VclKinectBridge = @"VclKinectBridge";

        internal const KinectHandle NoHandle = 7777;

        internal const int DefaultFrameHeight = 480;
        internal const int DefaultFrameWidth = 640;	
        internal const int DefaultImageSize = 640 * 480;
        internal const int DefaultColorBytesPerPixel = 4;
        internal const int DefaultDepthBytesPerPixel = 2;
        internal const int DefaultIRBytesPerPixel = 2;
        internal const int DefaultBayerBytesPerPixel = 1;
        internal const uint DefaultColorBufferSize = DefaultFrameHeight * DefaultFrameWidth * DefaultColorBytesPerPixel;
        internal const uint DefaultDepthBufferSize = DefaultFrameHeight * DefaultFrameWidth * DefaultDepthBytesPerPixel;
        internal const uint DefaultIRBufferSize = DefaultFrameHeight * DefaultFrameWidth * DefaultIRBytesPerPixel;
        internal const uint DefaultBayerBufferSize = DefaultFrameHeight * DefaultFrameWidth * DefaultBayerBytesPerPixel;

        internal const NuiImageResolution DefaultResolution = NuiImageResolution.NUI_IMAGE_RESOLUTION_640x480;

        internal const int MaxSkeletonData = 6;
        internal const bool DefaultNearMode = false;
#endregion
    }
}
