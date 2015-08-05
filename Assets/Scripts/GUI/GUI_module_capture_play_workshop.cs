using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUI_module_capture_play_workshop : MonoBehaviour {

    public Canvas _c_animation;
    public Canvas _c_record;
    public Canvas _c_compare;
    public Canvas _c_trial;
    public Canvas _c_wait;
    public Canvas _c_final_score;
    public Canvas _c_global_instruction;
    public Canvas _c_trial_instruction;
    public Canvas _c_invalid;
    public Text _overall_score;
    public Text _semantic_feedback;
    public Text _tips;
    public Text _warning;
    public Text _stars;

    public Text _instruction_trial;
    public Text _trial_number;
    public Text _global_instruction;
    public Text _skill_name;
    public Text _skill_name2;
    public GameObject _tips_panel;

    public Text _final_score;
    public Text _final_detail_score;
    public Text _final_stars;


    private Skill_data _current_skill_info;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetSkillInfo(Skill_data skill)
    {
        _current_skill_info = skill;
    }
    public void SetAnimationVisibility(bool state)
    {
        _c_animation.active = state;
    }

    public void SetRecordVisibility(bool state)
    {
        _c_record.active = state;
    }
    public void SetTrialVisibility(bool state)
    {
        _c_trial.active = state;
    }

    public void SetInvalidTrialVisibility(bool state)
    {
        _c_invalid.active = state;
    }


    public void SetComparisonVisibility(bool state, float overall_score = 0.0f, string semantic_feedback = "Feedback", string[] tips = null, bool valid = true)
    {
        if (state)
        {


            _overall_score.text = (int)(overall_score) + "%";

            _semantic_feedback.text = semantic_feedback.ToString();
            if (tips != null && valid == true)
            {
                string tips_output = "";
                foreach (string tip in tips)
                {
                    if (tip != "Invalid action!")
                    {
                        tips_output += "- " + tip + "\n";
                    }
                    
                }
                Debug.Log("Tips" + tips_output);
                _tips.text = tips_output;
            }
            //_tips_panel.SetActive(valid);
            
            
            //Stars output
            //G -> OK, F -> KO
            string star_text = "";
            int score_rounded = (int)(overall_score);
            score_rounded /= 20;
            //int score_rounded = (int)(overall_score)/20;
            for (int i = 0; i < 5; i++)
            {
                if (score_rounded >= i)
                {
                    star_text += "G";
                }
                else
                {
                    star_text += "F";
                }
                _stars.text = star_text;
            }
        }
        _c_compare.active = state;

    }

    public void SetFinalScoreVisibility(bool state, float value = 0.0f, string detail_score = "")
    {
        _c_final_score.active = state;
        _final_score.text = (int)(value) + "%";
        _final_detail_score.text = detail_score.ToString();
        //_final_stars
        //Stars output
        //G -> OK, F -> KO
        string star_text = "";
        //int score_rounded = (int)((value  + (value % 20 > 0 ? 1 : 0))/20);
        int score_rounded = (int)(value);
        score_rounded/= 20;
        //int score_rounded = (int)(overall_score)/20;
        for (int i = 0; i < 5; i++)
        {
            if (score_rounded >= i)
            {
                star_text += "G";
            }
            else
            {
                star_text += "F";
            }
            _final_stars.text = star_text;
        }
    }



    public void SetWaitVisibility(bool state)
    {
        _c_wait.active = state;
    }
    public void SetGlobalInstructionVisibility(bool state)
    {
        _c_global_instruction.active = state;
    }
    public void SetTrialInstructionVisibility(bool state)
    {
        _c_trial_instruction.active = state;
    }

    public void SetTrialGuiData(string description, string skill_name,int skill_number, int total_skill)
    {

        _instruction_trial.text = "Skill "+skill_number+"/"+total_skill+"\n"+skill_name+"\n\n"+description+"\n";
    }
    public void SetGlobalInstruction(string description)
    {
        _global_instruction.text = description.ToString();
    }

    public void SetTrialNumber(int current, int total)
    {
        _trial_number.text = "Trial " + current + "/" + total;
    }
    public void SetSkillName(string skill_name)
    {
        _skill_name.text = skill_name;
        _skill_name2.text = skill_name;

    }

    public void HideAll()
    {
        _c_animation.active = false;
        _c_record.active = false;
        _c_compare.active = false;
        _c_trial.active = false;
        _c_wait.active = false;
        _c_final_score.active = false;
        _c_compare.active = false;
        _c_global_instruction.active = false;
        _c_trial_instruction.active = false;
        _c_invalid.active = false;
    }

}
