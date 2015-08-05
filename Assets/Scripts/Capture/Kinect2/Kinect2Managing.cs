#region Description
/* 		Multiple Kinect for Windows version 2 Controller
 * 		Tcp/Ip Server , Kinect for Windows version 2 clients
 * 		Handles the messaging system
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 *  @version: 1.0
 */
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using Stateless;

using Kinect2.Remote;
#endregion
namespace Kinect2
{
	internal enum KinectAction : byte
	{
		Init = 0,
		Calibrate = 1,
		Sync = 2,
		Record = 3,
		Merge = 4,
		Reconstruct = 5,
		Reinit = 6,
		Done = 7,
		Stop = 8
	}
	
	[Flags]
	internal enum KinectState : byte
	{
		None = 0x00,
		Idle = 0x01,
		Syncing = 0x02,
		Calibrating = 0x04,
		Recording = 0x08,
		Paused = 0x10,//TODO: rename?
		StopRecording = Paused | Recording,
		StopCalibrating = Paused | Calibrating,
		StopSyncing = Paused | Syncing,		
		//Streaming = 0x20,
		Processing = 0x40,
		ProcessingCalibration = Calibrating | Processing,
		ProcessingSynchronization = Syncing | Processing,
		Reconstructing = Recording | Processing,		
		Transfering = 0x80,
		WaitingRecordingTransfer = Idle | Recording | Transfering,
		WaitingCalibratingTransfer = Idle | Calibrating | Transfering,
		WaitingSyncingTransfer = Idle | Syncing | Transfering,		
		RecordingTransfer = Recording | Transfering,
		SyncingTransfer = Syncing | Transfering,
		CalibratingTransfer = Calibrating | Transfering,		
	}
	
	internal sealed partial class Kinect2Managing : Capturer,IReconstructor,ISyncer,ICalibrator
	{
		#region Properties
		public override Device Type { get { return Device.MULTI_KINECT_WIMUS; } }

		internal KinectState State { get; private set; }
		internal float CurrentProcessProgress { get; private set; }
		internal float CurrentTransferProgress { get; private set; }
		internal float CurrentCaptureProgress { get; private set; }

		public float CalibrationConfidence { get; private set; }				
		public float SyncConfidence { get; private set; }				
		
		//public float RecordingConfidence { get; private set; }
		public float SyncRecordingConfidence { get; private set; }
		#endregion
		#region Unity
		void Awake()
		{
			this.InitConfig();
			this.ids = Enumerable.Repeat(0,this.config.ClientCount).Select((id,index) => id += index).ToArray();
			this.kinects = new List<Kinect2Controlling>();
			this.closed = new List<Kinect2Controlling>();
			this.fileReceivers = new List<FileReceiver>();
			this.InitStateMachine();
		}
		
		void Start()
		{
			StartCoroutine("AcceptConnections");			
		}
		
		void OnEnable()
		{//TODO: maybe @ start? ( onenable before start )
			if(this.config.Address !=  IPAddress.Loopback)
			{
				this.server = new TcpListener(this.config.Address,this.config.Port);	
				this.server.Server.NoDelay = true;
				this.server.Start();		
			}
			else
			{
				this.enabled = false;
			}
		}
		
		void Update () 
		{

		}
		
		void OnGUI()
		{

		}
		
		void OnDisable()
		{
			
		}
		
		void OnDestroy()
		{
			StopAllCoroutines();
			foreach(var receiver in this.fileReceivers)
			{
				receiver.Dispose();
			}
			if (this.server != null && this.server.Server.Connected)
			{
				this.server.Server.Disconnect(true);
			}
		}
		#endregion
		#region Helpers        
		private void InitConfig()
		{
			this.config = new Configuration();
			Standalone.Factory.Configure(this.config);		
			DirectoryMap.Add(MessageParameter.Record,this.config.RecordingOutputDirectory);
			DirectoryMap.Add(MessageParameter.Sync,this.config.AudioOutputDirectory);
			DirectoryMap.Add(MessageParameter.Calibrate,this.config.CalibrationOutputDirectory);
			ProgressMap.Add(KinectAction.Calibrate,new float[] { 0.25f,0.055f, 0.05f, 0.25f,0.055f, 0.05f, 0.25f,0.055f, 0.05f, });
			ProgressMap.Add(KinectAction.Sync,new float[] { 0.45f,0.55f });
			ProgressMap.Add(KinectAction.Record,new float[] { 0.95f, 0.05f });			
		}
		
