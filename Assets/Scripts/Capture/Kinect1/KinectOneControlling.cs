#region Description
/*
Kinect Controlling Behaviour
    TopMost Kinect Controller that manages the different Behaviours
        that make a Kinect Device ( prefab ) , behaves like a singleton in a way that persists through scenes
    Each LateUpdate uses the KinectData for visualization ect
        LateUpdate is used for ordering
    Current Behaviours :
        - BodyTracking & SkeletonExporting
        - ScanImageCapturing & ScanImageRecording
        - Settings
    Operations :
        - Stream Toggle
        - Record Toggle
        - Select Device
        - Position Kinect Model
            // when selected only
        - Show Color Video ( at the minimap )
        - Visualize Skeleton ( debug only atm )
        - Show Settings 

	TODO:
        -   Improve Skeleton Visualization?
        -   Depth Video too?
        -   Public Skin/Minimap renderer?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;

using VclKinectBridge;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed class KinectOneControlling : WithKinectHandle
    {
		#region Properties
        internal bool IsSelected
        {
            get { return this.isSelected; }
            set
            {
                this.isSelected = value;
                if (this.colorMinimap != null)
                {
                    this.colorTexture = this.colorMinimap.ColorTexture;
                }
                if (this.settings != null)
                {
                    this.settings.enabled = value;
                }
                this.kinectRenderer.material.color = value ? Color.cyan : this.capturer.enabled //TODO: Refactor colors?
											? Color.green : this.recorder.enabled ? Color.red : Color.black;
            }
        }
        internal bool IsStreaming
        {
            get { return this.capturer.enabled; }
            private set
            {
                if (this.capturer != null)
                {
                    this.capturer.enabled = value;
                    this.kinectRenderer.material.color = value ? Color.green : this.recorder.enabled //TODO: Refactor colors?
												? Color.red : this.IsSelected ? Color.cyan : Color.black;
                }
            }
        }
        internal bool IsTracking
        {
            get { return this.trackingEnabled; }//&& this.tracker.enabled;
            set
            {
                if (this.tracker != null)
                {
                    this.trackingEnabled = value;
                }
            }
        }
        internal bool IsRecording
        {
            get { return this.isRecording; }
            private set
            {
                if (this.recorder != null)
                {
                    this.isRecording = value;
                    this.kinectRenderer.material.color = this.recorder.enabled ? Color.red : this.capturer.enabled ? //TODO: Refactor colors?
                                             Color.green : this.IsSelected ? Color.cyan : Color.black;
                }
            }
        }
		#endregion
		#region Unity
        void Awake()
        {
            // get kinect specific behaviours
            this.capturer = GetComponent<ScanImageCapturing>();
            this.tracker = GetComponent<BodyTracking>();
            this.recorder = GetComponent<ScanImageRecording>();
            this.exporter = GetComponent<SkeletonOneExporting>();
            this.settings = GetComponent<KinectOneSettings>();
            // model renderer
            this.kinectRenderer = GetComponentInChildren<MeshRenderer>();
            // gui minimap video renderer
            this.colorMinimap = FindObjectOfType<MinimapColorRendering>();
            this.skin = Resources.Load<GUISkin>(GuiSkinLocation);
            DontDestroyOnLoad(this.gameObject);
        }

        void OnEnable()
        {
            this.name = KinectOneControlling.Identifier + this.Handle.ToString();
            if (this.IsValid) // pass handle to attached behaviours
            {
                if (this.capturer != null)
                {
                    this.capturer.Handle = this.Handle;
                }
                if (this.recorder != null)
                {
                    this.recorder.Handle = this.Handle;
                }
                if (this.tracker != null)
                {
                    this.tracker.Handle = this.Handle;
                }
            }
        }

        void LateUpdate()
        {
            if (this.IsSelected)
            {
                this.VisualizeColor(); // minimap color video visualization
                this.VisualizeSkeleton();// atm only debug visualize
            }
        }

        new void OnDisable()
        {
            base.OnDisable();//TODO:OBSOLETE?
        }

        void OnDestroy()
        {

        }
		#endregion
		#region Helpers
        internal void StreamStart()
        {
            if (this.capturer != null)
            {
                this.capturer.enabled = true;
            }
            if (this.trackingEnabled)
            {
                this.tracker.enabled = true;
            }
            Console.Log(this.name + " Started Streaming ! " + (this.trackingEnabled && this.tracker.enabled ? "  &Tracking !" : string.Empty));
            this.IsStreaming = true;//TODO: capturer check?
        }

        internal void StreamStop()
        {
            if (this.capturer != null)
            {
                this.capturer.enabled = false;
            }
            if (this.trackingEnabled)
            {
                this.tracker.enabled = false;
            }
            Console.Log(this.name + " Stopped Streaming ! " + (this.trackingEnabled && !this.tracker.enabled ? " &  Tracking !" : string.Empty));
            this.IsStreaming = false;//TODO: capturer check?          
        }

        internal void ToggleStream()
        {
            if (this.capturer != null)
            {
                this.capturer.enabled = !this.capturer.enabled;
            }
            if (this.trackingEnabled)
            {
                this.tracker.enabled = !this.tracker.enabled;
            }
            Console.Log(this.name + (this.capturer.enabled ? " Started " : " Stopped ") + " Streaming ! " 
                + (this.trackingEnabled ? " & Tracking !" : string.Empty));
            this.IsStreaming = !this.IsStreaming;//TODO: capturer check?
        }

        internal void RecordStart(string directory)
        {
            // disabling capturing while recording for performance reasons , TODO: investigate possible enabling
            this.capturer.enabled = false;
            this.capturer.AttachToRecorder();
            // setup recorder
            this.recorder.OutputDirectory = directory;
            this.recorder.Filename = this.name;
            //this.recorder.KinectInformation = new ScanImageZipFile.KinectInfo()
			//{
				//Extrinsics = this.settings.Extrinsics.ToArray(),
				//Intrinsics = this.settings.Intrinsics != null ? this.settings.Intrinsics.ToFloat() : new float[307244],
				//SyncInfo = this.settings.SyncInfo,
				//ScanResolution = ScanImageZipFile.ScanImageResolution.R640x480								
			//};
            this.recorder.enabled = true;
            // setup skeleton exporter
            if (this.IsTracking && this.exporter != null)
            {
                this.exporter.OutputDirectory = directory;
                this.exporter.Filename = this.name;
                this.exporter.enabled = true;
            }
            this.IsRecording = this.recorder.enabled;
        }

        internal void RecordStop()
        {
            this.recorder.enabled = false;
            this.capturer.enabled = true;
            if (this.exporter != null && this.exporter.enabled)
            {
                this.exporter.enabled = false;
            }
            this.IsRecording = this.recorder.enabled;
        }

        internal void SetPosition(Matrix4x4 matrix)//TODO:fix it
        {
            this.gameObject.transform.position = matrix.GetPosition();
            this.gameObject.transform.rotation = matrix.GetUnityRotation();
            this.gameObject.transform.localScale = matrix.GetScale();
        }

        private void VisualizeColor()
        {
            if (this.capturer != null && this.capturer.HasNewFrame && this.colorTexture != null)
            {
                var bytes = this.capturer.Frame.ColorBuffer;
                var colors = new Color32[NativeBindings.DefaultImageSize];
                for (int i = 0,pixel = 0; i < colors.Length; ++i)
                {
                    colors [i].b = bytes [pixel++];
                    colors [i].g = bytes [pixel++];
                    colors [i].r = bytes [pixel++];
                    colors [i].a = bytes [pixel++];
                }
                this.colorTexture.SetPixels32(colors);
                this.colorTexture.Apply();
            }
        }
        private void VisualizeSkeleton()
        {
            if (this.tracker != null && this.tracker.HasNewFrame)
            {
                var skeleton = this.tracker.Skeleton;
                SkeletonHelp.DebugSkeleton(skeleton, this.settings.Extrinsics);
            }
        }
		#endregion
		#region Fields
        private ScanImageCapturing capturer;
        private BodyTracking tracker;
        private ScanImageRecording recorder;
        private SkeletonOneExporting exporter;
        private MinimapColorRendering colorMinimap;
        private KinectOneSettings settings;
        private Renderer kinectRenderer;
        private Texture2D colorTexture;

        private GUISkin skin;

        private bool trackingEnabled, isSelected, isRecording;

        private static Vector3 ColorVideoPosition = new Vector3(0f, 0f, 50f);
        private static Quaternion ColorVideoRotation = Quaternion.identity;
        private const string Identifier = @"Kinect";
        private const string GuiSkinLocation = @"GUI/Skins/BlackUI";//TODO: global gui set? or editor settable through public?
        private const string ColorVideoPrefabLocation = @"Prefabs/ColorVideo";
		#endregion	
    }
}
