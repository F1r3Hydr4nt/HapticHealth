using UnityEngine;
using System.Collections;
using Fusion;

public class WiMuPlotter : MonoBehaviour {
	public FusedSkeleton_Main fusedSkeleton;
	// Use this for initialization
	void Start () {
		
		
		
		
		
		//  Create a new graph named "MouseX", with a range of 0 to 2000, colour green at position 100,100
		PlotManager.Instance.PlotCreate("0", 0f, 3f, Color.cyan, new Vector2(10,	10));
		//  Create a new graph named "MouseX", with a range of 0 to 2000, colour green at position 100,100
		PlotManager.Instance.PlotCreate("1", Color.yellow, "0");
		//print ("---- " + i + " " + WIMUsOrientation[ i ].ToString() );
	}
	
	// Update is called once per frame
	void Update () {
//		if (fusedSkeleton.WimusOnline) {
		//	PlotManager.Instance.PlotAdd ("0", Mathf.Abs(fusedSkeleton.totalMagnitudes [0]));
		//	PlotManager.Instance.PlotAdd ("1", Mathf.Abs(fusedSkeleton.totalMagnitudes [1]));
	//	} else {
			//PlotManager.Instance.PlotAdd ("0", 0);
			//PlotManager.Instance.PlotAdd ("1", 0);

	//	}
	}
}
