#region Description
/* 		Skeleton Text File Exporting Behaviour
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
using System.IO;
using System.Text;

using Vcl.Utilities;
using Vcl.Utilities.IO;

using Kinect2Joint = Vcl.Utilities.Joint;
#endregion
public class TextExporting : MonoBehaviour 
{
	#region Properties
	public string OutputDirectory { get; set; }
	public string Filename { get; set; }

	private bool HasNewJoints { get; set; }

	internal ulong Timestamp { get; set; }
	internal Kinect2Joint[] Joints 
	{
		private get 
		{
			this.HasNewJoints = false;
			return this.joints; 
		}
		set
		{
			this.joints = value;
			this.HasNewJoints = true;
		}
	}
	#endregion
	#region Unity
	void Start () 
	{
		this.enabled = false;
	}

	void OnEnable()
	{
		this.frameNumber = 0;
		this.hum = new K2Skeleton("Kinect2 Skeleton", new string[]
         {
			string.Format("version={0}", "1.2"),
			string.Format("frameNumber={0}", frameNumber),
			string.Format("timestamp={0}", (frameNumber * 32).ToString("X11")),
			string.Format("userCount={0}", 1),
			string.Format("userID={0}", 1),
			string.Format("state={0}", 1)
		});
		this.stream = new HumanoidStream(new IHumanoid[1] {hum });
		this.streamwriter = new SkelextV20Writer(Path.Combine(this.OutputDirectory,this.Filename));
	}

	void LateUpdate () 
	{
		if(this.HasNewJoints)
		{
			++frameNumber;
			foreach(var joint in joints)
			{
				hum[joint.Type].Center = joint.Center;
				hum[joint.Type].Rotation = joint.Rotation;
			}
			this.hum.SetInfo("frameNumber",this.frameNumber.ToString());
			this.hum.SetInfo("timestamp",this.Timestamp.ToString());
			this.streamwriter.Write(this.stream);
		}
	}

	void OnDisable()
	{
		this.streamwriter.Dispose();
	}
	#endregion
	#region Fields
	private HumanoidWriter streamwriter;
	private HumanoidStream stream;
	private IHumanoid hum;
	private Kinect2Joint[] joints;
	private int frameNumber;
	#endregion
}