		private IEnumerator AcceptConnections()
		{
			while (this.kinects.Count < this.config.ClientCount)
			{
				yield return new WaitForSeconds(this.config.ConnectionPollingTime);
				if (this.server.Pending())
				{
					this.CreateKinect2(this.server.AcceptTcpClient());
				} 
			}
			this.machine.Fire(KinectAction.Init);
			yield break;
		}
		
		private IEnumerator AcceptMessages()
		{	
			Console.Important("<color=green>Accepting Messages</color>");			
			while (this.enabled)
			{// every frame
				yield return new WaitForEndOfFrame();
				foreach (var kinect in this.kinects.Where((k2) => k2.HasMessage))
				{
					var message = kinect.Message;
					switch(message.Action)
					{
					case MessageType.Bye: // kinect disconnected / client closed
						this.closed.Add(kinect);
						break;
					case MessageType.Hello: // kinect connected
						kinect.Send(new Message(kinect.ID,MessageType.Hello,MessageParameter.None));					
						kinect.name = message.Info + " <" + kinect.name + ">";
						Console.LogIf(message.InfoSpecified,"Connected ! Kinect" + message.ID + " Name = " + message.Info);
						break;
					case MessageType.Merge: // gather files @ host
						this.AcceptFiles(message);						
						this.machine.Fire(KinectAction.Merge);
						break;
					case MessageType.Done: // operation completed
						if(message.ElapsedSpecified)
						{
							kinect.Elapsed = message.Elapsed;
						}
						this.machine.Fire(KinectAction.Done);
						break;
					}
					Console.Messaging.LogReceived(message);					
				}
				foreach(var kinect in this.kinects.Where((kinect) => !kinect.IsResponding))
				{// for not responding clients => send close
					Console.Important("<b><color=red> KINECT DISCONNECTED " + kinect.ID + "</color></b>");
					kinect.Send(new Message(kinect.ID,MessageType.Bye,MessageParameter.None));
					this.closed.Add(kinect);
				}
				if(this.closed.Count > 0)  // Check for closed clients			
				{
					foreach(var kinect in this.closed)
					{
						this.kinects.Remove(kinect); // remove from subscribers
						var receiver = this.fileReceivers.SingleOrDefault((transfer) => transfer.ID == kinect.ID);
						if(receiver != null)
						{
							receiver.Dispose();
							this.fileReceivers.Remove(receiver);
						}
						GameObject.DestroyImmediate(kinect.gameObject); // and destroy the objects
					}
					this.closed.Clear(); // clear closed clients ( they ve been removed )
					this.machine.Fire(KinectAction.Reinit);// start accepting connections again for the needed clients
					//this.enabled = false; // disabling stops accepting/sending messages
				}
			}
		}
		
		private IEnumerator WaitCapture(object state)		
		{
			float percentage = 0.0f;
			switch((KinectAction)state)
			{
			case KinectAction.Calibrate:
				percentage = CalibrationCaptureProgressPerSecond;				
				break;				
			case KinectAction.Sync:
				percentage = SynchronizationCaptureProgressPerSecond;								
				break;				
			case KinectAction.Record:
				percentage = RecordingCaptureProgressPerSecond;												
				break;				
			default:
				Console.Error("Wrong capture usage!");
				break;
			}
			this.CurrentCaptureProgress = 0.0f;
			while(this.CurrentCaptureProgress < 1.0f)
			{
				yield return new WaitForSeconds(1.0f);
				this.CurrentCaptureProgress += percentage;
			}
		}
		
