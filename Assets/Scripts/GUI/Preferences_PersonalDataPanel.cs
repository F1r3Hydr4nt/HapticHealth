using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Preferences_PersonalDataPanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Coach_and_train_scenario coach_and_train_scenario;

	public GameObject _tab_personaldata;
	public GameObject _tab_changepassword;
	public GameObject _title;
	public GameObject _subtitle;
	public GameObject _label_name;
	public GameObject _label_surname;
	public GameObject _label_birthdate;
	public GameObject _input_name;
	public GameObject _input_surname;
	public GameObject _input_birthdate;
	public GameObject _button_save;

	public Canvas _preferences_personaldata_canvas;
	public Coach_LeftPanel _coach_leftpanel;

	private User user;
	private Coach coach;
	private Player player;
	private Coach_scenario_state state;
	private Database_module _database;
	private Lang translations;
	

	void Start()
	{
		state = coach_and_train_scenario._coach_scenario_state;
		_database = new Database_module ();
		
		
	}
	
	
	public void SetPanelVisibility(bool state)
	{
		_preferences_personaldata_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_preferences_personaldata_canvas.active = false;
	}
	public void Show_panel()
	{
		_preferences_personaldata_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		translations = GlobalVariables.translations;
		_title.GetComponent<Text> ().text = translations.getString ("preferences");
		_subtitle.GetComponent<Text> ().text = translations.getString ("change_password");
		
		_tab_personaldata.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text= translations.getString ("personal_data") ;
		_tab_changepassword.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text= translations.getString ("change_password") ;
		
		_label_name.GetComponent<Text> ().text = translations.getString ("name");
		_label_surname.GetComponent<Text> ().text = translations.getString ("surname");
		_label_birthdate.GetComponent<Text> ().text = translations.getString ("birthdate");
		
		_button_save.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text = translations.getString ("save") ;
		

		
		user = GlobalVariables.user;
		string name = "";
		string surname="";
		string birthdate="";
		
		//Load the data depending if coach or player
		switch (user.usertype_id) {
		case (int)User_type.COACH:
			coach = _database.getCoach (GlobalVariables.user.id);
			name = coach.name;
			surname = coach.surname;
			birthdate = coach.birthdate;
			break;
		case  (int)User_type.PLAYER:
			player = _database.getPlayer(GlobalVariables.user.id);
			name = player.name;
			surname = player.surname;
			birthdate = player.birthdate;
			break;
		}
		
		_input_name.GetComponent<InputField> ().text = name;
		_input_surname.GetComponent<InputField> ().text = surname;
		_input_birthdate.GetComponent<InputField> ().text = birthdate;
		
		//Save the data in case is coach or player
		_button_save.GetComponent<Button>().onClick.AddListener(() => { 
			
			name = _input_name.GetComponent<InputField> ().text;
			surname = _input_surname.GetComponent<InputField> ().text;
			birthdate = _input_birthdate.GetComponent<InputField> ().text;
			
			//If the data are not empty it saves
			if (name!="" && surname!="" && birthdate!="")
			{
				switch (user.usertype_id) {
				case (int)User_type.COACH:
					coach = _database.getCoach (GlobalVariables.user.id);
					coach.name = name;
					coach.surname = surname;
					coach.birthdate = birthdate;
					_database.saveCoach(coach);
					break;
				case  (int)User_type.PLAYER:
					player = _database.getPlayer(GlobalVariables.user.id);
					player.name = name;
					player.surname = surname;
					player.birthdate = birthdate;
					_database.savePlayer(player);
					break;
				}
				Debug.Log ("Preferences saved");
				_coach_leftpanel.Refresh_panel();
			}
			else
			{
				Debug.Log ("Error saving preferences");
			}
			
			
		}); 
		
	}

}
