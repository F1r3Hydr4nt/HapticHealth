using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


/// <summary>
/// Contains global variables for project.
/// </summary>
	/***
	 * Description:
	 * Global variables
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/	
public static class GlobalVariables
{

    public static int user_id { get; set; }
	public static int usertype_id { get; set; }
	public static int selected_club_id { get; set; }
	public static int selected_team_id { get; set; }
	public static int selected_player_id { get; set; }
	public static int selected_discipline_id { get; set; }
	public static int selected_modality_id { get; set; }
	public static int selected_skill_id { get; set; }
	public static int selected_activity_id { get; set; }
	public static int selected_feedback_id { get; set; }
	public static int selected_heroe_id { get; set; }
	public static string locale { get; set; }
	public static string users_database { get; set; }
	public static string user_database { get; set; }
	public static Coach coach { get; set; }
	public static Lang translations { get; set; }
	public static User user { get; set; }
	public static string LatestSkeletonCapture { get; set; }
}
