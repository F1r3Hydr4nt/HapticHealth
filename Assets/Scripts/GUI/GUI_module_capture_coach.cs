using UnityEngine;
using System.Collections;

public class GUI_module_capture_coach : MonoBehaviour {

    /***
     * Description:
     * GUI module: made to activate or deactivate GUi elements
     * Version: 0.1
     * Autor:
     * Yvain Tisserand - MIRALab
     *****/


    public Canvas _c_animation;
    public Canvas _c_record;
    public Canvas _c_compare;
    

    // Use this for initialization
    void Start()
    {
        SetAnimationVisibility(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetAnimationVisibility(bool state)
    {
        _c_animation.active = state;
    }

    public void SetRecordVisibility(bool state)
    {
        _c_record.active = state;
    }

    public void SetComparisonVisibility(bool state)
    {
        _c_compare.active = state;
    }

}