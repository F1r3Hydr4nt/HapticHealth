using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;


public class Coach
{

	/***
	 * Description:
	 * Coach class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public string name { get; set; }
	public string surname { get; set; }
	public string birthdate { get; set; }
	public int active { get; set; }
	public List<Discipline> disciplines { get; set; }
	public List<Club> clubs { get; set; }
	public List<Activity> activities { get; set; }
	public List<Feedback> feedbacks { get; set; }


	// Constructor that takes no arguments. 
	public Coach()
	{

	}

	public Coach(int id,string name,string surname,string birthdate,int active)
	{
		this.id = id;
		this.name = name;
		this.surname = surname;
		this.birthdate = birthdate;
		this.active = active;
	}
}
