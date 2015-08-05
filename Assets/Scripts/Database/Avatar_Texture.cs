using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Avatar_Texture
{

	/***
	 * Description:
	 * Avatar class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public string name { get; set; }
	public string path { get; set; }
	public int avatar_id { get; set; }
	public int active { get; set; }


	// Constructor that takes no arguments. 
	public Avatar_Texture()
	{

	}

	public Avatar_Texture(int id,string name, string path,int avatar_id,int active)
	{
		this.id = id;
		this.name = name;
		this.path = path;
		this.avatar_id = avatar_id;
		this.active = active;

	}
}
