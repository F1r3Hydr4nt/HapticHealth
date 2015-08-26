using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

using System.Linq;
using System.Threading;
using System;

public class VideoImporter
{
	public bool videoImported=false;

	string filePath;
	string motionName;
	public VideoImporter (string videoPath, string name)
	{
		filePath = videoPath;
		motionName = name;
		Debug.Log ("Created Importer");
		FindAllFiles ();
		InitialiseFrameList ();
	}

	void InitialiseFrameList ()
	{
		
		videoFrames = new List<Texture2D>();
		videoFrameData = new List<byte[]>();
		foreach (string s in relevantFiles) {
			videoFrames.Add(new Texture2D(2,2));
		}
	}

	List<string> framesFiles;
	List<string> relevantFiles;
	public List<Texture2D> videoFrames;
	public List<byte[]> videoFrameData;
	void FindAllFiles ()
	{
		string[] files = Directory.GetFiles (FusedSkeleton_FromFile.recordDirectory + "/Videos/");
		relevantFiles = new List<string> ();
		foreach (string s in files) {
			if(s.Contains (motionName))relevantFiles.Add (s);
		}
		relevantFiles = relevantFiles.OrderBy(x=>x.Length).ThenBy(x=> x).ToList();
		//Debug.Log ("Importer Found frames: "+relevantFiles.Count);
	}
	public void ImportVideoData ()
	{
		//Debug.Log ("ImportVideo Importer");
		foreach (string s in relevantFiles) {
			byte[] fileData;
			fileData = File.ReadAllBytes(s);
			//Debug.Log("Attempting to asynchronously load: "+s);
			videoFrameData.Add(fileData);
		}
		videoImported = true;
		//Debug.Log ("Video Imported");
	}
	public void ImportVideoDataWithAsyncLoadTexture ()
	{
		//Debug.Log ("ImportVideo Importer");
		int frameCount = 0;
		List<AsyncFrameLoad> asyncLoaders = new List<AsyncFrameLoad> ();
		foreach (string s in relevantFiles) {
			AsyncFrameLoad loader = new AsyncFrameLoad(frameCount,s);
			//cant start coroutine outside of main thread
			//loader.StartCoroutine("LoadFrame");//();
			//Can't start a coroutine as a thread
		//	Thread oThread = new Thread (new ThreadStart (loader.LoadFrame));
			//Thread cant be type IEnumerator
			// Start the thread
		//	oThread.Start ();
			asyncLoaders.Add (loader);
			frameCount++;
		}
		bool allFramesLoaded = false;
		while (!allFramesLoaded) {
			allFramesLoaded = true;
			foreach(AsyncFrameLoad a in asyncLoaders){
				if(!a.loaded)allFramesLoaded = false;
			}
		}
		asyncLoaders = asyncLoaders.OrderBy (x => x.frameNumber).ToList();
		frameCount = 0;
		foreach (AsyncFrameLoad a in asyncLoaders) {
			videoFrames[frameCount] = a.www.texture;
			frameCount++;
		}
		videoImported = true;
		//Debug.Log ("Video Imported");
	}


	public List<Texture2D> ConvertFramesToTextures(){
		int frameCount = 0;
		foreach (Byte[] data in videoFrameData) {
			videoFrames[frameCount].LoadImage(data);
			frameCount++;
		}

		return videoFrames;
	}
}

public class AsyncFrameLoad{
	public WWW www;
	public int frameNumber;
	public string url;
	public bool loaded;
	public AsyncFrameLoad(int i, string s){
		frameNumber = i;
		url = s;
	}
	public IEnumerator LoadFrame(){
		www = new WWW (url);
		yield return www;
		loaded = true;
	}
}

