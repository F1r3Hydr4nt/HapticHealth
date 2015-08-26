using UnityEngine;
using System.Collections.Generic;

using System.IO;

using System.Linq;
using System.Threading;
using System;

class VideoExporter
{
	List<byte[]> videoFrames;
	string filePath;
	string motionName;
	public VideoExporter (string videoPath, string name, List<byte[]> frames)
	{
		filePath = videoPath;
		videoFrames = frames;
		motionName = name;
		//Debug.Log ("Video Exporter frames received "+frames.Count);
	}

	void FindAndDeletePreviousMotionFiles ()
	{
		string[] files = Directory.GetFiles (FusedSkeleton_FromFile.recordDirectory + "/Videos/");
		foreach (string s in files) {
						if (s.Contains (motionName)) {
								File.Delete (s);
								//Debug.Log ("Deleting previous file:" + s);
						}
				}
	}

	public void ExportVideo ()
	{
		FindAndDeletePreviousMotionFiles ();
		int frameNumber = 0;
		foreach (byte[] t in videoFrames) {
			//t.GetPixel(0,0);
			System.IO.File.WriteAllBytes(filePath+frameNumber.ToString()+".jpg", t); //app path n1!
			frameNumber++;
		}
	}

	public void TestThread(){
		while (true) {
			//Debug.Log("TestThread");
		}
	}
}

