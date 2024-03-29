﻿using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;


public class DbAccess
{
	private IDbConnection dbConnection;
	private IDbCommand dbCommand;
	private IDataReader reader;



	
	
	/// <summary>
	///  string to connect. The simpliest one looks like "URI=file:filename.db"
	/// </summary>
	/// <param name="connectionString">
	/// A <see cref="System.String"/>
	/// </param>
	public DbAccess (string connectionString)
	{
		OpenDB (connectionString);
	}
	
	/// <summary>
	///  The same as <see cref="DbAccess#Dbaccess" / >
	/// </summary>
	/// <param name="connectionString">
	/// A <see cref="System.String"/>
	/// </param>
	public void OpenDB (string connectionString)
	{
		dbConnection = new SqliteConnection (connectionString);
		dbConnection.Open ();
		Debug.Log ("Connected to db");
	}
	
	/// <summary>
	/// Closes connection to db
	/// </summary>
	public void CloseSqlConnection ()
	{
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
		Debug.Log ("Disconnected from db.");
	}
	
	
	/// <summary>
	///  Executes query given by sqlQuery
	/// </summary>
	/// <param name="sqlQuery">
	/// query
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// null, if any error
	/// result of query, otherwise
	/// A <see cref="SqliteDataReader"/>
	/// </returns>
	public IDataReader ExecuteQuery (string sqlQuery)
	{
		dbCommand = dbConnection.CreateCommand ();
		dbCommand.CommandText = sqlQuery;
		
		reader = dbCommand.ExecuteReader ();
		
		
		return reader;
	}
	
	/// <summary>
	///  Selects everything from table
	/// </summary>
	/// <param name="tableName">
	/// name of table
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// result of query
	/// A <see cref="SqliteDataReader"/>
	/// </returns>
	public IDataReader ReadFullTable (string tableName)
	{
		string query = "SELECT * FROM " + tableName;
		return ExecuteQuery (query);
	}
	
	/// <summary>
	/// Inserts data into table
	/// </summary>
	/// <param name="tableName">
	/// name of table to insert data
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="values">
	/// array of data in string representation
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <returns>
	/// result of query
	/// A <see cref="SqliteDataReader"/>
	/// </returns>
	public IDataReader InsertInto (string tableName, string[] values)
	{
		string query = "INSERT INTO " + tableName + " VALUES (" + values[0];
		for (int i = 1; i < values.Length; ++i) {
			query += ", " + values[i];
		}
		query += ")";
		return ExecuteQuery (query);
	}
	
	/// <summary>
	/// Inserts data into specific columns of table
	/// </summary>
	/// <param name="tableName">
	/// name of table
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="cols">
	/// name of columns
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <param name="values">
	/// values
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <returns>
	/// result of query
	/// A <see cref="SqliteDataReader"/>
	/// </returns>
	public IDataReader InsertIntoSpecific (string tableName, string[] cols, string[] values)
	{
		if (cols.Length != values.Length) {
			throw new Exception ("columns.Length != values.Length");
		}
		string query = "INSERT INTO " + tableName + "(" + cols[0];
		for (int i = 1; i < cols.Length; ++i) {
			query += ", " + cols[i];
		}
		query += ") VALUES (" + values[0];
		for (int i = 1; i < values.Length; ++i) {
			query += ", " + values[i];
		}
		query += ")";
		return ExecuteQuery (query);
	}
	
	/// <summary>
	/// deletes any data from table
	/// </summary>
	/// <param name="tableName">
	/// table name
	/// A <see cref="System.String"/>
	/// </param>
	/// <returns>
	/// result of query
	/// A <see cref="SqliteDataReader"/>
	/// </returns>
	public IDataReader DeleteContents (string tableName)
	{
		string query = "DELETE FROM " + tableName;
		return ExecuteQuery (query);
	}
	
	/// <summary>
	/// Creates table with specified columns
	/// </summary>
	/// <param name="name">
	/// table name to be created
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="col">
	/// array, containing names of columns
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <param name="colType">
	/// array, containing types of columns
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <returns>
	/// result of query
	/// A <see cref="SqliteDataReader"/>
	/// </returns>
	public IDataReader CreateTable (string name, string[] col, string[] colType)
	{
		if (col.Length != colType.Length) {
			throw new Exception ("columns.Length != colType.Length");
		}
		string query = "CREATE TABLE " + name + " (" + col[0] + " " + colType[0];
		for (int i = 1; i < col.Length; ++i) {
			query += ", " + col[i] + " " + colType[i];
		}
		query += ")";
		return ExecuteQuery (query);
	}
	
	/// <summary>
	/// Selects from table with specified parameters.
	/// Ex: SelectWhere("puppies", new string[] = {"breed"}, new string[] = {"earType"}, new string[] = {"="}, new string[] = {"floppy"});
	/// the same as command: SELECT breed FROM puppies WHERE earType = floppy
	/// </summary>
	/// <param name="tableName">
	/// name of table to select
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="items">
	/// item names
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <param name="col">
	/// array, containing columns of parameters
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <param name="operation">
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <param name="values">
	/// A <see cref="System.String[]"/>
	/// </param>
	/// <returns>
	/// result of query
	/// A <see cref="SqliteDataReader"/>
	/// </returns>
	public IDataReader SelectWhere (string tableName, string[] items, string[] col, string[] operation, string[] values)
	{
		if (col.Length != operation.Length || operation.Length != values.Length) {
			throw new Exception ("col.Length != operation.Length != values.Length");
		}
		string query = "SELECT " + items[0];
		for (int i = 1; i < items.Length; ++i) {
			query += ", " + items[i];
		}
		query += " FROM " + tableName + " WHERE " + col[0] + operation[0] + "'" + values[0] + "' ";
		for (int i = 1; i < col.Length; ++i) {
			query += " AND " + col[i] + operation[i] + "'" + values[i] + "' ";
		}
		Debug.Log (query);
		return ExecuteQuery (query);
		
	}



	
}
