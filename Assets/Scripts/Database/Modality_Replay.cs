using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class Modality_Replay
{

	/***
	 * Description:
	 * Modality class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	 
	public int id { get; set; }
	public string name { get; set; }
	public int active { get; set; }


	// Constructor that takes no arguments. 
	public Modality_Replay()
	{

	}

	public Modality_Replay(int id,string name,int active)
	{
		this.id = id;
		this.name = name;
		this.active = active;
	}
}
