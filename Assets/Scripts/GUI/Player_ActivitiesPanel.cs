using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player_ActivitiesPanel: MonoBehaviour
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
	public GameObject _tab_personaldata;
	public GameObject _tab_activities;
	public GameObject _tab_statistics;
	public GameObject _tab_feedbacks;
	public GameObject _subtitle;

	public GameObject _scrollbar;
	public GameObject _panel;
	public GameObject _no_elements_label;
	
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

		_tab_personaldata.SetActive(true);
		_tab_statistics.SetActive(true);
		_tab_feedbacks.SetActive(true);
		
		
		//Translations
		_subtitle.GetComponent<Text> ().text= translations.getString ("activities");
		_tab_personaldata.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("personal_data");
		_tab_activities.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("activities");
		_tab_statistics.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("statistics");
		_tab_feedbacks.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("feedbacks");

		

		
		//Data of the player
		int user_id = GlobalVariables.user_id;
		int player_id = GlobalVariables.selected_player_id;
		int team_id = GlobalVariables.selected_team_id;
		
		Player player;
		
		
		
		
		
		
		
		if (player_id != 0) {
			player = _database.getPlayer (player_id);
			heading_title.GetComponent<Text> ().text=player.name+" "+player.surname;
			

			rowRectTransform = itemPrefab.GetComponent<RectTransform>();
			containerRectTransform = GUI_parent.GetComponent<RectTransform>();
			
			
			//calculate the width and height of each child item.
			float width = containerRectTransform.rect.width / columnCount;
			float ratio = 1 /*width / rowRectTransform.rect.width*/;
			float height = rowRectTransform.rect.height * ratio;


			
			for (int i = GUI_parent.transform.childCount - 1; i >= 0; i--)
			{
				// objectA is not the attached GameObject, so you can do all your checks with it.
				GameObject objectA = GUI_parent.transform.GetChild(i).gameObject;
				objectA.transform.parent = null;
				// Optionally destroy the objectA if not longer needed
			} 
			



			
			List<Activity> activities = _database.getPlayerActivitiesFromTeam(player_id,team_id);

			if (activities.Count>0)
			{
			itemCount = activities.Count;
			columnCount = 1;

			int rowCount = itemCount / columnCount;
			if (rowCount == 0)
				rowCount = 1;
			if (itemCount % rowCount > 0)
				rowCount++;
			
			//adjust the height of the container so that it will just barely fit all its children
			float scrollHeight = height * rowCount;
			float scrollWidth = width * itemCount;
			
			
			
			int j = 0;
			
			for (int i = 0; i < itemCount; i++)
			{
				//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
				if (i % columnCount == 0)
					j++;
				
				//create a new item, name it, and set the parent
				Activity activity = activities[i];
				int id = activity.id;
				string datetime = activity.datetime;
				int skill_id = activity.skill_id;
				int statistics_id = activity.statistics_id;
				
				List<Feedback> feedbacks = activity.feedbacks;
				if (activity.feedbacks.Count==0)
				{
					GlobalVariables.selected_feedback_id = 0;
					GameObject.Find("ccpa_button_edit_feedback").GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("add_feedback");
				}
				else
				{
					GlobalVariables.selected_feedback_id = feedbacks[0].id; 
					GameObject.Find("ccpa_feedback").GetComponent<Text> ().text=activity.feedbacks[0].text;
					GameObject.Find("ccpa_button_edit_feedback").GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("edit_feedback");
				}

				GameObject.Find("ccpa_button_view_activity").GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("view");
				GameObject.Find("ccpa_label_skill").GetComponent<Text> ().text=translations.getString ("skill");
				GameObject.Find("ccpa_label_score").GetComponent<Text> ().text=translations.getString ("score");
				GameObject.Find("ccpa_label_feedback").GetComponent<Text> ().text=translations.getString ("feedback");
				
				string photo = player.photo;
				string player_name = player.name;
				string player_surname = player.surname;
				int club_id = player.club_id;
				
				Skill skill = _database.getSkill(skill_id);
				string skill_name = skill.name;
				
				
				
				
				Statistics statistics = _database.getStatistics(statistics_id);
				double score = statistics.overall_score;
				
				
				GameObject.Find("ccpa_label_player_name").GetComponent<Text> ().text=player_name+" "+player_surname;
				GameObject.Find("ccpa_skill").GetComponent<Text> ().text=skill_name;
				
				GameObject.Find("ccpa_score").GetComponent<Text> ().text=score.ToString();
				GameObject.Find("ccpa_label_datetime").GetComponent<Text> ().text=datetime;
				
				
				
				string[] parts = photo.Split('.');
				photo = "images/"+parts[0];
				
				Texture image = (Texture)Resources.Load(photo, typeof(Texture));
				GameObject.Find("ccpa_image").GetComponent<RawImage> ().texture=image;
				
				GameObject newItem = Instantiate(itemPrefab) as GameObject;
				newItem.name = id.ToString();
				newItem.transform.parent = GUI_parent.transform;
				newItem.transform.localScale = Vector3.one;
				//Debug.Log("textos dentro: "+newItem.GetComponentsInChildren<Button>().Length.ToString());
				Button[] buttons = newItem.GetComponentsInChildren<Button>();
				foreach (Button button in buttons)
				{
					
					if (button.name=="ccpa_button_view_activity")
					{
						button.onClick.AddListener(() => { 
							GlobalVariables.selected_activity_id = id;
							GlobalVariables.selected_player_id = player_id;
							GlobalVariables.selected_club_id = club_id;
							GlobalVariables.selected_team_id = team_id;
							Debug.Log("activity id: "+id); 
							
							coach_and_train_scenario.GoToActivity();
							
							
						}); 
					}
					
					else if (button.name=="ccpa_button_edit_feedback")
					{
						
						
						button.onClick.AddListener(() => { 
							GlobalVariables.selected_activity_id = id;
							GlobalVariables.selected_player_id = player_id;
							GlobalVariables.selected_club_id = club_id;
							GlobalVariables.selected_team_id = team_id;
							Debug.Log("activity id: "+id); 
							
							coach_and_train_scenario.GoToFeedback();
							
							
						}); 
					}
					
				}



				
				//move and size the new item
				RectTransform rectTransform = newItem.GetComponent<RectTransform>();
				
				containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight);
				containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, 0);
				
				float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
				float y = containerRectTransform.rect.height / 2 - height * j;
				rectTransform.offsetMin = new Vector2(x, y);
				
				x = rectTransform.offsetMin.x + width;
				y = rectTransform.offsetMin.y + height;
				rectTransform.offsetMax = new Vector2(x, y);
			}

				_scrollbar.SetActive(true);
				_no_elements_label.SetActive(false);
		}
		else
		{
				Debug.Log ("No elements to show");
				_no_elements_label.GetComponent<Text>().text = translations.getString ("no_elements");
				_scrollbar.SetActive(false);
				_no_elements_label.SetActive(true);


		}
			
			
		}


		
	}

	

}
