using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Animation_Replay
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
	public int hero_id { get; set; }
	public int skill_id { get; set; }
	public int active { get; set; }
	public int sent { get; set; }
	public int is_new { get; set; }


	// Constructor that takes no arguments. 
	public Animation_Replay()
	{

	}

	public Animation_Replay(int id,string path,string creationDate,int hero_id,int skill_id, int active,int sent, int is_new)
	{
		this.id = id;
		this.path = path;
		this.creationDate = creationDate;
		this.hero_id = hero_id;
		this.skill_id = skill_id;
		this.active = active;
		this.sent = sent;
		this.is_new = is_new;
	}
}
