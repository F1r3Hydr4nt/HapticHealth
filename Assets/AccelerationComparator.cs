using Fusion;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System;

public class AccelerationComparator
{
	public string score {
		get;
		set;
	}

	List<float> motionValues1, motionValues2;
	List<float> motionValuesRT1, motionValuesRT2;
	public AccelerationComparator(List<float> values1,List<float> values2){
		motionValues1 = values1;
		motionValues2 = values2;
		FindPeakIndices ();
		motionValuesRT1 = new List<float> ();
		motionValuesRT2 = new List<float> ();
	}
	int[] maxIndices;
	bool firstComplete = false;
	bool secondComplete = false;
	public void Reset(){
		firstComplete = false;
		secondComplete = false;
		motionValuesRT1 = new List<float> ();
		motionValuesRT2 = new List<float> ();
		success = false;
	}
	float maxCombinedPeak = 0f;
	void FindPeakIndices ()
	{
		float firstMax = 0f;
		float secondMax = 0f;
		maxIndices = new int[2];
		maxIndices [0] = maxIndices [1] = 0;
		for (int i = 0; i<motionValues1.Count; i++) {
//			Debug.Log ("Values: "+motionValues1[i]+" "+motionValues2[i]);
			if(motionValues1[i]>firstMax){
				maxIndices[0]=i;
				firstMax = motionValues1[i];
			}
			if(motionValues2[i]>secondMax){
				maxIndices[1]=i;
				secondMax = motionValues2[i];
			}
			if(((motionValues1[i]+motionValues2[i])*0.5f)>maxCombinedPeak)maxCombinedPeak = motionValues1[i]+motionValues2[i];
		}
		Debug.Log ("Found max values " + maxIndices [0] + " -> "+motionValues1[maxIndices[0]]+", " + maxIndices [1]+ " -> "+motionValues2[maxIndices[1]]);
	}
	float thresholdPercentage = 0.1f;//within 10% either sidep
	public bool CompareValues(int frameNo, float value1, float value2){
//		Debug.Log ("First hand: "+frameNo+". "+value1+ " should be close to " + motionValues1[frameNo]);
//		Debug.Log ("Second hand: "+frameNo+". "+value2+ " should be close to " + motionValues2[frameNo]);
		if (frameNo == maxIndices [0]) {
			Debug.Log("ComparingFrame");
			if(value1>(motionValues1[maxIndices[0]]*(1-thresholdPercentage))&&value1<(motionValues1[maxIndices[0]]*(1+thresholdPercentage))){
				Debug.Log ("First hand comparison correct: "+frameNo+". "+value1+ " within 10% of " + motionValues1[maxIndices[0]]);
				//Debug.Break ();
				firstComplete = true;
				return true;
			}else{
				Debug.Log ("First hand comparison INCORRECT: "+frameNo+". "+value1+ " NOT within 10% of " + motionValues1[maxIndices[0]]);
			}
		}
		if (frameNo == maxIndices [1]) {
			Debug.Log("ComparingFrame");
			if(value2>(motionValues2[maxIndices[1]]*(1-thresholdPercentage))&&value2<(motionValues2[maxIndices[1]]*(1+thresholdPercentage))){
				Debug.Log ("Second hand comparison correct: "+frameNo+". "+value2+ " within 10% of " + motionValues2[maxIndices[1]]);
				//Debug.Break ();
				secondComplete = true;
				return true;
			}else{
				Debug.Log ("Second hand comparison INCORRECT: "+frameNo+". "+value2+ " NOT within 10% of " + motionValues2[maxIndices[1]]);
			}
		}
		//if (frameNo == motionValues1.Count - 1)
						//Debug.Break ();
		if (frameNo==motionValues1.Count-1&&firstComplete && secondComplete) {
			//Reset ();
			TactorController.Instance.SendDurationToAll();
			Debug.Break ();
		}
		return false;
	}
	public void AddRealtimeValues(int frameNo, float value1, float value2){
		motionValuesRT1.Add (value1);
		motionValuesRT2.Add (value2);
	}
	public bool success = false;
	public string feedback = "";
	public void CheckHandAccelerationValues(){
		//Debug.Break ();
		float firstMax = 0f;
		float secondMax = 0f;
		for (int i = 0; i<motionValuesRT1.Count; i++) {
			//			Debug.Log ("Values: "+motionValues1[i]+" "+motionValues2[i]);
			if(motionValuesRT1[i]>firstMax){
				firstMax = motionValuesRT1[i];
			}
			if(motionValuesRT2[i]>secondMax){
				secondMax = motionValuesRT2[i];
			}
		}
		if (firstMax > (motionValues1 [maxIndices [0]] * (1 - thresholdPercentage))) {//greater then
						if (firstMax < (motionValues1 [maxIndices [0]] * (1 + thresholdPercentage))) {//less than
							if(secondMax  > (motionValues2 [maxIndices [1]] * (1 - thresholdPercentage))){//greater than
					if(secondMax < (motionValues2 [maxIndices [1]] * (1 + thresholdPercentage))){//less thansecondMax
						Debug.Log ("Success");
						success=true;
					}else{//second Hand too fast
						feedback = "Too FAST!";
						Debug.Log ("Second Hand too fast");
					}
				}else{//second hand too slow
					
					feedback = "Too SLOW!";
					Debug.Log ("Second Hand too slow");

				}
			}
				else {//first hand too fast
				
				Debug.Log ("First Hand too fast");
				feedback = "Too FAST!";
						}

				} else {//first hand too slow
			
			Debug.Log ("Second Hand too slow");
			feedback = "Too SLOW!";
				}
	}

	int CalculatePercentage (float combinedMax, float maxCombinedPeak)
	{
		Debug.Log ("Max combined = " + maxCombinedPeak);
		Debug.Log ("current = " + combinedMax);
		string result = "";
		float percentageOfSpeed = (combinedMax / maxCombinedPeak) * 100f;
		Debug.Log ("percentage = "+percentageOfSpeed);
		int correction = (int)((1f - (100f / percentageOfSpeed)) * 100f);
		Debug.Log ("correction = "+correction);
		return Mathf.Abs(correction);
	}
	
	public void CheckHandAccelerationCombinedValues(){
				//Debug.Break ();
				float combinedMax = 0f;
				for (int i = 0; i<motionValuesRT1.Count; i++) {
						if (((motionValuesRT1 [i] + motionValuesRT2 [i]) * 0.5f) > combinedMax)
								combinedMax = (motionValuesRT1 [i] + motionValuesRT2 [i]);
				}
				float diff = maxCombinedPeak - combinedMax;
				
				if ((combinedMax > maxCombinedPeak * (1 - thresholdPercentage))) {
						if (combinedMax < maxCombinedPeak * (1 + thresholdPercentage)) {
							feedback = "Great!"+'\n'+'\n';
				if(diff<0f)feedback+="Decrease speed slightly by: "+CalculatePercentage(combinedMax,maxCombinedPeak)+"%";
				else feedback+="Increase speed slightly by: "+CalculatePercentage(combinedMax,maxCombinedPeak)+"%";
						} else {
				feedback = "Too FAST!"+'\n'+'\n';
							feedback+="Decrease speed by: "+CalculatePercentage(combinedMax,maxCombinedPeak)+"%";
						}
				} else {
			feedback = "Too SLOW!"+'\n'+'\n';
					feedback+="Increase speed slightly by: "+CalculatePercentage(combinedMax,maxCombinedPeak)+"%";
				}
				score = (((100f - CalculatePercentage (combinedMax, maxCombinedPeak)) * 0.1f)).ToString();
		}
}

