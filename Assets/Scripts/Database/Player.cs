using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;


public class Player
{

	/***
	 * Description:
	 * Player class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public string name { get; set; }
	public string surname { get; set; }
	public double height { get; set; }
	public double weight { get; set; }
	public int righthanded { get; set; }
	public int gender { get; set; }
	public string photo { get; set; }
	public string thumbnail { get; set; }
	public string birthdate { get; set; }
	public int active { get; set; }
	public int club_id { get; set; }
	public int? squad_id { get; set; }
	public int sent { get; set; }
	public List<Team> teams { get; set; }
	public List<PlayerPosition> playerpositions { get; set; }
	public List<Activity> activities { get; set; }


	// Constructor that takes no arguments. 
	public Player()
	{

	}

	public Player(int id,string name,string surname,double height,double weight,int righthanded,int gender,string photo,string thumbnail,string birthdate,int club_id,int? squad_id,int active,int sent)
	{
		this.id = id;
		this.name = name;
		this.surname = surname;
		this.height = height;
		this.weight = weight;
		this.righthanded = righthanded;
		this.gender = gender;
		this.photo = photo;
		this.surname = surname;
		this.thumbnail = thumbnail;
		this.club_id = club_id;
		this.squad_id = squad_id;
		this.active = active;
		this.sent = sent;
	}
}
