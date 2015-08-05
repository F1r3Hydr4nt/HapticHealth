using Vcl.Utilities;
using Vcl.Utilities.Features;
using Vcl.Utilities.IO;
using Vcl.Comparison;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Compare.Utilities;

public class Compare_module : MonoBehaviour {

	/***
	 * Description:
	 * Comparison module: input reference motion and recorded motion
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/
	
	#region Mono Methods
		
	// Use this for initialization
	void Start () {
		DoneComparison = false;
		
		SetExecutableLocation(CResources.EXECUTABLE_FILE_PATH);
		SetReferenceDatabase(CResources.REFERENCE_DATABASE_FILE_PATH);
		SetWeightsDatabase(CResources.WEIGHTS_DATABASE_FILE_PATH);
		SetRecordedFile(CResources.DEFAULT_RECORDING_FILE_PATH);

		// The default scenario is the COACH & TRAIN
		DrawPlots = true;

		Initialise_comparison();
	}

	// Update is called once per frame
	void Update () {
		
		if (comparexe != null && comparexe.HasExited)
			comparexe = null;
	
		// Update time
		if (_is_comparing)
		{
			var time = UnityEngine.Time.time - _start_time;
			var min = (int)time / 60;
			var sec = (int)time % 60;
			var fraction = (time * 10) % 10;
			Time = string.Format("{00:00}:{1:00}:{2:00}", min, sec, fraction);
		}
	}

	void OnDestroy()
	{
		StopProcess();
	}

	#endregion

	#region Public Members
	
	#region Initialise Comparison

	/// <summary>
	/// Initialises the comparison module using the default settings. After the initialisation,
	/// the comparison module is ready to compare.
	/// </summary>
	public void Initialise_comparison()
	{
		if (comparexe != null)
			StopProcess();
	
		DoneComparison = false;
		_is_comparing = false;
		_start_time = 0f;

		AreRelated = false;
		SemanticFeedback = "";
		Tips = null;

		OverallScore = 0;
		PositionScore = 0;
		RotationScore = 0;
		LinearVelocityScore = 0;
		AngularVelocityScore = 0;
			
		PlotFiles = null;
		UserID = 0;
			
		DoneComparison = false;
				
		Time = "00:00:00";
			
		CurrentState = State.DONE;

		// For the 1st prototype we will use the weights file.
		UseWeightsFile = true;
	}

	/// <summary>
	/// Initialises the comparison module using the default settings for the specified activity.
	/// </summary>
	/// <param name="action_name">The comparison activity.</param>
	public void Initialise_comparison(Properties.Activity action)
	{
		Initialise_comparison();
		ActionName = action;
	}

	/// <summary>
	/// Initialises the comparison module using the specified reference and weights database paths.
	/// </summary>
	/// <param name="reference_database_filename">The reference database file path.</param>
	/// <param name="weights_database_filename">The weights database file path.</param>
	public void Initialise_comparison(string reference_database_filename, string weights_database_filename)
	{
		Initialise_comparison();
		SetReferenceDatabase(reference_database_filename);
		SetWeightsDatabase(weights_database_filename);
	}

	/// <summary>
	/// Initialises the comparison module using the specified reference and weights database paths as well as the specified activity.
	/// </summary>
	/// <param name="reference_database_filename">The reference database file path.</param>
	/// <param name="weights_database_filename">The weights database file path.</param>
	/// <param name="action_name">The comparison activity.</param>
	public void Initialise_comparison(string reference_database_filename, string weights_database_filename, Properties.Activity action_name)
	{
		Initialise_comparison(reference_database_filename, weights_database_filename);
		ActionName = action_name;
	}

	#endregion
	
	#region Compute Comparison
	
	/// <summary>
	/// Start the comparison process using the pre-setted or the default settings.
	/// </summary>
	public void Compute_comparison()
	{
		StartProcess();
	}

	/// <summary>
	/// Start the comparison process for the recorded motion of the file in the parameter
	/// using the pre-setted or the default activity.
	/// </summary>
	/// <param name="recorded_motion_filename">The file path of the recorded motion.</param>
	public void Compute_comparison(string recorded_motion_filename)
	{
		SetRecordedFile(recorded_motion_filename);
		Compute_comparison();
	}

	/// <summary>
	/// Start the comparison process for the recorded motion of the file in the parameter
	/// using as the reference the specified activity.
	/// </summary>
	/// <param name="recorded_motion_filename">The file path of the recorded motion.</param>
	/// <param name="action_name">The activity to compare with.</param>
	public void Compute_comparison(string recorded_motion_filename, Properties.Activity action_name)
	{
		ActionName = action_name;
		Compute_comparison(recorded_motion_filename);
	}

	/// <summary>
	/// Start the comparison process for the animation recorded data
	/// using the pre-setted or the default activity.
	/// </summary>
	/// <param name="recorded_motion">The recorded Animation_data.</param>
	public void Compute_comparison(Animation_data recorded_motion)
	{
		IHumanoid[] rm;
		Properties.From_animation_data(ref recorded_motion, out rm);
		Compute_comparison(rm);
	}

	/// <summary>
	/// Start the comparison process for the animation recorded data
	/// using as the reference the specified activity.
	/// </summary>
	/// <param name="recorded_motion">The recorded Animation_data.</param>
	/// <param name="action_name">The activity to compare with.</param>
	public void Compute_comparison(Animation_data recorded_motion, Properties.Activity action_name)
	{
		ActionName = action_name;
		Compute_comparison(recorded_motion);
	}

	/// <summary>
	/// Start the comparison process for the recorded Vcl.Utilities.IHumanoids
	/// using the pre-setted or the default activity.
	/// </summary>
	/// <param name="recorded_motion">The recorded Vcl.Utilities.IHumanoids.</param>
	public void Compute_comparison(HumanoidStream recorded_motion)
	{
		var filename = System.IO.Path.GetFullPath(CResources.DEFAULT_SAVE_RECORDING_FILE_PATH);
		using (var writer = new SkelextV20Writer(filename))
		{
			writer.Write(recorded_motion);
		}
		Compute_comparison(filename);
	}
	
	/// <summary>
	/// Start the comparison process for the recorded Vcl.Utilities.IHumanoids
	/// using as the reference the specified activity.
	/// </summary>
	/// <param name="recorded_motion">The recorded Vcl.Utilities.IHumanoids.</param>
	/// <param name="action_name">The activity to compare with.</param>
	public void Compute_comparison(HumanoidStream recorded_motion, Properties.Activity action_name)
	{
		ActionName = action_name;
		Compute_comparison(recorded_motion);
	}

	#endregion

	#region Update Arguments

	/// <summary>
	/// Sets the reference database.
	/// </summary>
	/// <param name="filename">Filename.</param>
	public void SetReferenceDatabase(string filename)
	{
		_ref_db_filename = System.IO.Path.GetFullPath(filename);
	}

	/// <summary>
	/// Sets the weights database.
	/// </summary>
	/// <param name="filename">Filename.</param>
	public void SetWeightsDatabase(string filename)
	{
		_wei_db_filename = System.IO.Path.GetFullPath(filename);
	}

	/// <summary>
	/// Sets the recorded file.
	/// </summary>
	/// <param name="filename">Filename.</param>
	public void SetRecordedFile(string filename)
	{
		_rec_filename = System.IO.Path.GetFullPath(filename);
	}

	/// <summary>
	/// Sets the comparison's executable location.
	/// </summary>
	/// <param name="filename">Filename.</param>
	private void SetExecutableLocation(string filename)
	{
		_exe_filename = System.IO.Path.GetFullPath(filename);
	}

	#endregion
	
	#region Plotting
	
	/// <summary>
	/// Gets the plot filename of a specific joint and feature.
	/// </summary>
	/// <returns>The plot filename.</returns>
	/// <param name="joint">The selected joint.</param>
	/// <param name="feature">The selected feature.</param>
	public string GetPlotFilename(JointType joint, FeatureType feature)
	{
		if (PlotFiles == null || PlotFiles.Length == 0)
			return null;
		
		var elements = AvailablePlots.Where(s => s.Contains(Vcl.Utilities.Joint.ToString(joint)) && s.Contains(Feature.ToString(feature)));
		if (elements.Count() == 0)
			return null;
		
		return GetPlotFilename(elements.FirstOrDefault());
	}

	public string GetPlotFilename(ScoreType score)
	{
		var sts = ScoreToString(score);

		if (PlotFiles == null || PlotFiles.Length == 0)
			return null;

		var elements = AvailablePlots.Where (s => s.Contains(sts));
		if (elements.Count () == 0)
			return null;

		return GetPlotFilename(elements.FirstOrDefault());
	}

	public string GetPlotFilename(string plotname)
	{
		int nameidx = System.Array.IndexOf(AvailablePlots, plotname);
		if (nameidx < 0)
			return null;
		string localpath = PlotFiles[nameidx].Trim();
		return System.IO.Path.GetFullPath(localpath);
	}

	private string ScoreToString(ScoreType score)
	{
		switch (score)
		{
		case ScoreType.Overall:
			return "Overall Score";
		case ScoreType.Position:
			return "Position Score";
		case ScoreType.Rotation:
			return "Rotation Score";
		case ScoreType.LinearVelocity:
			return "Linear Velocity Score";
		case ScoreType.AngularVelocity:
			return "Angular Velocity Score";
		case ScoreType.AlignmentDifference:
			return "Alignment Difference Score";
		}
		return "";
	}

	#endregion

	#region Properties

	/// <summary>
	/// When it is true, the comparator uses the weights file. Otherwise,
	/// it extracts the weights using the energy.
	/// </summary>
	public bool UseWeightsFile;
	/// <summary>
	/// When it is true, the comparator draws all the plots for the specified
	/// motion during the comparison task.
	/// For the PLAY & LEARN scenario we have to set this property as false, but
	/// for the COACH & TRAIN scenarion we need it to be true.
	/// </summary>
	public bool DrawPlots;

	/// <summary>
	/// The selected reference activity for the comparison.
	/// </summary>
	public Properties.Activity ActionName;

	/// <summary>
	/// True if the recorded motion is related to the reference motion, False otherwise.
	/// </summary>
	public bool AreRelated;
	/// <summary>
	/// The semantic feedback of the score.
	/// </summary>
	public string SemanticFeedback;
	/// <summary>
	/// Some tips for the user. The tips are sorted according to their importance.
	/// </summary>
	public string[] Tips;

	/// <summary>
	/// The overall score in the domain [0, 100].
	/// </summary>
	public float OverallScore;
	/// <summary>
	/// The position score in the domain [0, 100].
	/// </summary>
	public float PositionScore;
	/// <summary>
	/// The rotation score in the domain [0, 100].
	/// </summary>
	public float RotationScore;
	/// <summary>
	/// The linear velocity score in the domain [0, 100].
	/// </summary>
	public float LinearVelocityScore;
	/// <summary>
	/// The angular velocity score in the domain [0, 100].
	/// </summary>
	public float AngularVelocityScore;

	/// <summary>
	/// The available plot files. After the comparison,
	/// it will show the available features per joint to plot.
	/// </summary>
	public string[] AvailablePlots;
	/// <summary>
	/// The even indexes are the titles of the plots and the odd indexes are the absolute file paths.
	/// </summary>
	public string[] PlotFiles;
	/// <summary>
	/// The important trajectories.
	/// </summary>
	public SeriesVariable<Vec3f>[] Trajectories;
	/// <summary>
	/// The ID of the user.
	/// </summary>
	public ulong UserID;

	/// <summary>
	/// True if the comparison task has been finished.
	/// </summary>
	public bool DoneComparison;

	/// <summary>
	/// A stopwatch that shows the duration of the comparison process.
	/// </summary>
	public string Time = "00:00:00";

	/// <summary>
	/// The state of the module.
	/// </summary>
	public State CurrentState;

	#endregion

	#endregion

	#region Handle Process

	#region Start Process

	/// <summary>
	/// Starts the comparison process.
	/// </summary>
	private void StartProcess()
	{
		if (comparexe != null && CurrentState == State.START)
			StopProcess();

		comparexe = new System.Diagnostics.Process();
		comparexe.StartInfo.CreateNoWindow = true;
		comparexe.StartInfo.UseShellExecute = false;
		comparexe.StartInfo.RedirectStandardOutput = true;
		comparexe.StartInfo.RedirectStandardError = true;
		comparexe.StartInfo.FileName = System.IO.Path.GetFullPath(_exe_filename);
		comparexe.StartInfo.Arguments = 
			string.Format("{0} {1} {2} {3} {4}",
	              _ref_db_filename,
	              UseWeightsFile ? _wei_db_filename : "Null",
	              _rec_filename,
	              DrawPlots ? "-p" : "",
	              Properties.ActivityToString(ActionName));
		comparexe.EnableRaisingEvents = true;
		comparexe.OutputDataReceived += (sender, e) =>
		{
			lock (_buffer)
			{
				_buffer.Add(e.Data);
			}
		};
		comparexe.ErrorDataReceived += (sender, e) =>
		{
			if (e.Data != null)
				UnityEngine.Debug.LogError("Compare process says: " + e.Data);
		};
		comparexe.Exited += (sender, e) =>
		{
			comparexe.CancelOutputRead();
			comparexe.CancelErrorRead();
			UnityEngine.Debug.Log("Compare process exited!");
		};

		if(comparexe.Start())
		{
			comparexe.BeginOutputReadLine();
			comparexe.BeginErrorReadLine();
			StartCoroutine("StartConsole");
		}
	}
	
	#endregion

	#region Stop Process

	/// <summary>
	/// Stops the comparison process.
	/// </summary>
	private void StopProcess()
	{
		if (comparexe == null)
			return;
	
		StopCoroutine("StartConsole");
		comparexe.CloseMainWindow();
		comparexe = null;
	}

	#endregion

	#region Outputs

	/// <summary>
	/// Starts the reading the buffer of the console output.
	/// </summary>
	/// <returns>An enumerator.</returns>
	IEnumerator StartConsole()
	{
		_buffer = new List<string>();
		while(!comparexe.HasExited)
		{
			yield return new WaitForSeconds(1);

			lock (_buffer)
			{
				while(_buffer.Count > 0)
				{
					var line = _buffer[0];

					if (string.IsNullOrEmpty(line))
						{ }
					if (line.Contains("uid="))
						UserID = ulong.Parse(line.Substring(4));
					else if (line.Contains("act="))
						ActionName = Properties.ActivityFromString(line.Substring(4).ToString());
					else if (line.Contains("sfb="))
						SemanticFeedback = line.Substring(4);
					else if (line.Contains("tip="))
						Tips = line.Substring(4).Split(new string[] { ":|:" }, System.StringSplitOptions.RemoveEmptyEntries);
					else if (line.Contains("rel="))
						AreRelated = bool.Parse(line.Substring(4));
					else if (line.Contains("ovs="))
						OverallScore = float.Parse(line.Substring(4), ni) * 100f;
					else if (line.Contains("pss="))
						PositionScore = float.Parse(line.Substring(4), ni) * 100f;
					else if (line.Contains("rts="))
						RotationScore = float.Parse(line.Substring(4), ni) * 100f;
					else if (line.Contains("lvs="))
						LinearVelocityScore = float.Parse(line.Substring(4), ni) * 100f;
					else if (line.Contains("avs="))
						AngularVelocityScore = float.Parse(line.Substring(4), ni) * 100f;
					else if (line.Contains("plt="))
					{
						var pf = line.Substring(4).Split(new string[] { ":|:" }, System.StringSplitOptions.RemoveEmptyEntries);
						PlotFiles = pf.Where((str, i) => i % 2 == 1).ToArray();
						AvailablePlots = pf.Where ((str, i) => i % 2 == 0).ToArray();
					}
					else if (line.Contains("tjs="))
					{
						var ts = line.Substring(4).Split(new string[] { ":|:" }, System.StringSplitOptions.RemoveEmptyEntries);
						Trajectories = new SeriesVariable<Vec3f>[ts.Length];
						for (int i = 0; i < ts.Length; i++)
							Trajectories[i] = SeriesVariable<Vec3f>.FromString(ts[i]);
					}
					else if (line.Contains("Start"))
					{
						CurrentState = State.START;
						DoneComparison = false;
						_is_comparing = true;
						_start_time = UnityEngine.Time.time;
						Debug.Log("Compare started.");
					}
					else if (line.Contains("Compare done!"))
					{
						Debug.Log(string.Format("Compare done! Elapsed time: {0}", Time));
						CurrentState = State.DRAW_PLOTS;
						_is_comparing = false;
						_start_time = -1;
						DoneComparison = true;
					}
					else if (line.Contains("Done!"))
					{
						if (!AreRelated)
						{
							var tip_1 = Tips[0];

							Tips = new string[] { tip_1 };
						}

						CurrentState = State.DONE;
						yield break;
					}
					else if (line.Contains(":|:"))
					{
						var plots = new List<string>();
						plots.AddRange (PlotFiles);
						plots.AddRange (line.Substring(4).Split(new string[] { ":|:" }, System.StringSplitOptions.RemoveEmptyEntries));
						PlotFiles = plots.ToArray();
					}
					else
						Debug.Log(line);

					_buffer.RemoveAt(0);
				}
			}
		}	
		yield break;
	}

	#endregion

	#endregion

	#region Fields
		
	/// <summary>
	/// The comparison process.
	/// </summary>
	private System.Diagnostics.Process comparexe;
	/// <summary>
	/// A buffer that contains the messages of the comparison process.
	/// </summary>
	private List<string> _buffer;
		
	/// <summary>
	/// Contains information on if the Compare_model is comparing.
	/// </summary>
	private bool _is_comparing;
	/// <summary>
	/// Contains the start time of the comparison.
	/// </summary>
	private float _start_time;
		
	/// <summary>
	/// The file path of the last recorded motion.
	/// </summary>
	private string _rec_filename;
	/// <summary>
	/// The file path of the reference database.
	/// </summary>
	private string _ref_db_filename;
	/// <summary>
	/// The file path of the weights database.
	/// </summary>
	private string _wei_db_filename;
	/// <summary>
	/// The path of the comparison executable.
	/// </summary>
	private string _exe_filename;
					
	private System.Globalization.NumberFormatInfo ni = new System.Globalization.NumberFormatInfo()
	{
		NumberDecimalSeparator = ".",
		NumberDecimalDigits = 6
	};

	#endregion
	
}
