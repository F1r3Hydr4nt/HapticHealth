using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Feedback_InfoPanel: MonoBehaviour
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

	public GameObject _label_datetime;
	public GameObject _label_feedback;
	public GameObject _datetime;
	public GameObject _input_feedback;
	public GameObject _button_save;
	public GameObject _button_cancel;
	public GameObject _button_delete;

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
		_subtitle.GetComponent<Text> ().text= translations.getString ("feedback");
		
		_tab_trialdata.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("trial_data");
		_tab_feedback.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("feedback");
		_label_datetime.GetComponent<Text> ().text= translations.getString ("date");
		_label_feedback.GetComponent<Text> ().text= translations.getString ("feedback");



		//Data of the player
		int user_id = GlobalVariables.user_id;
		int player_id = GlobalVariables.selected_player_id;
		int activity_id = GlobalVariables.selected_activity_id;
		int feedback_id = GlobalVariables.selected_feedback_id;

		Feedback feedback;
		if (feedback_id != 0) 
		{
			feedback = _database.getFeedback (feedback_id);

		} 
		else 
		{
			int coach_id = GlobalVariables.coach.id;
			feedback = new Feedback(0,coach_id,activity_id,"",DateTime.Today.ToString("yyyy/MM/dd H:mm:ss"),0,0,1);
		}



		_input_feedback.GetComponent<InputField> ().text = feedback.text;
		_datetime.GetComponent<Text> ().text = feedback.datetime;

		_button_save.GetComponent<Button>().onClick.AddListener(() => { 
			feedback.text = _input_feedback.GetComponent<InputField> ().text;
			_database.saveFeedback(GlobalVariables.user_database,feedback);
		});
		_button_delete.GetComponent<Button>().onClick.AddListener(() => { 
			feedback.text = _input_feedback.GetComponent<InputField> ().text;
			feedback.active = 1;
			_database.saveFeedback(GlobalVariables.user_database,feedback);
		});
		_button_cancel.GetComponent<Button>().onClick.AddListener(() => { 
			_input_feedback.GetComponent<InputField> ().text = "";
		});

    }

}
