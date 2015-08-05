using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Animation_Player
{

	/***
	 * Description:
	 * Animation class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public int player_id { get; set; }
	public string path { get; set; }
	public string animation_clean_file { get; set; }
	public int active { get; set; }
	public int sent { get; set; }
	public int is_new { get; set; }

	// Constructor that takes no arguments. 
	public Animation_Player()
	{

	}

	public Animation_Player(int id,int player_id,string path,string animation_clean_file,int active,int sent,int is_new)
	{
		this.id = id;
		this.player_id = player_id;
		this.path = path;
		this.animation_clean_file = animation_clean_file;
		this.active = active;
		this.sent = sent;
		this.is_new = is_new;

	}
}
