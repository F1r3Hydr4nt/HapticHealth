using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Preferences_ChildPersonalDataPanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Play_and_learn_scenario play_and_learn_scenario;

	public GameObject _title;
	public GameObject _subtitle;
	public GameObject _label_name;
	public GameObject _label_surname;
	public GameObject _input_name;
	public GameObject _input_surname;
	public GameObject _button_save;

	public Canvas _preferences_personaldata_canvas;

	public Child_LeftPanel _child_leftpanel;

	private User user;
	private Coach coach;
	private Player player;
	private Play_scenario_state state;
	private Database_module _database;
	private Lang translations;
	

	void Start()
	{
		state = play_and_learn_scenario._play_scenario_state;
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
		_subtitle.GetComponent<Text> ().text = translations.getString ("personal_data");

		_label_name.GetComponent<Text> ().text = translations.getString ("name");
		_label_surname.GetComponent<Text> ().text = translations.getString ("surname");

		_button_save.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text = translations.getString ("save") ;
		

		
		user = GlobalVariables.user;
		string name = "";
		string surname="";
		string birthdate="";
		
		//Load the data depending if coach or player

		player = _database.getPlayer(GlobalVariables.user_id);
		name = player.name;
		surname = player.surname;
		birthdate = player.birthdate;

		
		_input_name.GetComponent<InputField> ().text = name;
		_input_surname.GetComponent<InputField> ().text = surname;

		//Save the data in case is coach or player
		_button_save.GetComponent<Button>().onClick.AddListener(() => { 
			
			name = _input_name.GetComponent<InputField> ().text;
			surname = _input_surname.GetComponent<InputField> ().text;

			//If the data are not empty it saves
			if (name!="" && surname!="")
			{

				player = _database.getPlayer(GlobalVariables.user_id);
				player.name = name;
				player.surname = surname;
				_database.savePlayer(player);
				_child_leftpanel.Refresh_panel();
				Debug.Log ("Preferences saved");
			}
			else
			{
				Debug.Log ("Error saving preferences");
			}
			
			
		}); 
		
	}

}
