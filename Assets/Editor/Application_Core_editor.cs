using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Application_core))]
public class Application_Core_editor : Editor {
	
	/***
	 * Description:
	 * Editor script to load parameters (scenario/device/modaliy)
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/


	private Application_core myTarget;

	void Awake()
	{
		myTarget = (Application_core)target;
	}
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		//myTarget = (Application_core) target;

		Rect r = EditorGUILayout.BeginHorizontal();
		myTarget.SetDevice((Device)EditorGUILayout.EnumPopup ("Device Setup: ", myTarget.GetDevice()));
		EditorGUILayout.EndHorizontal();

		if(myTarget.GetDevice() == Device.MULTI_KINECT){
			r = EditorGUILayout.BeginHorizontal();

			myTarget.SetKinectNumber((int)EditorGUILayout.IntSlider("Number of Kinects: ",myTarget.GetNumberOfKinect() , 1, 4));
			EditorGUILayout.EndHorizontal();
		}else if(myTarget.GetDevice() == Device.KINECT_WIMUS){
			r = EditorGUILayout.BeginHorizontal();
			myTarget.SetKinectNumber((int)EditorGUILayout.IntSlider("Number of Kinects: ",myTarget.GetNumberOfKinect() , 1, 4));
			EditorGUILayout.EndHorizontal();

			r = EditorGUILayout.BeginHorizontal();
			myTarget.SetWIMUsNumber((int)EditorGUILayout.IntSlider("Number of WIMUs: ",myTarget.GetNumberOfWIMUs() , 1, 16));
			EditorGUILayout.EndHorizontal();
		}

		r = EditorGUILayout.BeginHorizontal();
		myTarget.SetScenario((Scenario)EditorGUILayout.EnumPopup ("Scenario: ", myTarget.GetScenario()));
		EditorGUILayout.EndHorizontal();

		r = EditorGUILayout.BeginHorizontal();
		myTarget.SetModality((Modality)EditorGUILayout.EnumPopup ("Modality: ", myTarget.GetModality()));
		EditorGUILayout.EndHorizontal();

		if(GUI.changed)
			EditorUtility.SetDirty(myTarget); 

	}

}

