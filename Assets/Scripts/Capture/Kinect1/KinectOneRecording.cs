#region Description
/*	
Kinect Recording Behaviour
    a recording manager allowing for a centralized point of control

    TODO:
        -   Implement Toggling

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed class KinectOneRecording : ModalityRecording
    {
		#region Unity
        protected override void Awake()
        {
            base.Awake();
        }

        void OnEnable()
        {
            Console.Log("Kinect Recording Enabled ! ( at " + this.OutputDirectory + " )");			
        }

        // Use this for initialization
        void Start()
        {
	
        }
	
        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonUp(StartRecordingButton))
            {
                foreach (var kinect in this.kinects)
                {
                    kinect.RecordStart(this.OutputDirectory);
                }
                Console.Log("Recording Started!");
            }
            if (Input.GetButtonUp(StopRecordingButton))
            {
                foreach (var kinect in this.kinects)
                {
                    kinect.RecordStop();
                }
                Console.Log("Recording Stopped!");
            }
        }

        void OnDisable()
        {
            Console.Log("Kinect Recording Disabled !");
        }
		#endregion
		#region Helpers
        internal override void StartRecord()
        {
            foreach (var kinect in this.kinects)
            {
                kinect.RecordStart(this.OutputDirectory);
            }
            this.IsRecording = true;
            Console.Log("Recording Started!");
        }

        internal override void StopRecord()
        {
            foreach (var kinect in this.kinects)
            {
                kinect.RecordStop();
            }
            this.IsRecording = false;
            Console.Log("Recording Stopped!");
        }

        internal override void ToggleRecord()
        {
            throw new NotImplementedException("Kinect Recording Toggle not yet implemented !");
        }
		#endregion
		#region Fields
        internal KinectOneControlling[] kinects;//TODO: Expose through prop ? Or Public ?

        private const string ToggleRecordingButton = @"ToggleRecord";
        private const string StartRecordingButton = @"StartRecord";
        private const string StopRecordingButton = @"StopRecord";
		#endregion
    }
}
