using UnityEngine;
using System;
using System.Data;
using Mono.Data.SqliteClient;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;



public class JsonMikel
{

	/***
	 * Description:
	 * JsonMikel class
	 * Version: 0.1
	 * Autor:
	 * Mikel Rodríguez - Vicomtech
	 *****/

	public JsonMikel ()
	{

	}

	public string ToJson(List<DataToServer> dataToServer)
	{

		string salida = JsonConvert.SerializeObject(dataToServer);
		Debug.Log (salida);
		return salida;
	}


}
