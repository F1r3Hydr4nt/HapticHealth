using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Canvas _player_skill_canvas;
	//public GameObject panel;
	public GameObject _player_skill_itemPrefab;
	public GameObject _player_skill_GUI_parent;
	public int itemCount = 10, columnCount = 1;
	public Coach_and_train_scenario coach_and_train_scenario;

	public GameObject _player_activity_itemPrefab;
	public GameObject _player_activity_GUI_parent;

	public GameObject _title;
	public GameObject _subtitle;

	public GameObject _scrollbar;
	public GameObject _panel;
	public GameObject _no_elements_label;
	
	private Database_module _database;
	private float width;
	private float ratio;
	private float height;
	/*private RectTransform rowRectTransform;
	private RectTransform containerRectTransform;*/
	private Lang translations;

	private List<Skill> skills;
	private int skill_id,player_id;
	
	void Start()
	{
		_database = new Database_module();
	}
	
	public void SetPanelVisibility(bool state)
	{
		_player_skill_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_player_skill_canvas.active = false;
	}
	public void Show_panel()
	{
		_player_skill_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{

		//Elements
		 _database = new Database_module ();
		translations = GlobalVariables.translations;
		_title.GetComponent<Text>().text = translations.getString ("skills");
		_subtitle.GetComponent<Text>().text = translations.getString ("activities");

		skill_id = GlobalVariables.selected_skill_id;
		player_id = GlobalVariables.selected_player_id;
		loadSkills ();
		// If there are skills it shows them
		if (skills.Count > 0) 
		{
			
			if (skill_id == -1)
			{
				skill_id = skills [0].id;
				GlobalVariables.selected_skill_id = skill_id;
			} 
			
			loadSkillActivities (skill_id);

		} else 
		{
				
		}
		
    }

	private void loadSkills()
	{
		int user_id = GlobalVariables.user_id;
		int discipline_id = GlobalVariables.selected_discipline_id;
		skills = _database.getSkills (discipline_id);
		itemCount = skills.Count;
		columnCount = 1;
		
		RectTransform rowRectTransform = _player_skill_itemPrefab.GetComponent<RectTransform>();
		RectTransform containerRectTransform = _player_skill_GUI_parent.GetComponent<RectTransform>();
		
		//calculate the width and height of each child item.
		float width = containerRectTransform.rect.width / columnCount;
		float ratio = 1 /*width / rowRectTransform.rect.width*/;
		float height = rowRectTransform.rect.height * ratio;
		
		for (int i = _player_skill_GUI_parent.transform.childCount - 1; i >= 0; i--)
		{
			// objectA is not the attached GameObject, so you can do all your checks with it.
			GameObject objectA = _player_skill_GUI_parent.transform.GetChild(i).gameObject;
			objectA.transform.parent = null;
			// Optionally destroy the objectA if not longer needed
		} 
		
		
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
			
			//create a new item, name it, and set the parent
			string name = skills[i].name;
			int id = skills[i].id;
			
			GameObject newItem = Instantiate(_player_skill_itemPrefab) as GameObject;
			newItem.name = "skill_"+id.ToString();
			newItem.transform.parent = _player_skill_GUI_parent.transform;
			newItem.transform.localScale = Vector3.one;
			
			Button button = newItem.GetComponent<Button>();
			button.GetComponent<Button>().onClick.AddListener(() => { 
				GlobalVariables.selected_skill_id = id;
				Debug.Log("clicked button id: "+id+", and name: "+name); 
				coach_and_train_scenario.ReloadCoachLeftPanel();
				//coach_and_train_scenario.GoTos();
				loadSkillActivities(id);
				
				
			}); 
			button.GetComponentsInChildren<Text>()[0].text = name;

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

	}

	private void loadSkillActivities(int skill_id)
	{
		List<Activity> activities = _database.getPlayerActivities(player_id,skill_id);
		if (activities.Count > 0) {
						itemCount = activities.Count;
						columnCount = 1;
		
						RectTransform rowRectTransform = _player_activity_itemPrefab.GetComponent<RectTransform> ();
						RectTransform containerRectTransform = _player_activity_GUI_parent.GetComponent<RectTransform> ();
		
						//calculate the width and height of each child item.
						float width = containerRectTransform.rect.width / columnCount;
						float ratio = 1 /*width / rowRectTransform.rect.width*/;
						float height = rowRectTransform.rect.height * ratio;
		
						for (int i = _player_activity_GUI_parent.transform.childCount - 1; i >= 0; i--) {
								// objectA is not the attached GameObject, so you can do all your checks with it.
								GameObject objectA = _player_activity_GUI_parent.transform.GetChild (i).gameObject;
								objectA.transform.parent = null;
								// Optionally destroy the objectA if not longer needed
						} 
		
		
						int rowCount = itemCount / columnCount;
						if (rowCount == 0)
								rowCount = 1;
						if (itemCount % rowCount > 0)
								rowCount++;
		
						//adjust the height of the container so that it will just barely fit all its children
						float scrollHeight = height * rowCount;
						float scrollWidth = width * itemCount;
						containerRectTransform.offsetMin = new Vector2 (containerRectTransform.offsetMin.x, -scrollHeight);
						containerRectTransform.offsetMax = new Vector2 (containerRectTransform.offsetMax.x, 0);
		
		
						int j = 0;
		
						for (int i = 0; i < itemCount; i++) {
								//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
								if (i % columnCount == 0)
										j++;
			
								//create a new item, name it, and set the parent
								Activity activity = activities [i];
								int id = activity.id;
								string datetime = activity.datetime;
								skill_id = activity.skill_id;
								int statistics_id = activity.statistics_id;
								Player player = _database.getPlayer (player_id);
								int team_id = GlobalVariables.selected_team_id;
			
								List<Feedback> feedbacks = activity.feedbacks;
								if (activity.feedbacks.Count == 0) {
										GlobalVariables.selected_feedback_id = 0;
										GameObject.Find ("ccs_button_edit_feedback").GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("add_feedback");
								} else {
										GameObject.Find ("ccs_feedback").GetComponent<Text> ().text = activity.feedbacks [0].text;
										GameObject.Find ("ccs_button_edit_feedback").GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("edit_feedback");
								}

								GameObject.Find ("ccs_label_datetime").GetComponent<Text> ().text = datetime;

								GameObject.Find ("ccs_button_view_activity").GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("view");
								//GameObject.Find("ccpa_label_skill").GetComponent<Text> ().text=translations.getString ("skill");
								GameObject.Find ("ccs_label_score").GetComponent<Text> ().text = translations.getString ("score");
								GameObject.Find ("ccs_label_feedback").GetComponent<Text> ().text = translations.getString ("feedback");
			
								string photo = player.photo;
								string player_name = player.name;
								string player_surname = player.surname;
								int club_id = player.club_id;
			
								Skill skill = _database.getSkill (skill_id);
								string skill_name = skill.name;
			
			
			
			
								Statistics statistics = _database.getStatistics (statistics_id);
								double score = statistics.overall_score;
			
			
								//GameObject.Find("ccs_label_player_name").GetComponent<Text> ().text=player_name+" "+player_surname;
								//GameObject.Find("ccs_skill").GetComponent<Text> ().text=skill_name;
			
								GameObject.Find ("ccs_score").GetComponent<Text> ().text = score.ToString ();
			
			
			
			
								string[] parts = photo.Split ('.');
								photo = "images/" + parts [0];
			
								Texture image = (Texture)Resources.Load (photo, typeof(Texture));
								GameObject.Find ("ccs_image").GetComponent<RawImage> ().texture = image;
			
								GameObject newItem = Instantiate (_player_activity_itemPrefab) as GameObject;
								newItem.name = "activity_" + id.ToString ();
								newItem.transform.parent = _player_activity_GUI_parent.transform;
								newItem.transform.localScale = Vector3.one;
								//Debug.Log("textos dentro: "+newItem.GetComponentsInChildren<Button>().Length.ToString());
								Button[] buttons = newItem.GetComponentsInChildren<Button> ();
								foreach (Button button in buttons) {
				
										if (button.name == "ccs_button_view_activity") {
												button.onClick.AddListener (() => { 
														GlobalVariables.selected_activity_id = id;
														GlobalVariables.selected_player_id = player_id;
														GlobalVariables.selected_club_id = club_id;
														GlobalVariables.selected_team_id = team_id;
														Debug.Log ("activity id: " + id); 
						
														coach_and_train_scenario.GoToActivity ();
						
						
												}); 
										} else if (button.name == "ccs_button_edit_feedback") {
					
					
												button.onClick.AddListener (() => { 
														GlobalVariables.selected_activity_id = id;
														GlobalVariables.selected_player_id = player_id;
														GlobalVariables.selected_club_id = club_id;
														GlobalVariables.selected_team_id = team_id;
														Debug.Log ("activity id: " + id); 
						
														coach_and_train_scenario.GoToActivity ();
						
						
												}); 
										}
				
								}
			
								//move and size the new item
								RectTransform rectTransform = newItem.GetComponent<RectTransform> ();
			
								float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
								float y = containerRectTransform.rect.height / 2 - height * j;
								rectTransform.offsetMin = new Vector2 (x, y);
			
								x = rectTransform.offsetMin.x + width;
								y = rectTransform.offsetMin.y + height;
								rectTransform.offsetMax = new Vector2 (x, y);
						}
					_scrollbar.SetActive(true);
					_no_elements_label.SetActive(false);
					_panel.SetActive(true);

				} 
				else 
				{
					Debug.Log ("No elements to show");
					_no_elements_label.GetComponent<Text>().text = translations.getString ("no_elements");
					_scrollbar.SetActive(false);
					_no_elements_label.SetActive(true);	
					_panel.SetActive(false);
				}



	}

}
