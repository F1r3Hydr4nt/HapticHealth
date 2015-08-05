using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class BodyFusedView : MonoBehaviour 
{
	/***
	 * Description: 
	 * Rendering of a line-based skeleton 
	 * Skill class
	 * Version: 0.1
	 * Autor:
	 * Francois Destelle - DCU
	 *****/

	// Skeleton
	Dictionary<JointType, Vector3> jointPositions;
	Dictionary<JointType, Quaternion> jointOrientations;
	GameObject bodyObject,body;
	
	private Vector3 globalOffset = new Vector3(5, -15, -15);
	// Standalone implementation
	//private Vector3 globalOffset = new Vector3(5, -5, -15);
	private float globalScale = 25; 

	// Rendering
	private Material BoneMaterial;
	private Color32 color1, color2;
	public void SetColors( Color32 c1, Color32 c2 ) { color1 = c1; color2 = c2; }
	public GameObject rotations;
	
	// Colors
	public Material defaultMaterial;
	public Material onMaterial;
	public Material offMaterial;

	public bool rendering = true;
	public void disableRendering()
	{
		rendering = false;
		this.body.SetActive (false);
		Debug.LogWarning ("Disable rendering of the Capture.");		
	}
	public void enableRendering()
	{
		Debug.LogWarning ("Enable rendering of the Capture.");
		rendering = true;
		this.body.SetActive (true);
	}	

	private void updateMaterial() {
		if( bodyObject != null )
		for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++) {
			Transform jointObj = bodyObject.transform.FindChild (jt.ToString ());
			LineRenderer lr = jointObj.GetComponent<LineRenderer> ();
			lr.material = BoneMaterial;
		}
	}
	public void SetDefaultMaterial() {
		BoneMaterial = defaultMaterial;
		updateMaterial ();
	}
	public void SetOnMaterial() {
		BoneMaterial = onMaterial;
		updateMaterial ();
	}
	public void SetOffMaterial() {
		BoneMaterial = offMaterial;
		updateMaterial ();
	}
	
	// Access
	public BodySourceView sourceView;
	public ulong GetCurrentKinectId() 
	{
		return sourceView.currentId;
	}
	
	public void SetJointPosition(Windows.Kinect.JointType id, Vector3 pos)
	{
		if (jointPositions != null &&
		    id >= Windows.Kinect.JointType.SpineBase && id <= Windows.Kinect.JointType.ThumbRight) 
		{
			jointPositions[id] = pos;
		}
	}
	public void SetJointOrientation(Windows.Kinect.JointType id, Quaternion q)
	{
		if (jointOrientations != null &&
		    id >= Windows.Kinect.JointType.SpineBase && id <= Windows.Kinect.JointType.ThumbRight) 
		{
			jointOrientations[id] = q;
		}
	}
	
	private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
	private Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneMap = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType>()
	{
		//{ Windows.Kinect.JointType.FootLeft, Windows.Kinect.JointType.AnkleLeft },
		{ Windows.Kinect.JointType.AnkleLeft, Windows.Kinect.JointType.KneeLeft },
		{ Windows.Kinect.JointType.KneeLeft, Windows.Kinect.JointType.HipLeft },
		{ Windows.Kinect.JointType.HipLeft, Windows.Kinect.JointType.SpineBase },
		
		//{ Windows.Kinect.JointType.FootRight, Windows.Kinect.JointType.AnkleRight },
		{ Windows.Kinect.JointType.AnkleRight, Windows.Kinect.JointType.KneeRight },
		{ Windows.Kinect.JointType.KneeRight, Windows.Kinect.JointType.HipRight },
		{ Windows.Kinect.JointType.HipRight, Windows.Kinect.JointType.SpineBase },
		
		//{ Windows.Kinect.JointType.HandLeft, Windows.Kinect.JointType.WristLeft },
		{ Windows.Kinect.JointType.WristLeft, Windows.Kinect.JointType.ElbowLeft },
		{ Windows.Kinect.JointType.ElbowLeft, Windows.Kinect.JointType.ShoulderLeft },
		{ Windows.Kinect.JointType.ShoulderLeft, Windows.Kinect.JointType.SpineShoulder },
		
		//{ Windows.Kinect.JointType.HandRight, Windows.Kinect.JointType.WristRight },
		{ Windows.Kinect.JointType.WristRight, Windows.Kinect.JointType.ElbowRight },
		{ Windows.Kinect.JointType.ElbowRight, Windows.Kinect.JointType.ShoulderRight },
		{ Windows.Kinect.JointType.ShoulderRight, Windows.Kinect.JointType.SpineShoulder },
		
		{ Windows.Kinect.JointType.Head, Windows.Kinect.JointType.Neck },
		{ Windows.Kinect.JointType.Neck, Windows.Kinect.JointType.SpineShoulder },
		{ Windows.Kinect.JointType.SpineBase, Windows.Kinect.JointType.SpineMid },
		{ Windows.Kinect.JointType.SpineShoulder, Windows.Kinect.JointType.SpineMid }
	};
	
	// Special array for Fused skeleton
	private Windows.Kinect.JointType[] fusedJoints = 
	{
		Windows.Kinect.JointType.SpineMid,
		
		Windows.Kinect.JointType.ShoulderRight,
		Windows.Kinect.JointType.ElbowRight,
		Windows.Kinect.JointType.WristRight,
		Windows.Kinect.JointType.ShoulderLeft,
		Windows.Kinect.JointType.ElbowLeft,
		Windows.Kinect.JointType.WristLeft,
		
		Windows.Kinect.JointType.HipRight,
		Windows.Kinect.JointType.KneeRight,
		Windows.Kinect.JointType.AnkleRight,
		Windows.Kinect.JointType.HipLeft,
		Windows.Kinect.JointType.KneeLeft,
		Windows.Kinect.JointType.AnkleLeft,
		
		Windows.Kinect.JointType.Head,
		Windows.Kinect.JointType.Neck,
		Windows.Kinect.JointType.SpineBase,
		Windows.Kinect.JointType.SpineShoulder,

		// Additional ones
		Windows.Kinect.JointType.FootLeft,
		Windows.Kinect.JointType.FootRight,
		Windows.Kinect.JointType.HandLeft,
		Windows.Kinect.JointType.HandRight
	};
	
	void Update () 
	{        				        
		if (rendering) 
		{
//			print ("rendering");
			RefreshBodyObject ();
		}
	}
	
	void Start()
	{
	}

	public void Awake()
	{
		SetDefaultMaterial ();		
		bodyObject = CreateBodyObject ();

		jointPositions = new Dictionary<JointType, Vector3>();
		for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++) 
		{
			jointPositions.Add (jt, Vector3.zero);
		}
		jointOrientations = new Dictionary<JointType, Quaternion>();
		for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++) 
		{
			jointOrientations.Add (jt, Quaternion.identity);
		}
		//disableRendering ();		
	}

	private GameObject CreateBodyObject()
	{
		this.body = new GameObject("Fused Body");

		// We stop to HandTipRight to not include ThumbRight as an unused one
		for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++)
		{
			GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);			
			LineRenderer lr = jointObj.AddComponent<LineRenderer>();
			lr.SetVertexCount(2);
			lr.material = BoneMaterial;
			lr.SetWidth(1.0f, 0.2f);
			
			jointObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			jointObj.name = jt.ToString();
			jointObj.transform.parent = body.transform;

			if( /*jt == JointType.FootLeft || jt == JointType.FootRight || jt == JointType.HandLeft
			   || jt == JointType.HandRight ||*/ jt == JointType.HandTipLeft || jt == JointType.HandTipRight
			   || jt == JointType.ThumbLeft || jt == JointType.ThumbRight)
			{
				lr.enabled = false;
				jointObj.SetActive(false);
			}
		}
		
		return body;
	}
	
	// Rendering
	private GameObject rotSpine, rotRS, rotRE, 
				rotLS, rotLE, rotRW, rotLW, 
				rotLK, rotRK, rotLA, rotRA,
				rotLF, rotRF, rotLH, rotRH;
	private void RefreshBodyObject()
	{
		// Print orientations
		if( rotations == null )
			rotations = GameObject.Find("Rotations");
		if (rotSpine == null) {
			rotSpine = (GameObject)GameObject.Instantiate (rotations);
			rotRE = (GameObject)GameObject.Instantiate (rotations);
			rotLE = (GameObject)GameObject.Instantiate (rotations);
			rotLW = (GameObject)GameObject.Instantiate (rotations);
			rotRW = (GameObject)GameObject.Instantiate (rotations);
			rotLK = (GameObject)GameObject.Instantiate (rotations);
			rotRK = (GameObject)GameObject.Instantiate (rotations);
			rotLA = (GameObject)GameObject.Instantiate (rotations);
			rotRA = (GameObject)GameObject.Instantiate (rotations);
			rotLF = (GameObject)GameObject.Instantiate (rotations);
			rotRF = (GameObject)GameObject.Instantiate (rotations);
			rotLS = (GameObject)GameObject.Instantiate (rotations);
			rotRS = (GameObject)GameObject.Instantiate (rotations);
			rotRH = (GameObject)GameObject.Instantiate (rotations);
			rotLH = (GameObject)GameObject.Instantiate (rotations);
		}
		
		foreach (Windows.Kinect.JointType jt in fusedJoints) {
			Vector3 sourceJoint = jointPositions [jt];
			Vector3? targetJoint = null;
			
			// Check NaN problem from Bluetooth
			if( double.IsNaN( sourceJoint.x ) ) {
				print ( "--- " + jt.ToString() + " is Nan." );
				continue;
			}
			
			if (_BoneMap.ContainsKey (jt)) {
				targetJoint = jointPositions [_BoneMap [jt]];
			}
			
			Transform jointObj = bodyObject.transform.FindChild (jt.ToString ());
			jointObj.localPosition = GetVector3FromJoint (sourceJoint) + globalOffset; 
			
			LineRenderer lr = jointObj.GetComponent<LineRenderer> ();
			if (targetJoint.HasValue) {
				lr.SetPosition (0, jointObj.localPosition);
				lr.SetPosition (1, GetVector3FromJoint (targetJoint.Value) + globalOffset);
				lr.SetColors (color1, color2);
			} else {
				lr.enabled = false;
			}
			
			// Rotations
			if (true/*rotSpine != null*/) {
				if( jt == Windows.Kinect.JointType.SpineMid ) {
					rotSpine.transform.position = jointObj.localPosition;
					rotSpine.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.ShoulderRight ) {
					rotRS.transform.position = jointObj.localPosition;
					rotRS.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.ElbowRight ) {
					rotRE.transform.position = jointObj.localPosition;
					rotRE.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.ShoulderLeft ) {
					rotLS.transform.position = jointObj.localPosition;
					rotLS.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.ElbowLeft ) {
					rotLE.transform.position = jointObj.localPosition;
					rotLE.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.WristLeft ) {
					rotLW.transform.position = jointObj.localPosition;
					rotLW.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.WristRight ) {
					rotRW.transform.position = jointObj.localPosition;
					rotRW.transform.rotation = jointOrientations[ jt ];
				}
				else
				/*
				if( jt == Windows.Kinect.JointType.HandRight ) {
					rotRH.transform.position = jointObj.localPosition;
					rotRH.transform.rotation = jointOrientations[ jt ];
				}			
				else
				if( jt == Windows.Kinect.JointType.HandLeft ) {
					rotLH.transform.position = jointObj.localPosition;
					rotLH.transform.rotation = jointOrientations[ jt ];
				}
				else
				*/
				if( jt == Windows.Kinect.JointType.KneeRight ) {
					rotRK.transform.position = jointObj.localPosition;
					rotRK.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.KneeLeft ) {
					rotLK.transform.position = jointObj.localPosition;
					rotLK.transform.rotation = jointOrientations[ jt ];
				}
				if( jt == Windows.Kinect.JointType.HipLeft ) {
					rotLH.transform.position = jointObj.localPosition;
					rotLH.transform.rotation = jointOrientations[ jt ];
				}
				else
				if( jt == Windows.Kinect.JointType.HipRight ) {
					rotRH.transform.position = jointObj.localPosition;
					rotRH.transform.rotation = jointOrientations[ jt ];
				}
				if( jt == Windows.Kinect.JointType.AnkleRight ) {
					rotRA.transform.position = jointObj.localPosition;
					rotRA.transform.rotation = jointOrientations[ jt ];
				}			
				else
				if( jt == Windows.Kinect.JointType.AnkleLeft ) {
					rotLA.transform.position = jointObj.localPosition;
					rotLA.transform.rotation = jointOrientations[ jt ];
				}
				/*
				else
				if( jt == Windows.Kinect.JointType.FootRight ) {
					rotRF.transform.position = jointObj.localPosition;
					rotRF.transform.rotation = jointOrientations[ jt ];
				}			
				else
				if( jt == Windows.Kinect.JointType.FootLeft ) {
					rotLF.transform.position = jointObj.localPosition;
					rotLF.transform.rotation = jointOrientations[ jt ];
				}*/
			}
		}
		
		/*
		// Cosmetic purpose
		LineRenderer lrtemp = bodyObject.transform.FindChild (JointType.Neck.ToString ()).GetComponent<LineRenderer> ();
		lrtemp.SetPosition (0, bodyObject.transform.FindChild (JointType.ShoulderLeft.ToString ()).localPosition);
		lrtemp.SetPosition (1, bodyObject.transform.FindChild (JointType.ShoulderRight.ToString ()).localPosition);
		lrtemp.SetColors (color1, color2);

		lrtemp = bodyObject.transform.FindChild (JointType.SpineBase.ToString ()).GetComponent<LineRenderer> ();
		lrtemp.SetPosition (0, bodyObject.transform.FindChild (JointType.HipLeft.ToString ()).localPosition);
		lrtemp.SetPosition (1, bodyObject.transform.FindChild (JointType.HipRight.ToString ()).localPosition);
		lrtemp.SetColors (color1, color2);
		 */
	}

	private Vector3 GetVector3FromJoint(Vector3 joint)
	{
		return new Vector3(joint.x * globalScale, joint.y * globalScale, joint.z * globalScale);
	}
}