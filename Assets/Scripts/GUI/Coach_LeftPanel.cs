using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Coach_LeftPanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Coach_and_train_scenario coach_and_train_scenario;
	public GameObject _coach_name;
	public GameObject _label_currentselection;
	public GameObject button_club;
	public GameObject button_team;
	public GameObject button_player;
	public GameObject button_skill;

	public GameObject _button_language;
	public Canvas _coach_leftpanel_canvas;

	private Club club;
	private Team team;
	private Player player;
	private Skill skill;
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
		_coach_leftpanel_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_coach_leftpanel_canvas.active = false;
	}
	public void Show_panel()
	{
		_coach_leftpanel_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		translations = GlobalVariables.translations;


		
		Coach coach = _database.getCoach (GlobalVariables.user_id);
		_coach_name.GetComponent<Text>().text = coach.name + " " + coach.surname;
		_label_currentselection.GetComponent<Text> ().text = translations.getString ("current_selection");

		Refresh_button_currentselection ();
		Refresh_button_navigation ();

		
	}

	public void Refresh_button_currentselection(){
		state = coach_and_train_scenario._coach_scenario_state;
		//State of the application
		switch (state){
		case Coach_scenario_state.START_SCREEN:
		case Coach_scenario_state.MAIN_MENU:
		case Coach_scenario_state.PREFERENCES:
		case Coach_scenario_state.CLUB_MENU:
			//All buttons are not visibles
			button_club.SetActive(false);
			button_team.SetActive(false);
			button_player.SetActive(false);
			button_skill.SetActive(false);
			break;
		case Coach_scenario_state.TEAM_MENU:
			//Visible club button
			button_club.SetActive(true);
			button_team.SetActive(false);
			button_player.SetActive(false);
			button_skill.SetActive(false);
			//Text on buttons
			club = _database.getClub(GlobalVariables.selected_club_id);
			button_club.GetComponent<Button>().GetComponentInChildren<Text>().text=club.name;
			break;
		case Coach_scenario_state.PLAYER_MENU:
		case Coach_scenario_state.PLAYER_PERSONAL_DATA:
		case Coach_scenario_state.PLAYER_ACTIVITY_MENU:
		case Coach_scenario_state.PLAYER_STATISTIC_MENU:
		case Coach_scenario_state.PLAYER_FEEDBACK_MENU:
			//Visible club, team
			button_club.SetActive(true);
			button_team.SetActive(true);
			button_player.SetActive(false);
			button_skill.SetActive(false);
			//Text on buttons
			club = _database.getClub(GlobalVariables.selected_club_id);
			button_club.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=club.name;
			team = _database.getTeam(GlobalVariables.selected_team_id);
			button_team.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=team.name;
			
			break;
		case Coach_scenario_state.SKILL_MENU:
			//Visible club, team
/*			button_club.SetActive(true);
			button_team.SetActive(true);
			button_player.SetActive(true);
			button_skill.SetActive(false);
			//Text on buttons
			club = _database.getClub(GlobalVariables.selected_club_id);
			button_club.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=club.name;
			team = _database.getTeam(GlobalVariables.selected_team_id);
			button_team.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=team.name;
			player = _database.getPlayer(GlobalVariables.selected_player_id);
			button_player.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=player.name+" "+player.surname;
			break;*/
		case Coach_scenario_state.HERO_MENU:
			//Visible club, team
			button_club.SetActive(true);
			button_team.SetActive(true);
			button_player.SetActive(true);
			button_skill.SetActive(true);
			//Text on buttons
			club = _database.getClub(GlobalVariables.selected_club_id);
			button_club.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=club.name;
			team = _database.getTeam(GlobalVariables.selected_team_id);
			button_team.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=team.name;
			player = _database.getPlayer(GlobalVariables.selected_player_id);
			button_player.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=player.name+" "+player.surname;
			skill = _database.getSkill(GlobalVariables.selected_skill_id);
			button_skill.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=skill.name;
			break;
		}
	}

	public void Refresh_button_navigation(){
		string photo = "images/flags/"+GlobalVariables.locale+"_icon";
		Texture	image = (Texture)Resources.Load(photo, typeof(Texture));
		_button_language.GetComponent<Button> ().GetComponentsInChildren<RawImage> ()[0].texture = image;
	}

}
