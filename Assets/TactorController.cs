using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Diagnostics;

	public class TactorController : MonoBehaviour
	{
	public static TactorController Instance;

		SerialPort comPort = new SerialPort();
		Regex lineFormat = new Regex(@"\d+ \d+ \d+");
		
		public string comPortName = "COM7";
		
		// Makes new form of type GraphForm
		//Dino.Tools.WebMonitor.GraphForm myGraph = new Dino.Tools.WebMonitor.GraphForm(); 
		
		//This is code to implement multiple XBees
		const int MAX_SOURCES = 1;  //Max number of serial sources (starting from 0)

	public void Update(){
		if (Input.GetKeyDown (KeyCode.T)) {
			Toggle1();
			Toggle2();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			SendDurationTo1();
			SendDurationTo2();
		}
	}

	public void Awake()
		{
			Instance = this;
			// Popular list of available COM ports.
			ArrayList myPortList = new ArrayList();
			foreach (string name in System.IO.Ports.SerialPort.GetPortNames())
			{
				string tempName = "";
				if (name.Substring(0, 3) == "COM")
				{
					if ((name.Length == 5) && !Char.IsDigit((name.Substring(4, 1))[0]))
						tempName = name.Substring(0, 4);
					else if ((name.Length == 6) && !Char.IsDigit((name.Substring(5, 1))[0]))
						tempName = name.Substring(0, 5);
					else
						tempName = name;
				}
				myPortList.Add(tempName);
			}
		ConnectToTransmitter ();
		ChangeDuration ("500");
			//comPortNameList.DataSource = myPortList;
		}
		
		public void ConnectToTransmitter()
		{
			if (!comPort.IsOpen)
			{
				comPort.PortName = comPortName;
				comPort.BaudRate = 9600;
				comPort.DtrEnable = true;
				try
				{
					comPort.Open();
					if (comPort.IsOpen)
					{
						print( "Connected To Device");
					}
				}catch(Exception exc)
				{
				print( "Could not Connect");
				}
			}
		}
		
		// Stops all data capture
		private void DisconnectWithTransmitter(object sender, EventArgs e)
		{
			if (comPort.IsOpen)
			{
				comPort.Close();
				print( "Device Disconnected");
			}
		}
		
		public void SendDurationToBoth(){
		SendDurationTo1 ();
		SendDurationTo2 ();
		}
		
		byte[] tactors = {0, 1, 2, 3};
		byte[] duration = { 0, 0 };
		byte[] toggle = { 0xFF, 0xFF };
		
		private void SendDurationTo1()
		{
			if (comPort.IsOpen)
			{
				comPort.Write(tactors, 0, 1);
			comPort.Write(duration, 0, 2);
			print( "Send Duration 1");
			}
		}
		private void SendDurationTo2()
		{
			if (comPort.IsOpen)
			{
				comPort.Write(tactors, 1, 1);
			comPort.Write(duration, 0, 2);
			print( "Send Duration 2");
			}
		}
		
		private void ChangeDuration(string s)
		{
			try
			{
				UInt16 temp = (UInt16)Convert.ToInt16(s);
				duration[0] = (byte)(temp >> 8);
				duration[1] = (byte)(temp & 0x00FF);
			}
			catch (System.FormatException exc)
			{
				duration[0] = 0;
				duration[1] = 0; 
			}
		}
		
		
		private void Toggle1()
		{
			if (comPort.IsOpen)
			{
				comPort.Write(tactors, 0, 1);
				comPort.Write(toggle, 0, 2);
			}
		}
		
		private void Toggle2()
		{
			if (comPort.IsOpen)
			{
				comPort.Write(tactors, 1, 1);
				comPort.Write(toggle, 0, 2);
			}
		}

		
	}
