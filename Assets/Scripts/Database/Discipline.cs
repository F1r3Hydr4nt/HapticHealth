using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Discipline
{
	public int id { get; set; }
	public string name { get; set; }
	public string description { get; set; }
	public int modality_id { get; set; }
	public int active { get; set; }
	public List<Position> positions { get; set; }


	// Constructor that takes no arguments. 
	public Discipline()
	{

	}

	public Discipline(int id,string name,string description,int modality_id,int active)
	{
		this.id = id;
		this.name = name;
		this.description = description;
		this.modality_id = modality_id;
		this.active = active;
	}
}
