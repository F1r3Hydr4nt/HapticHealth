using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Activity
{
	/***
	 * Description:
	 * Activity class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public int player_id { get; set; }
	public int skill_id { get; set; }
	public int statistics_id { get; set; }
	public string datetime { get; set; }
	public int animation_id { get; set; }
	public int team_id { get; set; }
	public int active { get; set; }
	public int is_new { get; set; }
	public int sent { get; set; }
	public List<Feedback> feedbacks { get; set; }
	public Animation_Replay animation { get; set; }
	public Statistics statistics { get; set; }

	// Constructor that takes no arguments. 
	public Activity()
	{

	}

	public Activity(int id,int player_id,int skill_id,int statistics_id,string datetime,int animation_id,int team_id,int active,int is_new,int sent)
	{
		this.id = id;
		this.player_id = player_id;
		this.skill_id = skill_id;
		this.statistics_id = statistics_id;
		this.datetime = datetime;
		this.animation_id = animation_id;
		this.team_id = team_id;
		this.active = active;
		this.is_new = is_new;
		this.sent = sent;
	}
}
