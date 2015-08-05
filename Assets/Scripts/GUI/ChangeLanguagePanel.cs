using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class ChangeLanguagePanel: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public GameObject _title;
	public GameObject _button_english;
	public GameObject _button_basque;
	public GameObject _button_spanish;
	public Canvas _changelanguage_canvas;

	public Coach_LeftPanel _coach_leftpanel;

	private Lang translations;

	
	void Start()
	{


	}
	
	public void SetPanelVisibility(bool state)
	{
		_changelanguage_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_changelanguage_canvas.active = false;
	}
	public void Show_panel()
	{
		_changelanguage_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		Refresh_translations ();
		_button_english.GetComponent<Button>().onClick.AddListener(() => { 
			Debug.Log ("change language "+GlobalVariables.locale+" to gb");
			GlobalVariables.locale = "gb";
			translations =new Lang (Path.Combine (Application.dataPath, "Resources/lang.xml"), GlobalVariables.locale, false);
			GlobalVariables.translations = translations;
			Refresh_translations();
			_coach_leftpanel.Refresh_panel();
			
		}); 
		_button_basque.GetComponent<Button>().onClick.AddListener(() => { 
			Debug.Log ("change language "+GlobalVariables.locale+" to eu");
			GlobalVariables.locale = "eu";
			translations =new Lang (Path.Combine (Application.dataPath, "Resources/lang.xml"), GlobalVariables.locale, false);
			GlobalVariables.translations = translations;
			Refresh_translations();
			_coach_leftpanel.Refresh_panel();
		});
		_button_spanish.GetComponent<Button>().onClick.AddListener(() => { 
			Debug.Log ("change language "+GlobalVariables.locale+" to es");
			GlobalVariables.locale = "es";
			translations =new Lang (Path.Combine (Application.dataPath, "Resources/lang.xml"), GlobalVariables.locale, false);
			GlobalVariables.translations = translations;
			Refresh_translations();
			_coach_leftpanel.Refresh_panel();
		});
		
	}

	private void Refresh_translations()
	{
		translations = GlobalVariables.translations;
		_title.GetComponent<Text> ().text = translations.getString ("change_language");
	}

	
}
