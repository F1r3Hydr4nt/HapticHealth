using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;



public class Statistics
{
	
	/***
	 * Description:
	 * Statistics class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public int id { get; set; }
	public int player_id { get; set; }
	public double position_score { get; set; }
	public double linear_velocity_score { get; set; }
	public double angle_score { get; set; }
	public double angular_velocity_score { get; set; }
	public double overall_score { get; set; }
	public string semantic_feedback { get; set; }
	public int sent { get; set; }
	public List<Statistics_Plot> statistics_plots { get; set; }

	// Constructor that takes no arguments. 
	public Statistics()
	{

	}

	public Statistics(int id,int player_id,double position_score,double linear_velocity_score, double angle_score,double angular_velocity_score,double overall_score, string semantic_feedback, int sent)
	{
		this.id = id;
		this.player_id = player_id;
		this.position_score = position_score;
		this.linear_velocity_score = linear_velocity_score;
		this.angle_score = angle_score;
		this.angular_velocity_score = angular_velocity_score;
		this.overall_score = overall_score;
		this.semantic_feedback = semantic_feedback;
		this.sent = sent;
	}
}
