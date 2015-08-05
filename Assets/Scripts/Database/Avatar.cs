using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Avatar
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
	public int active { get; set; }
	public List<Avatar_Texture> avatar_textures { get; set; }


	// Constructor that takes no arguments. 
	public Avatar()
	{

	}

	public Avatar(int id,string name, string path,int active)
	{
		this.id = id;
		this.name = name;
		this.path = path;
		this.active = active;

	}
}
