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
	public VideoExporter (string videoPath, List<byte[]> frames)
	{
		filePath = videoPath;
		videoFrames = frames;
	}

	public void ExportVideo ()
	{
		int frameNumber = 0;
		foreach (byte[] t in videoFrames) {
			//t.GetPixel(0,0);
			System.IO.File.WriteAllBytes(filePath+frameNumber.ToString()+".png", t); //app path n1!
			frameNumber++;
		}
		Debug.Log("Exported Video");
	}
}

