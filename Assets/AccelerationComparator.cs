using Fusion;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System;

public class AccelerationComparator
{
	List<float> motionValues1, motionValues2;
	public AccelerationComparator(List<float> values1,List<float> values2){
		motionValues1 = values1;
		motionValues2 = values2;
		FindPeakIndices ();
	}
	int[] maxIndices;
	bool firstComplete = false;
	bool secondComplete = false;
	public void Reset(){
		firstComplete = false;
		secondComplete = false;
	}
	void FindPeakIndices ()
	{
		float firstMax = 0f;
		float secondMax = 0f;
		maxIndices = new int[2];
		maxIndices [0] = maxIndices [1] = 0;
		for (int i = 0; i<motionValues1.Count; i++) {
			Debug.Log ("Values: "+motionValues1[i]+" "+motionValues2[i]);
			if(motionValues1[i]>firstMax){
				maxIndices[0]=i;
				firstMax = motionValues1[i];
			}
			if(motionValues2[i]>secondMax){
				maxIndices[1]=i;
				secondMax = motionValues2[i];
			}
		}
		Debug.Log ("Found max values " + maxIndices [0] + " -> "+motionValues1[maxIndices[0]]+", " + maxIndices [1]+ " -> "+motionValues2[maxIndices[1]]);
	}
	float thresholdPercentage = 0.25f;//within 10% either side
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
			Reset ();
			Debug.Break ();
		}
		return false;
	}
}

