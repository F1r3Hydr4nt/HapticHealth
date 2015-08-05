using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CoachFeedbacks_Panel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Coach_and_train_scenario coach_and_train_scenario;
	public Canvas _player_activities_canvas;
	public GameObject GUI_parent;
	public GameObject itemPrefab;
	public int itemCount = 10, columnCount = 1;

	public GameObject heading_title;

	private User user;
	private Coach coach;

	private Database_module _database;
	private float width;
	private float ratio;
	private float height;
	private RectTransform rowRectTransform;
	private RectTransform containerRectTransform;
	private Lang translations;
	
	void Start()
	{
		_database = new Database_module ();
		
	}
	
	
	public void SetPanelVisibility(bool state)
	{
		_player_activities_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_player_activities_canvas.active = false;
	}
	public void Show_panel()
	{
		_player_activities_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		translations = GlobalVariables.translations;
		
		
		
		//Translations
		heading_title.GetComponent<Text> ().text= translations.getString ("feedbacks");
		
		user = GlobalVariables.user;
		coach = _database.getCoach (user.id);


		for (int i = GUI_parent.transform.childCount - 1; i >= 0; i--)
		{
			// objectA is not the attached GameObject, so you can do all your checks with it.
			GameObject objectA = GUI_parent.transform.GetChild(i).gameObject;
			objectA.transform.parent = null;
			// Optionally destroy the objectA if not longer needed
		} 


		List<Feedback> feedbacks = coach.feedbacks;
		itemCount = feedbacks.Count;
		columnCount = 1;
		
		RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform>();
		RectTransform containerRectTransform = GUI_parent.GetComponent<RectTransform>();
		
		//calculate the width and height of each child item.
		float width = containerRectTransform.rect.width / columnCount;
		float ratio = 1 /*width / rowRectTransform.rect.width*/;
		float height = rowRectTransform.rect.height * ratio;
		/*float height = containerRectTransform.rect.height;
		float ratio = height / rowRectTransform.rect.height;
		float width = rowRectTransform.rect.width *  ratio;*/
		
		
		int rowCount = itemCount / columnCount;
		if (rowCount == 0)
			rowCount = 1;
		if (itemCount % rowCount > 0)
			rowCount++;
		
		//adjust the height of the container so that it will just barely fit all its children
		float scrollHeight = height * rowCount;
		float scrollWidth = width * itemCount;
		containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight);
		containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, 0);

		int j = 0;
		
		for (int i = 0; i < itemCount; i++)
		{
			//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
			if (i % columnCount == 0)
				j++;

			Feedback feedback = feedbacks[i];
			int id = feedback.id;
			int activity_id = feedback.activity_id;
			Activity activity = _database.getActivity(activity_id);
			string datetime = activity.datetime;
			int player_id = activity.player_id;
			int skill_id = activity.skill_id;
			int statistics_id = activity.statistics_id;
			int team_id = activity.team_id;
			Player player = _database.getPlayer(player_id);
			Team team = _database.getTeam(team_id);
			int discipline_id = team.discipline_id;		
			

			GameObject.Find("ccf_label_skill").GetComponent<Text> ().text=translations.getString ("skill");
			GameObject.Find("ccf_label_score").GetComponent<Text> ().text=translations.getString ("score");
			GameObject.Find("ccf_label_feedback").GetComponent<Text> ().text=translations.getString ("feedback");
			GameObject.Find("ccf_label_team").GetComponent<Text> ().text=translations.getString ("team");
			GameObject.Find("ccf_label_club").GetComponent<Text> ().text=translations.getString ("club");
			
			
			string photo = player.photo;
			string player_name = player.name;
			string player_surname = player.surname;
			int club_id = player.club_id;
			
			Skill skill = _database.getSkill(skill_id);
			string skill_name = skill.name;
			
			
			
			
			Statistics statistics = _database.getStatistics(statistics_id);
			double score = statistics.overall_score;
			
			
			GameObject.Find("ccf_label_player_name").GetComponent<Text> ().text=player_name+" "+player_surname;
			GameObject.Find("ccf_skill").GetComponent<Text> ().text=skill_name;
			
			GameObject.Find("ccf_score").GetComponent<Text> ().text=score.ToString();
			GameObject.Find("ccf_label_datetime").GetComponent<Text> ().text=datetime;
			GameObject.Find("ccf_feedback").GetComponent<Text> ().text=feedback.text;
			
			
			string[] parts = photo.Split('.');
			photo = "images/"+parts[0];
			
			Texture image = (Texture)Resources.Load(photo, typeof(Texture));
			GameObject.Find("ccf_image").GetComponent<RawImage> ().texture=image;
			
			GameObject newItem = Instantiate(itemPrefab) as GameObject;
			newItem.name = id.ToString();
			newItem.transform.parent = GUI_parent.transform;
			newItem.transform.localScale = Vector3.one;
			//Debug.Log("textos dentro: "+newItem.GetComponentsInChildren<Button>().Length.ToString());
			Button[] buttons = newItem.GetComponentsInChildren<Button>();
			foreach (Button button in buttons)
			{
				
				if (button.name=="ccf_button_edit_feedback")
				{
					
					button.GetComponentsInChildren<Text> () [0].text = translations.getString ("edit");
					button.onClick.AddListener(() => { 
						GlobalVariables.selected_feedback_id = id;
						GlobalVariables.selected_activity_id = activity_id;
						GlobalVariables.selected_player_id = player_id;
						GlobalVariables.selected_club_id = club_id;
						GlobalVariables.selected_team_id = team_id;
						GlobalVariables.selected_discipline_id = discipline_id;
						Debug.Log("feedback id: "+id); 
						
						coach_and_train_scenario.GoToFeedback();
						
						
					}); 
				}
				
			}


			Debug.Log (newItem.name);
			
			//move and size the new item
			RectTransform rectTransform = newItem.GetComponent<RectTransform>();
			
			float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
			float y = containerRectTransform.rect.height / 2 - height * j;
			rectTransform.offsetMin = new Vector2(x, y);
			
			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2(x, y);

		}

		/*foreach (Activity activity in coach.activities)
		{
			string datetime = activity.datetime;
			int player_id = activity.player_id;
			int skill_id = activity.skill_id;
			int statistics_id = activity.statistics_id;

			//List<Feedback> feedbacks = activity.feedbacks;


			Player player = _database.getPlayer(player_id);
			string photo = player.photo;
			string player_name = player.name;
			string player_surname = player.surname;

			Skill skill = _database.getSkill(skill_id);
			string skill_name = skill.name;
			Team team=new Team();


			foreach (Team tt in player.teams)
			{
				Debug.Log("team discipline:"+tt.discipline_id+",skill discipline "+skill.discipline_id );
				if (tt.discipline_id == skill.discipline_id)
				{
					team = _database.getTeam(tt.id);
					Debug.Log(tt.id+" "+tt.name+" "+tt.discipline_id );
				}
			}

			string team_name = team.name;
			Club club = _database.getClub(player.club_id);
			string club_name = club.name;
			Statistics statistics = _database.getStatistics(statistics_id);
			double score = statistics.accuracy;


		}*/


    }

}
