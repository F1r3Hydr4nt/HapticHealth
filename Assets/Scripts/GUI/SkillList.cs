using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SkillList : MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

    public GameObject itemPrefab;
    public int itemCount = 10, columnCount = 1;
	public Coach_and_train_scenario coach_and_train_scenario;

    void Start()
    {
		//Elements
		Database_module _database = new Database_module ();
		int user_id = GlobalVariables.user_id;
		int discipline_id = GlobalVariables.selected_discipline_id;
		List<Skill> skills = _database.getSkills (discipline_id);
		itemCount = skills.Count;
		columnCount = 3;

        RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform>();
        RectTransform containerRectTransform = gameObject.GetComponent<RectTransform>();

        //calculate the width and height of each child item.
        float width = containerRectTransform.rect.width / columnCount;
        float ratio = 1 /*width / rowRectTransform.rect.width*/;
        float height = rowRectTransform.rect.height * ratio;
		/*float height = containerRectTransform.rect.height;
		float ratio = height / rowRectTransform.rect.height;
		float width = rowRectTransform.rect.width *  ratio;*/


        int rowCount = itemCount / columnCount;
		if (rowCount == 0)
			rowCount = 1;
        if (itemCount % rowCount > 0)
            rowCount++;

        //adjust the height of the container so that it will just barely fit all its children
        float scrollHeight = height * rowCount;
		float scrollWidth = width * itemCount;
        containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight);
        containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, 0);


        int j = 0;

        for (int i = 0; i < itemCount; i++)
        {
            //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
            if (i % columnCount == 0)
                j++;

            //create a new item, name it, and set the parent
			string name = skills[i].name;
			int id = skills[i].id;
			//string photo = players[i].photo;

			GameObject.Find("ccp_button_player_name").GetComponent<Text> ().text=name;
			/*string[] parts = photo.Split('.');
			photo = "images/"+parts[0];
			
			Texture image = (Texture)Resources.Load(photo, typeof(Texture));
			GameObject.Find("ccp_button_player_image").GetComponent<RawImage> ().texture=image;*/

            GameObject newItem = Instantiate(itemPrefab) as GameObject;
			newItem.name = id.ToString();
            newItem.transform.parent = gameObject.transform;
			newItem.transform.localScale = Vector3.one;
			newItem.GetComponent<Button>().onClick.AddListener(() => { 
				GlobalVariables.selected_skill_id = id;
				Debug.Log("clicked button id: "+id+", and name: "+name); 

				//coach_and_train_scenario.GoTos();


			}); 

			Debug.Log (newItem.name);

            //move and size the new item
            RectTransform rectTransform = newItem.GetComponent<RectTransform>();

            float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
            float y = containerRectTransform.rect.height / 2 - height * j;
            rectTransform.offsetMin = new Vector2(x, y);

            x = rectTransform.offsetMin.x + width;
            y = rectTransform.offsetMin.y + height;
            rectTransform.offsetMax = new Vector2(x, y);
        }


    }

}
