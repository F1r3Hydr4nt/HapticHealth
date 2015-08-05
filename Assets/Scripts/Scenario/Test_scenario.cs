using UnityEngine;
using System.Collections;

public class Test_scenario : MonoBehaviour {
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
	
	//Scenario and capture info
	private Device _device;
	private Modality _modality;
	private int _number_of_kinect = 0;
	private int _number_of_WIMUs = 0;
	
	
	public Test_state _test_state;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize_Modules(Capture_module capture, Compare_module compare, Animation_module animate,Interaction_module interact, GUI_module user_interface, Recorded_animation_module animate_trial){
		_test_state = Test_state.INITIALISATION;
		
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
		
		//Initialise database module
		
		//Initialise GUI module
		//TEMP
		Animation_data temp = new Animation_data();
		temp.Initialize_from_file("test_recorded_data/HB_Underarm_reference");
		_animate_trial.SetRecordedAnimation(temp);

	}

	public void Initialize_Scenario(Device device, Modality modality, int kinect = 0, int wimus = 0){
		_device = device;
		_modality = modality;
		_number_of_kinect = kinect;
		_number_of_WIMUs = wimus;
	}

	public void Start_recording(){
		print ("Start recording");
		_capture.Start_capture();
	}
	
	public void Stop_recording(){
		print ("Stop recording");
		_compare.Compute_comparison(_capture.Stop_capture());
		_user_interface.SetComparisonVisibility(true);
		_animate_trial.PlayAnimation();
	}


}
