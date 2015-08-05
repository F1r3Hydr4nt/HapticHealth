using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using System.Collections;


public class Database_module : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	
	private string connectionString = "";
	
	private string tableName = "";
	private string[] items = {};
	private string[] col = {};
	private string[] operation = {};
	private string[] values = {};
	
	
	
	
	public static int user_id=0;
	
	public Database_module ()
	{
		
	}
	
	/**
	 *  This function initializes the tracking 
	 * 
	 *  return: bool
	 * 
	 **/
	public bool initializeTracking ()
	{
		return true;
	}
	
	/**
	 *  This function initializes the comparison 
	 * 
	 *  return: bool
	 * 
	 **/
	public bool initializeComparison ()
	{
		return true;
	}
	
	/**
	 *  This function logs to the system
	 * 
	 *  string username: the username
	 *  string password: the password
	 * 
	 *  return: User
	 * 
	 **/
	public User login (string username, string password)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+ GlobalVariables.users_database;
		
		//Debug.Log ("password: "+password);
		password = HashPassword (password);
		//Debug.Log ("hash: "+password);
		
		
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		sql = "SELECT * FROM user WHERE username='" + username + "' AND password='" + password + "'";
		//Debug.Log ("sql: "+sql);
		dbCommand.CommandText = sql;
		reader = dbCommand.ExecuteReader ();
		
		int _id = 0;
		string _username = "";
		string _password = "";
		int _usertype_id = 0;
		string _creationDate = "";
		int _role = 0;
		int _sent = 1;
		int _is_new = 1;
		User user;
		
		while(reader.Read()) {
			_id = Int32.Parse(reader.GetString(0));
			_username = reader.GetString (1);
			_password = reader.GetString(2);
			_usertype_id = Int32.Parse(reader.GetString(3));
			_creationDate = reader.GetString(4);
			_role = Int32.Parse(reader.GetString(5));
			_sent = Int32.Parse(reader.GetString(6));
			_is_new = Int32.Parse(reader.GetString(7));
			
			// Set the id to the GlobalVariable user_id
			GlobalVariables.user_id = _id;
			GlobalVariables.user_database = "replay_user-" + _id+".db";
		}
		
		
		
		user = new User (_id, _username, _password, _usertype_id, _creationDate, _role, _sent, _is_new);
		
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		
		return user;
	}
	
	
	/**
	 *  This function creates a new user or updates an existing one
	 * 
	 *  User user: user to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public User saveUser (string database, User user)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = user.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		if (id == 0) 
		{
			// Insert new User
			
			sql= "INSERT INTO user (username,password,usertype_id,creationDate,role_id,sent,is_new) VALUES ('"+user.username+"','"+user.password+"',"+user.usertype_id.ToString()+",'"+user.creationDate+"',"+user.role_id.ToString()+",1,1)";
			//Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			
			// Select the last temporal index inserted to know 
			sql= "SELECT last_insert_rowid()";
			
			//Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				user.id = _id;
				
			}
			
		} 
		else 
		{
			// Update user
			sql= "UPDATE user SET username='"+user.username+"',password='"+user.password+"',usertype_id="+user.usertype_id.ToString()+",creationDate='"+user.creationDate+"',role_id="+user.role_id.ToString()+",sent=1"+
				" WHERE id="+id.ToString();
			//Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
		}
		
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return user;
		
	}
	
	
	/**
	 *  This function sets if a user has been modified or not
	 * 
	 *  string database: database that is going to be used (there are 2 replay_users and replay_user-ID)
	 *  int user_id: user identifier
	 *  int sent_or_not: mark as sent or not
	 * 
	 *  return: void
	 * 
	 **/
	private void userIsModified (string database, int user_id, int sent_or_not)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		
		// Update user
		sql= "UPDATE user SET sent="+sent_or_not.ToString()+
			" WHERE id="+user_id.ToString();
		//Debug.Log (sql);
		dbCommand.CommandText = sql;
		reader = dbCommand.ExecuteReader ();
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		
	}
	
	
	/**
	 *  This function gets the list of clubs of user
	 * 
	 *  int user_id: user identifier
	 * 
	 *  return: List<Club>
	 * 
	 **/
	public List<Club> getClubs(int user_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Club> clubs = new List<Club> ();
		// Select
		string sql = "SELECT * FROM club WHERE active=0";
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			Club club = getClub(_id);
			clubs.Add(club);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return clubs;
	}
	
	/**
	 *  This function gets a club from its identifier
	 * 
	 *  int club_id: club identifier
	 * 
	 *  return: Club
	 * 
	 **/
	public Club getClub(int club_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Club club = new Club ();
		// Select
		string sql = "SELECT * FROM club WHERE id="+club_id;
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _name = reader.GetString (1);
			int _modality_id = Int32.Parse(reader.GetString(2));
			string _photo = reader.GetString(3);
			string _thumbnail = reader.GetString(4);
			int _active = Int32.Parse(reader.GetString(5));
			club = new Club(_id,_name,_modality_id,_photo,_thumbnail,_active);
			//Debug.Log ("club_id:"+_id+", name:"+_name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return club;
	}
	
	/**
	 *  This function gets the list of teams of club
	 * 
	 *  int club_id: club identifier
	 * 
	 *  return: List<Team>
	 * 
	 **/
	public List<Team> getTeams(int club_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Team> teams = new List<Team> ();
		// Select
		string sql = "SELECT * FROM team WHERE club_id="+club_id+" AND active=0";
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			Team team = getTeam(_id);
			teams.Add(team);
			//Debug.Log ("team_id:"+team.id+", name:"+team.name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return teams;
	}
	
	/**
	 *  This function gets a team from its identifier
	 * 
	 *  int team_id: team identifier
	 * 
	 *  return: Team
	 * 
	 **/
	public Team getTeam(int team_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Team team = new Team();
		// Select
		string sql = "SELECT * FROM team WHERE id="+team_id;
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _name = reader.GetString (1);
			int _club_id = Int32.Parse(reader.GetString(2));
			int _discipline_id = Int32.Parse(reader.GetString(3));
			string _photo = reader.GetString (4);
			string _thumbnail = reader.GetString (5);
			int _active = Int32.Parse(reader.GetString(6));
			team = new Team(_id,_name,_club_id,_discipline_id,_photo,_thumbnail,_active);
			//Debug.Log ("team_id:"+_id+", name:"+_name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return team;
	}
	
	/**
	 *  This function gets the list of players of a team
	 * 
	 *  string team_id: team identifier
	 * 
	 *  return: List<Player>
	 * 
	 **/
	public List<Player> getPlayers(int team_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Player> players = new List<Player> ();
		// Select
		string sql = "SELECT p.* FROM player AS p ";
		sql += "INNER JOIN player_team AS pt ON pt.player_id = p.id ";
		sql += "WHERE pt.team_id ="+team_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			//Player player = new Player(_id,_name,_surname,_height,_weight,_righthanded,_gender,_photo,_thumbnail,_birthdate,_club_id,_squad_id,_active);
			Player player = getPlayer(_id);
			players.Add(player);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return players;
	}
	
	/**
	 *  This function gets a player from its identifier
	 * 
	 *  string player_id: player identifier
	 * 
	 *  return: Player
	 * 
	 **/
	public Player getPlayer(int player_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Player player = new Player();
		// Select
		string sql = "SELECT p.* FROM player AS p ";
		sql += "WHERE p.id ="+player_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _name = reader.GetString (1);
			string _surname = reader.GetString (2);
			double _height = Double.Parse (reader.GetString (3));
			double _weight = Double.Parse (reader.GetString (4));
			int _righthanded = Int32.Parse(reader.GetString(5));
			int _gender = Int32.Parse(reader.GetString(6));
			string _photo = reader.GetString (7);
			string _thumbnail = reader.GetString (8);
			string _birthdate = reader.GetString (9);
			int _club_id = Int32.Parse(reader.GetString(10));
			int? _squad_id=null;
			if(!reader.IsDBNull(11))
				_squad_id = Int32.Parse(reader.GetString(11));
			int _active = Int32.Parse(reader.GetString(12));
			int _is_sent = Int32.Parse(reader.GetString(13));
			//Load the player
			player = new Player(_id,_name,_surname,_height,_weight,_righthanded,_gender,_photo,_thumbnail,_birthdate,_club_id,_squad_id,_active,_is_sent);
			//Load the teams of the player
			/*List<Team> teams = getPlayerTeams(_id);
			player.teams = teams;
			List<Activity> activities = getPlayerActivities(_id);
			player.activities = activities;
			List<PlayerPosition> positions = getPlayerPositions(_id);
			player.playerpositions=positions;*/
			//Debug.Log ("player_id:"+_id+", name:"+_name+", surname:"+_surname);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return player;
	}
	
	/**
	 *  This function gets the list of teams of a player
	 * 
	 *  int player_id: player identifier
	 * 
	 *  return: List<Team>
	 * 
	 **/
	public List<Team> getPlayerTeams(int player_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Team> teams = new List<Team>();
		// Select
		string sql = "SELECT pt.* FROM player_team AS pt ";
		sql += "WHERE pt.player_id ="+player_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _player_id = Int32.Parse(reader.GetString(0));
			int _team_id = Int32.Parse(reader.GetString(1));
			
			Team team = getTeam(_team_id);
			teams.Add(team);
			//Debug.Log ("player_id:"+player_id+", team_id:"+team.id+", name:"+team.name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return teams;
	}
	
	
	/**
	 *  This function gets the list of positions of a player
	 * 
	 *  int player_id: player identifier
	 * 
	 *  return: List<Team>
	 * 
	 **/
	public List<PlayerPosition> getPlayerPositions(int player_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<PlayerPosition> positions = new List<PlayerPosition>();
		PlayerPosition position = new PlayerPosition ();
		
		// Select
		string sql = "SELECT pp.* FROM player_position AS pp ";
		sql += "WHERE pp.player_id ="+player_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _player_id = Int32.Parse(reader.GetString(0));
			int _team_id = Int32.Parse(reader.GetString(1));
			int _position_id = Int32.Parse(reader.GetString(2));
			
			position = new PlayerPosition(_position_id,_player_id,_team_id);
			positions.Add(position);
			
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return positions;
	}
	
	
	/**
	 *  This function gets the list of positions of a player by his team
	 * 
	 *  int player_id: player identifier
	 *  int team_id: team identifier
	 * 
	 *  return: List<Team>
	 * 
	 **/
	public List<PlayerPosition> getPlayerPositions(int player_id, int team_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<PlayerPosition> positions = new List<PlayerPosition>();
		PlayerPosition position = new PlayerPosition ();
		
		// Select
		string sql = "SELECT pp.* FROM player_position AS pp ";
		sql += "WHERE pp.player_id ="+player_id+" AND pp.team_id="+team_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _player_id = Int32.Parse(reader.GetString(0));
			int _team_id = Int32.Parse(reader.GetString(1));
			int _position_id = Int32.Parse(reader.GetString(2));
			
			position = new PlayerPosition(_position_id,_player_id,_team_id);
			positions.Add(position);
			
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return positions;
	}
	
	
	
	/**
	 *  This function gets the list of positions of a player
	 * 
	 *  int player_id: player identifier
	 * 
	 *  return: List<Team>
	 * 
	 **/
	public List<PlayerPosition2> getPlayerPositions2(int player_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		
		
		List<Team> teams =  getPlayerTeams (player_id);
		List<PlayerPosition2> playerpositions =  new List<PlayerPosition2>();
		PlayerPosition2 playerposition =  new PlayerPosition2();
		
		foreach(Team team in teams){
			
			dbConnection = new SqliteConnection (connectionString);
			dbConnection.Open ();
			
			playerposition =  new PlayerPosition2(team);
			List<Position> positions =  new List<Position>();
			// Select
			string sql = "SELECT pp.* FROM player_position AS pp ";
			sql += "WHERE pp.player_id ="+player_id+" AND pp.team_id="+team.id;
			
			//Debug.Log (sql);
			dbCommand = dbConnection.CreateCommand ();
			dbCommand.CommandText = sql;
			
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _player_id = Int32.Parse(reader.GetString(0));
				int _team_id = Int32.Parse(reader.GetString(1));
				int _position_id = Int32.Parse(reader.GetString(2));
				
				Position position = getPosition(_position_id);
				positions.Add(position);
				
				
			}
			playerposition.positions = positions;
			playerpositions.Add(playerposition);
		}
		
		
		/*if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;*/
		
		return playerpositions;
	}
	
	
	/**
	 *  This function gets the list of skills of a discipline
	 * 
	 *  int discipline_id: discipline identifier
	 * 
	 *  return: List<Skill>
	 * 
	 **/
	public List<Skill> getSkills(int discipline_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Skill> skills = new List<Skill>();
		// Select
		string sql = "SELECT * FROM skill AS s ";
		sql += "WHERE s.discipline_id ="+discipline_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			Skill skill = getSkill (_id);
			skills.Add(skill);
			//Debug.Log ("skill_id:"+skill.id+", name:"+skill.name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return skills;
	}
	
	/**
	 *  This function gets a skill from its identifier
	 * 
	 *  int skill_id: player identifier
	 * 
	 *  return: Skill
	 * 
	 **/
	public Skill getSkill(int skill_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Skill skill = new Skill();
		// Select
		string sql = "SELECT * FROM skill AS s ";
		sql += "WHERE s.id ="+skill_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _name_int = Int32.Parse(reader.GetString(1));
			string _name = translateText(_name_int,GlobalVariables.locale);
			int _description_int = Int32.Parse(reader.GetString(2));
			string _description = translateText(_description_int,GlobalVariables.locale);
			int _discipline_id = Int32.Parse(reader.GetString(3));
			int _demo_content_id = 0;
			if(!reader.IsDBNull(4))
				_demo_content_id = Int32.Parse(reader.GetString(4));
			int _in_use = Int32.Parse(reader.GetString(5));
			int _evolved_from = 0;
			if(!reader.IsDBNull(6))
				_evolved_from = Int32.Parse(reader.GetString(6));
			int _difficulty_level = Int32.Parse(reader.GetString(7));
			int _requires_skill = 0;
			if(!reader.IsDBNull(8))
				_requires_skill = Int32.Parse(reader.GetString(8));
			int _active = Int32.Parse(reader.GetString(9));
			
			skill = new Skill(_id,_name,_description,_discipline_id,_demo_content_id,_in_use,_evolved_from,_difficulty_level,_requires_skill,_active);
			
			//Debug.Log ("skill_id:"+skill.id+", name:"+skill.name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return skill;
	}
	
	/**
	 *  This function gets an activity from its identifier
	 * 
	 *  int activity_id: activity identifier
	 * 
	 *  return: Activity
	 * 
	 **/
	public Activity getActivity(int activity_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Activity activity = new Activity();
		// Select
		string sql = "SELECT * FROM activity AS a ";
		sql += "WHERE a.id ="+activity_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _player_id = Int32.Parse(reader.GetString(1));
			int _skill_id = Int32.Parse(reader.GetString(2));
			int _statistics_id = Int32.Parse(reader.GetString(3));
			string _datetime = reader.GetString(4);
			int _animation_id = Int32.Parse(reader.GetString(5));
			int _team_id = Int32.Parse(reader.GetString(6));
			int _active = Int32.Parse(reader.GetString(7));
			int _sent = Int32.Parse(reader.GetString(8));
			int _is_new = Int32.Parse(reader.GetString(9));
			
			activity = new Activity(_id,_player_id,_skill_id,_statistics_id,_datetime,_animation_id,_team_id,_active,_sent,_is_new);
			List<Feedback> feedbacks =  getActivityFeedbacks(_id);
			activity.feedbacks = feedbacks;
			//Animation_Replay animation = getAnimation (_animation_id);
			//activity.animation = animation;
			Statistics statistics = getStatistics(_statistics_id);
			activity.statistics = statistics;
			
			
			//Debug.Log ("activity_id:"+activity.id+", skill_id:"+activity.skill_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return activity;
	}
	
	/**
	 *  This function gets the list of activities of a player
	 * 
	 *  int player_id: player identifier
	 * 
	 *  return: List<Team>
	 * 
	 **/
	public List<Activity> getPlayerActivities(int player_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Activity> activities = new List<Activity>();
		// Select
		string sql = "SELECT * FROM activity AS a ";
		sql += "WHERE a.player_id ="+player_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			Activity activity = getActivity(_id);
			activities.Add(activity);
			//Debug.Log ("activity_id:"+activity.id+", skill_id:"+activity.skill_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return activities;
	}
	
	/**
	 *  This function gets the list of activities of a player depending on the skill
	 * 
	 *  int player_id: player identifier
	 *  int skill_id: skill identifier
	 * 
	 *  return: List<Activity>
	 * 
	 **/
	public List<Activity> getPlayerActivities(int player_id, int skill_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Activity> activities = new List<Activity>();
		// Select
		string sql = "SELECT * FROM activity AS a ";
		sql += "WHERE a.player_id ="+player_id+" AND a.skill_id="+skill_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			Activity activity = getActivity(_id);
			activities.Add(activity);
			//Debug.Log ("activity_id:"+activity.id+", skill_id:"+activity.skill_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return activities;
	}
	
	
	/**
	 *  This function gets the list of activities of a player depending on the team
	 * 
	 *  int player_id: player identifier
	 *  int team: team identifier
	 * 
	 *  return: List<Activity>
	 * 
	 **/
	public List<Activity> getPlayerActivitiesFromTeam(int player_id, int team_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Activity> activities = new List<Activity>();
		// Select
		string sql = "SELECT * FROM activity AS a ";
		sql += "WHERE a.player_id ="+player_id+" AND a.team_id="+team_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			Activity activity = getActivity(_id);
			activities.Add(activity);
			//Debug.Log ("activity_id:"+activity.id+", skill_id:"+activity.skill_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return activities;
	}
	
	
	/**
	 *  This function gets the statistics from its identifier
	 * 
	 *  int statistics_id: statistics identifier
	 * 
	 *  return: Statistics
	 * 
	 **/
	public Statistics getStatistics(int statistics_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Statistics statistics = new Statistics();
		
		// Select
		string sql = "SELECT * FROM statistics AS s ";
		sql += "WHERE s.id ="+statistics_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _player_id = Int32.Parse(reader.GetString(1));
			double _position_score = Double.Parse(reader.GetString(2));
			double _linear_velocity_score = Double.Parse(reader.GetString(3));
			double _angle_score = Double.Parse(reader.GetString(4));
			double _angular_velocity_score = Double.Parse(reader.GetString(5));
			double _overall_score = Double.Parse(reader.GetString(6));
			string _semantic_feedback = reader.GetString(7);
			int _sent = Int32.Parse(reader.GetString(8));
			
			statistics = new Statistics(_id,_player_id,_position_score,_linear_velocity_score,_angle_score,_angular_velocity_score,_overall_score,_semantic_feedback,_sent);
			statistics.statistics_plots = getStatisticsPlotsFromStatistics(_id);
			//Debug.Log ("activity_id:"+statistics.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return statistics;
	}
	
	
	/**
	 *  This function gets the statistics from an activity
	 * 
	 *  int activity_id: player identifier
	 * 
	 *  return: Statistics
	 * 
	 **/
	public Statistics getActivityStatistics(int activity_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		Activity activity = getActivity (activity_id);
		int statistics_id = activity.statistics_id;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Statistics statistics = new Statistics();
		
		// Select
		string sql = "SELECT * FROM statistics AS s ";
		sql += "WHERE s.statistics_id ="+statistics_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			statistics = getStatistics(_id);
			//Debug.Log ("activity_id:"+statistics.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return statistics;
	}
	
	
	
	/**
	 *  This function gets the animation from an activity
	 * 
	 *  int animation_id: animation identifier
	 * 
	 *  return: Animation
	 * 
	 **/
	public Animation_Replay getActivityAnimation(int activity_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		Activity activity = getActivity (activity_id);
		int animation_id = activity.animation_id;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Animation_Replay animation = new Animation_Replay();
		
		// Select
		string sql = "SELECT * FROM animation AS a ";
		sql += "WHERE a.id ="+animation_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			animation = getAnimation(_id);
			//Debug.Log ("activity_id:"+statistics.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return animation;
	}
	
	
	/**
	 *  This function gets the animation from its identifier
	 * 
	 *  int animation_id: animation identifier
	 * 
	 *  return: Animation
	 * 
	 **/
	public Animation_Replay getAnimation(int animation_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Animation_Replay animation = new Animation_Replay();
		
		// Select
		string sql = "SELECT * FROM animation AS a ";
		sql += "WHERE a.id ="+animation_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _path = reader.GetString(1);
			string _creationDate = reader.GetString(2);
			int _hero_id = Int32.Parse(reader.GetString(3));
			int _skill_id = Int32.Parse(reader.GetString(4));
			int _active = Int32.Parse(reader.GetString(5));
			int _sent = Int32.Parse(reader.GetString(6));
			int _is_new = Int32.Parse(reader.GetString(7));
			
			animation = new Animation_Replay(_id,_path,_creationDate,_hero_id,_skill_id,_active,_sent,_is_new);
			
			//Debug.Log ("activity_id:"+aniamtion.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return animation;
	}
	
	
	/**
	 *  This function gets a coach from its identifier
	 * 
	 *  int coach_id: coach identifier
	 * 
	 *  return: Coach
	 * 
	 **/
	public Coach getCoach(int coach_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Coach coach = new Coach();
		// Select
		string sql = "SELECT c.* FROM coach AS c ";
		sql += "WHERE c.id ="+coach_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _name = reader.GetString (1);
			string _surname = reader.GetString (2);
			string _birthdate = reader.GetString (3);
			int _active = Int32.Parse(reader.GetString(4));
			//Load the coach
			coach = new Coach(_id,_name,_surname,_birthdate,_active);
			List<Club> clubs = getClubs(_id);
			coach.clubs = clubs;
			List<Discipline> disciplines = getCoachDisciplines(_id);
			coach.disciplines = disciplines;
			List<Activity> activities = getCoachActivities(_id);
			coach.activities = activities;
			List<Feedback> feedbacks = getCoachFeedbacks(_id);
			coach.feedbacks = feedbacks;
			
			//Debug.Log ("coach_id:"+coach.id+", name:"+coach.name+", surname:"+coach.surname);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return coach;
	}
	
	
	/**
	 *  This function gets a coach from its identifier
	 * 
	 *  int coach_id: coach identifier
	 * 
	 *  return: Coach
	 * 
	 **/
	public List<Discipline> getCoachDisciplines(int coach_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Discipline> disciplines = new List<Discipline>();
		Discipline discipline = new Discipline();
		// Select
		string sql = "SELECT cd.* FROM coach_discipline AS cd ";
		sql += "WHERE cd.coach_id ="+coach_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _discipline_id = Int32.Parse(reader.GetString (1));
			
			//Load the discipline
			discipline = getDiscipline(_discipline_id);
			disciplines.Add(discipline);
			
			//Debug.Log ("coach_id:"+_id+", disicipline_id:"+discipline.name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return disciplines;
	}
	
	
	/**
	 *  This function gets a list of activities of a coach
	 * 
	 *  int coach_id: coach identifier
	 * 
	 *  return: List<Activity>
	 * 
	 **/
	public List<Activity> getCoachActivities(int coach_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Activity> activities = new List<Activity>();
		Activity activity = new Activity();
		// Select
		string sql = "SELECT a.* FROM activity AS a ";
		sql += "ORDER BY a.id DESC";
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			
			//Load the activity
			activity = getActivity(_id);
			activities.Add(activity);
			
			//Debug.Log ("activity_id:"+_id+", player_id:"+activity.player_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return activities;
	}
	
	
	/**
	 *  This function gets a list of feedbacks of a coach
	 * 
	 *  int coach_id: coach identifier
	 * 
	 *  return: List<Feedback>
	 * 
	 **/
	public List<Feedback> getCoachFeedbacks(int coach_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Feedback> feedbacks = new List<Feedback>();
		Feedback feedback = new Feedback();
		// Select
		string sql = "SELECT f.* FROM feedback AS f ";
		sql += "WHERE f.coach_id="+coach_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			
			//Load the activity
			feedback = getFeedback(_id);
			feedbacks.Add(feedback);
			
			//Debug.Log ("feedback_id:"+_id+", coach_id:"+feedback.coach_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return feedbacks;
	}
	
	
	/**
	 *  This function gets a list of feedbacks of a player for a team
	 * 
	 *  int player_id: player identifier
	 *  int team_id: team identifier
	 * 
	 *  return: List<Feedback>
	 * 
	 **/
	public List<Feedback> getPlayerFeedbacksFromTeam(int player_id,int team_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Feedback> feedbacks = new List<Feedback>();
		Feedback feedback = new Feedback();
		// Select
		string sql = "SELECT f.* FROM feedback AS f ";
		sql += "INNER JOIN activity AS a ON a.id = f.activity_id ";
		sql += "WHERE a.player_id="+player_id+" AND a.team_id="+team_id;
		sql += " ORDER BY f.id DESC";
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			
			//Load the activity
			feedback = getFeedback(_id);
			feedbacks.Add(feedback);
			
			//Debug.Log ("feedback_id:"+_id+", coach_id:"+feedback.coach_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return feedbacks;
	}
	
	/**
	 *  This function gets a feedback from its identifier
	 * 
	 *  int feedback_id: feedback identifier
	 * 
	 *  return: Feedback
	 * 
	 **/
	public Feedback getFeedback(int feedback_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Feedback feedback = new Feedback();
		// Select
		string sql = "SELECT * FROM feedback AS f ";
		sql += "WHERE f.id ="+feedback_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _coach_id = Int32.Parse(reader.GetString(1));
			int _activity_id = Int32.Parse(reader.GetString(2));
			string _text = reader.GetString(3);
			string _datetime = reader.GetString(4);
			int _active = Int32.Parse(reader.GetString(5));
			int _sent = Int32.Parse(reader.GetString(6));
			int _is_new = Int32.Parse(reader.GetString(7));
			
			feedback = new Feedback(_id,_coach_id,_activity_id,_text,_datetime,_active,_sent,_is_new);
			
			//Debug.Log ("feedback_id:"+feedback.id+", coach_id:"+feedback.coach_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return feedback;
	}
	
	
	/**
	 *  This function gets a list of feedback from its identifier
	 * 
	 *  int activity_id: activity identifier
	 * 
	 *  return: List<Feedback>
	 * 
	 **/
	public List<Feedback> getActivityFeedbacks(int activity_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Feedback feedback = new Feedback();
		List<Feedback> feedbacks = new List<Feedback>();
		// Select
		string sql = "SELECT * FROM feedback AS f ";
		sql += "WHERE f.activity_id ="+activity_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _coach_id = Int32.Parse(reader.GetString(1));
			int _activity_id = Int32.Parse(reader.GetString(2));
			string _text = reader.GetString(3);
			string _datetime = reader.GetString(4);
			int _active = Int32.Parse(reader.GetString(5));
			int _sent = Int32.Parse(reader.GetString(6));
			int _is_new = Int32.Parse(reader.GetString(7));
			
			feedback = new Feedback(_id,_coach_id,_activity_id,_text,_datetime,_active,_sent,_is_new);
			feedbacks.Add(feedback);
			
			
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return feedbacks;
	}
	
	/**
	 *  This function creates a new coach or updates an existing one
	 * 
	 *  Coach coach: coach to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public void saveCoach (Coach coach)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		
		int user_id = GlobalVariables.user_id;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		
		
		
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = coach.id;
		string sql="";
		
		if (id == 0) 
		{
			// Insert new User
			//dbaccess.InsertInto ("user",values );
		} 
		else 
		{
			// Update coach
			sql= "UPDATE coach SET name='"+coach.name+"',surname='"+coach.surname+"',birthdate='"+coach.birthdate+"',active="+coach.active+
				" WHERE id="+id.ToString();
			//Debug.Log (sql);
		}
		
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		
	}
	
	
	/**
	 *  This function creates a new player or updates an existing one
	 * 
	 *  Player player: player to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public void savePlayer (Player player)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		
		int user_id = GlobalVariables.user_id;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		
		
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = player.id;
		string sql="";
		
		if (id == 0) 
		{
			// Insert new User
			//dbaccess.InsertInto ("user",values );
		} 
		else 
		{
			// Update coach
			sql= "UPDATE player SET name='"+player.name+"',surname='"+player.surname+"',height="+player.height.ToString()+",weight="+player.weight.ToString()+",righthanded="+player.righthanded.ToString()+
				",gender="+player.gender.ToString()+",photo='"+player.photo+"',thumbnail='"+player.thumbnail+"',birthdate='"+player.birthdate+"',active="+player.active.ToString()+",sent="+player.sent.ToString()+
					" WHERE id="+id.ToString();
			//Debug.Log (sql);
		}
		
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		
	}
	
	
	/**
	 *  This function gets a discipline from its identifier
	 * 
	 *  int discipline_id: discipline identifier
	 * 
	 *  return: Discipline
	 * 
	 **/
	public Discipline getDiscipline(int discipline_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		
		Discipline discipline = new Discipline();
		// Select
		string sql = "SELECT d.* FROM discipline AS d ";
		sql += "WHERE d.id ="+discipline_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _name_int = Int32.Parse(reader.GetString(1));
			string _name = translateText(_name_int,GlobalVariables.locale);
			int _description_int = Int32.Parse(reader.GetString(2));
			string _description = translateText(_description_int,GlobalVariables.locale);
			int _modality_id = Int32.Parse(reader.GetString(3));
			int _active = Int32.Parse(reader.GetString(4));
			
			//Load the discipline
			discipline = new Discipline(_id,_name,_description,_modality_id,_active);
			List<Position> positions= getDisciplinePositions(_id);
			discipline.positions = positions;
			
			//Debug.Log ("discipline_id:"+discipline.id+", name:"+discipline.name+", description:"+discipline.description);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return discipline;
	}
	
	
	/**
	 *  This function gets a list of positions of a discipline
	 * 
	 *  int discipline_id: discipline identifier
	 * 
	 *  return: List<Position>
	 * 
	 **/
	public List<Position> getDisciplinePositions(int discipline_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Position> positions = new List<Position> ();
		Position position = new Position();
		// Select
		string sql = "SELECT dp.* FROM discipline_position AS dp ";
		sql += "WHERE dp.discipline_id ="+discipline_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _discipline_id = Int32.Parse(reader.GetString(0));
			int _position_id = Int32.Parse(reader.GetString(1));
			
			//Load the discipline
			position = getPosition(_position_id);
			positions.Add(position);
			
			//Debug.Log ("discipline_id:"+discipline.id+", name:"+discipline.name+", description:"+discipline.description);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return positions;
	}
	
	
	/**
	 *  This function gets a position from its identifier
	 * 
	 *  int position_id: position identifier
	 * 
	 *  return: Position
	 * 
	 **/
	public Position getPosition(int position_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		
		Position position = new Position();
		// Select
		string sql = "SELECT p.* FROM position AS p ";
		sql += "WHERE p.id ="+position_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _name_int = Int32.Parse(reader.GetString(1));
			string _name = translateText(_name_int,GlobalVariables.locale);
			int _active = Int32.Parse(reader.GetString(2));
			
			//Load the discipline
			position = new Position(_id,_name,_active);
			
			
			//Debug.Log ("position_id:"+position.id+", name:"+position.name);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return position;
	}
	
	
	/**
	 *  This function gets the heroe from its identifier
	 * 
	 *  int heroe_id: heroe identifier
	 * 
	 *  return: Heroe
	 * 
	 **/
	public Heroe getHeroe(int heroe_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Heroe heroe = new Heroe();
		
		// Select
		string sql = "SELECT * FROM heroe AS h ";
		sql += "WHERE h.id ="+heroe_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _name = reader.GetString(1);
			string _surname = reader.GetString(2);
			string _nickname = reader.GetString(3);
			string _photo = reader.GetString(4);
			string _thumbnail = reader.GetString(5);
			int _isNational = Int32.Parse(reader.GetString(6));
			int _isActive = Int32.Parse(reader.GetString(7));
			int _active = Int32.Parse(reader.GetString(8));
			
			heroe = new Heroe(_id,_name,_surname,_nickname,_photo,_thumbnail,_isNational,_isActive,_active);
			
			//Debug.Log ("activity_id:"+aniamtion.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return heroe;
	}
	
	
	/**
	 *  This function gets heroes of a skill
	 * 
	 *  int skill_id: skill identifier
	 * 
	 *  return: list of Heroe
	 * 
	 **/
	public List<Heroe> getHeroes(int skill_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		List<Animation_Heroe> animation_heroes = getAnimationHeroesFromSkill (skill_id);
		List<Heroe> heroes = new List<Heroe>();
		Heroe heroe = new Heroe();
		
		int itemCount = animation_heroes.Count;
		for (int i = 0; i < itemCount; i++) 
		{
			int heroe_id = animation_heroes[i].heroe_id;
			heroe = getHeroe(heroe_id);
			heroes.Add(heroe);
		}
		
		
		
		return heroes;
	}
	
	
	/**
	 *  This function gets animation_heroes of a skill
	 * 
	 *  int skill_id: skill identifier
	 * 
	 *  return: list of animation_heroe
	 * 
	 **/
	public List<Animation_Heroe> getAnimationHeroesFromSkill(int skill_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Animation_Heroe> animation_heroes = new List<Animation_Heroe>();
		Animation_Heroe animation_hero = new Animation_Heroe();
		
		// Select
		string sql = "SELECT * FROM animation_heroe AS ah ";
		sql += "WHERE ah.skill_id ="+skill_id;
		
		
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			animation_hero = getAnimationHeroe(_id);
			animation_heroes.Add(animation_hero);
			
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return animation_heroes;
	}
	
	
	
	/**
	 *  This function gets animation_heroe from identifier
	 * 
	 *  int animation_heroe_id: animation_heroe identifier
	 * 
	 *  return: animation_heroe
	 * 
	 **/
	public Animation_Heroe getAnimationHeroe(int animation_heroe_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		
		Animation_Heroe animation_hero = new Animation_Heroe();
		
		// Select
		string sql = "SELECT * FROM animation_heroe AS ah ";
		sql += "WHERE ah.id ="+animation_heroe_id;
		
		
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _path = reader.GetString(1);
			string _creationDate = reader.GetString(2);
			int _heroe_id = Int32.Parse(reader.GetString(3));
			int _skill_id = Int32.Parse(reader.GetString(4));
			int _avatar_id = Int32.Parse(reader.GetString(5));
			string _animation_controller = reader.GetString(6);
			string _animation_clean_file = reader.GetString(7);
			int _active = Int32.Parse(reader.GetString(8));
			int _sent = Int32.Parse(reader.GetString(9));
			
			
			animation_hero = new Animation_Heroe(_id,_path,_creationDate,_heroe_id,_skill_id,_avatar_id,_animation_controller,_animation_clean_file,_active,_sent);
			
			
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return animation_hero;
	}
	
	
	
	/**
	 *  This function plays a 3D demo of a skill performed by a heroe
	 * 
	 *  int skill_id: skill identifier
	 *  int heroe_id: heroe identifier
	 * 
	 *  return: void
	 * 
	 **/
	public void play3DDemo(int skill_id,int heroe_id)
	{
		// Play the 3D demo
		
		
	}
	
	
	/**
	 *  This function stops the 3D demo playing 
	 * 
	 * 
	 *  return: void
	 * 
	 **/
	public void stop3DDemo()
	{
		// Stop the 3D demo
		
		
	}
	
	/**
	 *  This function returns if a user has been detected
	 * 
	 * 
	 *  return: bool
	 * 
	 **/
	public bool isUserDetected()
	{
		
		return true;
		
	}
	
	
	/**
	 *  This function starts to record a motion
	 * 
	 * 
	 *  return: void
	 * 
	 **/
	public void recordMotion()
	{
		// Records a motion
		
		
	}
	
	
	/**
	 *  This function stops the recording and creates an animation
	 * 
	 * 
	 *  return: Animation
	 * 
	 **/
	public Animation_Replay stopRecordMotion()
	{
		// Stops the recording and creates an animation
		return new Animation_Replay ();
	}
	
	
	/**
	 *  This function saves the animation
	 * 
	 * 
	 *  return: bool
	 * 
	 **/
	public bool saveAnimationData(Animation_Replay animation)
	{
		// Saves the animation data
		return true;
	}
	
	
	/**
	 *  This function gets the animation of a heroe for a skill
	 * 
	 *  int heroe_id: heroe identifier
	 *  int skill_id: skill identifier
	 * 
	 *  return: Animation
	 * 
	 **/
	public Animation_Replay getHeroeAnimation(int heroe_id,int skill_id)
	{
		// 
		return new Animation_Replay ();
		
	}
	
	
	/**
	 *  This function gets the animation of a heroe for a skill
	 * 
	 *  Animation heroe_animation: heroe animation
	 *  Animation player_animation: player animation
	 * 
	 *  return: Statistics
	 * 
	 **/
	public Statistics computeComparison(Animation_Replay heroe_animation, Animation_Replay player_animation)
	{
		// 
		return new Statistics ();
		
	}
	
	
	/**
	 *  This function gets frame from kinect
	 * 
	 * 
	 *  return: Object
	 * 
	 **/
	public UnityEngine.Object catchKinectFrame()
	{
		// 
		return new UnityEngine.Object();
		
	}
	
	
	/**
	 *  This function returns if the user has internet access or not
	 * 
	 * 
	 *  return: bool
	 * 
	 **/
	public bool hasInternetAccess()
	{
		
		return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable ();
		
	}
	
	
	/**
	 *  Translates the text for the selected locale
	 * 
	 *  int literal_id: literal identifier
	 *  string locale: locale
	 * 
	 *  return: string
	 * 
	 **/
	private string translateText(int literal_id,string locale){
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		if (locale == null)
			locale = GlobalVariables.locale;
		
		string translation = "";
		string sql = "SELECT ll.text FROM literal_language AS ll ";
		sql+= "INNER JOIN language AS la ON la.id = ll.language_id ";
		sql+= "INNER JOIN literal AS li ON li.id = ll.literal_id ";
		sql+= "WHERE la.code ='"+locale+"' AND ll.literal_id ="+literal_id;
		
		//Debug.Log (sql);
		
		try {
			if (literal_id != null)
			{
				
				dbCommand = dbConnection.CreateCommand ();
				dbCommand.CommandText = sql;
				
				reader = dbCommand.ExecuteReader ();
				while(reader.Read()) {
					translation = reader.GetString(0);
					
				}	
			}
			
		} catch(Exception e) {
			translation = "";
		}    
		return translation;
		
	}
	
	
	/**
	 *  Hashes a password in the same way PHP does it
	 * 
	 *  string unhashedPassword: string to hash
	 * 
	 *  return: string
	 * 
	 **/
	
	public string HashPassword(string unhashedPassword)
	{
		return BitConverter.ToString(new SHA512Managed().ComputeHash(Encoding.Default.GetBytes(unhashedPassword))).Replace("-", String.Empty).ToLower();
	}
	
	
	/**
	 *  Updates the data of the online database
	 * 
	 * 
	 *  return: void
	 * 
	 **/
	public void updateOnlineDatabase()
	{
		StartCoroutine(updateOnlineDatabaseEnum());
	}
	
	IEnumerator updateOnlineDatabaseEnum()
	{
		
		// Update users
		List<DataToServer> users = usersToServer ();
		WWWForm form = new WWWForm();
		form.AddField("data", users.ToString());
		
		
		// Upload to a cgi script
		WWW w = new WWW("http://192.168.30.4/coach_silex/web/api", form);
		yield return w;
		if (!String.IsNullOrEmpty (w.error)) {
			//Debug.Log(w.error);
		} else {
			//Debug.Log("Finished Uploading Screenshot");
		}
		
	}
	
	
	
	
	public List<DataToServer> usersToServer()
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<DataToServer> dataToServers = new List<DataToServer>();
		DataToServer dataToServer;
		// Select
		string sql = "SELECT u.* FROM user AS u ";
		sql += "WHERE u.sent=1";
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _username = reader.GetString (1);
			string _password = reader.GetString (2);
			int _usertype_id = Int32.Parse(reader.GetString (3));
			string _creationdate = reader.GetString (4);
			int _active = Int32.Parse(reader.GetString(5));
			int _role_id = Int32.Parse(reader.GetString(6));
			int _sent = Int32.Parse(reader.GetString(7));
			int _is_new = Int32.Parse(reader.GetString(8));
			
			User user = new User(_id,_username,_password,_usertype_id,_creationdate,_role_id,_sent,_is_new);
			
			
			
			if (user.usertype_id==2)
			{
				// Update a player
				Player player = getPlayer (user.id);
				dataToServer = new DataToServer(user,null,player);
				
			}
			else
			{
				// Update a coach
				Coach coach = getCoach(user.id);
				dataToServer = new DataToServer(user,coach,null);
			}
			
			
			
			dataToServers.Add(dataToServer);
			
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return dataToServers;
	}
	
	
	/**
	 *  Change the language of the application
	 * 
	 * 
	 *  return: void
	 * 
	 **/
	public void changeLocale(string locale)
	{
		GlobalVariables.locale = locale;
	}
	
	
	public void downloadDatabase(string url)
	{
		WWW www = new WWW ("http://192.168.30.4/coach_silex/web/api/mikel.db");
		
		
		if (www.isDone) {
			//Debug.Log ("something reached");
		} else {
			//Debug.Log ("nothing reached");
		}
	}
	
	
	/**
	 *  This function creates a new activity or updates an existing one
	 * 
	 *  Activity activity: activity to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public Activity saveActivity (string database, Activity activity)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = activity.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		if (id == 0) 
		{
			// Insert new Activity
			
			sql= "INSERT INTO activity (player_id,skill_id,statistics_id,datetime,animation_id,active,sent,is_new) VALUES ("+activity.player_id.ToString()+","+activity.skill_id.ToString()+","+activity.statistics_id.ToString()+",'"+activity.datetime+"',"+activity.animation_id+",0,1,1)";
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			
			// Select the last temporal index inserted to know 
			sql= "SELECT last_insert_rowid()";
			
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				activity.id = _id;
				
			}
			
		} 
		else 
		{
			// Update activity
			sql= "UPDATE activity SET player_id="+activity.player_id.ToString()+",skill_id="+activity.skill_id.ToString()+",statistics_id="+activity.statistics_id.ToString()+",datetime='"+activity.datetime+"',animation_id="+activity.animation_id.ToString()+",active="+activity.active.ToString()+",sent=1,is_new=0"+
				" WHERE id="+id.ToString();
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
		}
		
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return activity;
		
	}
	
	
	/**
	 *  This function creates a new activity or updates an existing one
	 * 
	 *  Activity activity: activity to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public Statistics saveStatistics (string database, Statistics statistics)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader=null;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = statistics.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		if (id == 0) 
		{
			// Insert new Activity
			
			sql= "INSERT INTO statistics (player_id,position_score,linear_velocity_score,angle_score," +
				"angular_velocity_score,overall_score,semantic_feedback,sent) VALUES ("+statistics.player_id.ToString()+
					","+statistics.position_score.ToString()+","+statistics.linear_velocity_score.ToString()+
					","+statistics.angle_score.ToString()+","+statistics.angular_velocity_score.ToString()+
					","+statistics.overall_score.ToString()+","+statistics.semantic_feedback+",1)";
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			
			// Select the last temporal index inserted to know 
			sql= "SELECT last_insert_rowid()";
			
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				statistics.id = _id;
				
			}
			
		} 
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return statistics;
		
	}
	
	
	
	/**
	 *  This function creates a new feedback or updates an existing one
	 * 
	 *  Feedback feedback: feedback to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public Feedback saveFeedback (string database, Feedback feedback)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = feedback.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		if (id == 0) 
		{
			// Insert new Activity
			
			sql= "INSERT INTO feedback (coach_id,activity_id,text,datetime,active,sent,is_new) VALUES ("+feedback.coach_id.ToString()+","+feedback.activity_id.ToString()+",'"+feedback.text+"','"+feedback.datetime+"',0,1,1)";
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			
			// Select the last temporal index inserted to know 
			sql= "SELECT last_insert_rowid()";
			
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				feedback.id = _id;
				
			}
			
		} 
		else 
		{
			// Update feedback
			sql= "UPDATE feedback SET coach_id="+feedback.coach_id.ToString()+",activity_id="+feedback.activity_id.ToString()+",text='"+feedback.text+"',datetime='"+feedback.datetime+"',active="+feedback.active.ToString()+",sent=1,is_new=0"+
				" WHERE id="+id.ToString();
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
		}
		
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return feedback;
		
	}
	
	
	/**
	 *  This function creates a new animation or updates an existing one
	 * 
	 *  Animation animation: animation to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public Animation_Replay saveAnimation (string database, Animation_Replay animation)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = animation.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		if (id == 0) 
		{
			// Insert new Activity
			
			sql= "INSERT INTO animation (path,creationDate,hero_id,active,sent,is_new) VALUES ('"+animation.path+"','"+animation.creationDate+"',"+animation.hero_id+","+animation.active+",1,1)";
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			
			// Select the last temporal index inserted to know 
			sql= "SELECT last_insert_rowid()";
			
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				animation.id = _id;
				
			}
			
		} 
		else 
		{
			// Update animation
			sql= "UPDATE animation SET path="+animation.path+",creationDate="+animation.creationDate+",hero_id="+animation.hero_id.ToString()+",active="+animation.active+",sent=1,is_new=0"+
				" WHERE id="+id.ToString();
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
		}
		
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return animation;
		
	}
	
	
	/**
	 *  This function creates a new animation_player or updates an existing one
	 * 
	 *  Animation_Player animation: animation to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public Animation_Player saveAnimationPlayer (string database, Animation_Player animation)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = animation.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		if (id == 0) 
		{
			// Insert new Activity
			
			sql= "INSERT INTO animation_player (player_id,path,animation_clean_file,active,sent,is_new) VALUES ("+animation.id.ToString()+",'"+animation.path+"','"+animation.animation_clean_file+"',"+animation.active+",1,1)";
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			
			// Select the last temporal index inserted to know 
			sql= "SELECT last_insert_rowid()";
			
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				animation.id = _id;
				
			}
			
		} 
		else 
		{
			// Update animation
			sql= "UPDATE animation_player SET player_id="+animation.player_id+",path='"+animation.path+"',animation_clean_file='"+animation.animation_clean_file+"',active="+animation.active+",sent=1,is_new=0"+
				" WHERE id="+id.ToString();
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
		}
		
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return animation;
		
	}
	
	
	/**
	 *  This function creates a new animation_heroe or updates an existing one
	 * 
	 *  Animation_Heroe animation: animation to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public Animation_Heroe saveAnimationHeroe (string database, Animation_Heroe animation)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = animation.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		
		// Update animation
		sql= "UPDATE animation_heroe SET animation_clean_file='"+animation.animation_clean_file+"',active="+animation.active+",sent=1"+
			" WHERE id="+id.ToString();
		Debug.Log (sql);
		dbCommand.CommandText = sql;
		reader = dbCommand.ExecuteReader ();
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return animation;
		
	}
	
	
	/**
	 *  This function gets avatar from its identifier
	 * 
	 *  int avatar_id: avatar identifier
	 * 
	 *  return: avatar
	 * 
	 **/
	public Avatar getAvatar(int avatar_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Avatar avatar = new Avatar();
		
		// Select
		string sql = "SELECT * FROM avatar AS a ";
		sql += "WHERE a.id ="+avatar_id;
		
		
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _name =reader.GetString(1);
			string _path =reader.GetString(2);
			int _active = Int32.Parse(reader.GetString(3));
			avatar = new Avatar(_id,_name,_path,_active);
			List<Avatar_Texture> avatar_textures =  getAvatarTexturesFromAvatar(_id);
			avatar.avatar_textures = avatar_textures;
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return avatar;
	}
	
	/**
	 *  This function gets avatar_texture from its identifier
	 * 
	 *  int avatar_texture_id: avatar_texture identifier
	 * 
	 *  return: avatar_texture
	 * 
	 **/
	public Avatar_Texture getAvatarTexture(int avatar_texture_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Avatar_Texture avatar_texture = new Avatar_Texture();
		
		// Select
		string sql = "SELECT * FROM avatar_texture AS at ";
		sql += "WHERE at.id ="+avatar_texture_id+" AND at.active=0";
		
		
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			string _name =reader.GetString(1);
			string _path =reader.GetString(2);
			int _avatar_id = Int32.Parse (reader.GetString(3));
			int _active = Int32.Parse(reader.GetString(4));
			avatar_texture = new Avatar_Texture(_id,_name,_path,_avatar_id,_active);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return avatar_texture;
	}
	
	
	/**
	 *  This function gets list of avatar_texture from avatar identifier
	 * 
	 *  int avatar_id: avatar identifier
	 * 
	 *  return: list of avatar_texture
	 * 
	 **/
	public List<Avatar_Texture> getAvatarTexturesFromAvatar(int avatar_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Avatar_Texture avatar_texture = new Avatar_Texture();
		List<Avatar_Texture> avatar_textures = new List<Avatar_Texture>();
		
		// Select
		string sql = "SELECT * FROM avatar_texture AS at ";
		sql += "WHERE at.avatar_id ="+avatar_id+" AND at.active=0";
		
		
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			avatar_texture = getAvatarTexture(_id);
			avatar_textures.Add(avatar_texture);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return avatar_textures;
	}
	
	
	/**
	 *  This function creates a new statistics_plot or updates an existing one
	 * 
	 *  Statistics_Plot statistics_plot: statistics_plot to create or to update
	 * 
	 *  return: void
	 * 
	 **/
	public Statistics_Plot saveStatisticsPlot (string database, Statistics_Plot statistics_plot)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		connectionString = "URI=file:"+Application.dataPath + "/"+database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		int id = statistics_plot.id;
		string sql="";
		
		dbCommand = dbConnection.CreateCommand ();
		
		if (id == 0) 
		{
			// Insert new Statistics_Plot
			
			sql= "INSERT INTO statistics_plot (statistics_id,plot_path,active,sent,is_new) VALUES ("+statistics_plot.statistics_id.ToString()+",'"+statistics_plot.plot_path+"',"+statistics_plot.active+",1,1)";
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			
			// Select the last temporal index inserted to know 
			sql= "SELECT last_insert_rowid()";
			
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				statistics_plot.id = _id;
				
			}
			
		} 
		else 
		{
			// Update animation
			sql= "UPDATE animation_player SET statistics_id="+statistics_plot.statistics_id+",plot_path='"+statistics_plot.plot_path+"',active="+statistics_plot.active.ToString()+",sent=1,is_new=0"+
				" WHERE id="+id.ToString();
			Debug.Log (sql);
			dbCommand.CommandText = sql;
			reader = dbCommand.ExecuteReader ();
		}
		
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return statistics_plot;
		
	}
	
	
	/**
	 *  This function gets the statistics_plot from its identifier
	 * 
	 *  int statistics_plot_id: statistics_plot identifier
	 * 
	 *  return: Statistics_Plot
	 * 
	 **/
	public Statistics_Plot getStatisticsPlot(int statistics_plot_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Statistics_Plot statistics_plot = new Statistics_Plot();
		
		// Select
		string sql = "SELECT * FROM statistics_plot AS sp ";
		sql += "WHERE sp.id ="+statistics_plot_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _statistics_id = Int32.Parse(reader.GetString(1));
			string _plot_path = reader.GetString(2);
			int _active = Int32.Parse(reader.GetString(3));
			int _sent = Int32.Parse(reader.GetString(4));
			int _is_new = Int32.Parse(reader.GetString(5));
			
			statistics_plot = new Statistics_Plot(_id,_statistics_id,_plot_path,_active,_sent,_is_new);
			
			//Debug.Log ("activity_id:"+statistics.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return statistics_plot;
	}
	
	/**
	 *  This function gets the list of statistics_plot from statistics identifier
	 * 
	 *  int statistics_id: statistics identifier
	 * 
	 *  return: list Statistics_Plot
	 * 
	 **/
	public List<Statistics_Plot> getStatisticsPlotsFromStatistics(int statistics_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		List<Statistics_Plot> statistics_plots = new List<Statistics_Plot> ();
		Statistics_Plot statistics_plot = new Statistics_Plot();
		
		// Select
		string sql = "SELECT * FROM statistics_plot AS sp ";
		sql += "WHERE sp.statistics_id ="+statistics_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			statistics_plot = getStatisticsPlot(_id);
			statistics_plots.Add(statistics_plot);
			//Debug.Log ("activity_id:"+statistics.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return statistics_plots;
	}
	
	
	/**
	 *  This function gets the modality from its identifier
	 * 
	 *  int modality_id: modality identifier
	 * 
	 *  return: Modality
	 * 
	 **/
	public Modality_Replay getModality(int modality_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		Modality_Replay modality = new Modality_Replay();
		
		// Select
		string sql = "SELECT * FROM modality AS m ";
		sql += "WHERE m.id ="+modality_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			int _name_int = Int32.Parse(reader.GetString(1));
			string _name = translateText(_name_int,GlobalVariables.locale);
			int _active = Int32.Parse(reader.GetString(2));
			
			modality = new Modality_Replay(_id,_name,_active);
			
			//Debug.Log ("activity_id:"+aniamtion.id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return modality;
	}
	
	
	/**
	 *  This function gets modalities
	 *  
	 * 
	 *  return: list of Modalities
	 * 
	 **/
	public List<Modality_Replay> getModalities()
	{


			IDbConnection dbConnection;
			IDbCommand dbCommand;
			IDataReader reader;
			
			
			int user_id = GlobalVariables.user_id;
			
			
			connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
			Debug.Log (connectionString);
			dbConnection = new SqliteConnection (connectionString);
			dbConnection.Open ();
			
			

			Modality_Replay modality = new Modality_Replay();
			List<Modality_Replay> modalities = new List<Modality_Replay>();
			string sql = "SELECT * FROM modality AS m ";
			sql += "WHERE m.active=0";
			
			//Debug.Log (sql);
			dbCommand = dbConnection.CreateCommand ();
			dbCommand.CommandText = sql;
			
			reader = dbCommand.ExecuteReader ();
			while(reader.Read()) {
				int _id = Int32.Parse(reader.GetString(0));
				
				modality = getModality(_id);
				modalities.Add(modality);
				//Debug.Log ("activity_id:"+activity.id+", skill_id:"+activity.skill_id);
			}	
			
			
			if (dbCommand != null) {
				dbCommand.Dispose ();
			}
			dbCommand = null;
			if (reader != null) {
				reader.Dispose ();
			}
			reader = null;
			if (dbConnection != null) {
				dbConnection.Close ();
			}
			dbConnection = null;
		  

		
		return modalities;
	}
	
	
	/**
	 *  This function gets disciplines
	 *  
	 *  int modality_id: modality identifier
	 * 
	 *  return: list of Discipline
	 * 
	 **/
	public List<Discipline> getDisciplinesFromModality(int modality_id)
	{
		IDbConnection dbConnection;
		IDbCommand dbCommand;
		IDataReader reader;
		
		int user_id = GlobalVariables.user_id;
		
		
		connectionString = "URI=file:"+Application.dataPath + "/"+GlobalVariables.user_database;
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		
		
		List<Discipline> disciplines = new List<Discipline>();
		Discipline discipline = new Discipline();
		
		string sql = "SELECT * FROM discipline AS d ";
		sql += "WHERE d.modality_id="+modality_id;
		
		//Debug.Log (sql);
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sql;
		
		reader = dbCommand.ExecuteReader ();
		while(reader.Read()) {
			int _id = Int32.Parse(reader.GetString(0));
			
			discipline = getDiscipline(_id);
			disciplines.Add(discipline);
			//Debug.Log ("activity_id:"+activity.id+", skill_id:"+activity.skill_id);
		}	
		
		
		if (dbCommand != null) {
			dbCommand.Dispose ();
		}
		dbCommand = null;
		if (reader != null) {
			reader.Dispose ();
		}
		reader = null;
		if (dbConnection != null) {
			dbConnection.Close ();
		}
		dbConnection = null;
		
		return disciplines;
	}
	
}
