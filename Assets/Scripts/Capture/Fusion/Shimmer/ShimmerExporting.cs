#region Description
/*
Shimmer Exporting Behaviour
    passed the Data Received by the server to the different exporters
    uses LateUpdate for ordering

TODO:

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
    */
#endregion
#region Namespaces
using System;
using UnityEngine;
using ShimmerConnect;
//using Utilities;
#endregion

namespace Shimmer
{
    public sealed class ShimmerExporting : MonoBehaviour
    {	
        #region Properties
        //TODO: IDs?Everywhere?
        internal SensorData[] Data { private get; set; }
        internal string OutputDirectory{ get; set; }
        internal string Filename{ get; set; }
        internal int ID { get; set; }
        #endregion
        #region Unity
        void Awake()
        {
           
        }

        void OnEnable()
        {
            //this.exporter = new ShimmerTextExporter();
            // TODO:Refactor Timing?
            this.initialTimestamp = 0.0;
            this.count = 0;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            int length = 0;
            if (this.Data != null && (length = this.Data.Length) > 0)
            {
                // TODO:Refactor Timing?
                if (this.initialTimestamp <= double.Epsilon)
                {
                    this.initialTimestamp = this.Data [0].GetTimeStamp();
                }
                count += length;
                this.latestTimestamp = this.Data [length - 1].GetTimeStamp();
                DateTime now = DateTime.UtcNow;
                //this.exporter.AddMultipleData(this.Data);
                Console.Log("Got " + this.Data.Length + " Data with elapsed = " + (DateTime.UtcNow - now).Milliseconds);
            }
        }

        void OnDisable()
        {
            //this.exporter.Export(this.OutputDirectory, this.Filename);
            this.Data = null;
            // TODO:Refactor Timing?
            double elapsed = this.latestTimestamp - this.initialTimestamp;
            Console.Log("Done Exporting Elapsed = " + elapsed + " ( written " + this.count + " avg ms = " + elapsed / count);
        }
        #endregion
        #region Fields
        //private ShimmerTextExporter exporter;

        private double initialTimestamp, latestTimestamp;
        private int count;
        #endregion
    }
}
