using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class LoginPanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	
	public Canvas _login_canvas;
	public Coach_and_train_scenario coach_and_train_scenario;
	public C_a_T_Login c_a_t_login;
	public GameObject label_username;
	public GameObject label_password;
	public GameObject button_start;
	
	public GameObject input_username;
	public GameObject input_password;
	private Lang translations;
	private Database_module _database;
	
	void Start()
	{
		_database = new Database_module ();
		GlobalVariables.selected_heroe_id = 0;
		GlobalVariables.selected_activity_id = 0;
		GlobalVariables.selected_skill_id = 0;
		GlobalVariables.selected_player_id = 0;
		GlobalVariables.selected_discipline_id = 0;
		GlobalVariables.selected_team_id = 0;
		GlobalVariables.selected_club_id = 0;
		GlobalVariables.user_id = 0;
		GlobalVariables.usertype_id = 0;
		
	}
	
	public void SetPanelVisibility(bool state)
	{
		_login_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_login_canvas.active = false;
	}
	public void Show_panel()
	{
		_login_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		translations = GlobalVariables.translations;
		label_username.GetComponent<Text> ().text = translations.getString ("username");
		label_password.GetComponent<Text> ().text = translations.getString ("password");
		
		
		
		Button button = button_start.GetComponent<Button> ();
		
		
		Text[] texts = button.GetComponentsInChildren<Text>();
		foreach (Text button_text in texts)
		{
			button_text.text = translations.getString ("start");
		}
		
		button_start.GetComponent<Button>().onClick.AddListener(() => { 
			
			string username = input_username.GetComponent<InputField>().text;
			string password = input_password.GetComponent<InputField>().text;
			Login(username,password);
			
		});
		
		
	}
	
	private void Login(string username, string password){
		//string username = GameObject.Find("cl_input_username").GetComponent<InputField>().value;
		//string password = GameObject.Find("cl_input_password").GetComponent<InputField>().value;
		
		
		
		User user = _database.login (username, password); 
		GlobalVariables.user = user;
		
		bool internet = _database.hasInternetAccess ();
		Debug.Log (internet);
		
		
		
		if (user.id != 0) {
			
			if (user.usertype_id==(int)User_type.COACH)
			{
				
				//It's a coach
				Coach coach = _database.getCoach(user.id);
				GlobalVariables.coach = coach;
				
				
				Application.LoadLevel ("Coach_and_train");
				
			}
			else if (user.usertype_id==(int)User_type.PLAYER)
			{
				//It's a player
				/*_user_interface.SetLoginMenuVisibility(false);
				_user_interface.SetPlayerMainMenuVisibility(true);
				_user_interface.SetPlayerLeftPanelVisibility(true);*/
			}
			//_coach_scenario_state = Coach_scenario_state.MAIN_MENU;
		}
		else
		{
			Debug.Log ("no user");
			/*_user_interface.SetLoginErrorVisibility(true);
			_user_interface.CleanLoginTextfields();*/
		}
		Debug.Log(username+" "+password);
		
	}
	
	
	
}
