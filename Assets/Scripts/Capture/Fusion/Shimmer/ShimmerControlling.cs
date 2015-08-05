#region Description
/*
Shimmer Controlling Behaviour
        top level Shimmer3 controller that controls the different behaviours
        that make up a Shimmer3 prefab , behaves like a singleton in a way that persists through scenes
    Each LateUpdate uses the Data provides by the sensor to rotate its model
    Current Behaviours :
        - ShimmerExporting
        - ShimmerSettings
    Operations :
        - Toggle Recording/Exporting


TODO:
    -   Correct Axes investigate

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
    */
#endregion
#region Namespaces
using UnityEngine;
using UQuaternion = UnityEngine.Quaternion;
using ShimmerConnect;
//using Utilities;
#endregion

namespace Shimmer
{
    public sealed class ShimmerControlling : MonoBehaviour
    {
        #region Properties
        internal bool IsSelected
        {
            get { return this.isSelected;}
            set
            {
                this.isSelected = value;
                this.settings.enabled = value;
                this.shimmerRenderer.material.color = value ? Color.green : this.exporter.enabled //TODO: Refactor colors?
									? Color.red : Color.gray;
            }
        }
        internal SensorData Data { get; set; }
        internal string TrackingID { get; set; }
        #endregion
        #region Unity
        void Awake()
        {
            this.name = ShimmerIdentifier;
            this.exporter = GetComponent<ShimmerExporting>();
            this.settings = GetComponent<ShimmerSettings>();
            this.shimmerRenderer = GetComponent<MeshRenderer>();
            this.conversionRotation = UQuaternion.AngleAxis(90, Vector3.forward);//* UQuaternion.AngleAxis(90, Vector3.forward);
            //UQuaternion.AngleAxis(180, Vector3.up);
            DontDestroyOnLoad(this.gameObject);
        }

        void OnEnable()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
			if (this.Data != null)
            {
                var quaternion = this.Data.GetQuaternion();
				var acceleration = this.Data.GetAccel();
                double timestamp = this.Data.GetTimeStamp();
                if (this.previousTimestamp < double.Epsilon)
                {
                    this.previousTimestamp = timestamp;
                }
                
				// Pb sometimes here
				if( !double.IsNaN(quaternion[0]) ) {
					//Console.Log(this.Data.GetPortName() + " " + timestamp + " " + quaternion [0] + " " + quaternion [1] + " " + quaternion [2] + " " + quaternion [3]);
	                this.transform.rotation = new UQuaternion((float)quaternion [1], (float)quaternion [2], (float)quaternion [3], (float)quaternion [0]);
	                this.transform.rotation *= this.conversionRotation;
				}
				//Console.Log("Elapsed " + (timestamp - previousTimestamp) );
				this.previousTimestamp = timestamp;
			}
		}
		#endregion
		#region Helpers
        internal void StartRecord(string directory)
        {
            this.exporter.enabled = true;
            this.exporter.OutputDirectory = directory;
            this.exporter.Filename = this.name;
        }

        internal void StopRecord()
        {
            this.exporter.enabled = false;
        }

        internal void ToggleRecord(string directory)
        {
            this.exporter.OutputDirectory = directory;
            this.exporter.Filename = this.name;
            this.exporter.enabled = !this.exporter.enabled;
        }
        #endregion
        #region Fields
		public int index;
        private ShimmerExporting exporter;
        private ShimmerSettings settings;
        private MeshRenderer shimmerRenderer;
        private bool isSelected, isRecording;

        private UQuaternion conversionRotation;
        private double previousTimestamp;

        private const string ShimmerIdentifier = @"Shimmer";
        #endregion
    }
}
