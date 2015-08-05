#region Description
/*
Shimmer Recording Behaviour
    provides a centralized point of control for the Shimmer3 devices' recording

    TODO:
        -   

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using System.IO;
using UnityEngine;
//using Utilities;
#endregion

namespace Shimmer
{
    public sealed class ShimmerRecording : ModalityRecording
    {
		#region Unity
        protected override void Awake()
        {
            base.Awake();
            this.server = GetComponent<ShimmerReceiving>();
        }

        void OnEnable()
        {
            Console.Log("Shimmer Recording Enabled ! ( at " + this.OutputDirectory + " )");
        }

        void OnDisable()
        {
            Console.Log("Shimmer Recording Disabled !");
        }
		#endregion
		#region Helpers
        internal override void StartRecord()
        {
            foreach (var shimmer in this.shimmerControllers)
            {
                shimmer.StartRecord(this.OutputDirectory);
            }
            this.server.StartRecord();
            this.IsRecording = true;
        }

        internal override void StopRecord()
        {
            foreach (var shimmer in this.shimmerControllers)
            {
                shimmer.StopRecord();
            }
            this.server.StopRecord();
            this.IsRecording = false;
        }

        internal override void ToggleRecord()
        {
            foreach (var shimmer in this.shimmerControllers)
            {
                shimmer.ToggleRecord(this.OutputDirectory);
            }
            this.server.ToggleRecord();
            this.IsRecording = ! this.IsRecording;
        }
		#endregion	
		#region Fields
        private ShimmerReceiving server;
        internal ShimmerControlling[] shimmerControllers;
		#endregion
    }
}
