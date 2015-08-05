using UnityEngine;
using System.Collections;

public class test_record_apply_anim_avatar : MonoBehaviour {
    public Capture_module _capture;
    public Recorded_animation_module _animate_trial;
    private Animation_data latestAnimation;
    public float _recording_time = 3.0f;
	// Use this for initialization
	void Start () {
        _capture.Initialise_capture(Device.SINGLE_KINECT_2);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void User_detected()
    {
        Debug.Log("User detected");
        StartCoroutine(Start_recording_debug(_recording_time));
    }

    IEnumerator Start_recording_debug(float time)
    {
        Debug.Log("Start recording for " + time + " sec");
        _capture.Start_capture();     
        yield return new WaitForSeconds(time);
        print("Stop recording");
        latestAnimation = _capture.Stop_capture();
        //_animate_trial.SetRecordedAnim(latestAnimation);
        _animate_trial.GetRecordedAnimationPlayer().SetAnimationData(latestAnimation);
        _animate_trial.PlayAnimation();

    }

}
