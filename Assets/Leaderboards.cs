using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;

public class Leaderboards : MonoBehaviour {	
	static string filePath = Path.Combine (Environment.CurrentDirectory, "leaderboards.txt");
	public LeaderboardUI leaderboards;
	public GameObject EntryPanel;
	public InputField nickname;
	public Text scoreText;
	public void OkButton(){
		AddScoreIfInTopTen (int.Parse (scoreText.text.Replace(",","")), nickname.text.Replace(" ","_"));
		leaderboards.LoadBoards ();
		EntryPanel.SetActive (false);
		leaderboards.gameObject.SetActive (true);
	}
	void Awake(){
		//PlayerPrefs.SetInt ("Score", 1000);
		
		string commaSeparated = (PlayerPrefs.GetInt ("Score")).ToString("#,##0");
		scoreText.text = commaSeparated;
	}

	
	public void LoadMenu(){
		
		Application.LoadLevel ("KinectInitialisationScene");
	}
	public static string [] ReadLeaderboardFromFile(){
		print ("Attempting to read:" + filePath);
		return File.ReadAllLines (filePath);
	}

	static void WriteNewLeaderboard (List<string> newEntries)
	{
		File.Delete (filePath);
		File.WriteAllLines (filePath, newEntries.ToArray ());
	}

	public static void AddScoreIfInTopTen(float score, string name){
		string[] currentEntries = ReadLeaderboardFromFile ();
		List<float> scores = new List<float> ();
		print ("currentEntry "+score);
		foreach (string s in currentEntries) {
			print ("currentEntry "+s);
			scores.Add (float.Parse(s.Split(' ')[1]));
		}
		int i = 0;
		int newEntryIndex = 0;
		bool madeLeaderboard = false;

		while (i<scores.Count) {
						if (score >= scores [i]){
								madeLeaderboard = true;
								newEntryIndex = i;
						break;
				}
			i++;
		}
		if (scores.Count == 0) {
						madeLeaderboard = true;
						newEntryIndex = 0;
				}
		if (madeLeaderboard) {
						print ("madeLeaderboard");
						List<string> newEntries = new List<string> ();
						i = 0;
						while (i<newEntryIndex) {
								newEntries.Add (currentEntries [i]);
								i++;
						}
						newEntries.Add (name + " " + score);
						while (i<currentEntries.Length) {
								string [] split = currentEntries [i].Split (' ');
								newEntries.Add (split [0] + " " + split [1]);
								i++;
						}
						if (newEntries.Count > 10)
								newEntries.RemoveRange (10, newEntries.Count - 10);
						WriteNewLeaderboard (newEntries);
				} else
						print ("Not madeLeaderboard");
	}
}
