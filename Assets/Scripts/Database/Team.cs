using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class Team
{

	/***
	 * Description:
	 * Team class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	 
	public int id { get; set; }
	public string name { get; set; }
	public int club_id { get; set; }
	public int discipline_id { get; set; }
	public string photo { get; set; }
	public string thumbnail { get; set; }
	public int active { get; set; }


	// Constructor that takes no arguments. 
	public Team()
	{

	}

	public Team(int id,string name,int club_id,int discipline_id,string photo, string thumbnail,int active)
	{
		this.id = id;
		this.name = name;
		this.club_id = club_id;
		this.discipline_id = discipline_id;
		this.photo = photo;
		this.thumbnail = thumbnail;
		this.active = active;

	}
}
