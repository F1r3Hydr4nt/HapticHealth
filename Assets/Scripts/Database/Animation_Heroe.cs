using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Animation_Heroe
{

	/***
	 * Description:
	 * Animation class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public string path { get; set; }
	public string creationDate { get; set; }
	public int heroe_id { get; set; }
	public int skill_id { get; set; }
	public int avatar_id { get; set; }
	public string animation_controller { get; set; }
	public string animation_clean_file { get; set; }
	public int active { get; set; }
	public int sent { get; set; }



	// Constructor that takes no arguments. 
	public Animation_Heroe()
	{

	}

	public Animation_Heroe(int id,string path,string creationDate,int heroe_id,int skill_id,int avatar_id, string animation_controller, string animation_clean_file, int active,int sent)
	{
		this.id = id;
		this.path = path;
		this.creationDate = creationDate;
		this.heroe_id = heroe_id;
		this.skill_id = skill_id;
		this.avatar_id = avatar_id;
		this.animation_controller = animation_controller;
		this.animation_clean_file = animation_clean_file;
		this.active = active;
		this.sent = sent;

	}
}
