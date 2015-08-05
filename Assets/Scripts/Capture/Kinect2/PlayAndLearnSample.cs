#define KEYBOARD_INPUT
#define GUI_INPUT

#region Description
/* 		Sample Script to test Single Kinect 2 Capture & Compare
 * 		Use the above defined to include keyboard and/or gui inputs
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Kinect2.Local;
using Kinect2.IO;
using Compare.Utilities;
#endregion

[RequireComponent(typeof(GUILayer))]
public class PlayAndLearnSample : MonoBehaviour 
{
	void Awake()
	{
		//this.controller = this.GetComponent<Kinect2Controlling>();
		this.capturer = this.GetComponent<Capture_module>();	
		this.comparer = FindObjectOfType<Compare_module>();
	}

	void Start () 
	{
		this.enabled = this.capturer != null;
		_activities = new string[]
		{
			Properties.ActivityToString(Properties.Activity.HB_LHSS),
			Properties.ActivityToString(Properties.Activity.HB_RHUS),
			Properties.ActivityToString(Properties.Activity.HB_RHV),
			Properties.ActivityToString(Properties.Activity.P_S),
			Properties.ActivityToString(Properties.Activity.P_SASS),
			Properties.ActivityToString(Properties.Activity.J_BHS),
			Properties.ActivityToString(Properties.Activity.J_SAS),
			Properties.ActivityToString(Properties.Activity.GF_PK),
			Properties.ActivityToString(Properties.Activity.H_SFTH)
		};
		this.comparer.DrawPlots = false;

		// Reference Skeleton
		
		GameObject obj_i = GameObject.Find("Bip001 L Thigh001");
		skeleton_reference.Add(obj_i);
		
		obj_i = GameObject.Find("Bip001 L Calf001");
		skeleton_reference.Add(obj_i);
		
		obj_i = GameObject.Find("Bip001 R Thigh001");
		skeleton_reference.Add(obj_i);

		obj_i = GameObject.Find("Bip001 R Calf001");
		skeleton_reference.Add(obj_i);

		obj_i = GameObject.Find("Bip001 L UpperArm001");
		skeleton_reference.Add(obj_i);

		obj_i = GameObject.Find("Bip001 L Forearm001");
		skeleton_reference.Add(obj_i);

		obj_i = GameObject.Find("Bip001 R UpperArm001");
		skeleton_reference.Add(obj_i);

		obj_i = GameObject.Find("Bip001 R Forearm001");
		skeleton_reference.Add(obj_i);

		obj_i = GameObject.Find("Bip001 Spine001");	
		skeleton_reference.Add(obj_i);
		
		obj_i = GameObject.Find("skeleton");
		skeleton_reference.Add(obj_i);
	}

	void OnEnable()
	{
		this.capturer.Initialise_capture(Device.SINGLE_KINECT_2);						
	}

	void Update ()
	{
		#if KEYBOARD_INPUT
		if(Input.GetKeyUp(KeyCode.R))
			//Console.ImportantIf(this.controller.StartRecording(),"<b><color=red>Record !</color></b>");
			Console.ImportantIf(this.capturer.Start_capture() != null,"<b><color=red>Record !</color></b>");		
		if(Input.GetKeyUp(KeyCode.S))
			//Console.ImportantIf(this.controller.Stop(),"<b><color=magenta>Stop !</color></b>");
			Console.ImportantIf(this.capturer.Stop_capture() != null,"<b><color=magenta>Stop !</color></b>");
		if(Input.GetKeyUp(KeyCode.Backspace))
			//Console.ImportantIf(this.controller.Stop(),"<b><color=magenta>Cancelled !</color></b>");
			Console.ImportantIf(this.capturer.Stop_capture() != null,"<b><color=magenta>Stop !</color></b>");	
		#endif
	}

	void OnGUI()
	{
		#if GUI_INPUT	
		//GUI.enabled = true;			
		//GUI.enabled = this.controller.CanStop;		
		GUI.enabled = this.capturer.IsRecording;
		if(GUI.Button(new Rect(500.0f,100.0f,100.0f,50.0f),"Cancel"))
		{
			this.latestAnimation = this.capturer.Stop_capture();			
			this.filename = this.latestAnimation.Filename;
		}
		if(this.capturer != null)
		{
			GUI.enabled = true;
			var prev = selected;
			if (prev != (selected = GUI.SelectionGrid(new Rect(10, 10, 300, 300), selected, _activities, 1)))
			{
				SelectedActivity = Properties.ActivityFromString(_activities[selected]);
				this.comparer.ActionName = SelectedActivity;
			}
			GUI.Label(new Rect(400.0f,200.0f,50.0f,50.0f),"Rec Confidence");
			//GUI.Label(new Rect(400.0f,250.0f,50.0f,50.0f),(this.controller.RecordingConfidence * 100.0f).ToString("F0") + " %");			
			//GUI.enabled = this.controller.CanRecord;
			GUI.enabled = !this.capturer.IsRecording;			
			GUI.backgroundColor = this.capturer.IsRecording ? Color.green : Color.gray;			
			if(GUI.Button(new Rect(400.0f,100.0f,100.0f,50.0f),"Rec"))
				//this.controller.StartRecording();
				this.capturer.Start_capture();
			if(GUI.Button(new Rect(600.0f,100.0f,100.0f,100.0f),"Animate"))
			{
				Debug.Log(this.latestAnimation.getLenght());
				StartCoroutine("Animate");
			}
			GUI.enabled = !this.capturer.IsRecording;
			GUI.backgroundColor = this.capturer.IsRecording ? Color.gray : Color.green;
			if(GUI.Button(new Rect(400.0f,150.0f,100.0f,50.0f),"Compare"))
				this.Compare();
			GUI.Label(new Rect(100.0f,350.0f,100.0f,50.0f),"File : ");
			GUI.Label(new Rect(200.0f,350.0f,400.0f,50.0f),filename);		
			GUI.Label(new Rect(100.0f,400.0f,100.0f,50.0f),"Score : ");
			GUI.Label(new Rect(200.0f,400.0f,100.0f,50.0f),this.comparer.OverallScore.ToString());	
			GUI.Label(new Rect(300.0f,400.0f,100.0f,50.0f),"OK ? ");
			GUI.Label(new Rect(400.0f,400.0f,100.0f,50.0f),this.comparer.AreRelated.ToString());	
		}
		#endif
	}

	private IEnumerator Animate()
	{
		for( int i = 0; i <this.latestAnimation.getLenght(); ++i)
		{
			var frame = this.latestAnimation.GetFrame(i);
		
			Debug.Log("Frame " + i);

			Quaternion basicQuat_i = new Quaternion(frame[0][4], frame[0][5], frame[0][6], frame[0][7]); 
			skeleton_reference [9].transform.rotation = Quaternion.Euler(0,180, 0) * basicQuat_i; 

			skeleton_reference [0].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[13][4], frame[13][5], frame[13][6], frame[13][7]); 

			skeleton_reference [1].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[14][4], frame[14][5], frame[14][6], frame[14][7]); 

			skeleton_reference [2].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[17][4], frame[17][5], frame[17][6], frame[17][7]); 

			skeleton_reference [3].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[18][4], frame[18][5], frame[18][6], frame[18][7]); 

			skeleton_reference [4].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[5][4], frame[5][5], frame[5][6], frame[5][7]);  

			skeleton_reference [5].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[6][4], frame[6][5], frame[6][6], frame[6][7]); 

			skeleton_reference [6].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[9][4], frame[9][5], frame[9][6], frame[9][7]); 

			skeleton_reference [7].transform.rotation = Quaternion.Euler(180, 0, 0) * new Quaternion(frame[10][4], frame[10][5], frame[10][6], frame[10][7]); 

			yield return null;
		}
		yield break;
	}

	private void Compare()
	{
		this.comparer.Compute_comparison(this.latestAnimation.Filename);
	}

	//private Kinect2Controlling controller;
	private Capture_module capturer;
	private Compare_module comparer;
	private string filename;
	private int selected = 0;	
	private string[] _activities;
	public Properties.Activity SelectedActivity;
	private Animation_data latestAnimation;
	private List<GameObject> skeleton_reference = new List<GameObject>();
}
