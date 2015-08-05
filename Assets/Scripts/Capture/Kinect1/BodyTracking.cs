#region Description
/*
Kinect Body Tracking Behaviour
    Each Update Polls for New Skeleton Frame and Aquires it
    Exposes the Skeleton Frame through Skeleton Property 
    By Default Disabled

Switch Tracking On/Off by enabling/disabling

	TODO:
        -   Improve SkeletonFrame Property Choice
        -   Investigate better Skeleton parameters?
        -   Allow for skeleton parameter selection/modification?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using VclKinectBridge;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed class BodyTracking : WithKinectHandle
    {		
		#region Properties
        internal bool HasNewFrame { get; private set; }

        //public List<SkeletonFrame> Skeletons
        public SkeletonFrame Skeleton
        {
            get
            {
                ulong timestamp = (ulong)this.skeletonFrame.TimeStamp;
                return this.skeletonFrame.SkeletonData
										.Where((data) => data.UserIndex > 0)
										.Select((data) => new SkeletonFrame(data, timestamp))
										.FirstOrDefault();
                //.ToList ();
            }
        }
		#endregion
		#region Unity
        void Awake()
        {//Disabled by Default
            this.enabled = false;
        }

        void OnEnable()
        {
            if (!this.IsValid)
            {
                this.enabled = false;
            } else
            {
                //NativeBindings.KinectEnableSkeletonStream (this.Handle, DefaultNearMode, KinectSkeletonSelectionMode.SkeletonSelectionModeDefault,ref IntPtr.Zero);
                NativeBindings.KinectEnableSkeletonStream(this.Handle, NativeBindings.DefaultNearMode, KinectSkeletonSelectionMode.SkeletonSelectionModeDefault,
				                                           ref this.DefaultSkeletonSmooth);
                this.enabled = (NativeBindings.KinectStartSkeletonStream(this.Handle) == SuccessCheck);
            }
        }
	
        void Update()
        {
            DateTime now = DateTime.UtcNow;
            this.HasNewFrame = NativeBindings.KinectIsSkeletonFrameReady(this.Handle) &&
                NativeBindings.KinectGetSkeletonFrame(this.Handle, ref this.skeletonFrame) == SuccessCheck;
            Debugging.Log(this.Handle + " New SKELETON Frame ? " + this.HasNewFrame + " Time " + this.skeletonFrame.TimeStamp + " elapsed " + (DateTime.UtcNow - now).Milliseconds.ToString());
        }

        void OnDestroy()
        {
            //sensors get disposed by the manager script
        }
		#endregion
		#region Fields
        private NuiSkeletonFrame skeletonFrame = new NuiSkeletonFrame() { SkeletonData = new NuiSkeletonData[NativeBindings.MaxSkeletonData] };	
        private  NuiTransformSmoothParameters DefaultSkeletonSmooth = new NuiTransformSmoothParameters()
				{
					Smoothing = 0.5f,
					Correction = 0.5f,
					Prediction = 0.5f,
					JitterRadius = 0.05f,
					MaxDeviationRadius = 0.04f
				};
		#endregion	
    }
}
