#region Description
/*
Shimmer Sending Behaviour
    to allow control over the client sending the Shimmer3 devices' data
    Currently only opens the explorer to run the client manually

    TODO:
        -   Hard Coded Paths?
        -   Process/AppDomain implementation?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
    */
#endregion
#region Namespaces
using System.Diagnostics;
using UnityEngine;
using ShimmerConnect;
//using Utilities;
#endregion

namespace Shimmer
{
    public sealed class ShimmerSending : MonoBehaviour //TODO: Maybe OBSOLETE? Run Client Externally better?
    {
        #region Unity
        void Awake()
        {
            this.enabled = false;
        }

        void OnEnable()
        {
            this.process = new Process()
						{ 
							//StartInfo = new ProcessStartInfo(ClientLocation) 
							StartInfo = new ProcessStartInfo("explorer.exe")
							{ 
								Arguments = OpenClientLocation
								//CreateNoWindow = false , 
								//UseShellExecute = false 
							} 
						};
            this.process.Start();//TODO: Refactor with AppDomain?
			Console.Log("Client Started !");//TODO: switch logs
        }

        // Update is called once per frame
        void Update()
        {
	
        }

        void OnDisable()
        {
            //this.process.StandardInput.WriteLine();
            if (this.process != null && !this.process.HasExited)
            {
                UnityEngine.Debug.LogWarning("Stopping Client ....");
                this.process.Close();
                if (!this.process.WaitForExit(TerminationTimeout))
                {
                    this.process.Kill();
                    Console.Log("Client Killed !");
                } else
                {
                    Console.Log("Client Exited OK");
                }
            }
        }
            #endregion
            #region Fields
        private Process process;

        private const int TerminationTimeout = 2500;
        private const string OpenClientLocation = @"D:\WIMU\All-v4\Multi-Shimmer-Module-v4\Assets\ShimmerConnect\ShimmerConnect\bin\Debug";
        private const string ClientLocation = @"D:\WIMU\All-v4\Multi-Shimmer-Module-v4\Assets\ShimmerConnect\ShimmerConnect\bin\Debug\ShimmerConnect.exe";
            #endregion
    }
}
