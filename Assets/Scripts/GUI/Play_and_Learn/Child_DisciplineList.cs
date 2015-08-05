using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Child_DisciplineList : MonoBehaviour
{
	/***
             * Description:
             * Child ModalityList class
             * Version: 0.1
             * Autor:
             * Mikel Rodríguez - Vicomtech
             *****/
	public Canvas _discipline_canvas;
	//public GameObject panel;
	public GameObject itemPrefab;
	public int itemCount = 10, columnCount = 1;
	public Play_and_learn_scenario play_and_learn_scenario;
	public GameObject GUI_parent;
	public GameObject _title;
	
	private Database_module _database;
	private float width;
	private float ratio;
	private float height;
	private RectTransform rowRectTransform;
	private RectTransform containerRectTransform;
	private Lang translations;
	
	void Start()
	{
		_database = new Database_module();	
	}
	
	public void SetPanelVisibility(bool state)
	{
		_discipline_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_discipline_canvas.active = false;
	}
	public void Show_panel()
	{
		_discipline_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		translations = GlobalVariables.translations;
		_title.GetComponent<Text>().text = translations.getString ("disciplines");
		
		rowRectTransform = itemPrefab.GetComponent<RectTransform>();
		containerRectTransform = GUI_parent.GetComponent<RectTransform>();
		
		
		//calculate the width and height of each child item.
		float width = containerRectTransform.rect.width / columnCount;
		float ratio = 1 /*width / rowRectTransform.rect.width*/;
		float height = rowRectTransform.rect.height * ratio;

		
		Debug.Log("disciplines REFRESH PANEL");
				
		for (int i = GUI_parent.transform.childCount - 1; i >= 0; i--)
		{
			// objectA is not the attached GameObject, so you can do all your checks with it.
			GameObject objectA = GUI_parent.transform.GetChild(i).gameObject;
			objectA.transform.parent = null;
			// Optionally destroy the objectA if not longer needed
		} 
		

	
		int modality_id = GlobalVariables.selected_modality_id;

		List<Discipline> disciplines = _database.getDisciplinesFromModality(modality_id);
		itemCount = disciplines.Count;
		columnCount = 1;
		
		
		
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
			string name = disciplines[i].name;
			int id = disciplines[i].id;
			
			GameObject newItem = Instantiate(itemPrefab) as GameObject;
			newItem.name = "discipline_"+id.ToString();
			
			newItem.transform.parent = GUI_parent.transform;
			
			newItem.transform.localScale = Vector3.one;
			Button button = newItem.GetComponent<Button>();
			button.onClick.AddListener(() =>
			                           {
				GlobalVariables.selected_discipline_id = id;
				/*GlobalVariables.selected_team_id = -1;
				GlobalVariables.selected_player_id = -1;
				GlobalVariables.selected_skill_id = -1;*/
				Debug.Log("clicked button id: " + id + ", and name: " + name);
				
				play_and_learn_scenario.GoToSkills();
				play_and_learn_scenario.ReloadCoachLeftPanel();
				
				
			});
			button.GetComponentsInChildren<Text>()[0].text = name;


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