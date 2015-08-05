using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class Heroe
{

	/***
	 * Description:
	 * Heroe class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	 
	public int id { get; set; }
	public string name { get; set; }
	public string surname { get; set; }
	public string nickname { get; set; }
	public string photo { get; set; }
	public string thumbnail { get; set; }
	public int isNational { get; set; }
	public int isActive { get; set; }
	public int active { get; set; }


	// Constructor that takes no arguments. 
	public Heroe()
	{

	}

	public Heroe(int id,string name,string surname,string nickname,string photo,string thumbnail, int isNational,int isActive,int active)
	{
		this.id = id;
		this.name = name;
		this.surname = surname;
		this.nickname = nickname;
		this.photo = photo;
		this.thumbnail = thumbnail;
		this.isNational = isNational;
		this.isActive = isActive;
		this.active = active;
	}
}
