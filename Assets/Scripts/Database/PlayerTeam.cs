using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class PlayerTeam
{

	/***
	 * Description:
	 * PlayerTeam class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int player_id { get; set; }
	public int team_id { get; set; }


	// Constructor that takes no arguments. 
	public PlayerTeam()
	{

	}

	public PlayerTeam(int player_id,int team_id)
	{
		this.player_id = player_id;
		this.team_id = team_id;
	}
}
