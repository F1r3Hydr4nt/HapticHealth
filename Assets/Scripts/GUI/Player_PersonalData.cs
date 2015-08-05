using UnityEngine;
#if UNITY_EDITOR
 using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Windows.Forms;
using System;


public class Player_PersonalData: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Coach_and_train_scenario coach_and_train_scenario;
	public Canvas _player_personaldata_canvas;



	public GameObject heading_title;
	public GameObject _tab_personaldata;
	public GameObject _tab_activities;
	public GameObject _tab_statistics;
	public GameObject _tab_feedbacks;
	public GameObject _subtitle;
	public GameObject _label_player_name;
	public GameObject _label_player_surname;
	public GameObject _label_player_birthdate;
	public GameObject _label_player_height;
	public GameObject _label_player_weight;
	public GameObject _label_tg_gender;
	public GameObject _label_tg_hand;
	public GameObject _label_tg_positions;
	public GameObject _label_tg_squad;
	public GameObject _player_name;
	public GameObject _player_surname;
	public GameObject _player_birthdate;
	public GameObject _player_image;
	public GameObject _player_height;
	public GameObject _player_weight;
	public GameObject _tg_gender;
	public GameObject _tg_hand;
	public GameObject _tg_squad;


	public GameObject _button_changeimage;
	public GameObject _button_delete;
	public GameObject _button_save;
	public GameObject _button_cancel;

	public Player_InfoPanel _player_infopanel;

	private Database_module _database;
	private Lang translations;


	public GameObject itemPrefab;
	public int itemCount = 10, columnCount = 1;
	public GameObject GUI_parent;
	private float width;
	private float ratio;
	private float height;
	private RectTransform rowRectTransform;
	private RectTransform containerRectTransform;

	private string path;


    void Start()
    {
		_database = new Database_module ();
	
    }


	public void SetPanelVisibility(bool state)
	{
		_player_personaldata_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_player_personaldata_canvas.active = false;
	}
	public void Show_panel()
	{
		_player_personaldata_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		translations = GlobalVariables.translations;
		_button_delete.SetActive(true);
		_tab_activities.SetActive(true);
		_tab_statistics.SetActive(true);
		_tab_feedbacks.SetActive(true);
		
		
		//Translations
		_subtitle.GetComponent<Text> ().text= translations.getString ("personal_data");
		_tab_personaldata.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("personal_data");
		_tab_activities.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("activities");
		_tab_statistics.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("statistics");
		_tab_feedbacks.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("feedbacks");
		_button_delete.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("delete_player");
		_button_changeimage.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("change_image");
		_button_save.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("save");
		_button_cancel.GetComponent<UnityEngine.UI.Button> ().GetComponentsInChildren<Text> () [0].text = translations.getString ("cancel");

		foreach (Toggle toggle in _tg_gender.GetComponentsInChildren<Toggle>())
		{
			string name = toggle.name;
			string[] s_name = name.Split('_');
			if (0 == int.Parse(s_name[s_name.Length-1]) )
			{
				toggle.GetComponentsInChildren<Text>()[0].text = translations.getString ("male");
			}
			else
			{
				toggle.GetComponentsInChildren<Text>()[0].text =translations.getString ("female");
			}

		}

		foreach (Toggle toggle in _tg_hand.GetComponentsInChildren<Toggle>())
		{
			string name = toggle.name;
			string[] s_name = name.Split('_');
			if (0 == int.Parse(s_name[s_name.Length-1]) )
			{
				toggle.GetComponentsInChildren<Text>()[0].text = translations.getString ("righthanded");
			}
			else if (1 == int.Parse(s_name[s_name.Length-1]) )
			{
				toggle.GetComponentsInChildren<Text>()[0].text =translations.getString ("lefthanded");
			}
			else
			{
				toggle.GetComponentsInChildren<Text>()[0].text =translations.getString ("ambidextrous");
			}
			
		}

		foreach (Toggle toggle in _tg_squad.GetComponentsInChildren<Toggle>())
		{
			string name = toggle.name;
			string[] s_name = name.Split('_');
			if (0 == int.Parse(s_name[s_name.Length-1]) )
			{
				toggle.GetComponentsInChildren<Text>()[0].text = translations.getString ("yes");
			}
			else
			{
				toggle.GetComponentsInChildren<Text>()[0].text =translations.getString ("no");
			}
			
		}



		_label_player_name.GetComponent<Text> ().text= translations.getString ("name");
		_label_player_surname.GetComponent<Text> ().text= translations.getString ("surname");
		_label_player_birthdate.GetComponent<Text> ().text= translations.getString ("birthdate");
		_label_player_height.GetComponent<Text> ().text= translations.getString ("height");
		_label_player_weight.GetComponent<Text> ().text= translations.getString ("weight");
		_label_tg_gender.GetComponent<Text> ().text= translations.getString ("gender");
		_label_tg_hand.GetComponent<Text> ().text= translations.getString ("hand");
		_label_tg_squad.GetComponent<Text> ().text= translations.getString ("squad");
		_label_tg_positions.GetComponent<Text> ().text= translations.getString ("positions");

		//load de positions
		positionPanel ();

		//Data of the player
		int user_id = GlobalVariables.user_id;
		int player_id = GlobalVariables.selected_player_id;
		Player player;
		string photo;
		if (player_id != 0) {
			player = _database.getPlayer (player_id);
			heading_title.GetComponent<Text> ().text=player.name+" "+player.surname;
			
			
			photo = player.photo;
			string[] parts = photo.Split('.');
			photo = "images/"+parts[0];

			
			int gender = player.gender;
			
			foreach (Toggle toggle in _tg_gender.GetComponentsInChildren<Toggle>())
			{
				string name = toggle.name;
				string[] s_name = name.Split('_');
				if (gender == int.Parse(s_name[s_name.Length-1]) )
				{
					toggle.isOn = true;
				}

			}
			
			int hand = player.righthanded;
			foreach (Toggle toggle in _tg_hand.GetComponentsInChildren<Toggle>())
			{
				string name = toggle.name;
				string[] s_name = name.Split('_');
				if (gender == int.Parse(s_name[s_name.Length-1]) )
				{
					toggle.isOn = true;
				}
			}

			int? squad_id = player.squad_id;
			foreach (Toggle toggle in _tg_squad.GetComponentsInChildren<Toggle>())
			{
				string name = toggle.name;
				string[] s_name = name.Split('_');
				/*if (gender == int.Parse(s_name[s_name.Length-1]) )
				{
					toggle.isOn = true;
				}*/
			}
			
		}
		else 
		{
			heading_title.GetComponent<Text> ().text=translations.getString("new_player");

			photo = "images/user_noimage";
			player = new Player(0,"","",0.00,0.00,0,0,"","","2000-01-01",GlobalVariables.selected_club_id,null,0,1);
			_button_delete.SetActive(false);
			_tab_activities.SetActive(false);
			_tab_statistics.SetActive(false);
			_tab_feedbacks.SetActive(false);
		}
		
		_player_name.GetComponent<InputField> ().text=player.name;
		_player_surname.GetComponent<InputField> ().text=player.surname;
		//_player_birthdate.GetComponent<InputField> ().text=player.birthdate;
		_player_weight.GetComponent<InputField> ().text=player.weight.ToString();
		_player_height.GetComponent<InputField> ().text=player.height.ToString();
		Texture image = (Texture)Resources.Load(photo, typeof(Texture));
		_player_image.GetComponent<RawImage> ().texture=image;
		
		_button_save.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => { 
			string new_photo="";
			if (path!="")
			{

				string mili = Convert.ToDateTime("1970/01/01").Ticks.ToString();
				new_photo = mili+".jpg";
				File.Copy(path,UnityEngine.Application.dataPath+"/Resources/images/players/"+new_photo);
			}
			player_id = GlobalVariables.selected_player_id;
			string b_name = _player_name.GetComponent<InputField> ().text;
			string b_surname = _player_surname.GetComponent<InputField> ().text;
			double b_weight =  double.Parse(_player_weight.GetComponent<InputField> ().text);
			double b_height =  double.Parse(_player_height.GetComponent<InputField> ().text);
			int b_gender=0;
			
			foreach (Toggle toggle in _tg_gender.GetComponentsInChildren<Toggle>())
			{
				
				if (toggle.isOn)
				{
					string n = toggle.name;
					string[] s_n = n.Split('_');
					b_gender = int.Parse(s_n[s_n.Length-1]);
				}
			}
			
			int b_hand=0;
			
			foreach (Toggle toggle in _tg_hand.GetComponentsInChildren<Toggle>())
			{
				
				if (toggle.isOn)
				{
					string n = toggle.name;
					string[] s_n = n.Split('_');
					b_hand = int.Parse(s_n[s_n.Length-1]);
				}
			}
			
			
			int b_squad=0;
			
			foreach (Toggle toggle in _tg_squad.GetComponentsInChildren<Toggle>())
			{
				
				if (toggle.isOn)
				{
					string n = toggle.name;
					string[] s_n = n.Split('_');
					b_squad = int.Parse(s_n[s_n.Length-1]);
				}
			}
			
			if (GlobalVariables.selected_player_id ==0)
			{
				player = new Player(GlobalVariables.selected_player_id,b_name,b_surname,b_height,b_weight,b_hand,b_gender,"","","",GlobalVariables.selected_club_id, b_squad,0,1);
			}
			else
			{
				player = _database.getPlayer(GlobalVariables.selected_player_id);
				player.name = b_name;
				player.surname = b_surname;
				player.height = b_height;
				player.weight = b_weight;
				player.righthanded = b_hand;
				player.gender = b_gender;
				player.photo = "players/"+new_photo;
				player.sent =1;
				
			}
			
			_database.savePlayer(player);
			_player_infopanel.Refresh_panel();

			
		}); 
		
		_button_delete.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => { 

			if (MessageBox.Show(translations.getString("delete_text"), translations.getString("delete_player"), MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				// user clicked yes
				Debug.Log("OK");
			}
			else
			{
				// user clicked no
				Debug.Log("Cancel");
			}

//			if (EditorUtility.DisplayDialog(translations.getString("delete_player"),translations.getString("delete_text"),translations.getString("ok"),translations.getString("cancel")))
//			{
//				Debug.Log("OK");
//				player = _database.getPlayer(GlobalVariables.selected_player_id);
//				player.active = 1;
//				
//	
//				_database.savePlayer(player);
//				coach_and_train_scenario.GoToPlayers();
//			}
//			else
//			{
//				Debug.Log("Cancel");
//
//			}
		}); 


		

		/*#if UNITY_EDITOR
		_button_changeimage.GetComponent<UnityEngine.UI.Button> ().onClick.AddListener (() => {



				path = EditorUtility.OpenFilePanel(
							"Overwrite with jpg",
							"",
							"jpg");
						if (path.Length != 0) {
							WWW www = new WWW("file:///" + path);
							_player_image.GetComponent<RawImage> ().texture=www.texture;
			
						}
		


		});
		#else*/
		_button_changeimage.GetComponent<UnityEngine.UI.Button> ().onClick.AddListener (() => {
			

			System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
			openFileDialog.InitialDirectory = "";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = false;
			//openFileDialog.ShowDialog();
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				path =openFileDialog.FileName;
				WWW www = new WWW("file:///" + path);
				_player_image.GetComponent<RawImage> ().texture=www.texture;
				
			}
			
		});
		//#endif
	}



	private void positionPanel()
	{
		rowRectTransform = itemPrefab.GetComponent<RectTransform>();
		containerRectTransform = GUI_parent.GetComponent<RectTransform>();
		
		
		//calculate the width and height of each child item.
		float width = containerRectTransform.rect.width / columnCount;
		float ratio = 1 /*width / rowRectTransform.rect.width*/;
		float height = rowRectTransform.rect.height * ratio;
		
		
		for (int i = GUI_parent.transform.childCount - 1; i >= 0; i--)
		{
			// objectA is not the attached GameObject, so you can do all your checks with it.
			GameObject objectA = GUI_parent.transform.GetChild(i).gameObject;
			objectA.transform.parent = null;
			// Optionally destroy the objectA if not longer needed
		} 
		
		
		
		
		
		
		int user_id = GlobalVariables.user_id;
		int club_id = GlobalVariables.selected_club_id;
		int team_id = GlobalVariables.selected_team_id;

		List<Position> positions = _database.getDisciplinePositions (GlobalVariables.selected_discipline_id);
		itemCount = positions.Count;
		columnCount = 1;

		List<PlayerPosition> player_positions = _database.getPlayerPositions(GlobalVariables.selected_player_id,GlobalVariables.selected_team_id);
		
		
		
		int rowCount = itemCount / columnCount;
		if (rowCount == 0)
			rowCount = 1;
		if (itemCount % rowCount > 0)
			rowCount++;
		
		//adjust the height of the container so that it will just barely fit all its children
		float scrollHeight = height * rowCount;
		float scrollWidth = width * itemCount;
		
		
		
		int j = 0;
		
		for (int i = 0; i < itemCount; i++)
		{
			//this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
			if (i % columnCount == 0)
				j++;
			
			//create a new item, name it, and set the parent
			string name = positions[i].name;
			int id = positions[i].id;
			
			GameObject newItem = Instantiate(itemPrefab) as GameObject;
			newItem.name = "position_"+id.ToString();
			
			newItem.transform.parent = GUI_parent.transform;
			
			newItem.transform.localScale = Vector3.one;

			bool is_selected = false;

			foreach (PlayerPosition pp in player_positions)
			{

				if (pp.position_id == id)
				{
					is_selected = true;
				}

			}

			newItem.GetComponent<Toggle>().isOn = is_selected;
			newItem.GetComponentsInChildren<Text>()[0].text=name;
			//move and size the new item
			RectTransform rectTransform = newItem.GetComponent<RectTransform>();
			
			containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight);
			containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, 0);
			
			float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
			float y = containerRectTransform.rect.height / 2 - height * j;
			rectTransform.offsetMin = new Vector2(x, y);
			
			x = rectTransform.offsetMin.x + width;
			y = rectTransform.offsetMin.y + height;
			rectTransform.offsetMax = new Vector2(x, y);
		}
		
		






	}
	
}
