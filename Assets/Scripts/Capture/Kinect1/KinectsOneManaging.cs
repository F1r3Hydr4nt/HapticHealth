#region Description
/*
Kinect Managing Behaviour
    is the current Kinect Context and Top Level Controller
    Allows for Device Selection and Stream Toggling
    Used like a singleton in a way that it persists through scene changes along with the Kinects it creates
    Currently Creates as many Kinects as plugged in when awoken

	TODO:
        -   Custom Kinect Collection Behaviour tailored for our recording needs

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using VclKinectBridge;

using KinectHandle = System.Int32;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed class KinectsOneManaging : MonoBehaviour
    {
		#region Unity
        void Awake()
        {//TODO: Singleton Implementation?
            this.level = Application.loadedLevel;
            // Scene Cameras Init
            this.cameras = new List<Camera>();
            this.maincamera = GetComponent<Camera>();
            this.cameras.Add(maincamera);
            this.recorder = GetComponent<KinectOneRecording>();
            this.minimap = FindObjectOfType<MinimapColorRendering>();
            this.kinectPrefab = Resources.Load<GameObject>(KinectPrefabLocation);
            this.InitKinectContext();
            this.AssignKinectControllers();					
            DontDestroyOnLoad(this.gameObject);
        }

        private void InitKinectContext()
        {
            // get kinect handles of connected system devices
            IntPtr handles = IntPtr.Zero;
            this.kinectsCount = NativeBindings.KinectOpenConnectedDevices(ref handles);
            Console.Log("Found " + this.kinectsCount + " Kinects.");
            this.handles = new KinectHandle[this.kinectsCount];
            Marshal.Copy(handles, this.handles, 0, (int)this.kinectsCount);						
        }

        private void AssignKinectControllers()
        {
            // Scene Kinect Prefabs Init  
            Quaternion prefabCreationRotation = Quaternion.AngleAxis(90, Vector3.up);
            var rotation = Quaternion.identity;
            for (int i = 0; i < this.kinectsCount; ++i)
            {//TODO: assign kinect objects in order to destroy them?// position them in scene around manager object in a 90 degrees array
                rotation *= prefabCreationRotation;
                var kinectObject = Instantiate(this.kinectPrefab, new Vector3(0, 0, 1), rotation) as GameObject;
            }
            // Scene Kinect Controllers Init
            this.kinects = FindObjectsOfType<KinectOneControlling>();
            this.enabled = this.kinects.Where((kinect,i) =>
            {// disable if a device with an invalid handle was created , TODO: check status too?	
                kinect.Handle = i < this.kinectsCount ? this.handles [i] : 0;
                Debugging.Log(kinect.Handle + " Valid ? " + NativeBindings.KinectIsHandleValid(kinect.Handle) + 
                    " Status : " + NativeBindings.KinectGetKinectSensorStatus(kinect.Handle));
                return i < this.kinectsCount && NativeBindings.KinectIsHandleValid(this.handles [i]);
            }).Count() == this.kinectsCount;
						
            foreach (var kinect in this.kinects)
            {// get kinect cameras ( from each prefab created ) 
                var kinectCamera = kinect.GetComponent<Camera>();
                if (kinectCamera != null)
                {
                    this.cameras.Add(kinectCamera);
                }
            }
        }

        void OnLevelWasLoaded(int level)
        {
            if (this.level != level)
            {
                //this.AssignKinectControllers ();
            }
        }

        void OnEnable()
        {
            if (this.maincamera != null)
            {// default main camera position TODO: reevaluate placement
                this.maincamera.transform.position = new Vector3(5.0f, 5.0f, 5.0f);
                this.maincamera.transform.LookAt(new Vector3(0.0f, 0.0f, 0.0f));
                this.minimap.enabled = false;
                this.maincamera.enabled = true;
            }
            if (this.recorder != null)
            {// pass controllers to recording behaviour
                this.recorder.kinects = this.kinects;
                this.recorder.enabled = false;//default not enabled , if streaming toggle enabled
            }
            foreach (var kinect in this.kinects.Where((kinect) => kinect != null && kinect.IsValid))
            {// enable kinect controllers
                kinect.enabled = true;
            }
        }

        void Update()
        {
            if (Input.GetButtonUp(ToggleStreamingButton))
            {
                bool toggle = !this.recorder.enabled;
                this.recorder.enabled = this.kinects.Aggregate(toggle, (seed,kinect) =>
                {
                    kinect.ToggleStream();
                    return seed && kinect.IsStreaming;
                });
            } else
						if (Input.GetButtonUp(StartStreamingButton))
            {
                foreach (var kinect in this.kinects)
                {//TODO: bools for checks?
                    kinect.StreamStart();
                }
                this.recorder.enabled = true;
            } else
						if (Input.GetButtonUp(StopStreamingButton))
            {
                foreach (var kinect in this.kinects)
                {//TODO: bools for checks?
                    kinect.StreamStop();
                }
                this.recorder.enabled = false;

            }
            this.CheckCameraSwitch();	
        }

        private void CheckCameraSwitch()
        {
            var cameraSwitch = CameraButtons.SingleOrDefault((button) => Input.GetButtonUp(button));
            if (!string.IsNullOrEmpty(cameraSwitch))
            {
                Debugging.Log("Button = " + cameraSwitch);
                this.cameras [this.selectedCamera].enabled = false;
                var buttonIndex = CameraButtons.Select((button,i) => new { Button = button , Index = i})
																					.Where((button) => button.Button == cameraSwitch)
																					.FirstOrDefault().Index;
                Debugging.Log("Camera = " + buttonIndex);
                buttonIndex = buttonIndex < this.cameras.Count ? buttonIndex : 0;
                this.cameras [buttonIndex].enabled = true;
                if (this.selectedCamera > 0)
                {
                    this.kinects [this.selectedCamera - 1].IsSelected = false;
                }
                if (buttonIndex > 0)
                {
                    this.kinects [buttonIndex - 1].IsSelected = true;
                }								
                this.selectedCamera = buttonIndex;
                this.minimap.enabled = buttonIndex > 0;
            }
        }
			
        void OnDisable()
        {
            //TODO: Decide On Disabling
        }

        void OnDestroy()
        {//TODO: Maybe on App Quit?
            foreach (KinectHandle handle in this.handles)
            {
                NativeBindings.KinectCloseSensor(handle);
                Debugging.Log("Kinect " + handle + " Closed");
            }
            foreach (var kinect in this.kinects)
            {
                if (kinect != null)
                {
                    DestroyImmediate(kinect.gameObject);
                }
            }
        }
		#endregion
		#region Fields
        private int level, selectedCamera;
        private uint kinectsCount;
        private KinectOneControlling[] kinects;
        private KinectHandle[] handles;
        private GameObject kinectPrefab;
        private Camera maincamera;
        private KinectOneRecording recorder;
        private List<Camera> cameras;
        private MinimapColorRendering minimap;

        private const string ToggleStreamingButton = @"ToggleStream";
        private const string StartStreamingButton = @"StartStream";
        private const string StopStreamingButton = @"StopStream";
        private const string FreeRoamCameraButton = @"RoamCamera";
        private const string Kinect1CameraButton = @"Kinect1";
        private const string Kinect2CameraButton = @"Kinect2";
        private const string Kinect3CameraButton = @"Kinect3";
        private const string Kinect4CameraButton = @"Kinect4";
        private string[] CameraButtons = new string[]
				{
							FreeRoamCameraButton,Kinect1CameraButton,Kinect2CameraButton,Kinect3CameraButton,Kinect4CameraButton
				};
        private const string KinectPrefabLocation = @"Prefabs/KinectSimple";
		#endregion
    }
}
