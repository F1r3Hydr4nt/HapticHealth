using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class Child_MainMenu_Panel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Canvas _mainmenu_canvas;
	public Play_and_learn_scenario play_and_learn_scenario;
	public GameObject _title;
	public GameObject button_play;
	public GameObject button_preferences;

	
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
		 
		button_play.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text= translations.getString ("play") ;
		button_preferences.GetComponent<Button>().GetComponentsInChildren<Text>()[0].text = translations.getString ("preferences") ;

	}


	
}
