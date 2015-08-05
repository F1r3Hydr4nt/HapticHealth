#region Description
/*	
Base Behaviour for the different ModalityRecordings

TODO:
        -   Hard Coded Default Path

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;
using System.IO;
//using Utilities;
#endregion
public abstract class ModalityRecording : MonoBehaviour
{
	#region Properties
    internal string OutputDirectory
    {
        get{ return this.directory;}
        set
        {
            this.directory = string.IsNullOrEmpty(value) ? DefaultOutputDirectory : 
								Directory.Exists(value) ? value : Directory.CreateDirectory(value).FullName;
        }
    }
    public bool IsRecording { get; protected set; }
	#endregion
	#region Unity
    protected virtual void Awake()
    {
        this.directory = DefaultOutputDirectory;
    }
	#endregion
	#region Helpers
    internal abstract void  StartRecord();
    internal abstract void  StopRecord();
    internal abstract void  ToggleRecord();
	#endregion
	#region Fields
    private string directory;
	
    //private static string DefaultOutputDirectory = System.Environment.CurrentDirectory;
    private const string DefaultOutputDirectory = @"D:\TestData\UnityRecs";
#endregion
}
