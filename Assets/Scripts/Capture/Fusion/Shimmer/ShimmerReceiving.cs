#region Description
/*
Shimmer Receiving Behaviour
    encapsulated the TcpServer that receives the Shimmer3 devices' data
    and passes them to each Shimmer3 controller/prefab

TODO:
    -   Refactor/Impove Data Aquisition?
    -   Start -> OnEnable

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
    */
#endregion
#region Namespaces
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using ShimmerConnect;
//using Utilities;
using AHRS;
#endregion

namespace Shimmer
{
    public sealed class ShimmerReceiving : MonoBehaviour
    {
        #region Properties
        internal int SensorsCount { get; private set; }
        internal SensorData[] LatestData { get; private set; }
        #endregion
        #region Unity
        void Awake()
        {
            this.handler = new Sensor_Handler(Sensor_Handler.ServerType.TYPE_SERVER);          
			this.runThread = new Thread(new ThreadStart(handler.Run))
            {
                IsBackground = true,
                Name = "Shimmer Server",
                Priority = System.Threading.ThreadPriority.AboveNormal
            };
            this.SensorsCount = this.handler.GetNumSensors();
            this.LatestData = new SensorData[this.SensorsCount];
            this.enabled = false;
        }

        void OnEnable()
        {
            if (this.SensorsCount > 0)
            {
                this.shimmerControllers = FindObjectsOfType<ShimmerControlling>();
                this.shimmerExporters = FindObjectsOfType<ShimmerExporting>();
                foreach (var shimmer in this.shimmerControllers)
                {
                    shimmer.enabled = true;
                }
                Console.Log("Found " + this.shimmerControllers.Length + " controllers.");
            } else
            {
                this.enabled = false;
				Console.Log("No Shimmers expected... Disabling !");
            }
        }

        void Start()//TODO: Refactor with enabled?
        {
            this.runThread.Start();
            Console.Log("Server Started !");
        }

        // Update is called once per frame
		int frameNumber = 0;
        void Update()
        {
			// Get new data
			var data = this.handler.GetCurrentSensorData();
            if (data != null)
            {
				// Get datasets
				this.LatestData = data;
            
                for (int i = 0; i < this.SensorsCount; ++i)
                {
                    this.shimmerControllers [i].Data = this.LatestData [i];
                }

				frameNumber++;
            }
			// TODO update more frame (frameNumber below)
            if (this.handler.MaxThroughput)
            {
                for (int i = 0; i < this.SensorsCount; ++i)
                {
                    this.shimmerExporters [i].Data = this.handler [i];
                }
            }

			// Check for impact
			ImpactData impact;
			while( (impact = this.handler.PollImpact ()) != null )
			{
				Console.Important(">> Get an impact frame " + frameNumber + " " + impact.ToString() );
			}
		}
			
		public void OnDisable()//TODO: Refactor with enabled?
		{
			Console.Log("Turn off the server...");
			this.handler.Quit();
			if (this.runThread != null){
				//Console.Log("Thread not null");
				if(this.runThread.IsAlive)
				{
					//Console.Log("Aborting Live Thread ");
					this.runThread.Abort();
				}
			}
			Console.Log("Disable complete.");
		}
		
		// Send a global message
		public void SendToClients( String message ) {
			if(handler != null && message != null)
				handler.SendData( message );
		}

		// Send a specific message to a sensor
		public void SendToSensor( String message, int which ) {
			if(handler != null && message != null){
				handler.SendDataToSensor( message, which );
			}
		}
				
		#endregion
        #region Helpers
        internal void StartRecord()
        {
            this.handler.MaxThroughput = true;
        }

        internal void StopRecord()
        {
            this.handler.MaxThroughput = false;
        }

        internal void ToggleRecord()
        {
            this.handler.MaxThroughput = !this.handler.MaxThroughput;
        }
        #endregion  
        #region Fields
        private Sensor_Handler handler;
        private Thread runThread;
        private ShimmerControlling[] shimmerControllers;
        private ShimmerExporting[] shimmerExporters;
        private const int TerminationTimeout = 2500;
        #endregion
    }
}
