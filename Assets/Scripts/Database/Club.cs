using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class Club
{

	/***
	 * Description:
	 * Club class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	 
	public int id { get; set; }
	public string name { get; set; }
	public int modality_id { get; set; }
	public string photo { get; set; }
	public string thumbnail { get; set; }
	public int active { get; set; }


	// Constructor that takes no arguments. 
	public Club()
	{

	}

	public Club(int id,string name,int modality_id,string photo, string thumbnail,int active)
	{
		this.id = id;
		this.name = name;
		this.modality_id = modality_id;
		this.photo = photo;
		this.thumbnail = thumbnail;
		this.active = active;

	}
}
