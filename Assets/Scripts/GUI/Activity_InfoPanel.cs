using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Activity_InfoPanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Coach_and_train_scenario coach_and_train_scenario;
	public Canvas _activity_canvas;

	public GameObject _tab_trialdata;
	public GameObject _tab_feedback;

	public GameObject heading_title;
	public GameObject _subtitle;

	public GameObject _label_player_name;
	public GameObject _label_skill_name;
	public GameObject _label_skill_score;
	public GameObject _label_skill_date;
	public GameObject _player_name;
	public GameObject _skill_name;
	public GameObject _skill_score;
	public GameObject _skill_date;

	public GameObject _button_retry;

	private Database_module _database;
	private Lang translations;
	
	void Start()
	{
		_database = new Database_module ();
		
	}
	
	
	public void SetPanelVisibility(bool state)
	{
		_activity_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_activity_canvas.active = false;
	}
	public void Show_panel()
	{
		_activity_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		translations = GlobalVariables.translations;
		heading_title.GetComponent<Text> ().text= translations.getString ("activity");
		_subtitle.GetComponent<Text> ().text= translations.getString ("trial_data");

		_tab_trialdata.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("trial_data");
		_tab_feedback.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("feedback");
		_label_player_name.GetComponent<Text> ().text= translations.getString ("player");
		_label_skill_name.GetComponent<Text> ().text= translations.getString ("skill");
		_label_skill_date.GetComponent<Text> ().text= translations.getString ("date");
		_label_skill_score.GetComponent<Text> ().text= translations.getString ("score");




		int user_id = GlobalVariables.user_id;
		int player_id = GlobalVariables.selected_player_id;
		int activity_id = GlobalVariables.selected_activity_id;


		Activity activity = _database.getActivity (activity_id);
		string date = activity.datetime;
		int skill_id = activity.skill_id;

		GlobalVariables.selected_skill_id = skill_id;

		Skill skill = _database.getSkill (skill_id);
		Player player = _database.getPlayer (player_id);

		Statistics statistics = _database.getStatistics (activity.statistics_id);
		

		_player_name.GetComponent<Text> ().text=player.name+ " "+player.surname;
		_skill_name.GetComponent<Text> ().text=skill.name;
		_skill_date.GetComponent<Text> ().text=activity.datetime;
		_skill_score.GetComponent<Text> ().text = statistics.overall_score.ToString();





    }

}
