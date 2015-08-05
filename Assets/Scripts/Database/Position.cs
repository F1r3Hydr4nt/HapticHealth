using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class Position
{
	public int id { get; set; }
	public string name { get; set; }
	public int active { get; set; }


	// Constructor that takes no arguments. 
	public Position()
	{

	}

	public Position(int id,string name,int active)
	{
		this.id = id;
		this.name = name;
		this.active = active;
	}
}
