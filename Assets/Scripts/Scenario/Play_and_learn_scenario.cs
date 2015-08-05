using UnityEngine;
using System.Collections;

public class Play_and_learn_scenario : MonoBehaviour
{

    //Modules
    private Capture_module _capture;
    private Compare_module _compare;
    private Animation_module _animate;
    private Recorded_animation_module _animate_trial;
    private Interaction_module _interact;
    private GUI_module_play_and_learn _user_interface;
    private Database_module _database;

    //Scenario and capture info
    private Device _device;
    private Modality _modality;
    private int _number_of_kinect = 0;
    private int _number_of_WIMUs = 0;


    public Play_scenario_state _play_scenario_state;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initialize_Modules(Capture_module capture, Compare_module compare, Animation_module animate, Interaction_module interact, GUI_module_play_and_learn user_interface, Recorded_animation_module animate_trial)
    {
        _play_scenario_state = Play_scenario_state.START_SCREEN;

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
        _compare.DrawPlots = false;
        //Initialise database module

        //Initialise GUI module
        //TEMP
        _play_scenario_state = Play_scenario_state.MAIN_MENU;
        Animation_data temp = new Animation_data();
        temp.Initialize_from_file("test_recorded_data/HB_Underarm_reference");
        _animate_trial.SetRecordedAnimation(temp);

        Animation_data temp_2 = new Animation_data();
        temp_2.Initialize_from_file("test_recorded_data/HB_Underarm_reference_copy");
        _animate_trial.SetReferenceAnimation(temp_2);


        /*_user_interface.SetLoginMenuVisibility (true);*/
        _user_interface.SetChildMainMenuVisibility(true);
        _user_interface.SetChildLeftPanelVisibility(true);
    }
    public void Initialize_Scenario(Device device, Modality modality, int kinect = 0, int wimus = 0)
    {
        _device = device;
        _modality = modality;
        _number_of_kinect = kinect;
        _number_of_WIMUs = wimus;
    }




    public void GoToPersonalData()
    {
        hidePanels();

        _user_interface.SetPersonalDataPanelVisibility(true);
        _play_scenario_state = Play_scenario_state.PREFERENCES;

    }


    public void GoToChangeLanguage()
    {

        hidePanels();

        _user_interface.SetChangeLanguagePanelVisibility(true);
        _play_scenario_state = Play_scenario_state.PREFERENCES;

    }

    public void GoToMainMenu()
    {
        hidePanels();

        _user_interface.SetChildLeftPanelVisibility(true);
        _user_interface.SetChildMainMenuVisibility(true);
        _play_scenario_state = Play_scenario_state.MAIN_MENU;

    }

    public void GoToModalities()
    {
        hidePanels();

        _user_interface.SetChildLeftPanelVisibility(true);
        _user_interface.SetModalityPanelVisibility(true);
        _play_scenario_state = Play_scenario_state.MODALITY_MENU;
    }

    public void GoToDisciplines()
    {
        hidePanels();

        _user_interface.SetChildLeftPanelVisibility(true);
        _user_interface.SetDisciplinePanelVisibility(true);
        _play_scenario_state = Play_scenario_state.DISCIPLINE_MENU;
    }

    public void GoToSkills()
    {
        hidePanels();

        _user_interface.SetChildLeftPanelVisibility(true);
        _user_interface.SetSkillPanelVisibility(true);
        _play_scenario_state = Play_scenario_state.SKILL_MENU;
    }

    public void GoToHeroes()
    {
        hidePanels();

        _user_interface.SetChildLeftPanelVisibility(true);
        _user_interface.SetHeroePanelVisibility(true);
        _play_scenario_state = Play_scenario_state.HEROE_MENU;

    }

    private void hidePanels()
    {
        switch (_play_scenario_state)
        {
            case Play_scenario_state.DISCIPLINE_MENU:
                _user_interface.SetDisciplinePanelVisibility(false);
                break;
            case Play_scenario_state.MODALITY_MENU:
                _user_interface.SetModalityPanelVisibility(false);
                break;
            case Play_scenario_state.PREFERENCES:
                _user_interface.SetPersonalDataPanelVisibility(false);
                _user_interface.SetChangeLanguagePanelVisibility(false);
                break;
            case Play_scenario_state.MAIN_MENU:
                _user_interface.SetChildMainMenuVisibility(false);
                break;
            case Play_scenario_state.SKILL_MENU:
                _user_interface.SetSkillPanelVisibility(false);
                break;
            case Play_scenario_state.HEROE_MENU:
                _user_interface.SetHeroePanelVisibility(false);
                break;
        }

    }

    public void ReloadCoachLeftPanel()
    {
        _user_interface.child_leftpanel_GUI.Refresh_button_currentselection();
    }


}