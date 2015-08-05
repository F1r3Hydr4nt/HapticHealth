using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player_InfoPanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Coach_and_train_scenario coach_and_train_scenario;

	public Canvas _player_info_canvas;
	public GameObject _heading_name;
	public GameObject _label_playerinfo;
	public GameObject _label_player_name;
	public GameObject _label_player_surname;
	public GameObject _label_player_birthday;
	public GameObject _label_player_club;
	public GameObject _label_player_team;
	public GameObject _player_image;
	public GameObject _player_name;
	public GameObject _player_surname;
	public GameObject _player_birthday;
	public GameObject _player_club;
	public GameObject _player_team;

	public GameObject _button_edit;

	private Database_module _database;
	private Lang translations;
	
	void Start()
	{
		//Elements
		/*GUI_parent = new GameObject();
		GUI_parent.name = "Team_list";
		GUI_parent.transform.parent = _team_canvas.transform;*/
		//GUI_parent.transform.position = _team_canvas.transform.position;
		_database = new Database_module();
		
		
		
		
		//Refresh_panel();
		
	}
	
	public void SetPanelVisibility(bool state)
	{
		_player_info_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_player_info_canvas.active = false;
	}
	public void Show_panel()
	{
		_player_info_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		translations = GlobalVariables.translations;
		_button_edit.SetActive (true);
				//Data of the player
				int user_id = GlobalVariables.user_id;
				int player_id = GlobalVariables.selected_player_id;
				Player player;
		string photo;
				if (player_id != 0) {
						player = _database.getPlayer (player_id);
			
						photo = player.photo;
			
			
						string[] parts = photo.Split ('.');
						photo = "images/" + parts [0];
			
						
						_heading_name.GetComponent<Text> ().text = player.name + " " + player.surname;

						_button_edit.GetComponent<Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("edit");
				} else {
						photo ="images/user_noimage";
						player = new Player (0, "", "", 0.00, 0.00, 0, 0, "", "", "2000-01-01", GlobalVariables.selected_club_id, null, 0,1);
						_button_edit.SetActive (false);
				}
						//Translations

						_label_playerinfo.GetComponent<Text> ().text = translations.getString ("player_info");
						_label_player_name.GetComponent<Text> ().text = translations.getString ("name");
						_label_player_surname.GetComponent<Text> ().text = translations.getString ("surname");
						_label_player_birthday.GetComponent<Text> ().text = translations.getString ("birthdate");
						_label_player_club.GetComponent<Text> ().text = translations.getString ("club");
						_label_player_team.GetComponent<Text> ().text = translations.getString ("team");
						



						_player_name.GetComponent<Text> ().text = player.name;
						_player_surname.GetComponent<Text> ().text = player.surname;
						_player_birthday.GetComponent<Text> ().text = "1981-02-17";
						_player_club.GetComponent<Text> ().text = _database.getClub (GlobalVariables.selected_club_id).name;
						_player_team.GetComponent<Text> ().text = _database.getTeam (GlobalVariables.selected_team_id).name;
						Texture image = (Texture)Resources.Load (photo, typeof(Texture));
						_player_image.GetComponent<RawImage> ().texture = image;
				
		}
}
