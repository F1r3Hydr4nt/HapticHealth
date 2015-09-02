using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class LeaderboardUI : MonoBehaviour {
	public LeaderboardRow row;
	public Canvas canvas;
	public List<LeaderboardRow> rows;
	public float rowHeight = 60f;
	void Awake(){
		LoadBoards ();
	}
	public void LoadBoards(){
		string [] fileRows = Leaderboards.ReadLeaderboardFromFile ();
		string [] rows = new string[10];
		//if (fileRows.Length < 10) {
			int i = 0;
			for(;i<fileRows.Length;i++){
				rows[i] = fileRows[i];
			}
			while (i<10) {
			rows[i] = "AAA 000";
			i++;
				}
		//}
		Populate(rows);
	}

	public void Populate(string[]data){
		rows = new List<LeaderboardRow> ();
		for(int i = 0;i<10;i++){
			LeaderboardRow newRow = ((GameObject)GameObject.Instantiate(row.gameObject,row.transform.position,row.transform.rotation)).GetComponent<LeaderboardRow>();
			newRow.transform.SetParent(transform);
			newRow.Populate(i,data[i]);
			newRow.Position(i,60f,row.rectT);
			rows.Add(newRow);
		}
		Destroy (row.gameObject);
	}

}
