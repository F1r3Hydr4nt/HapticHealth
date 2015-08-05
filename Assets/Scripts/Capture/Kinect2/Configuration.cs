#region Description
/* 		Configuration for Kinect 2 Client/Server System
 * 		Configures :
 * 			Output Directories
 * 				Calibration Directory
 * 				Synchronization Directory
 * 				Recording Directory
 * 			Executable Files
 * 				Calibration
 * 				Synchronization
 * 				3D Reconstruction
 * 			Remote Connection Info
 * 				Server IP & Port
 * 				Clients Expected
 * 			Internal Constants
 * 				Connection Polling Time
 * 				File Transfer Polling Time
* 				Process Polling Time
* 		Currently loaded from text files, future implementations may change that
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 * @version: 1.0
 */
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
#endregion
namespace Kinect2
{
	internal sealed class Configuration 
	{
		#region Constructors
		public Configuration ()
		{
			this.LoadConfig();
			this.InitRemoting();
			this.InitDirectories();	
			this.InitTimings();
			this.InitCustom();
		}
		#endregion
		#region Properties
		internal IPAddress Address { get; private set; }
		internal int Port { get; private set; }
		internal int ClientCount { get; private set; }
		internal string DataDirectory { get; private set; }
		internal string AudioOutputDirectory { get; private set; }
		internal string CalibrationOutputDirectory { get; private set; }
		internal string RecordingOutputDirectory { get; private set; }
		internal string MeshOutputDirectory { get; private set; }	
		internal string ExecutableInputDirectory { get; private set; }
		internal string CalibrationExecutablePath { get; private set; }
		internal string SynchronizationExecutablePath { get; private set; }
		internal string ReconstructionExecutablePath { get; private set; }
		internal float ConnectionPollingTime { get; private set; }
		internal float FileTransferPollingTime { get; private set; }
		internal float ProcessWaitSeconds { get; private set; }
		#endregion
		#region Methods
		private void LoadConfig ()
		{
			//TODO: XML @ Runtime
			var paths = Resources.Load("Kinect2/Paths") as TextAsset;
			if(paths != null)
			{
				var pathsLookup = paths.text.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries)
					.ToDictionary(
						(str) => str.Split(new string[] { " " },2,StringSplitOptions.RemoveEmptyEntries).First(),
						(str) => str.Split(new string[] { " " },2,StringSplitOptions.RemoveEmptyEntries).Last());
				this.DataDirectoryName = pathsLookup.ContainsKey(DataDirectoryKey) ? pathsLookup[DataDirectoryKey] : DefaultDataDirectoryName;
				this.ExecutableDirectoryName = pathsLookup.ContainsKey(ExecutableDirectoryKey) ? pathsLookup[ExecutableDirectoryKey] : DefaultExecutableDirectoryName;
				this.AudioOutputDirectoryName = pathsLookup.ContainsKey(AudioOutputDirectoryKey) ? pathsLookup[AudioOutputDirectoryKey] : DefaultAudioOutputDirectoryName;
				this.CalibrationOutputDirectoryName = pathsLookup.ContainsKey(CalibrationOutputDirectoryKey) ? pathsLookup[CalibrationOutputDirectoryKey] : DefaultCalibrationOutputDirectoryName;
				this.RecordingOutputDirectoryName = pathsLookup.ContainsKey(RecordingOutputDirectoryKey) ? pathsLookup[RecordingOutputDirectoryKey] : DefaultRecordingOutputDirectoryName;
				this.MeshOutputDirectoryName = pathsLookup.ContainsKey(MeshOutputDirectoryKey) ? pathsLookup[MeshOutputDirectoryKey] : DefaultMeshOutputDirectoryName;				
				this.CalibrationExecutableName = pathsLookup.ContainsKey(CalibrationExecutableKey) ? pathsLookup[CalibrationExecutableKey] : DefaultCalibrationExecutableName;
				this.SynchronizationExecutableName = pathsLookup.ContainsKey(SynchronizationExecutableKey) ? pathsLookup[SynchronizationExecutableKey] : DefaultSynchronizationExecutableName;
				this.ReconstructionExecutableName = pathsLookup.ContainsKey(ReconstructionExecutableKey) ? pathsLookup[ReconstructionExecutableKey] : DefaultReconstructionExecutableName;								
			}
			else
			{
				this.DataDirectoryName = DefaultDataDirectoryName;
				this.ExecutableDirectoryName = DefaultExecutableDirectoryName;
				this.AudioOutputDirectoryName = DefaultAudioOutputDirectoryName;				
				this.CalibrationOutputDirectoryName = DefaultCalibrationOutputDirectoryName;
				this.RecordingOutputDirectoryName = DefaultRecordingOutputDirectoryName;
				this.MeshOutputDirectoryName = DefaultMeshOutputDirectoryName;
				this.CalibrationExecutableName = DefaultCalibrationExecutableName;
				this.SynchronizationExecutableName = DefaultSynchronizationExecutableName;
				this.ReconstructionExecutableName = DefaultReconstructionExecutableName;
			}
			var timings = Resources.Load("Kinect2/Timings") as TextAsset;
			if(timings != null)
			{			
				var timingsLookup = timings.text.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries)
					.ToDictionary(
						(str) => str.Split(new string[] { " " },2,StringSplitOptions.RemoveEmptyEntries).First(),
						(str) => float.Parse(str.Split(new string[] { " " },2,StringSplitOptions.RemoveEmptyEntries).Last()));
				this.ConnectionPollingTime = timingsLookup.ContainsKey(ConnectionPollingTimeKey) ? timingsLookup[ConnectionPollingTimeKey] : DefaultConnectionPollingTime;
				this.FileTransferPollingTime = timingsLookup.ContainsKey(FileTransferPollingTimeKey) ? timingsLookup[FileTransferPollingTimeKey] : DefaultFileTransferPollingTime;
				this.ProcessWaitSeconds = timingsLookup.ContainsKey(ProcessWaitSecondsKey) ? timingsLookup[ProcessWaitSecondsKey] : DefaultProcessWaitSeconds;
			}
			else
			{
				this.ConnectionPollingTime = DefaultConnectionPollingTime;
				this.FileTransferPollingTime = DefaultFileTransferPollingTime;
				this.ProcessWaitSeconds = DefaultProcessWaitSeconds;
			}
			//TODO: check if needed for address
			var networking = Resources.Load("Kinect2/Networking") as TextAsset;			
			if(networking != null)
			{
				var networkingLookup = networking.text.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries)
					.ToDictionary(
						(str) => str.Split(new string[] { " " },2,StringSplitOptions.RemoveEmptyEntries).First(),
						(str) => str.Split(new string[] { " " },2,StringSplitOptions.RemoveEmptyEntries).Last());
				IPAddress address;
				if(IPAddress.TryParse(networkingLookup.ContainsKey(AddressKey) ? networkingLookup[AddressKey] : DefaultAddress,out address))
				{
					this.Address = address; 
				}
				else
				{
					this.Address = IPAddress.Parse(DefaultAddress);
				}
				int port;
				if(int.TryParse(networkingLookup.ContainsKey(PortKey) ? networkingLookup[PortKey] : DefaultPort.ToString(),out port))
				{
					this.Port = port;
				}
				else
				{
					this.Port = DefaultPort;
				}
				int clientscount;
				if(int.TryParse(networkingLookup.ContainsKey(ClientsCountKey) ? networkingLookup[ClientsCountKey] : DefaultClientsCount.ToString(),out clientscount))
				{
					this.ClientCount = clientscount;
				}
				else
				{
					this.ClientCount = DefaultClientsCount;
				}
			}
			else
			{
				this.Address = IPAddress.Parse(DefaultAddress);
				this.Port= DefaultPort;
				this.ClientCount = DefaultClientsCount;
			}
		}
		
		private void InitRemoting ()
		{
			var localIPs = Dns.GetHostAddresses(Dns.GetHostName());
			this.Address = localIPs.Any() ? localIPs.FirstOrDefault() : IPAddress.Loopback;
			this.Port = DefaultPort;
		}
		
		private void InitDirectories ()
		{
			#region Init Directories
			this.DataDirectory = Path.Combine(Environment.CurrentDirectory,this.DataDirectoryName);
			this.ExecutableInputDirectory = Path.Combine(this.DataDirectory,this.ExecutableDirectoryName);
			this.CalibrationExecutablePath = Path.Combine(this.ExecutableInputDirectory,this.CalibrationExecutableName);
			this.SynchronizationExecutablePath = Path.Combine(this.ExecutableInputDirectory,this.SynchronizationExecutableName);
			this.ReconstructionExecutablePath = Path.Combine(this.ExecutableInputDirectory,this.ReconstructionExecutableName);			
			if (!Directory.Exists(this.DataDirectory))
			{
				try
				{
					var info = Directory.CreateDirectory(this.DataDirectory);
					this.DataDirectory = info.FullName;			
				}
				catch
				{
					this.DataDirectory = Environment.CurrentDirectory;
				}
			}
			this.AudioOutputDirectory = Path.Combine(this.DataDirectory, this.AudioOutputDirectoryName);
			if (!Directory.Exists(this.AudioOutputDirectory))
			{
				try
				{
					var info = Directory.CreateDirectory(this.AudioOutputDirectory);
					this.AudioOutputDirectory = info.FullName;			
				}
				catch
				{
					this.AudioOutputDirectory = Environment.CurrentDirectory;
				}
			}
			this.CalibrationOutputDirectory = Path.Combine(this.DataDirectory, this.CalibrationOutputDirectoryName);
			if (!Directory.Exists(this.CalibrationOutputDirectory))
			{
				try
				{
					var info = Directory.CreateDirectory(this.CalibrationOutputDirectory);
					this.CalibrationOutputDirectory = info.FullName;			
				}
				catch
				{
					this.CalibrationOutputDirectory = Environment.CurrentDirectory;
				}
			}
			this.RecordingOutputDirectory = Path.Combine(this.DataDirectory, this.RecordingOutputDirectoryName);
			if (!Directory.Exists(this.RecordingOutputDirectory))
			{
				try
				{
					var info = Directory.CreateDirectory(this.RecordingOutputDirectory);
					this.RecordingOutputDirectory = info.FullName;
				}
				catch
				{
					this.RecordingOutputDirectory = Environment.CurrentDirectory;
				}
			}
			this.MeshOutputDirectory = Path.Combine(this.DataDirectory, this.MeshOutputDirectoryName);
			if (!Directory.Exists(this.MeshOutputDirectory))
			{
				try
				{
					var info = Directory.CreateDirectory(this.MeshOutputDirectory);
					this.MeshOutputDirectory = info.FullName;
				}
				catch
				{
					this.MeshOutputDirectory = Environment.CurrentDirectory;
				}
			}
			#endregion
		}
		
		private void InitTimings ()
		{
			
		}

		private void InitCustom ()
		{
			var current = Environment.CurrentDirectory;
			var ipFile = Path.Combine(current,CustomIpFileName);
			if(File.Exists(ipFile))
		     {
				var lines = File.ReadAllLines(ipFile);
				var address = lines.Where((line) =>
          		 {
					IPAddress test;
					return IPAddress.TryParse(line, out test);
				}).Select((line) => IPAddress.Parse(line)).FirstOrDefault();
				if(address != IPAddress.None)
				{
					this.Address = address;
				}
			}
			var clientFile = Path.Combine(current,CustomClientsFileName);
			if(File.Exists(clientFile))
			{
				var lines = File.ReadAllLines(clientFile);
				var count = lines.Where((line) =>
                  {
					int test;
					return int.TryParse(line, out test);
				}).Select((line) => int.Parse(line)).FirstOrDefault();
				if(count > 0 && count < 5)
				{
					this.ClientCount = count;
				}
			}
			var portFile = Path.Combine(current,CustomPortFileName);			
			if(File.Exists(portFile))
			{
				var lines = File.ReadAllLines(portFile);
				var port = lines.Where((line) =>
                {
					int test;
					return int.TryParse(line, out test);
				}).Select((line) => int.Parse(line)).FirstOrDefault();
				if(port > 1000 && port < 35000)
				{
					this.Port = port;
				}
			}
		}
		#endregion
		#region Fields
		private const string DefaultAddress = "195.251.117.232";		
		private const int DefaultClientsCount = 4;
		private const int DefaultPort = 22500;
		
		private const string AddressKey = "Address";
		private const string PortKey = "Port";
		private const string ClientsCountKey = "ClientCount";
		
		private string DataDirectoryName;
		private string ExecutableDirectoryName;
		private string AudioOutputDirectoryName;
		private string CalibrationOutputDirectoryName;
		private string RecordingOutputDirectoryName;
		private string CalibrationExecutableName;
		private string SynchronizationExecutableName;
		private string ReconstructionExecutableName;
		private string MeshOutputDirectoryName;
		
		private const string DataDirectoryKey = "DataDirectoryName";
		private const string ExecutableDirectoryKey = "ExecutableDirectoryName";		
		private const string AudioOutputDirectoryKey = "AudioOutputDirectoryName";				
		private const string CalibrationOutputDirectoryKey = "CalibrationOutputDirectoryName";						
		private const string RecordingOutputDirectoryKey = "RecordingOutputDirectoryName";								
		private const string MeshOutputDirectoryKey = "MeshOutputDirectoryName";		
		private const string CalibrationExecutableKey = "CalibrationExecutableName";										
		private const string SynchronizationExecutableKey = "SynchronizationExecutableName";												
		private const string ReconstructionExecutableKey = "ReconstructionExecutableName";
														
		private const string DefaultDataDirectoryName = "Data";
		private const string DefaultExecutableDirectoryName = "Executables";
		private const string DefaultAudioOutputDirectoryName = "Audio";
		private const string DefaultCalibrationOutputDirectoryName = "Calibration";
		private const string DefaultRecordingOutputDirectoryName = "Recordings";
		private const string DefaultMeshOutputDirectoryName = "Meshes";
		private const string DefaultCalibrationExecutableName = @"calibration_standalone.exe";
		private const string DefaultSynchronizationExecutableName = @"syncer_standalone.exe";
		private const string DefaultReconstructionExecutableName = @"";		
		
		private const string ConnectionPollingTimeKey = "ConnectionPollingTime";
		private const string FileTransferPollingTimeKey = "FileTransferPollingTime";		
		private const string ProcessWaitSecondsKey = "ProcessWaitSeconds ";	
		
		private const float DefaultConnectionPollingTime = 1.0f;// in seconds
		private const float DefaultFileTransferPollingTime = 1.0f;// in seconds	
		private const float DefaultProcessWaitSeconds = 2.0f;// in seconds

        private const string CustomIpFileName = @"ip.txt";
		private const string CustomClientsFileName = @"clients.txt";
		private const string CustomPortFileName = @"port.txt";
		#endregion
	}
}
