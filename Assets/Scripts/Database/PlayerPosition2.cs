using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;


public class PlayerPosition2
{
	public Team team { get; set; }
	public List<Position> positions { get; set; }



	// Constructor that takes no arguments. 
	public PlayerPosition2()
	{

	}

	public PlayerPosition2(Team team)
	{
		this.team = team;
	}
}
