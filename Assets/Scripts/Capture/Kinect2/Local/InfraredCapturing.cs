#region Description
/* 		Kinect 2 Infrared Capturing
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using UnityEngine;

using Windows.Kinect;
#endregion
namespace Kinect2.Local
{
	internal class InfraredCapturing : MonoBehaviour,IRecorder
	{
		#region Properties
		
		#endregion
		#region Unity
		void Start () 
		{
			
		}
		
		void Update () 
		{
			
		}
		#endregion
		#region IRecorder implementation
		public bool StartRecording ()
		{
			return false;
		}
		
		public bool CanRecord { get { return false; } }
		
		public bool IsRecording { get { return false; } }
		
		public float RecordingConfidence { get { return 0.0f; } }
		#endregion
		#region Fields
		
		#endregion
	}
}
