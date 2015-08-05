using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public GameObject BodySourceManager;
	public ulong currentId = 0;
	public GameObject rotations;

	private Vector3 globalOffset = new Vector3( 5, -15, -15 );
	private float globalScale = 25;

	// Standalone implementation
	//private Vector3 globalOffset = new Vector3(5, -5, -15);

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
	private Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType> _BoneMap = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.JointType>()
    {
		{ Windows.Kinect.JointType.FootLeft, Windows.Kinect.JointType.AnkleLeft },
        { Windows.Kinect.JointType.AnkleLeft, Windows.Kinect.JointType.KneeLeft },
        { Windows.Kinect.JointType.KneeLeft, Windows.Kinect.JointType.HipLeft },
        { Windows.Kinect.JointType.HipLeft, Windows.Kinect.JointType.SpineBase },
        
        { Windows.Kinect.JointType.FootRight, Windows.Kinect.JointType.AnkleRight },
        { Windows.Kinect.JointType.AnkleRight, Windows.Kinect.JointType.KneeRight },
        { Windows.Kinect.JointType.KneeRight, Windows.Kinect.JointType.HipRight },
        { Windows.Kinect.JointType.HipRight, Windows.Kinect.JointType.SpineBase },
        
        { Windows.Kinect.JointType.HandTipLeft, Windows.Kinect.JointType.HandLeft },
        { Windows.Kinect.JointType.ThumbLeft, Windows.Kinect.JointType.HandLeft },
        { Windows.Kinect.JointType.HandLeft, Windows.Kinect.JointType.WristLeft },
        { Windows.Kinect.JointType.WristLeft, Windows.Kinect.JointType.ElbowLeft },
        { Windows.Kinect.JointType.ElbowLeft, Windows.Kinect.JointType.ShoulderLeft },
        { Windows.Kinect.JointType.ShoulderLeft, Windows.Kinect.JointType.SpineShoulder },
        
        { Windows.Kinect.JointType.HandTipRight, Windows.Kinect.JointType.HandRight },
        { Windows.Kinect.JointType.ThumbRight, Windows.Kinect.JointType.HandRight },
        { Windows.Kinect.JointType.HandRight, Windows.Kinect.JointType.WristRight },
        { Windows.Kinect.JointType.WristRight, Windows.Kinect.JointType.ElbowRight },
        { Windows.Kinect.JointType.ElbowRight, Windows.Kinect.JointType.ShoulderRight },
        { Windows.Kinect.JointType.ShoulderRight, Windows.Kinect.JointType.SpineShoulder },
        
		{ Windows.Kinect.JointType.SpineBase, Windows.Kinect.JointType.SpineMid },
		{ Windows.Kinect.JointType.SpineMid, Windows.Kinect.JointType.SpineShoulder },
		{ Windows.Kinect.JointType.SpineShoulder, Windows.Kinect.JointType.Neck },
		{ Windows.Kinect.JointType.Neck, Windows.Kinect.JointType.Head },
    };

	void Start()
	{
	}

    void Update () 
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
		Windows.Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
			        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            }
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
			if (body != null) 
			{
				if (body.IsTracked) 
				{
					if (!_Bodies.ContainsKey (body.TrackingId)) 
					{
						_Bodies [body.TrackingId] = CreateBodyObject (body.TrackingId);
					}
											
					RefreshBodyObject (body, _Bodies [body.TrackingId]);
					currentId = body.TrackingId;
				}
			}
        }
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
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
        }
        
        return body;
    }
  
	// Don't need to display this rotation
	private List<JointType> noDisplayRot = new List<JointType> ()
	{
		JointType.Head,
		JointType.Neck,
		//JointType.ShoulderLeft,
		//JointType.ShoulderRight,
		JointType.FootLeft,
		JointType.FootRight,
		JointType.HandLeft,
		JointType.HandRight,
		JointType.ThumbLeft,
		JointType.ThumbRight,
		JointType.HandTipLeft,
		JointType.HandTipRight,
		//JointType.HipLeft,
		//JointType.HipRight,
		JointType.SpineBase,
		JointType.SpineShoulder
	};

	private Dictionary<Windows.Kinect.JointType, GameObject> rot = new Dictionary<JointType, GameObject>();
	private int rotinit = 0;
	private void RefreshBodyObject(Windows.Kinect.Body body, GameObject bodyObject)
    {
		// Print orientations
		if( rotations == null )
			rotations = GameObject.Find("Rotations");
		if (rotinit == 0) {
			for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++)
				rot.Add( jt, (GameObject)GameObject.Instantiate (rotations) );
			rotinit = 1;
		}

		for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++)
        {
			Windows.Kinect.Joint sourceJoint = body.Joints[jt];
			Windows.Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.FindChild(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint) + globalOffset; 
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value) + globalOffset);

                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            else
            {
                lr.enabled = false;
            }

			// Rotations
			if( !noDisplayRot.Contains( jt ) ) {
				rot[ jt ].transform.position = jointObj.localPosition;
				rot[ jt ].transform.rotation = new Quaternion( body.JointOrientations[jt].Orientation.X, 
			                                                   body.JointOrientations[jt].Orientation.Y,
			                                                   body.JointOrientations[jt].Orientation.Z,
			                                                   body.JointOrientations[jt].Orientation.W );
			}
        }
    }
    
	private static Color GetColorForState(Windows.Kinect.TrackingState state)
    {
        switch (state)
        {
		case Windows.Kinect.TrackingState.Tracked:
            return Color.green;

		case Windows.Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
    {
		return new Vector3(joint.Position.X * globalScale, joint.Position.Y * globalScale, joint.Position.Z * globalScale);
    }
}