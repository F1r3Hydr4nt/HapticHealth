using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using System.Threading;
using Shimmer;
using System;

public class UIController:MonoBehaviour
{
	public GameObject endPanel;
	public void HasTPosed ()
	{
		if (!hasTPosed) {
			hasTPosed = true;
			MoveOntoNextStage();
		}
	}
	public static bool hasTPosed = false;
	public WiMuPlotter wiMuPlotter;
	public Text instructions, timer, scoreOutOfTen;
	public GameObject score;
	public static UIController Instance;
	public FusedSkeleton_Main fusedSkeletonMain;
	void Awake(){
		Instance = this;
		//Invoke ("MoveOntoNextStage", 2f);
	}
	public GameObject feedbackPanel;
	public void DisplayFeedback (AccelerationComparator comparator)
	{
		score.SetActive (false);
		instructions.text = comparator.feedback;
		scoreOutOfTen.text = comparator.score;
		feedbackPanel.SetActive (true);
		Invoke ("DisplayScore",2f);
	}

	void DisplayScore(){
		feedbackPanel.SetActive (false);
		score.SetActive (true);
		//LeanTween.scale (score, Vector3.one * 2f, 2f).setEase (LeanTweenType.easeInOutSine);

	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.M))
						MoveOntoNextStage ();
	}
	public void PromptTpose(){
		instructions.text = "Please T-Pose to calibrate the wearables.";
		feedbackPanel.SetActive (true);
	}
	public void PromptFirstMotionObserve(){
		score.SetActive (false);
		feedbackPanel.SetActive (false);
		fusedSkeletonMain.DisableRenderers (true, false);
	}
	void PlayFirstMotion(){
		instructions.text = "Observe the following Motion.";
		feedbackPanel.SetActive (true);
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
						DisplayFeedback (wiMuPlotter.comparator);
		attempts++;
		if (attempts == 3)
						firstMotionCompletedSuccessfully = true;
	}
	int attempts = 0;
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
			DisplayEndPanel();
			break;
		}
	}

	void DisplayEndPanel(){
		ReloadLevel ();
	}

	void ReloadLevel(){
		ShimmerReceiving.Instance.OnDisable ();
		Application.LoadLevel ("FusedSkeleton");
		}
}

