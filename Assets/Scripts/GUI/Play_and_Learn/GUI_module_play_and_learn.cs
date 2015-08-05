using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GUI_module_play_and_learn : MonoBehaviour {

	/***
	 * Description:
	 * GUI module: made to activate or deactivate canvas
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/


	private Database_module _database;
	private Lang translations;

	public Child_MainMenu_Panel child_mainmenu_GUI;
	public Preferences_ChildPersonalDataPanel preferences_personaldata_GUI;
	public Child_ChangeLanguagePanel changelanguage_GUI;
	public Child_LeftPanel child_leftpanel_GUI;
	public Child_ModalityList modality_GUI;
	public Child_DisciplineList discipline_GUI;
	public Child_SkillList skill_GUI;
	public Child_HeroeList heroe_GUI;


	// Use this for initialization
	void Start () {
		_database = new Database_module ();
		GlobalVariables.locale = "es";
		GlobalVariables.user_id = 1;
		translations = new Lang (Path.Combine (Application.dataPath, "Resources/lang.xml"), GlobalVariables.locale, false);
		GlobalVariables.translations = translations;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void SetChildMainMenuVisibility(bool state){
		child_mainmenu_GUI.SetPanelVisibility (state);
	}

	public void SetChildLeftPanelVisibility(bool state){
		child_leftpanel_GUI.SetPanelVisibility (state);
	}


	public void SetModalityPanelVisibility(bool state){
		modality_GUI.SetPanelVisibility(state);
	}

	public void SetDisciplinePanelVisibility(bool state){
		discipline_GUI.SetPanelVisibility(state);
	}

	public void SetSkillPanelVisibility(bool state){
		skill_GUI.SetPanelVisibility(state);
	}


	public void SetPersonalDataPanelVisibility(bool state){
		preferences_personaldata_GUI.SetPanelVisibility (state);
	}



	public void SetChangeLanguagePanelVisibility(bool state){
		changelanguage_GUI.SetPanelVisibility (state);
	}



	public void SetHeroePanelVisibility(bool state){
		heroe_GUI.SetPanelVisibility (state);
	}

}
