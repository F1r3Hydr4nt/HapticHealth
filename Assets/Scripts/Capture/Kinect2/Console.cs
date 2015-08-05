#define IMPORTANT_LOG
#define SIMPLE_LOG
#define ERROR_LOG
#define MESSAGING_LOG
#define FILETRANSFER_LOG
#define STANDALONE_LOG
#define STATEMACHINE_LOG

using UnityEngine;

using System.Diagnostics;

using Logger = UnityEngine.Debug;

internal sealed class Console 
{
	[Conditional("IMPORTANT_LOG")]
	public static void Important(string important)
	{
		Logger.LogWarning(important);
	}
	
	[Conditional("IMPORTANT_LOG")]
	public static void ImportantIf(bool flag,string important)
	{
		if(flag)
			Logger.LogWarning(important);
	}
	
	[Conditional("SIMPLE_LOG")]	
	public static void Log(string log)
	{
		Logger.Log(log);
	}
	
	[Conditional("SIMPLE_LOG")]
	public static void LogIf(bool flag,string log)
	{
		if(flag)
			Logger.Log(log);
	}
	
	[Conditional("ERROR_LOG")]	
	public static void Error(string log)
	{
		Logger.LogError(log);
	}
	#region Messaging
	internal sealed class Messaging
	{
		[Conditional("MESSAGING_LOG")]
		public static void LogSend(Message msg)
		{
			Logger.LogWarning("<color=orange>SENT</color> " + msg.ToString());
		}
		
		[Conditional("MESSAGING_LOG")]
		public static void LogReceived(Message msg)
		{
			Logger.LogWarning("<color=lime>Received</color> = "  + msg.ToString());			
		}
	}
	#endregion
	#region FileTransfer
	internal sealed class FileTransfer
	{
		[Conditional("FILETRANSFER_LOG")]
		public static void OK(string filename)
		{
			Logger.LogWarning("<color=green>File Transfer Completed !</color> <color=aqua>" + filename +  "</color>");
		}
		
		[Conditional("FILETRANSFER_LOG")]
		public static void Next(string filename)
		{
			Logger.LogWarning("Waiting for next ...<color=magenta>"  + filename +  "</color>");
		}
		
		[Conditional("FILETRANSFER_LOG")]
		public static void Error(string error)
		{
			Logger.LogError("File Transfer ERROR : " + error);
		}
	}
	#endregion
	#region Standalone
	internal sealed class Standalone
	{
		[Conditional("STANDALONE_LOG")]
		public static void Start(string process)
		{
			Logger.LogWarning("<color=lime><b> STARTED </b></color> Process  = " + process);
		}
		
		[Conditional("STANDALONE_LOG")]
		public static void Progress(float progress)
		{
			Logger.Log("<color=lightblue>Standalone Progress = "  + progress + "</color>");
		}
		
		[Conditional("STANDALONE_LOG")]
		public static void Log(string log)
		{
			Logger.Log(log);
		}
		
		[Conditional("STANDALONE_LOG")]
		public static void Exit(string process,int exitcode,double duration)
		{
			Logger.LogWarning("<color=red><b> EXITED </b></color> Process  = " + process + " with Code = "  + exitcode + " Duration (secs) = " + duration.ToString("F0"));
		}
		
		
		[Conditional("STANDALONE_LOG")]
		public static void Error(string error)
		{
			Logger.LogError(error);
		}
	}
	#endregion
	#region StateMachine
	internal sealed class StateMachine
	{
		[Conditional("STATEMACHINE_LOG")]
		public static void Unhandled(string state,string trigger)
		{
			Logger.LogWarning("Unhandled Trigger : <b>" +  state +  "</b> # <b>" + trigger +  "</b>");
		}
		
		[Conditional("STATEMACHINE_LOG")]
		public static void Transition(bool reentry, string from, string to)
		{
			if(reentry)
			{
				Logger.LogWarning("From -> <b><color=red>" + from + "</color></b> To -> <b><color=green>" + to + "</color></b>");
			}
			else
			{
				Logger.LogWarning("<color=yellow> REENTRY ( " + to + " )</color>");
			}
		}
	}
	#endregion
}
