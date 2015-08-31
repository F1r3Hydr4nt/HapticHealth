using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System.Threading;

using System;

public class UIController:MonoBehaviour
{
	public void HasTPosed ()
	{
		if (!hasTPosed) {
			hasTPosed = true;
			MoveOntoNextStage();
		}
	}
	public static bool hasTPosed = false;
	public WiMuPlotter wiMuPlotter;
	public Text instructions, timer;
	public static UIController Instance;
	public FusedSkeleton_Main fusedSkeletonMain;
	void Awake(){
		Instance = this;
		//Invoke ("MoveOntoNextStage", 2f);
	}

	public void DisplayFeedback (string s)
	{
		instructions.text = s;
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.M))
						MoveOntoNextStage ();
	}
	public void PromptTpose(){
		instructions.text = "Please T-Pose to calibrate the wearables.";
	}
	public void PromptFirstMotionObserve(){
		fusedSkeletonMain.DisableRenderers (true, false);
	}
	void PlayFirstMotion(){
		instructions.text = "Observe the following Motion.";
		HapticHealthController.Instance.PlaybackPrerecordedMotion ("firstMotion", false);
	}
	public void PromptFirstMotionCopy(){
		wiMuPlotter.StartComparingSignals ();
		instructions.text = "Attempt to copy that motion accurately.";
	}
	public bool firstMotionCompletedSuccessfully = false;
	int currentStage = 0;

	void CheckIfMotionCompletedSuccessfully ()
	{
		wiMuPlotter.StopComparing ();
		if(wiMuPlotter.comparator.success){
			firstMotionCompletedSuccessfully=true;
			DisplayFeedback ("Congratulations!");
				} else
						DisplayFeedback (wiMuPlotter.comparator.feedback);
	}

	public void MoveOntoNextStage(){
		if (currentStage == 6 && !firstMotionCompletedSuccessfully)
						currentStage -= 2;
		else currentStage++;
		switch (currentStage) {
		case 1:
			PromptTpose();
			break;
		case 2:
			break;
		case 3:
			PromptFirstMotionObserve();
			break;
		case 4:
			print ("PlayFirstMotion");
			PlayFirstMotion();
			break;
		case 5:
			print ("PromptCopy");
			PromptFirstMotionCopy();
			break;
		case 6:
			print ("CheckMotion");
			CheckIfMotionCompletedSuccessfully();
			break;
		case 7:
			CheckIfMotionCompletedSuccessfully();
			break;
		}
	}
}

