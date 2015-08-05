using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Coach_and_train_scenario_initialization : MonoBehaviour {
	
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
	private Database_module _database;

	public GUI_module_initialization _user_interface;

	//Scenario and capture info
	private Device _device;
	private Modality _modality;
	private int _number_of_kinect = 0;
	private int _number_of_WIMUs = 0;

    public Initialization_scene_state _coach_scenario_state;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize_Modules(Capture_module capture, Compare_module compare, Animation_module animate,Interaction_module interact, GUI_module_initialization _user_capture_interface, Recorded_animation_module animate_trial){
        _coach_scenario_state = Initialization_scene_state.INITIALISATION;

		_capture = capture;
		_compare = compare;
		_animate = animate;
		_interact = interact;
		_user_interface = _user_capture_interface;
		
		_animate_trial = animate_trial;
		//Initialise 3D environment

		//Initialise 3D animation

		//Initialise capture module
		_capture.Initialise_capture(_device);
		//Initialise comparison module

		//Initialise database module

		//Initialise GUI module
		//TEMP
        
		Animation_data temp = new Animation_data();
		temp.Initialize_from_file("test_recorded_data/HB_Underarm_reference");
		_animate_trial.SetRecordedAnimation(temp);

		Animation_data temp_2 = new Animation_data();
		temp_2.Initialize_from_file("test_recorded_data/HB_Underarm_reference_copy");
		_animate_trial.SetReferenceAnimation(temp_2);


        _user_interface.SetAnimationVisibility(true);
	}
	public void Initialize_Scenario(Device device, Modality modality, int kinect = 0, int wimus = 0){
		_device = device;
		_modality = modality;
		_number_of_kinect = kinect;
		_number_of_WIMUs = wimus;
	}

	

	}
