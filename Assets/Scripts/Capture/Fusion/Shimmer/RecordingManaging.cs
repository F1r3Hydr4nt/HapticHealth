#region Description
/*
Recording Manager Behaviour
    a manager for all the modalities recording,
    provides a centralized point of control and the GUI to control the recording

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
public sealed class RecordingManaging : ModalityRecording
{
	#region Properties
    internal bool CanRecord { private get; set; }
	#endregion
	#region Unity
    protected override void Awake()
	{		
        base.Awake();
        //this.kinect = FindObjectOfType<KinectRecording>();
        this.shimmer = FindObjectOfType<ShimmerRecording>();
        this.skin = Resources.Load<GUISkin>(GuiSkinLocation);
        GroupRect = new Rect(Screen.width - GroupWidth, GroupY, GroupWidth, GroupHeight);
    }

    void OnLevelWasLoaded(int level)
    {
        //this.kinect = FindObjectOfType<KinectRecording>();
        this.shimmer = FindObjectOfType<ShimmerRecording>();
        this.enabled = /*this.kinect != null ||*/ this.shimmer != null;
        this.inRecordLevel = Application.loadedLevelName == "record";
    }

    // Use this for initialization
    void Start()
    {
        this.enabled = /*this.kinect != null ||*/ this.shimmer != null;
    }
	
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp(StartRecordingButton))
        {
            this.StartRecord();
        } else
				if (Input.GetButtonUp(StopRecordingButton))
        {
            this.StopRecord();
        } else
				if (Input.GetButtonUp(ToggleRecordingButton))
        {
            this.ToggleRecord();
        }
    }

    void OnGUI()
    {
        GUI.skin = this.skin;
        GUI.BeginGroup(GroupRect);
        GUI.Label(RecordToggleRect, RecordToggleText);
        /*if (this.kinect != null)
        {
            this.kinect.enabled = GUI.Toggle(KinectToggleRect, this.kinect.enabled, KinectToggleText);
        }*/
        if (this.shimmer != null)
        {
            this.shimmer.enabled = GUI.Toggle(ShimmerToggleRect, this.shimmer.enabled, ShimmerToggleText);
        }
		/*
        if (GUI.Button(OutputSelectRect, OutputSelectText))
        {
            //this.OutputDirectory = EditorUtility.OpenFolderPanel(OutputSelectFolderDialogText, 
			//								Path.GetPathRoot(System.Environment.CurrentDirectory), string.Empty);
            if (!string.IsNullOrEmpty(this.OutputDirectory))
            {
                this.SetupOutputDirectories();
            }					
        }	
        */
        if (this.CanRecord && !this.inRecordLevel && GUI.Button(RecordGoRect, RecordGoText))
        {
            Application.LoadLevelAsync("record");
        }
        GUI.EndGroup();
    }
	#endregion
	#region Helpers
    internal override void StartRecord()
    {
        /*if (this.kinect != null && this.kinect.enabled && !this.kinect.IsRecording)
        {
            this.kinect.StartRecord();
        }*/
        if (this.shimmer != null && this.shimmer.enabled && !this.shimmer.IsRecording)
        {
            this.shimmer.StartRecord();
        }
    }

    internal override void StopRecord()
    {
        /*if (this.kinect != null && this.kinect.enabled && this.kinect.IsRecording)
        {
            this.kinect.StopRecord();
        }*/
        if (this.shimmer != null && this.shimmer.enabled && this.shimmer.IsRecording)
        {
            this.shimmer.StopRecord();
        }
    }

    internal override void ToggleRecord()
    {
        /*if (this.kinect != null && this.kinect.enabled)
        {
            this.kinect.ToggleRecord();
        }*/
        if (this.shimmer != null && this.shimmer.enabled)
        {
            this.shimmer.ToggleRecord();
        }
    }

    private void SetupOutputDirectories()
    {
        //this.kinect.OutputDirectory = this.OutputDirectory;
        this.shimmer.OutputDirectory = this.OutputDirectory;
    }
	#endregion
	#region Fields
    //private KinectRecording kinect;
    private ShimmerRecording shimmer;
    private GUISkin skin;
    private bool inRecordLevel;
    // group
    private static Rect GroupRect;
    private const int GroupWidth = 350;
    private const int GroupHeight = 90;
    private const int GroupX = 150;
    private const int GroupY = 5;
    // go to record 
    private const int RecordGoWidth = 75;
    private const int RecordGoHeight = 40;
    private const int RecordGoX = RecordToggleX + RecordToggleWidth;
    private const int RecordGoY = 5;
    private static Rect RecordGoRect = new Rect(RecordGoX, RecordGoY, RecordGoWidth, RecordGoHeight);
    private const string RecordGoText = @"Record";
    // output select 
    private const int OutputSelectWidth = 75;
    private const int OutputSelectHeight = 40;
    private const int OutputSelectX = RecordToggleX + RecordToggleWidth;
    private const int OutputSelectY = 5 + RecordGoHeight;
    private static Rect OutputSelectRect = new Rect(OutputSelectX, OutputSelectY, OutputSelectWidth, OutputSelectHeight);
    private const string OutputSelectText = @"Output";
    private const string OutputSelectFolderDialogText = @"Select Output Folder ...";
    // record label
    private const int RecordToggleWidth = 250;
    private const int RecordToggleHeight = 30;
    private const int RecordToggleX = 5 ;
    private const int RecordToggleY = 5;
    private static Rect RecordToggleRect = new Rect(RecordToggleX, RecordToggleY, RecordToggleWidth, RecordToggleHeight);
    private const string RecordToggleText = @"Toggle Modality Recording";
    // kinect toggle
    private const int KinectToggleWidth = 50;
    private const int KinectToggleHeight = 30;
    private const int KinectToggleX = RecordToggleX;
    private const int KinectToggleY = RecordToggleY + RecordToggleHeight;
    private static Rect KinectToggleRect = new Rect(KinectToggleX, KinectToggleY, KinectToggleWidth, KinectToggleHeight);
    private const string KinectToggleText = @"Kinect";
    // shimmer toggle
    private const int ShimmerToggleWidth = KinectToggleWidth;
    private const int ShimmerToggleHeight = KinectToggleHeight;
    private const int ShimmerToggleX = RecordToggleX;
    private const int ShimmerToggleY = KinectToggleY + KinectToggleHeight ;
    private static Rect ShimmerToggleRect = new Rect(ShimmerToggleX, ShimmerToggleY, ShimmerToggleWidth, ShimmerToggleHeight);
    private const string ShimmerToggleText = @"Shimmer";

    private const string GuiSkinLocation = @"GUI/Skins/BlackUI";//TODO: global gui set? or editor settable through public?
    private const string ToggleRecordingButton = @"ToggleRecord";
    private const string StartRecordingButton = @"StartRecord";
    private const string StopRecordingButton = @"StopRecord";
	#endregion
}
