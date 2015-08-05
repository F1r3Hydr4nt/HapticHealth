using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GUI_module : MonoBehaviour {

	/***
	 * Description:
	 * GUI module: made to activate or deactivate canvas
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/

	public Canvas _c_login;
	public Canvas _c_coach_mainmenu;
	public Canvas _c_player_mainmenu;
	public Canvas _c_coach_leftpanel;
	public Canvas _c_player_leftpanel;
	public Canvas _c_toolbar;

	public Canvas _c_players_panel;
	public Canvas _c_discipline_panel;
	public Canvas _c_player_info_panel;
	public Canvas _c_trials;
	public Canvas _c_record;
	public Canvas _c_compare;
	public Canvas _c_personaldata_panel;
	public Canvas _c_changepassword_panel;
	public Canvas _c_changelanguage_panel;

	public Canvas _c_coach_activities_panel;
	public Canvas _c_coach_feedbacks_panel;

	public Canvas _c_clubs_panel;
	public Canvas _c_teams_panel;
	public Canvas _c_player_personal_data_panel;
	public Canvas _c_player_activity_panel;
	public Canvas _c_player_statistics_panel;
	public Canvas _c_player_feedbacks_panel;
	public Canvas _c_skills_panel;

	public Canvas _c_activity_panel;
	public Canvas _c_feedback_panel;

	public Canvas _c_heroes_panel;

	private Database_module _database;
	private Lang translations;

	public LoginPanel login_GUI;
	public Coach_MainMenu_Panel coach_mainmenu_GUI;
	public Preferences_PersonalDataPanel preferences_personaldata_GUI;
	public Preferences_ChangePasswordPanel preferences_changepassword_GUI;
	public ChangeLanguagePanel changelanguage_GUI;
	public Coach_LeftPanel coach_leftpanel_GUI;
	public ClubList club_GUI;
	public TeamList team_GUI;
	public PlayerList player_GUI;
	public Player_InfoPanel player_infopanel_GUI;
	public Player_PersonalData player_personaldata_GUI;
	public Player_ActivitiesPanel player_activities_GUI;
	public Player_FeedbacksPanel player_feedbacks_GUI;
	public Activity_InfoPanel activity_GUI;
	public Feedback_InfoPanel feedback_GUI;
	public HeroeList heroe_GUI;
	public SkillPanel skill_GUI;
	public CoachActivities_Panel coach_activities_GUI;
	public CoachFeedbacks_Panel coach_feedbacks_GUI;

	// Use this for initialization
	void Start () {
		_database = new Database_module ();
		GlobalVariables.locale = "gb";
		translations = new Lang (Path.Combine (Application.dataPath, "Resources/lang.xml"), GlobalVariables.locale, false);

		
		GlobalVariables.translations = translations;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetLoginMenuVisibility(bool state){
		login_GUI.SetPanelVisibility(state);
		print ("Change Visibility of login to: "+state);
		//_c_login.active = state;
		if (state == true) {
			SetLoginErrorVisibility (false);
			CleanLoginTextfields();
		}
	}

	public void CleanLoginTextfields(){
		GameObject.Find ("cl_input_username").GetComponent<InputField>().text = "";
		GameObject.Find("cl_input_password").GetComponent<InputField>().text = "";
	}

	public void SetLoginErrorVisibility(bool state){
		print ("Change Visibility of login error to: "+state);
		GameObject.Find("cl_label_login_error").GetComponent<Text>().enabled = state;
	}


	public void SetCoachMainMenuVisibility(bool state){
		coach_mainmenu_GUI.SetPanelVisibility (state);
	}

	public void SetPlayerMainMenuVisibility(bool state){
		_c_player_mainmenu.active = state;
	}
	
	public void SetCoachLeftPanelVisibility(bool state){
		coach_leftpanel_GUI.SetPanelVisibility (state);
	}

	public void SetPlayerLeftPanelVisibility(bool state){
		_c_player_leftpanel.active = state;
	}

	public void SetPlayersPanelVisibility(bool state){
		player_GUI.SetPanelVisibility (state);
	}
	public void SetDisciplinePanelVisibility(bool state){
		_c_discipline_panel.active = state;
	}
	public void SetPlayerInfoPanelVisibility(bool state){
		player_infopanel_GUI.SetPanelVisibility (state);
	}
	public void SetTrialsVisibility(bool state){
		_c_trials.active = state;
	}

	public void SetRecordVisibility(bool state){
		_c_record.active = state;
	}

	public void SetComparisonVisibility(bool state){

		_c_compare.active = state;
	}

	public void SetToolBarVisibility(bool state){
		_c_toolbar.active = state;
	}

	public void SetPersonalDataPanelVisibility(bool state){
		preferences_personaldata_GUI.SetPanelVisibility (state);
	}

	public void SetChangePasswordPanelVisibility(bool state){
		preferences_changepassword_GUI.SetPanelVisibility (state);
	}

	public void SetChangeLanguagePanelVisibility(bool state){
		changelanguage_GUI.SetPanelVisibility (state);
	}

	public void SetClubsPanelVisibility(bool state){
		club_GUI.SetPanelVisibility (state);
	}

	public void SetTeamsPanelVisibility(bool state){
		team_GUI.SetPanelVisibility(state);
		//_c_teams_panel.active = state;        
	}

	public void SetPlayerPersonalDataPanelVisibility(bool state){
		player_personaldata_GUI.SetPanelVisibility (state);
	}

	public void SetPlayerActivityPanelVisibility(bool state){
		player_activities_GUI.SetPanelVisibility (state);
	}

	public void SetPlayerStatisticsPanelVisibility(bool state){
		_c_player_statistics_panel.active = state;
	}

	public void SetPlayerFeedbacksPanelVisibility(bool state){
		player_feedbacks_GUI.SetPanelVisibility (state);
	}


	public void SetSkillsPanelVisibility(bool state){
		skill_GUI.SetPanelVisibility (state);
	}


	public void SetCoachActivitiesPanelVisibility(bool state){
		coach_activities_GUI.SetPanelVisibility (state);
	}

	public void SetCoachFeedbacksPanelVisibility(bool state){
		coach_feedbacks_GUI.SetPanelVisibility (state);
	}

	public void SetActivityPanelVisibility(bool state){
		activity_GUI.SetPanelVisibility (state);
	}
	
	public void SetFeedbackPanelVisibility(bool state){
		feedback_GUI.SetPanelVisibility (state);
	}

	public void SetHeroesPanelVisibility(bool state){
		heroe_GUI.SetPanelVisibility (state);
	}

}
