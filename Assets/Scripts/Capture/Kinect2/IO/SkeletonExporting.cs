#region Description
/* 		Kinect 2 Skeleton Exporting Behavior
 * 
 * 	@author: Nick Zioulis, nzioulis@iti.gr, Visual Computing Lab, CERTH
 * @date:	Jan 2015
 *  @version: 1.0
 */
#endregion
#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using UnityEngine;

using Kinect2;
using Kinect2.Local;
#endregion
namespace Kinect2.IO
{
	[RequireComponent(typeof(SkeletonCapturing))]
	public sealed class SkeletonExporting : MonoBehaviour 
	{
		#region Properties
		internal FrameSpan Elapsed { get; private set; }
		internal string Filename { get; private set; }
		#endregion
		#region Unity
		void Awake()
		{
			this.tracker = GetComponent<SkeletonCapturing>();
			var controller = FindObjectOfType<Kinect2Controlling>();
			if(controller != null)
			{
				this.outputDirectory = Path.Combine(controller.Config.DataDirectory,OutputSubDirectory);				
			}
			else
			{
				this.outputDirectory = Path.Combine(Environment.CurrentDirectory,OutputSubDirectory);								
            }
			this.enabled = this.tracker != null && this.tracker.CanRecord;			
        }

		void Start ()
		{
		
		}

		void OnEnable()
		{
			var path = Path.Combine(this.outputDirectory,
			                                     DateTime.UtcNow.ToString("yy-MM-dd-HH-mm-ss"));
			if(!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			this.Filename = Path.Combine(path,DefaultFileName);
			Console.Important("EXPORTING @ " + this.Filename);
			this.writer = SklxtWriter.Constructor.Start().New(this.Filename).Construct();
			this.enabled = this.writer.Start();		
		}

		void LateUpdate () 
		{
			//if(this.tracker.HasNewFrame)
			{
				this.writer.Write(this.tracker.CurrentFrame);
			}
		}

		void OnDisable()
		{
			if(this.writer != null && this.writer.CanWrite)
			{
				this.Elapsed = this.writer.Stop();
			}
		}

		void OnDestroy()
		{

		}
		#endregion
		#region Fields
		private SkeletonCapturing tracker;
		private SklxtWriter writer;

		private string outputDirectory;

		private const string DefaultFileName = "tmp.sklxt";
		private const string OutputSubDirectory = "PlayAndLearn";
		#endregion
	}
}
