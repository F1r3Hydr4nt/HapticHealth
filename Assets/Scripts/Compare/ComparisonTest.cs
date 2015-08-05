using UnityEngine;
using System.Collections;
using Vcl.Utilities;
using Vcl.Utilities.Features;
using Compare.Utilities;

public class ComparisonTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (string.IsNullOrEmpty(RecordedFilename))
			RecordedFilename = CResources.DEFAULT_RECORDING_FILE_PATH;
		Comparator.Initialise_comparison();

		GameObject goa = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		goa.transform.position = new Vector3(0, 0, -20);
		lra = goa.AddComponent<LineRenderer>();
		lra.material = new Material(Shader.Find ("Particles/Additive"));
		lra.SetColors(Color.blue, Color.cyan);
		lra.SetWidth(0.5f, 0.5f);
		
		GameObject gob = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		gob.transform.position = new Vector3(0, 0, -20);
		lrb = gob.AddComponent<LineRenderer>();
		lrb.material = new Material(Shader.Find ("Particles/Additive"));
		lrb.SetColors(Color.green, Color.yellow);
		lrb.SetWidth(0.5f, 0.5f);
	}

	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 150, 50), "Init Comparator"))
		{
			Comparator.Initialise_comparison();
		}

		if (GUI.Button(new Rect(10, 70, 150, 50), "Start Comparator"))
		{
			Comparator.ActionName = ActionCode;
			Comparator.Compute_comparison(RecordedFilename);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Comparator.DoneComparison)
		{
			GameObject.Find("Test/Score").guiText.text = Comparator.AreRelated ? Comparator.OverallScore.ToString("F2") : "";
			GameObject.Find("Test/ScoreStr").guiText.text = Comparator.AreRelated ? Comparator.SemanticFeedback : "Try again";
			if ((Comparator.CurrentState == State.DRAW_PLOTS || Comparator.CurrentState == State.DONE) && Comparator.AreRelated)
			{
				string new_plot_filename = "";
				if (DrawScorePlot)
				{
					new_plot_filename = Comparator.GetPlotFilename(SelectedScoreForPlot);
					DrawFeaturePlot = false;
				}
				else
				{
					new_plot_filename = Comparator.GetPlotFilename(SelectedJointForPlot, SelectedFeatureForPlot);
					DrawScorePlot = false;
					DrawFeaturePlot = true;
				}

				if (new_plot_filename != null && new_plot_filename != PlotFilename)
				{
					PlotFilename = new_plot_filename;

					var bytes = System.IO.File.ReadAllBytes(PlotFilename);
					var plot = new Texture2D(1, 1);
					plot.LoadImage(bytes);
					GameObject.Find("Test/Plot").guiTexture.texture = plot;
				}
			}
			if (Comparator.Tips != null)
			{
				if (Comparator.Tips.Length > 0)
					GameObject.Find("Test/Tip1").guiText.text = Comparator.Tips[0];
				else
					GameObject.Find("Test/Tip1").guiText.text = "";

				if (Comparator.Tips.Length > 1)
					GameObject.Find("Test/Tip2").guiText.text = Comparator.Tips[1];
				else
					GameObject.Find("Test/Tip2").guiText.text = "";

				if (Comparator.Tips.Length > 2)
					GameObject.Find("Test/Tip3").guiText.text = Comparator.Tips[2];
				else
					GameObject.Find("Test/Tip3").guiText.text = "";
			}

			if (Comparator.Trajectories != null)
			{
				if (!a_started)
					StartCoroutine("DrawLineA");
				if (!b_started)
					StartCoroutine("DrawLineB");
			}
		}
	}
	
	private IEnumerator DrawLineA()
	{
		a_started = true;
		return DrawLine(Comparator.Trajectories[0].OriginalXSeries, lra);
	}
	private bool a_started = false;
	
	private IEnumerator DrawLineB()
	{
		b_started = true;
		return DrawLine(Comparator.Trajectories[0].OriginalYSeries, lrb);
	}
	private bool b_started = false;

	private IEnumerator DrawLine(Vec3f[] traj, LineRenderer lr)
	{
		lr.SetVertexCount(traj.Length);
		
		for (int i = 0; i < traj.Length; ++i) {
			lr.SetPosition(i, new Vector3(float.NaN, float.NaN, float.NaN));
		}
		
		for (int i = 0; i < traj.Length; ++i) {
			lr.SetPosition(i, Trans(traj[i]));
			yield return new WaitForSeconds(0.05f);
		}
	}

	private Vector3 Trans(Vec3f vec){
		return vec * (true ? 50f : 0.1f) + (Vec3f.UnitZ * 100f);
	}

	public bool DrawScorePlot = true;
	public ScoreType SelectedScoreForPlot = ScoreType.Overall;

	public bool DrawFeaturePlot = false;
	public JointType SelectedJointForPlot = JointType.R_Elbow;
	public FeatureType SelectedFeatureForPlot = FeatureType.Extension;
	public string PlotFilename;

	public Compare_module Comparator;
	public Properties.Activity ActionCode;
	public string RecordedFilename;

	private LineRenderer lra;
	private LineRenderer lrb;
}
