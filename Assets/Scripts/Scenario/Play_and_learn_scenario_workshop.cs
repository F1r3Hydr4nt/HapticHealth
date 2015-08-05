using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Skill_data
{
    public int _anim_index;
    public float _anim_duration;
    public float _recording_duration;
    public string _anim_name;
    public string _display_name;
    public string _description_trial;
    public Camera_view _camera_view;
    public Compare.Utilities.Properties.Activity _compare_activity;
    public Sport _target_sport;
    public float _score;
    
    public Skill_data(int anim_index, float anim_duration, float recording_duration, string anim_name, string display_name, string description_trial, Compare.Utilities.Properties.Activity compare_activity, Camera_view camera_view, Sport target_sport)
    {
        _anim_index = anim_index;
        _anim_duration = anim_duration;
        _recording_duration = recording_duration;
        _anim_name = anim_name;
        _display_name = display_name;
        _description_trial = description_trial;
        _compare_activity = compare_activity;
        _camera_view = camera_view;
        _target_sport = target_sport;
        _score = 0.0f;

    }
}


public class Play_and_learn_scenario_workshop : MonoBehaviour {

	/***
	 * Description:
	 * Coach and train scenario for the Bilbao Workshop
     * 3 handball skills
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/
    public Modality _modality;
    public bool _debug_mode = false; //-> go directly to the recording part
    public bool _invalid_trial = true; 
    public int _number_of_trials = 1;

    private int _current_trials = 0;
    private int _current_skill = 0;

    public float _time_between_scene = 5.0f;
    private int _trial_number;
    private Animation_data latestAnimation;

	//Modules
	public Capture_module _capture;
	public Compare_module _compare;
	public Animation_module _animate;
	public Recorded_animation_module _animate_trial;
    public Interaction_module _interact;
    public Database_module _database;
    public KinectManager _kinect;
    public GUI_module_capture_play_workshop _user_interface;

	//Scenario and capture info
	private Device _device;
	private int _number_of_kinect = 0;
	private int _number_of_WIMUs = 0;

    public Capture_scene_state _scenario_state;
    
    private List<Skill_data> _skills_list; //key -> official name
    public Text _countdown;
    public AvatarController _avatar_controller;
    public int _number_of_invalid_action_limit = 2;
    private int _current_invalid_action = 0;
    private String _basque_global_instruction;
    private String _gaelic_global_instruction;

    public GameObject _basque_decor;
    public GameObject _gaelic_decor;

	// Use this for initialization
	void Start () {
        _skills_list = new List<Skill_data>();
        /*
        _skills_list.Add(new KeyValuePair<string, Skills_data>("L_Ath1_H_RHUS_02", "Right-handed underarm shot"));
        _skills_list.Add(new KeyValuePair<string, string>("L_Ath1_H_RHV02", "Right-handed volley"));
        _skills_list.Add(new KeyValuePair<string, string>("L_Ath1_H_LHSS_03", "Left-handed Slice shot"));
         * */
        //int anim_index, float anim_duration, string anim_name, string display_name, string description_trial, Compare.Utilities.Properties.Activity compare_activity
        if (_modality == Modality.BASQUE_MODALITY)
        {
            _skills_list.Add(new Skill_data(2, 5.0f, 3.0f, "L_Ath1_H_RHV02", "Right-handed volley", "Hit the ball with the right hand over your head, without bound", Compare.Utilities.Properties.Activity.HB_RHV, Camera_view.FRONT_POINT_OF_VIEW, Sport.HANDBALL));
            _skills_list.Add(new Skill_data(1, 5.0f, 3.0f, "L_Ath1_H_RHUS_02", "Right-handed underarm shot", "Hit the ball with the right hand upwards", Compare.Utilities.Properties.Activity.HB_RHUS, Camera_view.FRONT_POINT_OF_VIEW, Sport.HANDBALL));
            _skills_list.Add(new Skill_data(3, 5.0f, 3.0f, "L_Ath1_H_LHSS_03", "Left-handed Slice shot", "Hit the ball with the left hand, in a horizontal plane, to lead the ball above the lower section of the front wall", Compare.Utilities.Properties.Activity.HB_LHSS, Camera_view.FRONT_POINT_OF_VIEW, Sport.HANDBALL));
            _basque_global_instruction = "Welcome!\n\nYou are going to try to do three Handball skills:\n\n- Right-handed volley\n- Right-handed underarm shot\n- Left-handed Slice shot\n\nYou will try two times each skill";
            _user_interface.SetGlobalInstruction(_basque_global_instruction);
            _basque_decor.SetActive(true);
        }
        else if (_modality == Modality.GAELIC_MODALITY)
        {
            _skills_list.Add(new Skill_data(1, 3.0f, 3.0f, "L_Ath1_H_RHUS_02", "Strike from the Hand", "Striking from the Hand is the most common method of passing the sliotar or shooting for a score in Hurling. It is important that players are taught to strike from the dominant and non-dominant side from an early age.", Compare.Utilities.Properties.Activity.H_SFTH, Camera_view.LEFT_POINT_OF_VIEW, Sport.HURLING));
            _skills_list.Add(new Skill_data(1, 3.0f, 3.0f, "L_Ath1_H_RHUS_02", "Punk Kick", "The Punt Kick is one of the most common foot passing techniques in Gaelic football. It may also be used to kick for a score when approaching the goal straight on.", Compare.Utilities.Properties.Activity.GF_PK, Camera_view.LEFT_POINT_OF_VIEW, Sport.GAELIC_FOOTBALL));
            _gaelic_global_instruction = "Welcome!\nYou are going to try to do one Hurling skill:\n- Strike from the Hand\n\nThen you are going to try one Gaelic Football skill:\n- Punk Kick\n\nYou will try two times each skill";
            _user_interface.SetGlobalInstruction(_gaelic_global_instruction);
            _gaelic_decor.SetActive(true);
        }

