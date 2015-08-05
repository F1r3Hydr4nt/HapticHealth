#region Description
/*
Modality Managing Behaviour
    a top level manager that controls the different modalities
    Currently Toggles Kinect/Shimmer scenes to load their respective contexts

	TODO:

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;
//using UnityEditor;
using System.IO;
//using VclUnityKinect;
using Shimmer;
//using Utilities;
#endregion
public class ModalityManager : MonoBehaviour
{
	#region Unity
    void Awake()
    {
        this.skin = Resources.Load <GUISkin>(GuiSkinLocation);	
        this.recorder = GetComponent<RecordingManaging>();
        this.InitLogo();	
        DontDestroyOnLoad(this.gameObject);
    }

    void InitLogo()
    {
        int width = Screen.width;
        int height = Screen.height;
        this.KinectX = (int)(width * 0.35);
        this.ShimmerWidth = this.KinectWidth = (int)(width * 0.1);
        this.ShimmerHeight = this.KinectHeight = (int)(height * 0.1);
        this.ShimmerY = this.KinectY = 5;
        this.ShimmerX = (int)(width * 0.55);
        this.KinectRect = new Rect(this.KinectX, this.KinectY, this.KinectWidth, this.KinectHeight);
        this.ShimmerRect = new Rect(this.ShimmerX, this.ShimmerY, this.ShimmerWidth, this.ShimmerHeight);
    }

    void OnLevelLoaded(int level)
    {
        this.InitLogo();
    }

    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
        if (this.asyncOperation != null && !this.asyncOperation.isDone)
        {
            Console.Log(this.asyncOperation.progress * 100 + "%");
        }
    }

    void OnGUI()
    {
        GUI.skin = this.skin;
        if (!this.kinectContextLoaded && GUI.Button(KinectRect, KinectText))
        {
            this.asyncOperation = Application.LoadLevelAsync("kinects");
            this.kinectContextLoaded = true;
        }
        if (!this.shimmerContextLoaded && GUI.Button(ShimmerRect, ShimmerText))
        {
            this.asyncOperation = Application.LoadLevelAsync("wimus");
            this.shimmerContextLoaded = true;
        }
        if (this.kinectContextLoaded || this.shimmerContextLoaded)
        {
            this.recorder.CanRecord = true;
        }
    }

    void OnDestroy()
    {

    }
	#endregion
	#region Fields
    private RecordingManaging recorder;
    private GUISkin skin;
    private AsyncOperation asyncOperation;

    private int KinectX, KinectY, KinectWidth, KinectHeight, ShimmerX, ShimmerY, ShimmerWidth, ShimmerHeight;
    private Rect KinectRect, ShimmerRect;
    private bool kinectContextLoaded, shimmerContextLoaded;

    private const string KinectText = @"Kinect";
    private const string ShimmerText = @"Shimmer";

    private const string GuiSkinLocation = @"GUI/Skins/BlackUI";
	#endregion
}
