using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class Feedback
{

	/***
	 * Description:
	 * Animation class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public int coach_id { get; set; }
	public int activity_id { get; set; }
	public string text { get; set; }
	public string datetime { get; set; }
	public int active { get; set; }
	public int sent { get; set; }
	public int is_new { get; set; }

	// Constructor that takes no arguments. 
	public Feedback()
	{

	}

	public Feedback(int id,int coach_id,int activity_id,string text,string datetime,int active,int sent,int is_new)
	{
		this.id = id;
		this.coach_id = coach_id;
		this.activity_id = activity_id;
		this.text = text;
		this.datetime = datetime;
		this.active = active;
		this.sent = sent;
		this.is_new = is_new;
	}
}