        _capture.Activate_visualization(false);
        _animate_trial.AvatarVisibilityState(false);
        _capture.Initialise_capture(Device.SINGLE_KINECT_2);
        _compare.DrawPlots = false;
        
        Wait_user();
        
	}



	public void Initialize_Modules(Capture_module capture, Compare_module compare, Animation_module animate,Interaction_module interact, Recorded_animation_module animate_trial){
        _scenario_state = Capture_scene_state.INITIALISATION;

		_capture = capture;
		_compare = compare;
		_animate = animate;
		_interact = interact;
		
		_animate_trial = animate_trial;
		//Initialise 3D environment

		//Initialise 3D animation

		//Initialise capture module
		_capture.Initialise_capture(_device);
		//Initialise comparison module
        _compare.Initialise_comparison();
        _compare.DrawPlots = false;
		//Initialise database module

		//Initialise GUI module
		//TEMP
        
		Animation_data temp = new Animation_data();
		temp.Initialize_from_file("test_recorded_data/HB_Underarm_reference");
		_animate_trial.SetRecordedAnimation(temp);

		Animation_data temp_2 = new Animation_data();
		temp_2.Initialize_from_file("test_recorded_data/HB_Underarm_reference_copy");
		_animate_trial.SetReferenceAnimation(temp_2);
        
        Wait_user();

	}
	public void Initialize_Scenario(Device device, Modality modality, int kinect = 0, int wimus = 0){
		_device = device;
		_modality = modality;
		_number_of_kinect = kinect;
		_number_of_WIMUs = wimus;
	}



    public void Wait_user()
    {

        for (int i = 0; i < _skills_list.Count; i++)
        {
            _skills_list[i]._score = 0.0f;

        }
            StopAllCoroutines();
            _animate.Stop_animation();
            _scenario_state = Capture_scene_state.WAITING_USER;
            _capture.Activate_visualization(false);
            _user_interface.HideAll();
            _user_interface.SetWaitVisibility(true);
            
            //_animate.Look_at_center();

            
    }

    void Update()
    {
        /*
        if (Input.GetButton("Fire1") && _debug_mode == true)
        {
            StopAllCoroutines();
            StartCoroutine(Start_recording_debug(3));
        }*/
    }

    void User_detected()
    {
        Debug.Log("User detected");
        _user_interface.SetWaitVisibility(false);
        if (_debug_mode)
        {
            StartCoroutine(Start_recording_debug(3));
        }
        else
        {
            _capture.Activate_visualization(false);
            _current_trials = 0;
            _current_skill = 0;

            StartCoroutine(Show_instruction_screen(_time_between_scene));
        }
    }

    IEnumerator Show_instruction_screen(float waitTime)
    {
        Debug.Log("Instruction screen");
        
        _user_interface.SetGlobalInstructionVisibility(true);
        yield return new WaitForSeconds(waitTime);
        _user_interface.SetGlobalInstructionVisibility(false);
        Debug.Log("Instruction screen [OK]");
        Perform_next_trial();
    }


    void User_left()
    {
        Debug.Log("User left");
        StopAllCoroutines();
        _user_interface.HideAll();
        _user_interface.SetWaitVisibility(true);
    }

    public void Perform_next_skill()
    {
        if (_current_skill+1 < _skills_list.Count)
        {
            //Start the first trial
            _current_skill++;
            _current_trials = 0;
            Perform_next_trial();
        }
        else
        {
            //The user tried all the proposed skills -> we show the final scoreboard
            float average_score = 0.0f;
            string details_score = "";
            for (int i = 0; i < _skills_list.Count; i++ )
            {
                average_score += _skills_list[i]._score;
                details_score += "\nBest \"" + _skills_list[i]._display_name + "\" trial: " + (int)(_skills_list[i])._score+"%\n";
            }
            average_score = average_score / _skills_list.Count;
            _user_interface.SetFinalScoreVisibility(true, average_score, details_score);
        }

    }

    public void Perform_next_trial()
    {
        Debug.Log("Trial: " + _current_trials + "/" + _number_of_trials + "  for Skill: " + _current_skill + "/" + _skills_list.Count);
        if (_current_trials < _number_of_trials)
        {
            //The user make another (or the first) trial of the same skill
            
            if (_current_trials == 0)
            {
                StartCoroutine(Run_3D_animation(_time_between_scene));
            }
            else
            {
                NewTrial();
            }
            
        }
        else
        {
            Perform_next_skill();
        }

    }

    
    public void Visualize_comparison()
    {


    }
    
    public void Visualize_final_score()
    {


    }
    float _skill_duration = 0.0f;

    IEnumerator Run_3D_animation(float waitTime)
    {
        _user_interface.SetTrialGuiData(_skills_list[_current_skill]._description_trial, _skills_list[_current_skill]._display_name, _current_skill+1, _skills_list.Count);
        _user_interface.SetSkillName(_skills_list[_current_skill]._display_name);

        _user_interface.SetTrialInstructionVisibility(true);
        Debug.Log("Duration: " + _skill_duration);
        yield return new WaitForSeconds(_time_between_scene);
        _user_interface.SetTrialInstructionVisibility(false);

        _scenario_state = Capture_scene_state.VIEW_HERO_ANIM;
        _skill_duration = _animate.PlayAnimation(_current_skill, _skills_list[_current_skill]._target_sport);
        _animate.SetAnimSpeed(1.0f);
        _animate.SetCameraView(_skills_list[_current_skill]._camera_view);

        _user_interface.SetAnimationVisibility(true);
        Debug.Log("Duration: " + _skills_list[_current_skill]._anim_duration);
        yield return new WaitForSeconds(_skills_list[_current_skill]._anim_duration * 1.5f);
        
        _animate.SetAnimSpeed(0.5f);
        yield return new WaitForSeconds(_skills_list[_current_skill]._anim_duration * 2);
        /*
        _animate.SetAnimSpeed(0.25f);
        yield return new WaitForSeconds(_skills_list[_current_skill]._anim_duration * 2);*/
        _user_interface.SetAnimationVisibility(false);
        _animate.Stop_animation();
        NewTrial();
    }


	public void NewTrial(){        
       

        _animate_trial.AvatarVisibilityState(false);
        _scenario_state = Capture_scene_state.PREPARE_NEW_TRIAL;

        

		//Clean recorded data for the capturing module
		_capture.Prepare_new_capture();
        _capture.Activate_visualization(true);
        _animate.Look_at_user_avatar();
        _avatar_controller.New_record();
		//Initialise comparison module with the corresponding reference
		//TODO: load the corresponding file
		//var default_act = Compare.Utilities.Properties.Activity.GF_PK;
        _compare.Initialise_comparison(_skills_list[_current_skill]._compare_activity);

        /*
        Debug.Log("Instruction screen capture");
        _user_interface.SetTrialVisibility(true);
        yield return new WaitForSeconds(waitTime);
        _user_interface.SetTrialVisibility(false);
        Debug.Log("Instruction screen capture [OK]");
        */
        _current_trials++;
        StartCoroutine(Start_recording(_skills_list[_current_skill]._recording_duration));
		Debug.Log ("New trial ");
        
	}

    public void SaveTrial()
    {
        Debug.Log("Trial saved");
    }

	IEnumerator Start_recording(float time){
		
        Debug.Log("Start recording for "+_skill_duration+" sec");
        _compare.DoneComparison = false;

        _countdown.text = "Trial "+_current_trials+"/"+_number_of_trials;
        _user_interface.SetRecordVisibility(true);        
        yield return new WaitForSeconds(_time_between_scene);

        _countdown.text = "Get Ready";

        yield return new WaitForSeconds(3);
        _countdown.text = "3";

        _countdown.text = "Get Ready";
        yield return new WaitForSeconds(2);
        _countdown.text = "3";
        yield return new WaitForSeconds(1);
        _countdown.text = "2";
        yield return new WaitForSeconds(1);
        _countdown.text = "1";
        yield return new WaitForSeconds(1);
        _capture.Start_capture();
        _avatar_controller.Start_recording();
        _countdown.text = "Recording";
        yield return new WaitForSeconds(time);
        latestAnimation = _capture.Stop_capture();
        List<Quaternion[]> anim_data = _avatar_controller.Stop_recording();
        print("Stop recording");
        _capture.Activate_visualization(false);
        _user_interface.SetRecordVisibility(false);
        
        _compare.Compute_comparison(latestAnimation.Filename);
        //yield return new WaitForSeconds(time);
        //_animate_trial.SetRecordedAnim(latestAnimation);
        while(!_compare.DoneComparison){
            //wait end of the comparison
            Debug.Log("Wait end of comparison process");
            yield return new WaitForSeconds(0.5f);
        }
        if ((_compare.AreRelated == false) && (_invalid_trial == true))
        {
            _user_interface.SetInvalidTrialVisibility(true);
            yield return new WaitForSeconds(_time_between_scene);
            _user_interface.SetInvalidTrialVisibility(false);
            //StartCoroutine(Start_recording(time));
            Perform_next_trial();
        }else{
        if (_compare.OverallScore > _skills_list[_current_skill]._score)
        {
            _skills_list[_current_skill]._score = _compare.OverallScore;
        }

        _user_interface.SetComparisonVisibility(true, _compare.OverallScore, _compare.SemanticFeedback.ToString(), _compare.Tips, _compare.AreRelated);
        _animate.Look_at_comparison_avatar();
           //_animate_trial.GetRecordedAnimationPlayer().SetAnimationData(latestAnimation);
            _animate_trial.GetRecordedAnimationPlayer().SetAnimationDataFromController(anim_data);
            _animate_trial.AvatarVisibilityState(true);
            _animate_trial.PlayAnimation();
            /*
            _animate_trial.AvatarVisibilityState(true);
            _animate_trial.PlayAnimation();
        */
            _user_interface.SetComparisonVisibility(true, _compare.OverallScore);
            yield return new WaitForSeconds(_skill_duration * 2);

            _user_interface.SetComparisonVisibility(false);
            _animate_trial.AvatarVisibilityState(false);
 
            Perform_next_trial();
        }

	}

    IEnumerator Start_recording_debug(float time)
    {
        _avatar_controller.New_record();
        Debug.Log("Start recording for " + _skill_duration + " sec");
        _avatar_controller.Start_recording();
        _capture.Start_capture();
        _user_interface.SetRecordVisibility(true);
        yield return new WaitForSeconds(time);
        print("Stop recording");
        latestAnimation = _capture.Stop_capture();
        List<Quaternion[]> anim_data_from_controller = _avatar_controller.Stop_recording();
        Debug.Log("Received " + anim_data_from_controller.Count + " frames");
        _animate_trial.GetRecordedAnimationPlayer().SetAnimationDataFromController(anim_data_from_controller);
        _capture.Activate_visualization(false);
        _user_interface.SetRecordVisibility(false);
        
        _compare.Compute_comparison(latestAnimation.Filename);
        //yield return new WaitForSeconds(time);
        //_animate_trial.SetRecordedAnim(latestAnimation);
        //_animate_trial.GetRecordedAnimationPlayer().SetAnimationData(latestAnimation);
        _animate_trial.AvatarVisibilityState(true);
        _animate_trial.PlayAnimation();

        
        //TODO
        
        while (!_compare.DoneComparison)
        {
            //wait end of the comparison
            Debug.Log("Wait end of comparison process");
            yield return new WaitForSeconds(0.5f);
        }
        //yield return new WaitForSeconds(1.5f);
        _user_interface.SetComparisonVisibility(true, _compare.OverallScore, _compare.SemanticFeedback.ToString(), _compare.Tips);
        yield return new WaitForSeconds(_skill_duration * 2);

    }
	public void Stop_recording(){
		print ("Stop recording");
        _capture.Activate_visualization(false);
		_user_interface.SetRecordVisibility(false);
		//_compare.Compute_comparison(_capture.Stop_capture());
		_user_interface.SetComparisonVisibility(true);
        _animate_trial.AvatarVisibilityState(true);
		_animate_trial.PlayAnimation();
	}


	public void Logout(){
		//Functions TODO

	}

	



	
	}
