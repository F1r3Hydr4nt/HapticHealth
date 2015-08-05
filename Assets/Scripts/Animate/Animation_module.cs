using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Animation_module : MonoBehaviour
{

    /***
     * Description:
     * Animation module: made to load and control animation of the National Heroes
     * Version: 0.1
     * Autor:
     * Yvain Tisserand - MIRALab
     *****/

    public Animator _animator_handball_1;
    public Animator _animator_handball_2;
    public Animator _animator_handball_3;

    
    public Animator _animator_gaelic_football;
    public Animator _animator_hurling;
    
    public GameObject _body_handball_1;
    public GameObject _body_handball_2;
    public GameObject _body_handball_3;

    public GameObject _body_gaelic_football;
    public GameObject _body_hurling;


    public Transform _body_root_handball_1;
    public Transform _body_root_handball_2;
    public Transform _body_root_handball_3;
    public Transform _body_root_gaelic_football;
    public Transform _body_root_hurling;

    private Transform _target;

    public Transform _camera_transform;
    
    private Vector3 _initial_position;
    public float _camera_speedMod = 2.0f;
    public bool _active_auto_camera;
    public int _distance_camera;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.0F;
    public Camera_view _camera_view;
    private Vector3 _offset = Vector3.zero;

    public Transform _user_avatar_position;
    public Transform _comparison_avatar_position;





    // Use this for initialization
    void Start()
    {
        _body_handball_1.SetActive(false);
        _body_handball_2.SetActive(false);
        _body_handball_3.SetActive(false);
        _body_gaelic_football.SetActive(false);
        _body_hurling.SetActive(false);
        

        _initial_position = new Vector3(_camera_transform.position.x, _camera_transform.position.y, _camera_transform.position.z);
    }

    public void SetCameraView(Camera_view camera_view)
    {
        _camera_view = camera_view;
    }
    public void SetAnimSpeed(float speed)
    {
        if (_target_sport == Sport.GAELIC_FOOTBALL)
        {
            _animator_gaelic_football.speed = speed;
        }
        else if (_target_sport == Sport.HURLING)
        {
            _animator_hurling.speed = speed;

        }
        else if (_target_sport == Sport.HANDBALL)
        {
            if (_current_skill == 0)
            {
                _animator_handball_1.speed = speed;
                            }
            else if (_current_skill == 1)
            {
                _animator_handball_2.speed = speed;
            }
            else if (_current_skill == 2)
            {
                _animator_handball_3.speed = speed;
            }
            
        }
    }


    // Update is called once per frame
    void Update()
    {//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
        if (_active_auto_camera)
        {
            if (_camera_view == Camera_view.FRONT_POINT_OF_VIEW)
            {
                _offset = new Vector3(2, 1, -3);
            }
            else if (_camera_view == Camera_view.LEFT_POINT_OF_VIEW)
            {
                _offset = new Vector3(-2, 1, -2);
            }
            else if (_camera_view == Camera_view.RIGHT_POINT_OF_VIEW)
            {
                _offset = new Vector3(2, 1, 0);
            }
            else
            {
                //Default
                _offset = new Vector3(2, 1, -3);
            }

            Vector3 destination = _target.position;
            //destination.z = _camera_transform.position.z;
            _camera_transform.position = Vector3.SmoothDamp(_camera_transform.position, destination + _offset, ref velocity, 20 * Time.deltaTime * _camera_speedMod);
            _camera_transform.LookAt(_target.position);
            /*
            _camera_transform.RotateAround(_body_root.position, new Vector3(0.0f, 0.5f, 0.0f), 20 * Time.deltaTime * _camera_speedMod);
            _camera_transform.LookAt(_body_root.position);//makes the camera look to it
             */
        }

    }
    private Sport _target_sport = Sport.GAELIC_FOOTBALL;
    private int _current_skill = 1;

    public float PlayAnimation(int anim_index, Sport target_sport)
    {
        _target_sport = target_sport;
        if (_target_sport == Sport.GAELIC_FOOTBALL)
        {
            _animator_gaelic_football.speed = 1.0f;
            _body_gaelic_football.SetActive(true);
            _target = _body_root_gaelic_football;
            
        }
        else if (_target_sport == Sport.HURLING)
        {
            _body_hurling.SetActive(true);
            _animator_hurling.speed = 1.0f;
            _target = _body_root_hurling;

        }
        else if(_target_sport == Sport.HANDBALL)
        {
            Debug.Log("Try to load anim: "+anim_index);
            _current_skill = anim_index;
            if (anim_index == 0)
            {
                
                _body_handball_1.SetActive(true);
                _animator_handball_1.speed = 1.0f;
                _target = _body_root_handball_1;
            }
            else if (anim_index == 1)
            {
                
                _body_handball_2.SetActive(true);
                _animator_handball_2.speed = 1.0f;
                _target = _body_root_handball_2;
            }
            else if (anim_index == 2)
            {
                
                _body_handball_3.SetActive(true);
                _animator_handball_3.speed = 1.0f;
                _target = _body_root_handball_3;
            }
            

        }
        
       
        _active_auto_camera = true;
        Debug.Log("Play animation: " + anim_index + " [Ok]");
        //_animator.animation.clip.averageDuration
        return 5.0f;
    }

    private AnimationState AddAnimationClip(GameObject testAnim, string animationString)
    {
        AnimationClip theAnimation = testAnim.animation.GetClip(animationString);
        this.animation.AddClip(theAnimation, animationString);
        return this.animation[animationString];
    }

    public void Look_at_user_avatar()
    {
        _camera_transform.position = _initial_position;
        _camera_transform.LookAt(_user_avatar_position.position);
    }
    public void Look_at_comparison_avatar()
    {
        _camera_transform.position = _initial_position;
        _camera_transform.LookAt(_comparison_avatar_position.position);
    }
    public void Look_at_center()
    {
        _camera_transform.position = _initial_position;
        _camera_transform.LookAt(_user_avatar_position.position);
    }

    public void Stop_animation()
    {
        _active_auto_camera = false;
        _body_gaelic_football.SetActive(false);
        _body_hurling.SetActive(false);
        _body_handball_1.SetActive(false);
        _body_handball_2.SetActive(false);
        _body_handball_3.SetActive(false);
        /*
        if (_target_sport == Sport.GAELIC_FOOTBALL)
        {
            _body_gaelic_football.SetActive(false);
            _target = _body_root_gaelic_football;
        }
        else if (_target_sport == Sport.HURLING)
        {
            _body_hurling.SetActive(false);
            _target = _body_root_hurling;

        }
        else if (_target_sport == Sport.HANDBALL)
        {
            if (_current_skill == 0)
            {
                
                _body_handball_1.SetActive(false);
                
            }
            else if (_current_skill == 1)
            {
                
                _body_handball_2.SetActive(false);
                
            }
            else if (_current_skill == 2)
            {
                
                _body_handball_3.SetActive(false);
                
            }

        }
        */
        
        Debug.Log("Stop animation [Ok]");
        _camera_transform.position = _initial_position;
        _camera_transform.LookAt(new Vector3(0, 0, 0));
    }

}
