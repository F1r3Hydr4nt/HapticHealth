using UnityEngine;
using System.Collections;

[System.Serializable]
public class Application_core : MonoBehaviour {
	/***
	 * Description:
	 * This module is the core of the application, developper can select high level parameters to generate application
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/
	
	//Public variables
	//Scenario
	public Coach_and_train_scenario _coach_scenario;
	public Play_and_learn_scenario _play_scenario;
	public Interact_and_preserve_scenario _interact_scenario;
	public Test_scenario _test_scenario;
	
	public Coach_and_train_scenario_capture _coach_capture;
	public C_a_T_Login _c_a_t_login;
	
	//Modules
	public Capture_module _capture;
	public Compare_module _compare;
	public Animation_module _animate;
	public Interaction_module _interact;
	public GUI_module _user_interface;
	public GUI_module_play_and_learn _user_interface_play_and_learn;
	public GUI_module_capture_coach _user_capture_interface;
	public Recorded_animation_module _animate_trials;
	
	
	//Public hidden variables
	[HideInInspector]
	public Device _device;
	[HideInInspector]
	public Scenario _scenario;
	[HideInInspector]
	public Modality _modality;
	
	private int _number_of_kinect = 0;
	private int _number_of_WIMUs = 0;
	
	// Use this for initialization
	void Start () {
		
		if(_scenario == Scenario.PLAY_AND_LEARN){
			// This is the users database where to read
			GlobalVariables.user_database="pal_replay_user.db";
			GlobalVariables.user_id=1;
			/*_play_scenario.enabled = true;
			_play_scenario.Initialize_Modules(_capture,_compare,_animate, _interact, _user_interface_play_and_learn, _animate_trials);
			_play_scenario.Initialize_Scenario(_device,_modality,_number_of_kinect, _number_of_WIMUs);*/
		}else if(_scenario == Scenario.COACH_AND_TRAIN){
			

			if (GlobalVariables.user_id>0)
			{
				//If not in capture module		
				_coach_scenario.enabled = true;
				_coach_scenario.Initialize_Modules(_capture,_compare,_animate, _interact, _user_interface, _animate_trials);
				_coach_scenario.Initialize_Scenario(_device,_modality,_number_of_kinect, _number_of_WIMUs);
				
			}
			else
			{
				_coach_scenario.enabled = false;
				_c_a_t_login.Initialize_Modules(_capture,_compare,_animate, _interact, _user_interface, _animate_trials);
				_c_a_t_login.Initialize_Scenario(_device,_modality,_number_of_kinect, _number_of_WIMUs);
				
			}
			/*_coach_scenario.enabled = true;*/
			
			
		}else if(_scenario == Scenario.INTERACT_AND_PRESERVE){
			_interact_scenario.enabled = true;
		}else if(_scenario == Scenario.TEST_DEBUG){
			_test_scenario.enabled = true;
			_test_scenario.Initialize_Modules(_capture,_compare,_animate, _interact, _user_interface, _animate_trials);
			_test_scenario.Initialize_Scenario(_device,_modality,_number_of_kinect, _number_of_WIMUs);
		}else if (_scenario == Scenario.COACH_AND_TRAIN_CAPTURE) {
			_coach_capture.enabled = true;
			_coach_capture.Initialize_Modules(_capture, _compare, _animate, _interact,_user_capture_interface, _animate_trials);
			_coach_capture.Initialize_Scenario(_device, _modality, _number_of_kinect, _number_of_WIMUs);
		}
		else {
			Debug.Log ("Scenario not choose [ERROR]");
		}
		// This is the users database where to read
		GlobalVariables.users_database="replay_users.db";
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//Get FUNCTIONS
	public Scenario GetScenario(){
		return _scenario;
	}
	public Modality GetModality(){
		return _modality;
	}
	public Device GetDevice(){
		return _device;
	}
	public int GetNumberOfKinect(){
		return _number_of_kinect;
	}
	public int GetNumberOfWIMUs(){
		return _number_of_WIMUs;
	}
	
	//Set FUNCTIONS
	public void SetScenario(Scenario input_scenario){
		_scenario = input_scenario;
	}
	public void SetModality(Modality input_modality){
		_modality = input_modality;
	}
	public void SetDevice(Device input_device){
		_device = input_device;
	}
	public void SetKinectNumber(int kinect){
		_number_of_kinect = kinect;
	}
	public void SetWIMUsNumber(int WIMUs){
		_number_of_WIMUs = WIMUs;
	}
}
