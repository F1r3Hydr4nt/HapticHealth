using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Coach_and_train_scenario : MonoBehaviour {
	
	/***
	 * Description:
	 * Coach and train scenario
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/


	//Modules
	private Capture_module _capture;
	private Compare_module _compare;
	private Animation_module _animate;
	private Recorded_animation_module _animate_trial;
	private Interaction_module _interact;
	private GUI_module _user_interface;
	private Database_module _database;

	//Scenario and capture info
	private Device _device;
	private Modality _modality;
	private int _number_of_kinect = 0;
	private int _number_of_WIMUs = 0;

	
	public Coach_scenario_state _coach_scenario_state;
	public GameObject button_club;
	public GameObject button_team;
	public GameObject button_player;
	public GameObject button_skill;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize_Modules(Capture_module capture, Compare_module compare, Animation_module animate,Interaction_module interact, GUI_module user_interface, Recorded_animation_module animate_trial){
		_coach_scenario_state = Coach_scenario_state.START_SCREEN;

		_capture = capture;
		_compare = compare;
		_animate = animate;
		_interact = interact;
		_user_interface = user_interface;
		_animate_trial = animate_trial;
		//Initialise 3D environment

		//Initialise 3D animation

		//Initialise capture module
		_capture.Initialise_capture(_device);
		//Initialise comparison module
        _compare.Initialise_comparison();
        _compare.DrawPlots = true;
		//Initialise database module

		//Initialise GUI module
		//TEMP
		_coach_scenario_state = Coach_scenario_state.MAIN_MENU;
		Animation_data temp = new Animation_data();
		temp.Initialize_from_file("test_recorded_data/HB_Underarm_reference");
		_animate_trial.SetRecordedAnimation(temp);

		Animation_data temp_2 = new Animation_data();
		temp_2.Initialize_from_file("test_recorded_data/HB_Underarm_reference_copy");
		_animate_trial.SetReferenceAnimation(temp_2);


		/*_user_interface.SetLoginMenuVisibility (true);*/
		_user_interface.SetCoachMainMenuVisibility(true);
		_user_interface.SetCoachLeftPanelVisibility(true);
	}
	public void Initialize_Scenario(Device device, Modality modality, int kinect = 0, int wimus = 0){
		_device = device;
		_modality = modality;
		_number_of_kinect = kinect;
		_number_of_WIMUs = wimus;
	}

	public void Login(string username, string password){
		//string username = GameObject.Find("cl_input_username").GetComponent<InputField>().value;
		//string password = GameObject.Find("cl_input_password").GetComponent<InputField>().value;

		_database = new Database_module ();

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
				//Application.LoadLevel ("Coach_and_train");
				_user_interface.SetLoginMenuVisibility(false);
				_user_interface.SetCoachMainMenuVisibility(true);
				_user_interface.SetCoachLeftPanelVisibility(true);
			}
			else if (user.usertype_id==(int)User_type.PLAYER)
			{
				//It's a player
				_user_interface.SetLoginMenuVisibility(false);
				_user_interface.SetPlayerMainMenuVisibility(true);
				_user_interface.SetPlayerLeftPanelVisibility(true);
			}
			_coach_scenario_state = Coach_scenario_state.MAIN_MENU;
		}
		else
		{
			Debug.Log ("no user");
			_user_interface.SetLoginErrorVisibility(true);
			_user_interface.CleanLoginTextfields();
		}
		Debug.Log(username+" "+password);

	}

	public void NewTrial(){
		
		_coach_scenario_state = Coach_scenario_state.NEW_TRIAL;
		
		_user_interface.SetCoachLeftPanelVisibility(false);
		_user_interface.SetPlayerInfoPanelVisibility(false);
		_user_interface.SetTrialsVisibility(false);
		_user_interface.SetToolBarVisibility(true);
		_user_interface.SetRecordVisibility(true);
		
		//Clean recorded data for the capturing module
		_capture.Prepare_new_capture();
		
		
		//Initialise comparison module with the corresponding reference
		//TODO: load the corresponding file
		var default_act = Compare.Utilities.Properties.Activity.DEFAULT;
		_compare.Initialise_comparison(default_act);
		
		Debug.Log ("New trial ");
	}
	
	public void Start_recording(){
		print ("Start recording");
		_capture.Start_capture();
	}
	
	public void Stop_recording(){
		print ("Stop recording");
		_user_interface.SetRecordVisibility(false);
		_compare.Compute_comparison(_capture.Stop_capture());
		_user_interface.SetComparisonVisibility(true);
		_animate_trial.PlayAnimation();
	}
	
	public void VisualizeTrial(string trial_name){
		Debug.Log ("Visualize existing trial: "+trial_name);
		_user_interface.SetTrialsVisibility(false);
	}
	
	public void Logout(){
		hidePanels ();
		/*_user_interface.SetLoginMenuVisibility(true);
		_user_interface.SetCoachLeftPanelVisibility (false);
		_user_interface.SetPlayerLeftPanelVisibility (false);*/
		
		_coach_scenario_state = Coach_scenario_state.START_SCREEN;
		GlobalVariables.user_id = 0;
		Application.LoadLevel ("Login");
	}
	
	public void GoToMainMenu(){
		hidePanels ();
		
		//If it's coach
		if (GlobalVariables.user.usertype_id == (int)User_type.COACH) 
		{
			_user_interface.SetCoachLeftPanelVisibility (true);
			_user_interface.SetCoachMainMenuVisibility (true);
			
			//UpdateCoachPanel ();
		} else 
		{
			_user_interface.SetPlayerLeftPanelVisibility(true);
			_user_interface.SetPlayerMainMenuVisibility(true);
		}
		
		_coach_scenario_state = Coach_scenario_state.MAIN_MENU;
		
	}
	
	
	
	
	public void GoToPersonalData(){
		hidePanels ();
		
		//If it's coach
		if (GlobalVariables.usertype_id == 1) 
		{
			//_user_interface.SetCoachLeftPanelVisibility(true);
			_user_interface.SetCoachMainMenuVisibility(false);
			
			//UpdateCoachPanel ();
		} else 
		{
			//_user_interface.SetPlayerLeftPanelVisibility(true);
			_user_interface.SetPlayerMainMenuVisibility(false);
		}
		
		_user_interface.SetPersonalDataPanelVisibility (true);
		_user_interface.SetChangePasswordPanelVisibility (false);
		_coach_scenario_state = Coach_scenario_state.PREFERENCES;
		
	}
	
	public void GoToChangePassword(){
		_user_interface.SetPersonalDataPanelVisibility (false);
		_user_interface.SetChangePasswordPanelVisibility (true);	
		_coach_scenario_state = Coach_scenario_state.PREFERENCES;
		
	}
	
	public void GoToChangeLanguage(){
		
		hidePanels ();
		
		_user_interface.SetChangeLanguagePanelVisibility (true);
		_coach_scenario_state = Coach_scenario_state.PREFERENCES;
		
	}
	
	/*public void saveCoach(Coach coach){
		//string name = GameObject.Find("ccp_input_name").GetComponent<InputField>().value;
		//string surname = GameObject.Find("ccp_input_surname").GetComponent<InputField>().value;
		//string birthdate = GameObject.Find("ccp_input_birthdate").GetComponent<InputField>().value;
			
		_database = new Database_module ();

		_database.saveCoach (coach);
		
		Debug.Log("Coach saved");
		
	}*/
	
	
	public void GoToClubs(){
		hidePanels ();
		
		_user_interface.SetClubsPanelVisibility(true);
		_coach_scenario_state = Coach_scenario_state.CLUB_MENU;
		//UpdateCoachPanel ();
	}
	
	
	public void GoToTeams()
	{
		hidePanels ();
		
		Debug.Log ("The club selected: " + GlobalVariables.selected_club_id);
		_user_interface.SetTeamsPanelVisibility(true);
		
		_coach_scenario_state = Coach_scenario_state.TEAM_MENU;
		//UpdateCoachPanel ();
	}
	
	public void GoToPlayers()
	{
		Debug.Log ("The team selected: " + GlobalVariables.selected_team_id);
		hidePanels ();
		
		_user_interface.SetPlayersPanelVisibility (true);
		
		_coach_scenario_state = Coach_scenario_state.PLAYER_MENU;
		//UpdateCoachPanel ();
	}
	
	public void GoToPlayerPersonalData()
	{
		Debug.Log ("The player selected: " + GlobalVariables.selected_player_id);
		hidePanels ();
		
		_user_interface.SetPlayerPersonalDataPanelVisibility (true);
		_user_interface.SetPlayerActivityPanelVisibility (false);
		_user_interface.SetPlayerStatisticsPanelVisibility (false);
		_user_interface.SetPlayerFeedbacksPanelVisibility (false);
		_user_interface.SetPlayerInfoPanelVisibility (true);
		
		_coach_scenario_state = Coach_scenario_state.PLAYER_PERSONAL_DATA;
		//UpdateCoachPanel ();
	}
	
	public void GoToNewPlayer()
	{
		GlobalVariables.selected_player_id = 0;
		Debug.Log ("The player selected: " + GlobalVariables.selected_player_id);
		hidePanels ();
		
		_user_interface.SetPlayerPersonalDataPanelVisibility (true);
		_user_interface.SetPlayerActivityPanelVisibility (false);
		_user_interface.SetPlayerStatisticsPanelVisibility (false);
		_user_interface.SetPlayerFeedbacksPanelVisibility (false);
		_user_interface.SetPlayerInfoPanelVisibility (true);
		
		_coach_scenario_state = Coach_scenario_state.PLAYER_PERSONAL_DATA;
		//UpdateCoachPanel ();
	}
	
	public void GoToPlayerActivities()
	{
		Debug.Log ("The player selected: " + GlobalVariables.selected_player_id);
		hidePanels ();
		_user_interface.SetPlayerActivityPanelVisibility (true);
		_user_interface.SetPlayerInfoPanelVisibility (true);
		_coach_scenario_state = Coach_scenario_state.PLAYER_ACTIVITY_MENU;
		//UpdateCoachPanel ();
	}
	
	
	public void GoToPlayerStatistics()
	{
		Debug.Log ("The player selected: " + GlobalVariables.selected_player_id);
		hidePanels ();
		_user_interface.SetPlayerStatisticsPanelVisibility (true);
		_user_interface.SetPlayerInfoPanelVisibility (true);
		_coach_scenario_state = Coach_scenario_state.PLAYER_STATISTIC_MENU;
		//UpdateCoachPanel ();
	}
	
	public void GoToPlayerFeedbacks()
	{
		Debug.Log ("The player selected: " + GlobalVariables.selected_player_id);
		hidePanels ();
		_user_interface.SetPlayerFeedbacksPanelVisibility (true);
		_user_interface.SetPlayerInfoPanelVisibility (true);
		_coach_scenario_state = Coach_scenario_state.PLAYER_FEEDBACK_MENU;
		//UpdateCoachPanel ();
	}
	
	public void GoToSkills()
	{
		Debug.Log ("The player selected: " + GlobalVariables.selected_player_id);
		hidePanels ();
		_user_interface.SetSkillsPanelVisibility(true);
		_user_interface.SetPlayerInfoPanelVisibility (true);
		_coach_scenario_state = Coach_scenario_state.SKILL_MENU;
		//UpdateCoachPanel ();
	}
	
	public void GoToActivity()
	{
		Debug.Log ("The activity selected: " + GlobalVariables.selected_activity_id);
		hidePanels ();
		_user_interface.SetPlayerInfoPanelVisibility(true);
		_user_interface.SetActivityPanelVisibility (true);
		_coach_scenario_state = Coach_scenario_state.ACTIVITY;
		//UpdateCoachPanel ();
	}
	
	public void GoToFeedback()
	{
		Debug.Log ("The feedback selected: " + GlobalVariables.selected_feedback_id);
		hidePanels ();
		_user_interface.SetPlayerInfoPanelVisibility(true);
		_user_interface.SetFeedbackPanelVisibility (true);
		_coach_scenario_state = Coach_scenario_state.FEEDBACK;
		//UpdateCoachPanel ();
	}
	
	
	
	
	public void GoToCoachActivities(){
		hidePanels ();
		_user_interface.SetCoachActivitiesPanelVisibility(true);
		
		_coach_scenario_state = Coach_scenario_state.COACH_ACTIVITY_MENU;
		//UpdateCoachPanel ();
	}
	
	
	public void GoToCoachFeedbacks(){
		
		hidePanels ();
		
		_user_interface.SetCoachFeedbacksPanelVisibility (true);
		
		_coach_scenario_state = Coach_scenario_state.COACH_FEEDBACK_MENU;
		//UpdateCoachPanel ();
	}
	
	
	public void GoToHeroes(){
		
		hidePanels ();
		
		_user_interface.SetHeroesPanelVisibility(true);
		_user_interface.SetPlayerInfoPanelVisibility (true);
		
		_coach_scenario_state = Coach_scenario_state.HERO_MENU;
		//UpdateCoachPanel ();
	}
	
	private void hidePanels()
	{
		switch (_coach_scenario_state){
		case Coach_scenario_state.PLAYER_MENU:
			_user_interface.SetPlayersPanelVisibility(false);
			break;
		case Coach_scenario_state.TRIALS_MENU:
			_user_interface.SetPlayerInfoPanelVisibility(false);
			_user_interface.SetTrialsVisibility(false);;
			break;
		case Coach_scenario_state.DISCIPLINE_MENU:
			_user_interface.SetDisciplinePanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility(false);
			break;
		case Coach_scenario_state.PREFERENCES:
			_user_interface.SetPersonalDataPanelVisibility (false);
			_user_interface.SetChangePasswordPanelVisibility (false);
			_user_interface.SetChangeLanguagePanelVisibility (false);
			break;
		case Coach_scenario_state.CLUB_MENU:
			_user_interface.SetClubsPanelVisibility(false);
			break;
		case Coach_scenario_state.TEAM_MENU:
			_user_interface.SetTeamsPanelVisibility(false);
			break;
		case Coach_scenario_state.MAIN_MENU:
			_user_interface.SetCoachMainMenuVisibility(false);
			_user_interface.SetPlayerMainMenuVisibility(false);
			break;
		case Coach_scenario_state.SKILL_MENU:
			_user_interface.SetSkillsPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		case Coach_scenario_state.COACH_ACTIVITY_MENU:
			_user_interface.SetCoachActivitiesPanelVisibility(false);
			break;
		case Coach_scenario_state.COACH_FEEDBACK_MENU:
			_user_interface.SetCoachFeedbacksPanelVisibility(false);
			break;
		case Coach_scenario_state.PLAYER_PERSONAL_DATA:
			_user_interface.SetPlayerPersonalDataPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		case Coach_scenario_state.PLAYER_ACTIVITY_MENU:
			_user_interface.SetPlayerActivityPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		case Coach_scenario_state.PLAYER_STATISTIC_MENU:
			_user_interface.SetPlayerStatisticsPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		case Coach_scenario_state.PLAYER_FEEDBACK_MENU:
			_user_interface.SetPlayerFeedbacksPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		case Coach_scenario_state.ACTIVITY:
			_user_interface.SetActivityPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		case Coach_scenario_state.FEEDBACK:
			_user_interface.SetFeedbackPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		case Coach_scenario_state.HERO_MENU:
			_user_interface.SetHeroesPanelVisibility(false);
			_user_interface.SetPlayerInfoPanelVisibility (false);
			break;
		}
		
	}
	
	public void ReloadCoachLeftPanel(){
		_user_interface.coach_leftpanel_GUI.Refresh_button_currentselection();
	}

	public void GoToInitializationScene(){
		Application.LoadLevel ("Coach_and_train_initialization");
	}

	public void GoToAnimationScene(){
		Application.LoadLevel ("Coach_and_train_animation");
	}

	public void GoToCaptureScene(){
		Application.LoadLevel ("Coach_and_train_capturing");
	}

	public void GoToComparisonScene(){
		Application.LoadLevel ("Coach_and_train_comparison");
	}

	public void GoToViewComparisonScene(){
		Application.LoadLevel ("Coach_and_train_viewcomparison");
	}



}
