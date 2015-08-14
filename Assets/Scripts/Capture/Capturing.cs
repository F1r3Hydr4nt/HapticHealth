#region Description
/* 		Base Recording Behaviour
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Nov 2014
 *  @version: 1.0
 */
#endregion
#region Namespaces
using UnityEngine;

using System.Collections.Generic;
using System.Linq;
#endregion
interface IRecorder
{
	bool CanRecord { get; }
	bool IsRecording { get; }
	float RecordingConfidence { get; }
	bool StartRecording();
}

interface ICalibrator
{
	bool CanCalibrate { get; }
	bool IsCalibrating { get; }
	float CalibrationConfidence { get; }					
	bool StartCalibration();
}

interface ISyncer
{
	bool CanSync { get; }
	bool IsSyncing { get; }
	float SyncConfidence { get; }		
	float SyncRecordingConfidence { get; }
	bool StartSynchronization();
}

interface IReconstructor
{
	bool CanReconstruct { get; }
	bool IsReconstructing { get; }
	bool StartReconstruction();
}

interface ICancellation
{
	bool CanStop { get; }
	bool Stop();
}

interface IDevice
{
	 Device Type { get; }
}

public abstract class Capturer : MonoBehaviour,IDevice,IRecorder,ICancellation
{
	public abstract Device Type { get; }

	public abstract bool CanRecord { get; }
	public abstract bool IsRecording { get; }
	public abstract bool CanStop{ get; }
	public float RecordingConfidence { get; protected set; }
	public abstract bool StartRecording();
	public abstract bool Stop ();
}

internal sealed class CompositeRecorder : IRecorder, ICancellation
{
	internal static CompositeRecorder FromRecorders(params IRecorder[] recorders)
	{
		return new CompositeRecorder(recorders);
	}

	internal CompositeRecorder (IEnumerable<IRecorder> recorders)
	{
		this.recorders = recorders.ToArray();
	}
	#region IRecorder implementation
	public bool CanRecord { get { return this.recorders.Any((recorder) => recorder.CanRecord); } }	

	public bool StartRecording()
	{
		return this.recorders.Where((recorder) => recorder.CanRecord).All((recorder) => recorder.StartRecording());
	}

	public bool IsRecording { get { return this.recorders.Any((recorder) => recorder.IsRecording); } }

	public float RecordingConfidence { get { return this.recorders.Where ((recorder) => recorder.CanRecord).Average((recorder) => recorder.RecordingConfidence); } }
	#endregion
	#region ICancellation implementation
	public bool Stop ()
	{
		return this.recorders.Where((recorder) => recorder.IsRecording)
			.Cast<ICancellation>()
			.All((cancellation) => cancellation.CanStop && cancellation.Stop());
	}

	public bool CanStop { get { return this.recorders.Any((recorder) => recorder is ICancellation && recorder.IsRecording); } }
	#endregion
	
	private IRecorder[] recorders;
}
