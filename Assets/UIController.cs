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
		score.gameObject.transform.parent.gameObject.SetActive (false);
		instructions.text = comparator.feedback;
		scoreOutOfTen.text = comparator.score;
		feedbackPanel.SetActive (true);
		Invoke ("DisplayScore",2f);
	}

	void DisplayScore(){
		feedbackPanel.SetActive (false);
		score.gameObject.transform.parent.gameObject.SetActive (true);
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
		instructions.text = "Observe the following Motion.";
		feedbackPanel.SetActive (true);
		fusedSkeletonMain.DisableRenderers (true, false);
	}
	void PlayFirstMotion(){
		score.gameObject.transform.parent.gameObject.SetActive (false);
		feedbackPanel.SetActive (false);
		HapticHealthController.Instance.PlaybackPrerecordedMotion ("firstMotion", false);
	}
	public void PromptFirstMotionCopy(){
		feedbackPanel.SetActive (true);
		wiMuPlotter.StartComparingSignals ();
		instructions.text = "Attempt to copy that motion accurately.";
	}
	public bool firstMotionCompletedSuccessfully = false;
	int currentStage = 0;

	void CheckIfMotionCompletedSuccessfully ()
	{
		wiMuPlotter.StopComparing ();
						DisplayFeedback (wiMuPlotter.comparator);
		totalScore += float.Parse (wiMuPlotter.comparator.score);
		attempts++;
		if (attempts == 3)
						firstMotionCompletedSuccessfully = true;
	}
	int attempts = 0;
	float totalScore = 0f;
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
		case 8:
			ReloadLevel();
			break;
		}
	}
	public Text endScore, endText;
	public HapticHealthController controller;
	void DisplayEndPanel(){
		endScore.text = Round(totalScore,1).ToString();
		endText.text = "GREAT!" + '\n' + "You scored:";
		endPanel.SetActive (true);
		controller.readingKeyboard = false;
		score.gameObject.transform.parent.gameObject.SetActive (false);
		//ReloadLevel ();

	}
	public static float Round(float value, int digits)
	{
		float mult = Mathf.Pow(10.0f, (float)digits);
		return Mathf.Round(value * mult) / mult;
	}
	public InputField inputField;
	public LeaderboardUI leaderboard;
	public void OkButtonClick(){
		if (inputField.text != "") {
			endPanel.SetActive(false);
			string nickname = inputField.text;
			Leaderboards.AddScoreIfInTopTen(totalScore,nickname);
			leaderboard.LoadBoards();
			controller.readingKeyboard = true;
		}
	}

	void ReloadLevel(){
		ShimmerReceiving.Instance.OnDisable ();
		Application.LoadLevel ("FusedSkeleton");
		}
}

