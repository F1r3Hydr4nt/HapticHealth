using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class User
{

	/***
	 * Description:
	 * User class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/
	 
	public int id { get; set; }
	public string username { get; set; }
	public string password { get; set; }
	public int usertype_id { get; set; }
	public string creationDate { get; set; }
	public int role_id { get; set; }
	public int sent { get; set; }
	public int is_new { get; set; }


	// Constructor that takes no arguments. 
	public User()
	{

	}

	public User(int id,string username,string password,int usertype_id,string creationDate,int role_id,int sent,int is_new)
	{
		this.id = id;
		this.username = username;
		this.password = password;
		this.usertype_id = usertype_id;
		this.creationDate = creationDate;
		this.role_id = role_id;
		this.sent = sent;
		this.is_new = is_new;
	}
}
