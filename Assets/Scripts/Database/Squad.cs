using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class Squad
{

	/***
	 * Description:
	 * Squad class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	
	public int id { get; set; }
	public string name { get; set; }
	public int active { get; set; }


	// Constructor that takes no arguments. 
	public Squad()
	{

	}

	public Squad(int id,string name,int active)
	{
		this.id = id;
		this.name = name;
		this.active = active;

	}
}