		private IEnumerator WaitTransfer(object state)
		{
			var progress = ProgressMap[(KinectAction)state];
			int index = 0;
			this.CurrentTransferProgress = 0.0f;
			while(this.enabled)
			{
				if(this.fileReceivers.All((receiver) => receiver.Completed))
				{// when a batch was completed
					this.CurrentTransferProgress += progress.Skip(index++).First();
					foreach(var kinect in this.kinects)
					{// notify a file transfer was ok
						kinect.Send(new Message(kinect.ID,MessageType.Ok,MessageParameter.Merge));
					}
					if(!this.fileReceivers.All((transfer) => transfer.TryNext()))//if more start next
					{// if there is no more files => finish
						Console.FileTransfer.Error("No More Files!");
						this.CurrentTransferProgress = 1.0f;
						break;
					}
				}
				yield return new WaitForSeconds(this.config.FileTransferPollingTime);
			}
			foreach(var kinect in this.kinects)
			{// notify file merging done
				kinect.Send(new Message(kinect.ID,MessageType.Done,MessageParameter.Merge));
			}
			Console.FileTransfer.OK("Waiting Ended !");
			yield break;
		}	
		
		private void AcceptFiles(Message msg)
		{
			var receiver = this.fileReceivers.SingleOrDefault((receive) => receive.ID == msg.ID);
			if(receiver != null && !receiver.Pending)
			{// get corresponding receiver	=> add files from msg info { filename#suffix01#suffix02#... }
				var folder = Directory.CreateDirectory(Path.Combine(DirectoryMap[msg.Parameter],msg.Time.ToString("MM-dd-HH-mm-ss"))).FullName;	
				var xtensions = msg.Info.Split(new string[] { "#"},StringSplitOptions.RemoveEmptyEntries);
				var filename = xtensions.FirstOrDefault();
				receiver.AddFiles(Path.Combine(folder,filename),xtensions.Skip(1).ToArray());//TODO: error handling		
			}
			else
			{
				Console.Important("<color=red>Pending merge requests ... ignored last merge command</color>");
			}
		}
		
		private void SendToAll(Message msg)
		{
			foreach(var kinect in this.kinects)
			{
				msg.ID = kinect.ID;
				kinect.Send(msg);
			}
		}
		
		private void CreateKinect2 (TcpClient client)
		{
			var go = new GameObject("Kinect",typeof(Kinect2Controlling));
			var controller = go.GetComponent<Kinect2Controlling>();
			controller.CreateWith(client,ids.Except(this.kinects.Select((kinect) => kinect.ID).ToArray()).FirstOrDefault()); // find missing id to assign
			go.name = go.name + controller.ID;
			this.kinects.Add(controller);
			this.fileReceivers.Add(new FileReceiver(new ConnectionInfo(this.config.Address,this.config.Port, controller.ID)));
		}
		
		private string FindMostRecent(string directory)
		{
			return Directory.GetDirectories(directory, "*", SearchOption.TopDirectoryOnly)
				.OrderByDescending((dir) =>
				                   {
					var name = dir.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).Last();
					DateTime dt;
					if (DateTime.TryParseExact(name, "MM-dd-HH-mm-ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out dt))
					{
						return dt.Ticks;
					}
					else
					{
						return 0L;
					}
				}).ToArray().FirstOrDefault();
		}
		#endregion
		#region Fields
		private Configuration config;
		private TcpListener server;
		private int[] ids;
		private List<Kinect2Controlling> kinects,closed;
		private List<FileReceiver> fileReceivers;
		
		private static Dictionary<MessageParameter,string> DirectoryMap = new Dictionary<MessageParameter, string>();	
		private static Dictionary<KinectAction,float[]> ProgressMap = new Dictionary<KinectAction, float[]>();
		
		private const float RecordingCaptureProgressPerSecond = 0.05f;
		private const float CalibrationCaptureProgressPerSecond = 0.1f;
		private const float SynchronizationCaptureProgressPerSecond = 0.066666f;		
		#endregion
	}
}
