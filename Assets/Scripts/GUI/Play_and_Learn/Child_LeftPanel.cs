using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Child_LeftPanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Play_and_learn_scenario play_and_learn_scenario;
	public GameObject _player_name;
	public GameObject _label_currentselection;
	public GameObject button_modality;
	public GameObject button_discipline;
	public GameObject button_skill;

	public GameObject _button_language;
	public Canvas _coach_leftpanel_canvas;

	private Modality_Replay modality;
	private Discipline discipline;
	private Skill skill;
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
		
		Player player = _database.getPlayer(GlobalVariables.user_id);
		_player_name.GetComponent<Text>().text = player.name + " " + player.surname;
		_label_currentselection.GetComponent<Text> ().text = translations.getString ("current_selection");

		Refresh_button_currentselection ();
		Refresh_button_navigation ();

		
	}

	public void Refresh_button_currentselection(){
		state = play_and_learn_scenario._play_scenario_state;
		//State of the application
		switch (state){
		case Play_scenario_state.START_SCREEN:
		case Play_scenario_state.MAIN_MENU:
		case Play_scenario_state.PREFERENCES:
		case Play_scenario_state.MODALITY_MENU:
			//All buttons are not visibles
			button_modality.SetActive(false);
			button_discipline.SetActive(false);
			button_skill.SetActive(false);
			break;
		case Play_scenario_state.DISCIPLINE_MENU:
			button_modality.SetActive(true);
			button_discipline.SetActive(false);
			button_skill.SetActive(false);
			//Text on buttons
			modality = _database.getModality(GlobalVariables.selected_modality_id);
			button_modality.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=modality.name;
			
			break;
		case Play_scenario_state.SKILL_MENU:
			button_modality.SetActive(true);
			button_discipline.SetActive(true);
			button_skill.SetActive(false);
			//Text on buttons
			modality = _database.getModality(GlobalVariables.selected_modality_id);
			button_modality.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=modality.name;
			discipline = _database.getDiscipline(GlobalVariables.selected_discipline_id);
			button_discipline.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=discipline.name;
			
			break;
		case Play_scenario_state.HEROE_MENU:
			button_modality.SetActive(true);
			button_discipline.SetActive(true);
			button_skill.SetActive(true);
			//Text on buttons
			modality = _database.getModality(GlobalVariables.selected_modality_id);
			button_modality.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=modality.name;
			discipline = _database.getDiscipline(GlobalVariables.selected_discipline_id);
			button_discipline.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text=discipline.name;
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
