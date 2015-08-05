/*	
Microsoft Kinect SDK v1.8 Enumerations
MS OpenTech Kinect Common Bridge Enumerations
ScanZipImageFile Enumerations

TODO:

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/

namespace VclKinectBridge
{
    public enum NuiImageResolution : int
    {
        NUI_IMAGE_RESOLUTION_INVALID	= -1,
        NUI_IMAGE_RESOLUTION_80x60		= 	0,
        NUI_IMAGE_RESOLUTION_320x240	= 1 ,
        NUI_IMAGE_RESOLUTION_640x480	= 2 ,
        NUI_IMAGE_RESOLUTION_1280x960	= 3 
    }
	
    public enum KinectSkeletonSelectionMode : int
    {
        SkeletonSelectionModeDefault    = 0,
        SkeletonSelectionModeClosest1   = 1,
        SkeletonSelectionModeClosest2   = 2,
        SkeletonSelectionModeSticky1    = 3,
        SkeletonSelectionModeSticky2    = 4,
        SkeletonSelectionModeActive1    = 5,
        SkeletonSelectionModeActive2    = 6,
    } 
	
    public enum NuiSkeletonTrackingState : int
    {
        NotTracked = 0,
        PositionOnly,
        SkeletonTracked
    }
	
    public enum NuiSkeletonPositionTrackingState : int
    {
        NotTracked = 0,
        Inferred,
        Tracked
    }
	
    public enum NuiSkeletonPositionIndex : int
    {
        HipCenter = 0,
        Spine,
        ShoulderCenter,
        Head,
        ShoulderLeft,
        ElbowLeft,
        WristLeft,
        HandLeft,
        ShoulderRight,
        ElbowRight,
        WristRight,
        HandRight,
        HipLeft,
        KneeLeft,
        AnkleLeft,
        FootLeft,
        HipRight,
        KneeRight,
        AnkleRight,
        FootRight,
        Count
    }

    public enum ScanImageCompression : int
    {
        NoCompression 	= 0,
        Default		 			= 1,
    }

    // KinectSensor Device Status
    public enum KinectSensorStatus : int
    {
        // This NuiSensorChooser has a connected and started sensor.
        KinectSensorStatusNone                      = 0,
        /// <summary>
        /// This NuiSensorChooser has a connected and started sensor.
        /// </summary>
        KinectSensorStatusStarted                   = 1,
        /// <summary>
        /// The available sensor is not powered.  If it receives power we
        /// will try to use it automatically.
        /// </summary>
        KinectSensorStatusNotPowered                = 2,
        /// <summary>
        /// There is not enough bandwidth on the USB controller available
        /// for this sensor. Can recover in some cases
        /// </summary>
        KinectSensorStatusInsufficientBandwidth     = 3,
        /// <summary>
        /// Available sensor is in use by another application.
        /// Will recover once the other application releases its sensor
        /// </summary>
        KinectSensorStatusConflict                  = 4,
        /// <summary>
        /// Don't have a sensor yet, a sensor is initializing, you may not get it
        /// Can't trust the state of the sensor yet
        /// </summary>
        KinectSensorStatusInitializing               = 5,
        /// <summary>
        /// Available sensor is not genuine.
        /// </summary>
        KinectSensorStatusNotGenuine                = 6,
        /// <summary>
        /// Available sensor is not supported
        /// </summary>
        KinectSensorStatusNotSupported              = 7,
        /// <summary>
        /// Available sensor has an error
        /// </summary>
        KinectSensorStatusError                     = 8,
    }

    public enum KinectStreamStatus
    {
        KinectStreamStatusError     = 0,
        KinectStreamStatusEnabled   = 1,
        KinectStreamStatusDisabled  = 2
    } 
}
