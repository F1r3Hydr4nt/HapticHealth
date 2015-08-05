#region Description
/* 		Kinect 2 Skeleton Capturing
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using FileHelpers;
using Kinect2.IO;
using Fusion;
using BlackBeltCoder;
#endregion
internal sealed class FusionReader
{
	// Recorded motion
	string recordFile = "";
	bool reading = true;
	StreamReader reader;
	ScanFormatted scanner;

	public FusionReader( string filename )
	{
		recordFile = filename;

		// Reading file...
		Console.Important("Ready to read '" + recordFile + "'.");
		reader = new StreamReader( recordFile );
		if (reader == null)
		{
			Console.Important("Recorded file '" + recordFile + "' can't be read.");
		}
		
		PassHeader ();
		scanner = new ScanFormatted();
	}

	// Simple joint / orientation structure
	/*
		    0		SpineBase
			1		SpineMid	
			2		Neck
			3		Head
			4		ShoulderLeft
			5		ElbowLeft
			6		WristLeft
			7		HandLeft
			8		ShoulderRight
			9		ElbowRight
			10		WristRight
			11		HandRight
			12		HipLeft
			13		KneeLeft
			14		AnkleLeft
			15		FootLeft
			16		HipRight
			17		KneeRight
			18		AnkleRight
			19		FootRight
			20      SpineShoulder
			21      HandTipLeft
			22      ThumbLeft
			23      HandTipRight
			24      ThumbRight
        */
	public Vector3 [] jointPositions = new Vector3[ 25 ];
	public Quaternion [] jointOrientations = new Quaternion[ 25 ];
	public int currentTimeStamp = 0;		
	public int lastTimeStamp = 0;
	public void UpdateNextFrame() 
	{
		if( reading && reader != null && scanner != null )
		{
			// Pass the first data
			string temp = reader.ReadLine();
			Console.Log("file : "+reader.ReadToEnd());
			// Parse the timestamp			
			scanner.Parse(temp, "Frame %d %d %d %f %f %f %f");		
			object [] _results = scanner.Results.ToArray();		
			if (_results.Length == 7)		
			{		
				lastTimeStamp = currentTimeStamp;		
				currentTimeStamp = (int) _results[ 1 ];		
			}

			temp = reader.ReadLine();
			temp = reader.ReadLine();

			// Read and parse the joints
			for(int i = 0; i < 25; ++i)
			{
				string line = reader.ReadLine();
				if( line != null )
				{
					// Parse the line (first data is not useful)
					scanner.Parse(line, "%d %f %f %f %f %f %f %f");
					object [] results = scanner.Results.ToArray();
					if (results.Length == 8)
					{
						jointPositions[i].x = (float)results[1];
						jointPositions[i].y = (float)results[2];
						jointPositions[i].z = (float)results[3];
						jointOrientations[i].x = (float)results[4]; 
						jointOrientations[i].y = (float)results[5]; 
						jointOrientations[i].z = (float)results[6]; 
						jointOrientations[i].w = (float)results[7]; 

						//Console.Log(i + ">> Read " + jointPositions[i].ToString() + " / " + jointOrientations[i].ToString());
					}
					else
					{
						Console.Log("Error in recorded file format: '" + line + "'.");
					}
				}
				else
				// Go on to the begining
				{
				Console.Log("line = "+line.ToString());
					startTime = Time.time;
					Console.Log("Reader rewinding the file.");
					reader.BaseStream.Position = 0;
					reader.DiscardBufferedData();
					PassHeader();
					break;
				}
			}
		}
	}

	// Go on until data
	public const int headernumLine = 37;
	private void PassHeader()
	{
		for( int i = 0; i < headernumLine; ++i )
		{
			string line = reader.ReadLine();
		}
	}

	public void Start_Reading()
	{
		Tick ();
		startTime = Time.time;
		Console.Important ("Reading a recorded file.");
		reading = true;
	}
	System.Diagnostics.Stopwatch stopwatch;
	void Tick(){
		stopwatch = System.Diagnostics.Stopwatch.StartNew ();
	}
	long stopWatchMilliseconds = 0;
	void Tock(){
		stopWatchMilliseconds = stopwatch.ElapsedMilliseconds;
		Console.Log ("FusionReader/Playback time: "+stopwatch.Elapsed);
	}
	
	float startTime = 0f;
	public void Stop_Reading()
	{
		Console.Important ("Stop reading a recorded file.");
		reading = false;
		if( reader != null )
		{
			reader.Close ();
			reader.Dispose ();
		}
	}
}

