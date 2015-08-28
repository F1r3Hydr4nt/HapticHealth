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

	void Update(){
		if (Input.GetKeyDown (KeyCode.M))
						MoveOntoNextStage ();
	}
	public void PromptTpose(){
		instructions.text = "Please T-Pose to calibrate the wearables.";
	}
	public void PromptFirstMotionObserve(){
		fusedSkeletonMain.DisableRenderers (true, false);
		Invoke ("PlayFirstMotion", 2f);
		instructions.text = "Observe the following recorded motion & prepare to copy it.";
	}
	void PlayFirstMotion(){
		HapticHealthController.Instance.PlaybackPrerecordedMotion ("firstMotion", true);
		Invoke ("MoveOntoNextStage", 4f);
	}
	public void PromptFirstMotionCopy(){
		wiMuPlotter.StartComparingSignals ();
		instructions.text = "Attempt to copy that motion as close as possible.";
	}
	public void PromptFirstMotionResult(bool isPass){
		if (isPass) {
						instructions.text = "Attempt to copy that motion as close as possible.";
		}
		else {
			instructions.text = "Attempt to copy that motion as close as possible.";
		}
	}
	int currentStage = 0;
	public void MoveOntoNextStage(){
		currentStage++;
		switch (currentStage) {
		case 1:
			PromptTpose();
			break;
		case 2:
			PromptFirstMotionObserve();
			break;
		case 3:
			PromptFirstMotionCopy();
			break;
		case 4:
			break;
		}
	}
}

