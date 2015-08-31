#region Description
/*
Kinect Settings Behaviour
    Controls each Kinect device's settings and properties
    Shows GUI when enabled
    Current Options :
        -   Load Intrinsics from folder
        -   Load Extrinsics from file
        -   Rename Kinect 
        -   Toggle Skeleton Tracking

	TODO:
        - Expand uses?
        - Hard Coded Default Locations

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

using Intrinsics;
//using ScanImageZipFile;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed  class KinectOneSettings : MonoBehaviour
    {
		#region Properties
        public KinectIntrinsics Intrinsics { get; private set; }
        public Matrix4x4 Extrinsics { get; private set; }
        //public SynchronizationInfo SyncInfo { get; private set; }
		#endregion
		#region Unity
        void Awake()
        {
            this.kinect = GetComponent<KinectOneControlling>();
            this.skin = Resources.Load<GUISkin>(GuiSkinLocation);
            //this.SyncInfo = new SynchronizationInfo(0UL);
            this.Extrinsics = Matrix4x4.identity;
        }

        void OnEnable()
        {
            this.enabled = this.kinect != null;
        }

        void OnGUI()
        {
            GUI.skin = this.skin;
            GUI.Box(GroupRect, GroupTitle);
            GUI.BeginGroup(GroupRect);
            this.kinect.IsTracking = GUI.Toggle(TrackingRect, this.kinect.IsTracking, TrackingToggleText);
            GUI.Label(LabelRect, LabelText);
            this.kinect.name = GUI.TextField(TextRect, this.kinect.name, NameMaxLength);
            if (GUI.Button(IntrinsicsRect, IntrinsicsLoadText))
            {
                LoadIntrinsics(EditorUtility.OpenFolderPanel(IntrinsicsFolderSelectText, IntrinsicsDefaultLocation, this.kinect.name));
            }
            if (GUI.Button(ExtrinsicsRect, ExtrinsicsLoadText))
            {
                LoadExtrinsics(EditorUtility.OpenFilePanel(ExtrinsicsSelectText, ExtrinsicsDefaultLocation, ExtrinsicsFileExtension));
            }
            GUI.EndGroup();

        }

        void OnDisable()
        {

        }
		#endregion
		#region Helpers
        void LoadIntrinsics(string folder)
        {
            if (!string.IsNullOrEmpty(folder))
            {
                this.Intrinsics = KinectIntrinsicsProvider.GetKinectIntrinsics(folder);
            }
        }

        void LoadExtrinsics(string file)
        {
            if (!string.IsNullOrEmpty(file))
            {
                this.Extrinsics = File.ReadAllLines(file)
									.SelectMany((line) => line.Split(new char[]{ ' ' }, StringSplitOptions.RemoveEmptyEntries)
									.Select((parsee) => float.Parse(parsee, System.Globalization.NumberStyles.Float)).ToArray())
									.ToArray().ToMatrix();
                Console.Log("Extrinsics Loaded for " + this.kinect.name + " . ( from " + file + " )");
                this.kinect.SetPosition(this.Extrinsics);
            }
        }
		#endregion
		#region Fields
        private GUISkin skin;
        private KinectOneControlling kinect;

        private const string GuiSkinLocation = @"GUI/Skins/BlackUI";
        // group panel
        private const int GroupWidth = 300;
        private const int GroupHeight = 450;
        private static Rect GroupRect = new Rect(Screen.width - GroupWidth, Screen.height - GroupHeight, GroupWidth, GroupHeight);
        private static Rect BoxRect = new Rect(Screen.width - GroupWidth - 5, Screen.height - GroupHeight - 5, GroupWidth - 10, GroupHeight - 10);
        private const string GroupTitle = @"Kinect Settings";
        //	name field
        private const int LabelWidth = 50;
        private const int LabelHeight = 30;
        private const string LabelText = @"Name : ";
        private const int TextWidth = 125;
        private const int TextHeight = 30;
        private const int LabelX = 10;
        private const int LabelY = 25;
        private static Rect LabelRect = new Rect(LabelX, LabelY, LabelWidth, LabelHeight);
        private const int TextX = LabelX + LabelWidth;
        private const int TextY = LabelY;
        private static Rect TextRect = new Rect(TextX, TextY, TextWidth, TextHeight);
        private const int NameMaxLength = 15;
        // tracking toggle
        private const int TrackingToggleWidth = 100;
        private const int TrackingToggleHeight = 30;
        private const int TrackingToggleX = LabelX;
        private const int TrackingToggleY = LabelY + LabelHeight;
        private static Rect TrackingRect = new Rect(TrackingToggleX, TrackingToggleY, TrackingToggleWidth, TrackingToggleHeight);
        private const string TrackingToggleText = @"Skeleton Tracking";
        // intrinsics load 
        private const int IntrinsicsLoadWidth = 125;
        private const int IntrinsicsLoadHeight = 70;
        private const int IntrinsicsX = LabelX;
        private const int IntrinsicsY = TrackingToggleY + TrackingToggleHeight;
        private static Rect IntrinsicsRect = new Rect(IntrinsicsX, IntrinsicsY, IntrinsicsLoadWidth, IntrinsicsLoadHeight);
        private const string IntrinsicsLoadText = @"Intrinsics";
        private const string IntrinsicsFolderSelectText = @"Select Intrinsics Folder ...";
        private const string IntrinsicsDefaultLocation = @"D:\Data\KinectIntrinsics";
        // extrinsics load 
        private const int ExtrinsicsLoadWidth = 125;
        private const int ExtrinsicsLoadHeight = 70;
        private const int ExtrinsicsX = IntrinsicsX + IntrinsicsLoadWidth;
        private const int ExtrinsicsY = IntrinsicsY;
        private static Rect ExtrinsicsRect = new Rect(ExtrinsicsX, ExtrinsicsY, ExtrinsicsLoadWidth, ExtrinsicsLoadHeight);
        private const string ExtrinsicsLoadText = @"Extrinsics";
        private const string ExtrinsicsSelectText = @"Select Extrinsics File ...";
        private const string ExtrinsicsDefaultLocation = @"D:\Data\KinectExtrinsics";
        private const string ExtrinsicsFileExtension = @"extrinsics";				
		#endregion	
    }
}