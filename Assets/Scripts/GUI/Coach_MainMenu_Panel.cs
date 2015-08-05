using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class Coach_MainMenu_Panel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Canvas _mainmenu_canvas;
	public Coach_and_train_scenario coach_and_train_scenario;
	public GameObject _title;
	public GameObject button_player;
	public GameObject button_activity;
	public GameObject button_feedback;
	
	private Lang translations;
	
	void Start()
	{




	}
	
	public void SetPanelVisibility(bool state)
	{
		_mainmenu_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_mainmenu_canvas.active = false;
	}
	public void Show_panel()
	{
		_mainmenu_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		translations = GlobalVariables.translations;
		_title.GetComponent<Text> ().text = translations.getString ("mainmenu");
	
		 
		button_player.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text= translations.getString ("players") ;
		button_activity.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text = translations.getString ("activities") ;
		button_feedback.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text = translations.getString ("feedbacks") ;

			
		
	}


	
}
