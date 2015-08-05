using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Child_HeroeList : MonoBehaviour
{
	/***
             * Description:
             * Child HeroeList class
             * Version: 0.1
             * Autor:
             * Mikel Rodríguez - Vicomtech
             *****/
	public Canvas _heroe_canvas;
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
		_heroe_canvas.active = state;
		if (state)
		{
			Refresh_panel();
		}
	}
	
	public void Hide_panel()
	{
		_heroe_canvas.active = false;
	}
	public void Show_panel()
	{
		_heroe_canvas.active = true;
		Refresh_panel();
		
	}
	
	public void Refresh_panel()
	{
		
		translations = GlobalVariables.translations;
		_title.GetComponent<Text>().text = translations.getString ("heroes");
		
		rowRectTransform = itemPrefab.GetComponent<RectTransform>();
		containerRectTransform = GUI_parent.GetComponent<RectTransform>();
		
		
		//calculate the width and height of each child item.
		float width = containerRectTransform.rect.width / columnCount;
		float ratio = 1 /*width / rowRectTransform.rect.width*/;
		float height = rowRectTransform.rect.height * ratio;

		
		Debug.Log("heroes REFRESH PANEL");
				
		for (int i = GUI_parent.transform.childCount - 1; i >= 0; i--)
		{
			// objectA is not the attached GameObject, so you can do all your checks with it.
			GameObject objectA = GUI_parent.transform.GetChild(i).gameObject;
			objectA.transform.parent = null;
			// Optionally destroy the objectA if not longer needed
		} 
		

	
		int modality_id = GlobalVariables.selected_modality_id;
		int discipline_id = GlobalVariables.selected_discipline_id;
		int skill_id = GlobalVariables.selected_skill_id;

		List<Heroe> heroes = _database.getHeroes (skill_id);
		itemCount = heroes.Count;
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
			string name = heroes[i].name+" "+heroes[i].surname;
			int id = heroes[i].id;
			
			GameObject newItem = Instantiate(itemPrefab) as GameObject;
			newItem.name = "heroe_"+id.ToString();
			
			newItem.transform.parent = GUI_parent.transform;
			
			newItem.transform.localScale = Vector3.one;
			Button button = newItem.GetComponent<Button>();
			button.onClick.AddListener(() =>
			                           {
				GlobalVariables.selected_heroe_id = id;
				Debug.Log("I will have to go to the recording part");
				

				
				
			});
			button.GetComponentsInChildren<Text>()[0].text = name;
			string photo = heroes[i].photo;
			string[] parts = photo.Split('.');
			photo = "images/"+parts[0];
			Texture image = (Texture)Resources.Load(photo, typeof(Texture));
			button.GetComponentsInChildren<RawImage>()[0].texture=image;

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