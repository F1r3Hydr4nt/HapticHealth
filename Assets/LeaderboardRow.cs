using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeaderboardRow : MonoBehaviour {
	public RectTransform rectT;
	public Text rank,name,score;
	// Use this for initialization
	public void Populate (int r,string s) {
		print (s);
		string[] split = s.Split (' ');
		rank.text = (r+1).ToString ()+".";
		print (split [0]);
		name.text = split [0];
		print (split [1]);
		//string commaSeparated = int.Parse(split [1]).ToString("#,##0");
		score.text = split[1];
	}
	public void Position(int index, float height,RectTransform r){
		print (height);
		//height = 0;
		transform.SetParent (r.parent);
		//rectT.localPosition = new Vector3 ();
		rectT.localScale = new Vector3 (1, 1, 1);
		rectT.anchorMin = r.anchorMin;
		rectT.anchorMax = r.anchorMax;
		rectT.offsetMin = r.offsetMin+new Vector2(0,-(index*height));
		rectT.offsetMax = r.offsetMax+new Vector2(0,-(index*height));
		//rectT.offsetMin = new Vector2 (0, 0);
	}


}
