using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Preferences_ChangePasswordPanel: MonoBehaviour
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
	public GameObject _label_oldpassword;
	public GameObject _label_newpassword;
	public GameObject _label_retype;
	public GameObject _input_oldpassword;
	public GameObject _input_newpassword;
	public GameObject _input_retype;
	public GameObject _button_save;

	public Canvas _change_password_canvas;


	private User user;
	private string oldpassword="";
	private string newpassword="";
	private string retype="";
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
		_change_password_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_change_password_canvas.active = false;
	}
	public void Show_panel()
	{
		_change_password_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		translations = GlobalVariables.translations;
		_title.GetComponent<Text> ().text = translations.getString ("preferences");
		_subtitle.GetComponent<Text> ().text = translations.getString ("change_password");
		
		_tab_personaldata.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text= translations.getString ("personal_data") ;
		_tab_changepassword.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text= translations.getString ("change_password") ;

		_label_oldpassword.GetComponent<Text> ().text = translations.getString ("old_password");
		_label_newpassword.GetComponent<Text> ().text = translations.getString ("new_password");
		_label_retype.GetComponent<Text> ().text = translations.getString ("retype_password");

		_button_save.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text = translations.getString ("save") ;
				
		user = GlobalVariables.user;
		
		
		_button_save.GetComponent<Button>().onClick.AddListener(() => { 
			
			oldpassword = _database.HashPassword(_input_oldpassword.GetComponent<InputField> ().text);
			newpassword = _input_newpassword.GetComponent<InputField> ().text;
			retype = _input_retype.GetComponent<InputField> ().text;
			
			//If the data are not empty it saves
			if (oldpassword!="" && newpassword!="" && retype!="")
			{
				if (oldpassword ==user.password)
				{
					if (newpassword == retype)
					{
						user.password = _database.HashPassword(newpassword);
						user = _database.saveUser(GlobalVariables.user_database,user);
						user = _database.saveUser(GlobalVariables.users_database,user);
						Debug.Log ("Preferences saved");
					}
					else
					{
						Debug.Log ("Error new password and retype are not equal");
					}
					
				}
				else
				{
					Debug.Log ("Error old password is not the same");
				}
			}
			else
			{
				Debug.Log ("Error inputs are empty");
			}
			
		}); 
		
	}

}
