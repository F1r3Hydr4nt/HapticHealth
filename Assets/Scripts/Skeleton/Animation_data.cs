using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;

using Kinect2.IO;

//public class Animation_data : MonoBehaviour {
public class Animation_data
{	
	/***
	 * Description:
	 * Animation structure: contains animation (list of HBP skeleton frames)
	 * Version: 0.1
	 * Autor:
	 * Yvain Tisserand - MIRALab
	 *****/
	public string Filename;
	public string name;

	//private List<HBP.SkeletonBone[]> _motion;
	private List<SkeletonFrame> motion;
	private int numberFrames = 0;

	//TODO MOVE THE REFERENCE
	private int _joint_number = 21;

	public Animation_data ()
	{
		this.motion = new List<SkeletonFrame>();			
		this.numberFrames = 0;		
	}

	void Start ()
	{
		//_motion = new List<HBP.SkeletonBone[]>();
		this.motion = new List<SkeletonFrame>();		
		this.numberFrames = 0;
	}

	void Update () 
	{
	
	}

	public void Reset()
	{
		if( this.motion != null ) 
		{
			this.motion.Clear();
			this.numberFrames = 0;
		}		
	}

	//public void AddFrame(HBP.SkeletonBone[] frame)
	public void AddFrame(SkeletonFrame frame)	
	{
		this.motion.Add(frame);
		this.numberFrames++;
		Console.ImportantIf(frame.UserCount > 1,"<color=orange><b>MORE THAN 1 USERS CAPTURED!</b></color>");
	}

	//public HBP.SkeletonBone[] GetFrame(int frame_number)
	public Skeleton GetFrame(int frame_number)	
	{
		if(this.motion[frame_number].Skeletons.Any()) // was a null check for hbp.skeletonbones
		{
			Console.ImportantIf(this.motion[frame_number].Skeletons.Count() > 1,"<color=orange><b>MORE THAN 1 USERS CAPTURED!</b></color>");			
			return this.motion[frame_number].Skeletons.First();
		}
		else
		{
			Debug.Log("Try to read animation frame out of range");
			return null;
		}
	}
	public int getLenght()
	{
		return numberFrames;
	}

	public void Initialize_from_file(string path)
	{
		TextAsset txt_file = (TextAsset)Resources.Load(path, typeof(TextAsset));
		StringReader reader = null; 
		reader = new StringReader(txt_file.text);
		//TextReader txt_file = Resources.Load(path.ToString()) as TextAsset;
		//TextReader txt_file = new StringReader(path.ToString());
		/*****************************************************************************************************/
		//                      ITI Parse data from TEXT files to reference and recorded motion lists
		/*****************************************************************************************************/
		// TODO SWITCH		
		//motion = new List<HBP.SkeletonBone[]>();
		numberFrames = 0;

		// Parse the file
		try
		{

			while (reader.ReadLine() != null)
			{
				HBP.SkeletonBone[] n_Bone = new HBP.SkeletonBone[_joint_number];	
				
				
				for (int i = 1; i < _joint_number + 1; i++)
				{
					n_Bone[i - 1] = new HBP.SkeletonBone(); 
					
					string t_line = reader.ReadLine();
					
					string[] t_values = t_line.Split(' ');
					
					///////////////// Data parsing to HBP.SkeletonBone
					
					n_Bone[i - 1].posx = float.Parse(t_values[1]);
					n_Bone[i - 1].posy = float.Parse(t_values[2]);
					n_Bone[i - 1].posz = float.Parse(t_values[3]);
					
					n_Bone[i - 1].m11 = float.Parse(t_values[4]);
					n_Bone[i - 1].m12 = float.Parse(t_values[5]);
					n_Bone[i - 1].m13 = float.Parse(t_values[6]);
					n_Bone[i - 1].m21 = float.Parse(t_values[7]);
					n_Bone[i - 1].m22 = float.Parse(t_values[8]);
					n_Bone[i - 1].m23 = float.Parse(t_values[9]);
					n_Bone[i - 1].m31 = float.Parse(t_values[10]);
					n_Bone[i - 1].m32 = float.Parse(t_values[11]);
					n_Bone[i - 1].m33 = float.Parse(t_values[12]);
					
					n_Bone[i - 1].id = Marshal.StringToBSTR(t_values[0]);
					
				}					
				// Add new frame in the list
				// TODO SWITCH
				//motion.Add(n_Bone);				
				numberFrames++;
				
				///// parse empty lines
				for (int j = 0; j < 1; j++) 
				{
					reader.ReadLine();
				}
			}
		
		}
		catch
		{
			Debug.Log("There was a problem during import of external animation");
		}
		
		Debug.Log("Number of frames: "+numberFrames);
	}
}
