
/***
 * Description:
 * ENUM use in the application
 * Version: 0.1
 * Autor:
 * Yvain Tisserand - MIRALab
 *****/

public enum Device
{
    SINGLE_KINECT_1 = 0x1,
    SINGLE_KINECT_2 = 0x2,
    MULTI_KINECT = 0x3,
    KINECT_WIMUS = 0x4,
    MULTI_KINECT_WIMUS = 0x5
}
public enum Scenario
{
    PLAY_AND_LEARN = 0x1,
    COACH_AND_TRAIN = 0x2,
    INTERACT_AND_PRESERVE = 0x3,
    COACH_AND_TRAIN_CAPTURE = 0x4,
    TEST_DEBUG = 0x5,
	COACH_AND_TRAIN_INITIALIZATION = 0x6,
	COACH_AND_TRAIN_ANIMATION = 0x7,
	COACH_AND_TRAIN_CAPTURING = 0x8,
	COACH_AND_TRAIN_COMPARISON = 0x9,
	COACH_AND_TRAIN_VIEWCOMPARISON = 0x10

}

public enum Modality
{
    GAELIC_MODALITY = 0x1,
    BASQUE_MODALITY = 0x2
}


public enum Coach_scenario_state
{
    INITIALISATION = 0x1,
    START_SCREEN = 0x2,
    MAIN_MENU = 0x3,
    CLUB_MENU = 0x4,
    TEAM_MENU = 0x5,
    PLAYER_MENU = 0x6,
    DISCIPLINE_MENU = 0x7,
    TRIALS_MENU = 0x8,
    VIEW_TRIAL = 0x9,
    NEW_TRIAL = 0x10,
    PREFERENCES = 0x11,
    PLAYER_PERSONAL_DATA = 0x12,
    PLAYER_ACTIVITY_MENU = 0x13,
    PLAYER_STATISTIC_MENU = 0x14,
    PLAYER_FEEDBACK_MENU = 0x15,
    SKILL_MENU = 0x16,
    COACH_ACTIVITY_MENU = 0x17,
    COACH_FEEDBACK_MENU = 0x18,
    ACTIVITY = 0x19,
    FEEDBACK = 0x20,
    HERO_MENU = 0X21
}

public enum Animation_player_state
{
    INITIALISATION = 0x1,
    STOP = 0x2,
    PLAY = 0x3,
    PAUSE = 0x4
}

public enum Test_state
{
    INITIALISATION = 0x1,
    RECORD = 0x2,
    COMPARE = 0x3
}

public enum User_type
{
    COACH = 0x1,
    PLAYER = 0x2
}

public enum Capture_scene_state
{
    INITIALISATION = 0x1,
    VIEW_HERO_ANIM = 0x2,
    PREPARE_NEW_TRIAL = 0x3,
    RECORD = 0x4,
    STOP_RECORDING = 0x5,
    COMPARE = 0x6,
    VIEW_COMPARISON = 0x7,
    WAITING_USER = 0x8
}

public enum Initialization_scene_state
{
	INITIALISATION = 0x1,
	VIEW_HERO_ANIM = 0x2,
	PREPARE_NEW_TRIAL = 0x3,
	RECORD = 0x4,
	STOP_RECORDING = 0x5,
	COMPARE = 0x6,
	VIEW_COMPARISON = 0x7,
	WAITING_USER = 0x8
}

public enum Play_scenario_state
{
    INITIALISATION = 0x1,
    START_SCREEN = 0x2,
    MAIN_MENU = 0x3,
    PREFERENCES = 0x4,
    MODALITY_MENU = 0x5,
    DISCIPLINE_MENU = 0x6,
    SKILL_MENU = 0x7,
    HEROE_MENU = 0x8

}


public enum Skill_enum
{
	BACKHAND_SHOT = 0x5,
	RIGHTHANDED_UNDERARM_SHOT = 0x9,
	RIGHTHANDED_VOLLEY = 0x10,
	LEFTHANDED_SLICE_SHOT = 0x11,
	SERVE = 0x14,
	STRAIGHTARM_SIDE_SHOT = 0x15,
	SIDEARM_SHOT = 0x16
}


public enum Camera_view
{
	LEFT_POINT_OF_VIEW = 0x1,
	FRONT_POINT_OF_VIEW = 0x2,
	RIGHT_POINT_OF_VIEW = 0x3
}

public enum Sport
{
	GAELIC_FOOTBALL = 0x1,
	HURLING = 0x2,
	HANDBALL = 0x3
}