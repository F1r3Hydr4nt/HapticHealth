using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Skill
{

	/***
	 * Description:
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public int discipline_id { get; set; }
	public int demo_content_id { get; set; }
	public int in_use { get; set; }
	public int evolved_from { get; set; }
	public int difficulty_level { get; set; }
	public int requires_skill { get; set; }
	public int active { get; set; }
	

	// Constructor that takes no arguments. 
	public Skill()
	{

	}

	public Skill(int id,string name,string description,int discipline_id,int demo_content_id,int in_use,int evolved_from,int difficulty_level,int requires_skill,int active)
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.discipline_id = discipline_id;
		this.demo_content_id = demo_content_id;
		this.in_use = in_use;
		this.evolved_from = evolved_from;
		this.difficulty_level = difficulty_level;
		this.requires_skill = requires_skill;
		this.active = active;
	}
}
