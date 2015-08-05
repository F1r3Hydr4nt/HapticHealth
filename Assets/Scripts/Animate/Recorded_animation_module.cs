using UnityEngine;
using System.Collections;

public class Recorded_animation_module : MonoBehaviour {

	
	/***
	 * Description:
	 * Recorded animation module: handle the 2 animations: reference and recorded trial
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/


	public Recorded_animation_player _reference_animation_player;
	public Recorded_animation_player _recorded_animation_player;
    public GameObject _avatar_objects;
	private Animation_player_state _animation_state = Animation_player_state.INITIALISATION;
	private int _speed = 1;
	private int _frame_number = 0;
	// Use this for initialization
	void Start () {

	}

    public Recorded_animation_player GetRefAnimationPlayer()
    {
        return _reference_animation_player;
    }
    public Recorded_animation_player GetRecordedAnimationPlayer()
    {
        return _recorded_animation_player;
    }

	// Update is called once per frame
	void Update () {
		if(_animation_state == Animation_player_state.PLAY){
		if (_frame_number%_speed == 0)
			{
                //Debug.Log("APPLY ANIMATION ON THE USER AVATAR");
				if (!_recorded_animation_player.ApplyNextMotionFrame()/* || !_reference_animation_player.ApplyNextMotionFrame()*/)
				{
                   // Debug.Log("\t -> next frame = 0");
                    _recorded_animation_player.ResetMotion();
                   // _recorded_animation_player.RotateToInitialPosition();
                    /*if (_reference_animation_player)
                    {
                        _reference_animation_player.ResetMotion();
                    }*/
                    /*if (_recorded_animation_player)
                    {
                        _recorded_animation_player.ResetMotion();
                    }*/
				}
			}
			_frame_number++;
		}
	}
	public void SetReferenceAnimation(Animation_data reference_motion){
		_reference_animation_player.SetAnimationData(reference_motion);
	}

	public void SetRecordedAnimation(Animation_data recorded_motion){
		_recorded_animation_player.SetAnimationData(recorded_motion);
	}

    public void SetReferenceAnim(Animation_data reference_motion)
    {
        _reference_animation_player.SetAnimationDataRef(reference_motion);
    }

    public void SetRecordedAnim(Animation_data recorded_motion)
    {
        Debug.Log("2 Anim lenght: " + recorded_motion.getLenght());
        _recorded_animation_player.SetAnimationDataRef(recorded_motion);
    }



    public void AvatarVisibilityState(bool state)
    {
        _frame_number = 0;
        _speed = 1;
        _avatar_objects.active = state;
    }
	public void PlayAnimation(){
		_animation_state = Animation_player_state.PLAY;
        

	}
	public void PauseAnimation(){
		_animation_state = Animation_player_state.PAUSE;
	}
	public void StopAnimation(){
		_animation_state = Animation_player_state.STOP;
		_reference_animation_player.ResetMotion();
		_recorded_animation_player.ResetMotion();
	}
	public void ChangeSpeed(int speed){
		_speed = speed;
	}


}
