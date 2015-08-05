using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class PlayerPosition
{

	/***
	 * Description:
	 * PlayerPosition class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int position_id { get; set; }
	public int player_id { get; set; }
	public int team_id { get; set; }



	// Constructor that takes no arguments. 
	public PlayerPosition()
	{

	}

	public PlayerPosition(int position_id,int player_id,int team_id)
	{
		this.position_id = position_id;
		this.player_id = player_id;
		this.team_id = team_id;
	}
}
