using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Statistics_Plot
{
	
	/***
	 * Description:
	 * Statistics class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public int statistics_id { get; set; }
	public string plot_path { get; set; }
	public int active { get; set; }
	public int sent { get; set; }
	public int is_new { get; set; }

	// Constructor that takes no arguments. 
	public Statistics_Plot()
	{

	}

	public Statistics_Plot(int id,int statistics_id, string plot_path, int active, int sent, int is_new)
	{
		this.id = id;
		this.statistics_id = statistics_id;
		this.plot_path = plot_path;
		this.active = active;
		this.sent = sent;
		this.is_new = is_new;

	}
}
