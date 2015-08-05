using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerActivities: MonoBehaviour
{
	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public Coach_and_train_scenario coach_and_train_scenario;
	public GameObject heading_title;


    void Start()
    {
		//Data of the player
		Database_module _database = new Database_module ();
		int user_id = GlobalVariables.user_id;
		int player_id = GlobalVariables.selected_player_id;
		Player player;
		if (player_id != 0) {
			player = _database.getPlayer (player_id);
			heading_title.GetComponent<Text> ().text=player.name+" "+player.surname;



		}



    }

}
