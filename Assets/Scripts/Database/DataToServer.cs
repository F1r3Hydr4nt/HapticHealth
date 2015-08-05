using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;


[Serializable]
public class DataToServer
{

	/***
	 * Description:
	 * DataToServer class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	
	public User user { get; set; }
	public Coach coach { get; set; }
	public Player player { get; set; }

	// Constructor that takes no arguments. 
	public DataToServer()
	{

	}

	public DataToServer(User user, Coach coach, Player player)
	{
		this.user = user;
		this.coach = coach;
		this.player = player;
	}
}
