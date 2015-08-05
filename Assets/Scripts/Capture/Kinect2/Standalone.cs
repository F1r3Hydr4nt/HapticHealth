#region Description
/* 		Standalone Process Wrapper & Factory
 * 		with file/console logging capabilities & progress reporting
 * 			Calibration
 * 			Synchronization
* 			3D Reconstruction
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 * @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
#endregion

namespace Kinect2
{
	internal enum StandaloneType : int
	{
		None = 0,
		Calibration = 1,
		Synchronization = 2,
		Reconstruction = 3,
		//Evaluation = 4
	}
	
	internal sealed class Standalone
	{
		#region Factory
		internal sealed class Factory
		{
			private static string CalibrationExe,CalibrationWorkingDir;
			private static string SynchronizationExe;
			private static string ReconstructionExe;
			private static string LogFile = "log.txt";
			
			internal static void Configure(Configuration config)
			{
				CalibrationExe = config.CalibrationExecutablePath;
				CalibrationWorkingDir = config.ExecutableInputDirectory;
				SynchronizationExe = config.SynchronizationExecutablePath;
				ReconstructionExe = config.ReconstructionExecutablePath;
			}
			
			internal static Standalone Create(StandaloneType type,string arguments,bool log = false)
			{
				Standalone executable = new Standalone();
				switch(type)
				{
				case StandaloneType.Calibration:
					executable.StartInfo = new ProcessStartInfo()
					{
						FileName = CalibrationExe,
						Arguments = arguments,	
						WorkingDirectory = CalibrationWorkingDir,
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true					
					};
					break;
				case StandaloneType.Reconstruction:
					var workingdir = Path.GetDirectoryName(ReconstructionExe);
					executable.StartInfo = new ProcessStartInfo()
					{
						FileName = ReconstructionExe,
						WorkingDirectory = workingdir,
						//Arguments = workingdir + " " + arguments,	
						Arguments = arguments + DefaultReconstructionParameters,							
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						//CreateNoWindow = true
					};
					break;
				case StandaloneType.Synchronization:
					executable.StartInfo = new ProcessStartInfo()
					{
						FileName = SynchronizationExe,
						Arguments = arguments,	
						UseShellExecute = false,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true					
					};
					break;
					//					case StandaloneType.Evaluation:
					//						break;
				case StandaloneType.None:
				default:
					break;
				}
				if(log)
				{
					executable.IsLogging = true;
					executable.logger = new StreamWriter(Path.Combine(arguments.Split(new string[]{" "},StringSplitOptions.RemoveEmptyEntries)[1],LogFile));
					executable.process.OutputDataReceived += (sender, e) => 
					{
						executable.logger.WriteLine(e.Data);
						Console.Standalone.Log(e.Data);
					};
					executable.process.ErrorDataReceived += (sender, e) =>
					{
						executable.logger.WriteLine(e.Data);
						Console.Standalone.Error(e.Data);
					};
				}
				return executable;				
			}
		}
		#endregion
		#region Constructors
		private Standalone()
		{
			this.process = new Process();
		}
		#endregion
		#region Properties
		internal bool Done { get { return this.process.HasExited; } }
		internal int ExitCode { get { return this.process.ExitCode; } }
		internal TimeSpan Elapsed { get { return this.exited.Ticks == 0 ? DateTime.UtcNow - this.started : this.exited.ToUniversalTime() - this.started; } }
		internal string FileName { get { return this.process.StartInfo.FileName; } }
		internal float Progress { get; private set; }
		
		private ProcessStartInfo StartInfo { get { return this.process.StartInfo; } set { this.process.StartInfo = value; } }
		private bool IsLogging { get; set; }
		#endregion
		#region Methods
		internal bool Start()
		{
			if(this.process.StartInfo != null && this.process.Start())
			{
				this.started = DateTime.UtcNow;
				this.process.Exited += (sender, e) => this.exited = this.process.ExitTime;
				this.process.OutputDataReceived += (sender, e) => 
				{
					if(e.Data.StartsWith(ProgressIdentifier))
					{
						this.Progress = float.Parse (e.Data.Substring(1),ni);
					}
				};
				this.process.ErrorDataReceived += (sender, e) =>
				{
					
				};
				this.process.BeginOutputReadLine();			
				this.process.BeginErrorReadLine();
				return true;
			}
			return false;
		}
		
		internal void Stop()
		{
			if(this.started.Ticks > 0)
			{
				if(this.process.HasExited)
				{
					this.process.Close();		
				}
				else
				{
					this.process.Kill();
				}
				if(this.IsLogging)
				{
					this.logger.Close();
				}
			}
		}
		#endregion
		#region Fields
		private Process process;
		private StreamWriter logger;
		private DateTime started,exited;
		
		private const string ProgressIdentifier = "@";
		private static NumberFormatInfo ni = new NumberFormatInfo()
		{
			NumberDecimalDigits = 2,
			NumberDecimalSeparator = ".",
		};
		private const string DefaultReconstructionParameters = @" -input meshfromrecordings -start 00:00:01 /end 00:00:015 /dbbox /mode StoreAndPlay /viewer";
		#endregion
	}
}
