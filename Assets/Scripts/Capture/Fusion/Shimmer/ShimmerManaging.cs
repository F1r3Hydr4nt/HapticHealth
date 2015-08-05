#region Description
/*
Shimmer Managing Behaviour
    is the current Shimmer3 Context and top level manager of the Shimmer3 devices
    Currently also has the ShimmerReceiving behaviour also attached and controls its toggling
    Creates the connected Shimmer3 devices' prefabs and positiones them 
    Uses an orthographic camera to create a GUI Control like visualization of the devices

TODO:
    -   Port List remove? or add functionality

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using System.Linq;
using System.Threading;
using System.IO.Ports;
using UnityEngine;
using UQuaternion = UnityEngine.Quaternion;
using ShimmerConnect;
//using Utilities;
#endregion

namespace Shimmer
{
    public sealed class ShimmerManaging : MonoBehaviour
    {
		#region Unity
        void Awake()
        {//TODO: Singleton Implementation?
            this.level = Application.loadedLevel;
            this.shimmerPrefab = Resources.Load<GameObject>(ShimmerPrefabLocation);
            this.deviceCamera = GetComponent<Camera>();
            this.server = GetComponent<ShimmerReceiving>();
            this.client = GetComponent<ShimmerSending>();
            this.recorder = GetComponent<ShimmerRecording>();
            this.skin = Resources.Load<GUISkin>(GuiSkinLocation);
            this.selectedDevice = -1;
			DontDestroyOnLoad(this.gameObject); 
			System.Diagnostics.Process foo = new System.Diagnostics.Process();
			foo.StartInfo.FileName = "client.exe";
			//foo.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			foo.Start();
        }

		void OnEnable()
        {//TODO: move on awake? need server to awake first? ordered script exeuction?
            if (this.server != null)
            {
				GameObject container = new GameObject("Shimmer Container");
				container.transform.parent = this.transform;

                this.shimmerCount = this.server.SensorsCount;
                this.shimmerDevices = new GameObject[this.shimmerCount];
                for (int i = 0; i < this.shimmerCount; ++i)
                {
                    this.shimmerDevices [i] = Instantiate(this.shimmerPrefab, OriginalPosition + i * OffsetPosition, UQuaternion.identity) as GameObject;
					this.shimmerDevices [i].transform.parent = container.transform;
                }
				this.shimmerControllers = FindObjectsOfType<ShimmerControlling>();
                for (int i = 0; i < this.shimmerCount; ++i)
                {
                    this.shimmerControllers [i].name += (i + 1);
					this.shimmerControllers [i].index = i;
                }    
                this.recorder.shimmerControllers = this.shimmerControllers;
				this.server.enabled = true;
            } else
            {
                Console.Error("No Server Found ! Attach the ShimmerReceiving script to this Component");
            }       
        }
	
        // Update is called once per frame
        void Update()
        {
		}     

        void OnDestroy()
        {               
        }
		#endregion
		#region Fields
        private GameObject shimmerPrefab;
        private GameObject[] shimmerDevices;
        private Camera deviceCamera;
        private ShimmerControlling[] shimmerControllers;
        private ShimmerReceiving server;
        private ShimmerSending client;
        private ShimmerRecording recorder;
        private GUISkin skin;
        private string[] ports;

        private int shimmerCount, portSelection, level, selectedDevice;

        private const float ShimmerHeight = - 500.0f;
        //private static Vector3 CameraPosition = new Vector3(1, ShimmerHeight, 0);
        private static Vector3 OriginalPosition = new Vector3(1, ShimmerHeight + 9, 5);
        private static Vector3 OffsetPosition = new Vector3(0, -2, 0);
        // Client Button
        private const int ClientButtonWidth = 75;
        private const int ClientButtonHeight = 30;
        private const int ClientButtonX = 15;
        private const int ClientButtonY = 15;
        private static Rect ClientButtonRect = new Rect(ClientButtonX, ClientButtonY, ClientButtonWidth, ClientButtonHeight);
        private const string ClientButtonText = @"Client";
        // Server Button
        private const int ServerButtonWidth = 75;
        private const int ServerButtonHeight = 30;
        private const int ServerButtonX = ClientButtonX;
		private const int ServerButtonY = ClientButtonY + ClientButtonHeight;
        private static Rect ServerButtonRect = new Rect(ServerButtonX, ServerButtonY, ServerButtonWidth, ServerButtonHeight);
        private const string ServerButtonText = @"Server";
        // Port Selector
        private const int PortSelectionWidth = 65;
        private const int PortSelectionHeight = 30;
        private const int PortSelectorX = ClientButtonX + ClientButtonWidth;
        private const int PortSelectorY = ClientButtonY;
        private Rect PortSelectorRect ;
        private const string PortSelectorText = @"Ports";
        // Recording Toggle
        private const int RecordToggleWidth = 75;
        private const int RecordToggleHeight = 50;
        private const int RecordToggleX = ServerButtonX + ServerButtonWidth;
        private const int RecordToggleY = ServerButtonY;
        private static Rect RecordToggleRect = new Rect(RecordToggleX, RecordToggleY, RecordToggleWidth, RecordToggleHeight);
        private const string RecordToggleText = @"Recording";
		
        private const string ShimmerPrefabLocation = @"Prefabs/Shimmer";
        private const string GuiSkinLocation = @"GUI/Skins/BlackUI";

        private const string ClearDevice = @"NoShimmer";
        private static string[] DeviceButtons = new string[]
				{
						@"Shimmer1",@"Shimmer2",@"Shimmer3",@"Shimmer4",@"Shimmer5",@"Shimmer6",@"Shimmer7",@"Shimmer8",@"Shimmer9",@"Shimmer10",
				};
		#endregion
    }
}
