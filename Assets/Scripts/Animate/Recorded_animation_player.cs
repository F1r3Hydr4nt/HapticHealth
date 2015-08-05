using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Kinect2.IO;
using System.Runtime.InteropServices;
using Windows.Kinect;


public class Recorded_animation_player : MonoBehaviour
{

    /***
     * Description:
     * Animation player for reference and recorded trial
     * Version: 0.1
     * Autor:
     * Yvain Tisserand - MIRALab
     *****/

    private Animation_data _anim;
    static private Skeleton _skel;

    public bool mirroredMovement = true;

    public Transform Root;
    /*
    protected readonly Dictionary<int, KinectInterop.JointType> boneIndex2JointMap = new Dictionary<int, KinectInterop.JointType>
	{
		{0, KinectInterop.JointType.SpineBase},
		{1, KinectInterop.JointType.SpineMid},
		{20, KinectInterop.JointType.SpineShoulder},
		{2, KinectInterop.JointType.Neck},
		{3, KinectInterop.JointType.Head},
		
		{4, KinectInterop.JointType.ShoulderLeft},
		{5, KinectInterop.JointType.ElbowLeft},
		{6, KinectInterop.JointType.WristLeft},
		{7, KinectInterop.JointType.HandLeft},
		
		{21, KinectInterop.JointType.HandTipLeft},
		{22, KinectInterop.JointType.ThumbLeft},
		
		{8, KinectInterop.JointType.ShoulderRight},
		{9, KinectInterop.JointType.ElbowRight},
		{10, KinectInterop.JointType.WristRight},
		{11, KinectInterop.JointType.HandRight},
		
		{23, KinectInterop.JointType.HandTipRight},
		{24, KinectInterop.JointType.ThumbRight},
		
		{12, KinectInterop.JointType.HipLeft},
		{13, KinectInterop.JointType.KneeLeft},
		{14, KinectInterop.JointType.AnkleLeft},
		{15, KinectInterop.JointType.FootLeft},
		
		{16, KinectInterop.JointType.HipRight},
		{17, KinectInterop.JointType.KneeRight},
		{18, KinectInterop.JointType.AnkleRight},
		{19, KinectInterop.JointType.FootRight},
	};

    protected readonly Dictionary<int, List<KinectInterop.JointType>> specIndex2JointMap = new Dictionary<int, List<KinectInterop.JointType>>
	{
		{25, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.SpineShoulder} },
		{26, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.SpineShoulder} },
	};

    protected readonly Dictionary<int, KinectInterop.JointType> boneIndex2MirrorJointMap = new Dictionary<int, KinectInterop.JointType>
	{
		{0, KinectInterop.JointType.SpineBase},
		{1, KinectInterop.JointType.SpineMid},
		{20, KinectInterop.JointType.SpineShoulder},
		{2, KinectInterop.JointType.Neck},
		{3, KinectInterop.JointType.Head},
		
		{4, KinectInterop.JointType.ShoulderLeft},
		{5, KinectInterop.JointType.ElbowLeft},
		{6, KinectInterop.JointType.WristLeft},
		{7, KinectInterop.JointType.HandLeft},
		
		{21, KinectInterop.JointType.HandTipLeft},
		{22, KinectInterop.JointType.ThumbLeft},
		
		{8, KinectInterop.JointType.ShoulderRight},
		{9, KinectInterop.JointType.ElbowRight},
		{10, KinectInterop.JointType.WristRight},
		{11, KinectInterop.JointType.HandRight},
		
		{23, KinectInterop.JointType.HandTipRight},
		{24, KinectInterop.JointType.ThumbRight},
		
		{12, KinectInterop.JointType.HipLeft},
		{13, KinectInterop.JointType.KneeLeft},
		{14, KinectInterop.JointType.AnkleLeft},
		{15, KinectInterop.JointType.FootLeft},
		
		{16, KinectInterop.JointType.HipRight},
		{17, KinectInterop.JointType.KneeRight},
		{18, KinectInterop.JointType.AnkleRight},
		{19, KinectInterop.JointType.FootRight},
	};

    protected readonly Dictionary<int, List<KinectInterop.JointType>> specIndex2MirrorJointMap = new Dictionary<int, List<KinectInterop.JointType>>
	{
		{25, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.SpineShoulder} },
		{26, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.SpineShoulder} },
	};

    private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
	{
		{0, HumanBodyBones.Hips},
		{1, HumanBodyBones.Spine},
//        {2, HumanBodyBones.Chest},
		{2, HumanBodyBones.Neck},
		{3, HumanBodyBones.Head},
		
        {4, HumanBodyBones.LeftShoulder},
		{5, HumanBodyBones.LeftUpperArm},
		{6, HumanBodyBones.LeftLowerArm},
		{7, HumanBodyBones.LeftHand},

		{8, HumanBodyBones.RightShoulder},
		{9, HumanBodyBones.RightUpperArm},
		{10, HumanBodyBones.RightLowerArm},
		{11, HumanBodyBones.RightHand},
		
		
		{12, HumanBodyBones.LeftUpperLeg},
		{13, HumanBodyBones.LeftLowerLeg},
		{14, HumanBodyBones.LeftFoot},
		{15, HumanBodyBones.LeftToes},
		
		{16, HumanBodyBones.RightUpperLeg},
		{17, HumanBodyBones.RightLowerLeg},
		{18, HumanBodyBones.RightFoot},
		{19, HumanBodyBones.RightToes},
		
		
		
        {20, HumanBodyBones.Chest},

        {21, HumanBodyBones.LeftIndexProximal},
		{22, HumanBodyBones.LeftThumbProximal},

        {23, HumanBodyBones.RightIndexProximal},
		{24, HumanBodyBones.RightThumbProximal},

	};
	/*
     * {0, KinectInterop.JointType.SpineBase},
		{1, KinectInterop.JointType.SpineMid},
		{20, KinectInterop.JointType.SpineShoulder},
		{2, KinectInterop.JointType.Neck},
		{3, KinectInterop.JointType.Head},
		
		{4, KinectInterop.JointType.ShoulderLeft},
		{5, KinectInterop.JointType.ElbowLeft},
		{6, KinectInterop.JointType.WristLeft},
		{7, KinectInterop.JointType.HandLeft},
		
		{21, KinectInterop.JointType.HandTipLeft},
		{22, KinectInterop.JointType.ThumbLeft},
		
		{8, KinectInterop.JointType.ShoulderRight},
		{9, KinectInterop.JointType.ElbowRight},
		{10, KinectInterop.JointType.WristRight},
		{11, KinectInterop.JointType.HandRight},
		
		{23, KinectInterop.JointType.HandTipRight},
		{24, KinectInterop.JointType.ThumbRight},
		
		{12, KinectInterop.JointType.HipLeft},
		{13, KinectInterop.JointType.KneeLeft},
		{14, KinectInterop.JointType.AnkleLeft},
		{15, KinectInterop.JointType.FootLeft},
		
		{16, KinectInterop.JointType.HipRight},
		{17, KinectInterop.JointType.KneeRight},
		{18, KinectInterop.JointType.AnkleRight},
		{19, KinectInterop.JointType.FootRight},*/

    private readonly Dictionary<int, HumanBodyBones> boneIndex2MecanimMap = new Dictionary<int, HumanBodyBones>
	{
		{0, HumanBodyBones.Hips},
		{1, HumanBodyBones.Spine},
//        {2, HumanBodyBones.Chest},
		{3, HumanBodyBones.Neck},
		{4, HumanBodyBones.Head},
		
		{5, HumanBodyBones.LeftUpperArm},
		{6, HumanBodyBones.LeftLowerArm},
		{7, HumanBodyBones.LeftHand},
		{8, HumanBodyBones.LeftIndexProximal},
		{9, HumanBodyBones.LeftIndexIntermediate},
		{10, HumanBodyBones.LeftThumbProximal},
		
		{11, HumanBodyBones.RightUpperArm},
		{12, HumanBodyBones.RightLowerArm},
		{13, HumanBodyBones.RightHand},
		{14, HumanBodyBones.RightIndexProximal},
		{15, HumanBodyBones.RightIndexIntermediate},
		{16, HumanBodyBones.RightThumbProximal},
		
		{17, HumanBodyBones.LeftUpperLeg},
		{18, HumanBodyBones.LeftLowerLeg},
		{19, HumanBodyBones.LeftFoot},
		{20, HumanBodyBones.LeftToes},
		
		{21, HumanBodyBones.RightUpperLeg},
		{22, HumanBodyBones.RightLowerLeg},
		{23, HumanBodyBones.RightFoot},
		{24, HumanBodyBones.RightToes},
		
		{25, HumanBodyBones.LeftShoulder},
		{26, HumanBodyBones.RightShoulder},
	};

    protected readonly Dictionary<int, KinectInterop.JointType> boneIndex2JointMap = new Dictionary<int, KinectInterop.JointType>
	{
		{0, KinectInterop.JointType.SpineBase},
		{1, KinectInterop.JointType.SpineMid},
		{2, KinectInterop.JointType.SpineShoulder},
		{3, KinectInterop.JointType.Neck},
		{4, KinectInterop.JointType.Head},
		
		{5, KinectInterop.JointType.ShoulderLeft},
		{6, KinectInterop.JointType.ElbowLeft},
		{7, KinectInterop.JointType.WristLeft},
		{8, KinectInterop.JointType.HandLeft},
		
		{9, KinectInterop.JointType.HandTipLeft},
		{10, KinectInterop.JointType.ThumbLeft},
		
		{11, KinectInterop.JointType.ShoulderRight},
		{12, KinectInterop.JointType.ElbowRight},
		{13, KinectInterop.JointType.WristRight},
		{14, KinectInterop.JointType.HandRight},
		
		{15, KinectInterop.JointType.HandTipRight},
		{16, KinectInterop.JointType.ThumbRight},
		
		{17, KinectInterop.JointType.HipLeft},
		{18, KinectInterop.JointType.KneeLeft},
		{19, KinectInterop.JointType.AnkleLeft},
		{20, KinectInterop.JointType.FootLeft},
		
		{21, KinectInterop.JointType.HipRight},
		{22, KinectInterop.JointType.KneeRight},
		{23, KinectInterop.JointType.AnkleRight},
		{24, KinectInterop.JointType.FootRight},
	};

    protected readonly Dictionary<int, List<KinectInterop.JointType>> specIndex2JointMap = new Dictionary<int, List<KinectInterop.JointType>>
	{
		{25, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.SpineShoulder} },
		{26, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.SpineShoulder} },
	};

    protected readonly Dictionary<int, KinectInterop.JointType> boneIndex2MirrorJointMap = new Dictionary<int, KinectInterop.JointType>
	{
		{0, KinectInterop.JointType.SpineBase},
		{1, KinectInterop.JointType.SpineMid},
		{2, KinectInterop.JointType.SpineShoulder},
		{3, KinectInterop.JointType.Neck},
		{4, KinectInterop.JointType.Head},
		
		{5, KinectInterop.JointType.ShoulderRight},
		{6, KinectInterop.JointType.ElbowRight},
		{7, KinectInterop.JointType.WristRight},
		{8, KinectInterop.JointType.HandRight},
		
		{9, KinectInterop.JointType.HandTipRight},
		{10, KinectInterop.JointType.ThumbRight},
		
		{11, KinectInterop.JointType.ShoulderLeft},
		{12, KinectInterop.JointType.ElbowLeft},
		{13, KinectInterop.JointType.WristLeft},
		{14, KinectInterop.JointType.HandLeft},
		
		{15, KinectInterop.JointType.HandTipLeft},
		{16, KinectInterop.JointType.ThumbLeft},
		
		{17, KinectInterop.JointType.HipRight},
		{18, KinectInterop.JointType.KneeRight},
		{19, KinectInterop.JointType.AnkleRight},
		{20, KinectInterop.JointType.FootRight},
		
		{21, KinectInterop.JointType.HipLeft},
		{22, KinectInterop.JointType.KneeLeft},
		{23, KinectInterop.JointType.AnkleLeft},
		{24, KinectInterop.JointType.FootLeft},
	};

    protected readonly Dictionary<int, List<KinectInterop.JointType>> specIndex2MirrorJointMap = new Dictionary<int, List<KinectInterop.JointType>>
	{
		{25, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderRight, KinectInterop.JointType.SpineShoulder} },
		{26, new List<KinectInterop.JointType> {KinectInterop.JointType.ShoulderLeft, KinectInterop.JointType.SpineShoulder} },
	};
    //JOINT LIST FOR SKELETON DRAWING
    public Transform Joint_LeftHand;
    public Transform Joint_LeftFoot;
    public Transform Joint_RightHand;
    public Transform Joint_RightFoot;

    public Transform Joint_RightToe;
    public Transform Joint_LeftToe;

    public LineRenderer line_1;
    public LineRenderer line_2;
    public LineRenderer line_3;

    // The body root node
    protected Transform bodyRoot;

    private int motionCounter = 0;
    public Transform[] _bones;
    private Quaternion originalRotation;
    private Quaternion[] initialRotations;
    public GameObject offsetNode;
    // Slerp smooth factor
    public float SmoothFactor = 3.0f;
    List<Quaternion[]> _anim_from_controller;
    // Use this for initialization
    void Start()
    {

        // Holds our _bones for later.
        _bones = new Transform[25];

        // Initial rotations of said _bones.
        initialRotations = new Quaternion[_bones.Length];

        // Map _bones to the points the Kinect tracks.
        //Map_bones();

        bodyData = new KinectInterop.BodyData();
	    bodyData.joint = new KinectInterop.JointData[_bones.Length];
        for (int i = 0; i < _bones.Length; i++)
        {
                bodyData.joint[i] = new KinectInterop.JointData();
                bodyData.joint[i].position = new Vector3();        
                bodyData.joint[i].orientation = new Quaternion();
                bodyData.joint[i].kinectPos = new Vector3();
                bodyData.joint[i].jointType = boneIndex2JointMap[i];
        }

        Debug.Log("init [OK]");

            Map_Bones_auto();

        // Get initial rotations to return to later.
        GetInitialRotations();

        print("init Draw skeleton lines");
        //draw_skeleton();

        print("init Draw skeleton lines OK");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetAnimationData(Animation_data anim)
    {

        _anim = anim;
        motionCounter = 0;
    }

    public void SetAnimationDataRef(Animation_data anim)
    {
        _anim = anim;
        motionCounter = 0;

    }
    public void SetAnimationDataFromController(List<Quaternion[]> anim_from_controller)
    {
        Debug.Log("Recorded controller :" + anim_from_controller.Count);
        _anim_from_controller = anim_from_controller;
        motionCounter = 0;
    }
    

    public void Play()
    {

    }

    public void Pause()
    {


    }

    public void ResetMotion()
    {
        motionCounter = 0;
        ApplyNextMotionFrame();
    }


    public bool ApplyNextMotionFrame()
    {

        // Yvain TODO
        // get skeleton -> skeleton = _anim.GetFrame(motionCounter);
        // get individual joints as float[] -> joint = skeleton[i]
        // where i = 0 to 24 following Kinect 2 joint ids
        // and joint is a float[] => float[ Confidence, PositionX,PositionY,PositionZ,QuaternionX,QuaternionY,QuaternionZ,QuaternionW ]
        // the confidence value is 0 for not tracked , 1 for infered and 2 for tracked

        motionCounter = motionCounter + 1;
        //Debug.Log("\tFrame: " + motionCounter + " bones lenght: " + _bones.Length);
        /*
         * BACKUP OPTION
        if (motionCounter >= _anim_from_controller.Count)
        {
            return false;
        }

        for (var boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            if (_bones[boneIndex] != null){
                _bones[boneIndex].rotation = Quaternion.Slerp(_bones[boneIndex].rotation, _anim_from_controller[motionCounter][boneIndex] * initialRotations[boneIndex], 3.0f * Time.deltaTime);
                //_bones[boneIndex].rotation = _anim_from_controller[motionCounter][boneIndex] *initialRotations[boneIndex];
                               
            }

        }
        draw_skeleton();
         */
        
        
        if (motionCounter >= _anim.getLenght())
        {
            return false;
        }
        _skel = _anim.GetFrame(motionCounter);
        bool flipJoint = !mirroredMovement;
        
         float[] skeleton_data;
               for (var boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
                {
                    if (_bones[boneIndex] != null)
                    {
                        skeleton_data = _skel[boneIndex];
                        if (skeleton_data[0] == 2.0f) //Joint tracked
                        {
                            Debug.Log("\t\tBone [" + boneIndex + "] : Quaternion(" + skeleton_data[4] + "," + skeleton_data[5] + "," + skeleton_data[6] + "," + skeleton_data[7] + ")");
                            //_bones[i].rotation = new Quaternion(skeleton_data[5], skeleton_data[6], skeleton_data[7], skeleton_data[8]);
                            TransformSkeletonBone(boneIndex, flipJoint, new Quaternion(skeleton_data[4], skeleton_data[5], skeleton_data[6], skeleton_data[7]));
                        }
                    }
                    else
                    {
                        Debug.Log("\t\tBone [" + boneIndex + "] not set");
                    }
                }

        /*
        ProcessSkeleton(_skel);
        
        for (var boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            //TransformBone(bodyData.joint[boneIndex], boneIndex, !mirroredMovement);
            
            if (!_bones[boneIndex])
                continue;

            if (boneIndex2JointMap.ContainsKey(boneIndex))
            {
                KinectInterop.JointType joint = !mirroredMovement ? boneIndex2JointMap[boneIndex] : boneIndex2MirrorJointMap[boneIndex];
                TransformBone(bodyData.joint[boneIndex], boneIndex, !mirroredMovement);
            }
            /*else if (boneIndex2JointMap.ContainsKey(boneIndex))
            {
                // special bones (clavicles)
                List<KinectInterop.JointType> alJoints = !mirroredMovement ? specIndex2JointMap[boneIndex] : specIndex2MirrorJointMap[boneIndex];
                
                if (alJoints.Count >= 2)
                {
                    //Debug.Log(alJoints[0].ToString());
                    Vector3 baseDir = alJoints[0].ToString().EndsWith("Left") ? Vector3.left : Vector3.right;
                    TransformSpecialBone(bodyData.joint[(int)alJoints[0]], bodyData.joint[(int)alJoints[1]], boneIndex, baseDir, !mirroredMovement);
                }
            }*/
        //}

        //        float[] skeleton_data;
        //        for (var boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        //        {
        //            if (_bones[boneIndex] != null)
        //            {
        //                skeleton_data = _skel[boneIndex];
        //                if (skeleton_data[0] == 2.0f) //Joint tracked
        //                {
        //                    Debug.Log("\t\tBone [" + boneIndex + "] : Quaternion(" + skeleton_data[4] + "," + skeleton_data[5] + "," + skeleton_data[6] + "," + skeleton_data[7] + ")");
        //                    //_bones[i].rotation = new Quaternion(skeleton_data[5], skeleton_data[6], skeleton_data[7], skeleton_data[8]);
        //                    TransformSkeletonBone(boneIndex, flipJoint, new Quaternion(skeleton_data[4], skeleton_data[5], skeleton_data[6], skeleton_data[7]));
        //                }
        //            }
        //            else
        //            {
        //                Debug.Log("\t\tBone [" + boneIndex + "] not set");
        //            }
        //        }

    
        //draw_skeleton();

        return true;
    }


    // Apply the rotations tracked by kinect to the joints.
    protected void TransformBone(KinectInterop.JointData jointData, int boneIndex, bool flip)
    {
        Transform boneTransform = _bones[boneIndex];
        if (boneTransform == null)
            return;

        int iJoint = (int)jointData.jointType;
        if (iJoint < 0 || jointData.trackingState == KinectInterop.TrackingState.NotTracked)
            return;

        // Get Kinect joint orientation
        Quaternion jointRotation = Quaternion.identity;
        if (flip)
            jointRotation = jointData.normalRotation;
        else
            jointRotation = jointData.mirroredRotation;
        if (jointRotation == Quaternion.identity)
            return;

        // Smoothly transition to the new rotation
        Quaternion newRotation = Kinect2AvatarRot(jointRotation, boneIndex);

        //		if(AvatarController.smoothFactor != 0f)
        //			boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, AvatarController.smoothFactor * Time.deltaTime);
        //		else
        boneTransform.rotation = newRotation;
        _bones[boneIndex].rotation = newRotation;

    }

    // Apply the rotations tracked by kinect to a special joint
    protected void TransformSpecialBone(KinectInterop.JointData jointData, KinectInterop.JointData jointParentData, int boneIndex, Vector3 baseDir, bool flip)
    {
        Transform boneTransform = _bones[boneIndex];
        if (boneTransform == null)
            return;

        if (jointData.trackingState == KinectInterop.TrackingState.NotTracked ||
           jointParentData.trackingState == KinectInterop.TrackingState.NotTracked)
        {
            return;
        }

        Vector3 jointDir = GetJointDirection(jointData.direction, false, true);
        Quaternion jointRotation = Quaternion.FromToRotation(baseDir, jointDir);

        if (!flip)
        {
            Vector3 mirroredAngles = jointRotation.eulerAngles;
            mirroredAngles.y = -mirroredAngles.y;
            mirroredAngles.z = -mirroredAngles.z;

            jointRotation = Quaternion.Euler(mirroredAngles);
        }

        if (jointRotation != Quaternion.identity)
        {
            // Smoothly transition to the new rotation
            Quaternion newRotation = Kinect2AvatarRot(jointRotation, boneIndex);

            //			if(AvatarController.smoothFactor != 0f)
            //				boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, AvatarController.smoothFactor * Time.deltaTime);
            //			else
            boneTransform.rotation = newRotation;
        }

    }

    // Converts kinect joint rotation to avatar joint rotation, depending on joint initial rotation and offset rotation
    protected Quaternion Kinect2AvatarRot(Quaternion jointRotation, int boneIndex)
    {
        Quaternion newRotation = jointRotation * initialRotations[boneIndex];
        /*
        if (offsetNode != null)
        {
            Vector3 totalRotation = newRotation.eulerAngles + offsetNode.transform.rotation.eulerAngles;
            newRotation = Quaternion.Euler(totalRotation);
        }
        */
        return newRotation;
    }

    // returns the joint direction of the specified user, relative to the parent joint
    public Vector3 GetJointDirection(Vector3 jointDir, bool flipX, bool flipZ)
    {
        if (flipX)
            jointDir.x = -jointDir.x;

        if (flipZ)
            jointDir.z = -jointDir.z;

        return jointDir;
    }

    public bool allowHandRotations = false; // This value is inherited but not really used

    private KinectInterop.BodyData bodyData;

    // float[] => float[ Confidence, PositionX,PositionY,PositionZ,QuaternionX,QuaternionY,QuaternionZ,QuaternionW ]
    private void ProcessSkeleton(Skeleton skeleton)
    {
        //string debugText = String.Empty;

        // (goe): How to initialize BodyData???

        // convert Kinect positions to world positions
        // bodyFrame.bodyData[i].position = bodyPos;

        // Translate skeleton to body data
        
        for (int i = 0; i < _bones.Length; ++i)
        {
            // Get the skeleton info
            float[] boneData = skeleton[i];
            

            // Set the status
            /*if (boneData[0] != 2)
                bodyData.joint[i].trackingState = KinectInterop.TrackingState.NotTracked;
            else*/
                bodyData.joint[i].trackingState = KinectInterop.TrackingState.Tracked;

            // Add position and orientations
            bodyData.joint[i].position = new Vector3(boneData[1], boneData[2], boneData[3]);
            bodyData.joint[i].orientation = new Quaternion(boneData[4], boneData[5], boneData[6], boneData[7]);
            // Add joint type id
            bodyData.joint[i].jointType = (KinectInterop.JointType)i; //Supposed that the id in the list and the jointType are the same value
        }
        // set the body general location and position
        bodyData.position = bodyData.joint[0].position;
        bodyData.orientation = bodyData.joint[0].orientation;

        // Begin the location and orientation processation
        // process special cases
        if (bodyData.joint[(int)KinectInterop.JointType.HipLeft].trackingState == KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.SpineBase].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.HipRight].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            bodyData.joint[(int)KinectInterop.JointType.HipLeft].trackingState = KinectInterop.TrackingState.Inferred;
            bodyData.joint[(int)KinectInterop.JointType.HipLeft].position = bodyData.joint[(int)KinectInterop.JointType.SpineBase].position +
                (bodyData.joint[(int)KinectInterop.JointType.SpineBase].position - bodyData.joint[(int)KinectInterop.JointType.HipRight].position);
            bodyData.joint[(int)KinectInterop.JointType.HipLeft].direction = bodyData.joint[(int)KinectInterop.JointType.HipLeft].position -
                bodyData.joint[(int)KinectInterop.JointType.SpineBase].position;
        }

        if (bodyData.joint[(int)KinectInterop.JointType.HipRight].trackingState == KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.SpineBase].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.HipLeft].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            bodyData.joint[(int)KinectInterop.JointType.HipRight].trackingState = KinectInterop.TrackingState.Inferred;

            bodyData.joint[(int)KinectInterop.JointType.HipRight].position = bodyData.joint[(int)KinectInterop.JointType.SpineBase].position +
                (bodyData.joint[(int)KinectInterop.JointType.SpineBase].position - bodyData.joint[(int)KinectInterop.JointType.HipLeft].position);
            bodyData.joint[(int)KinectInterop.JointType.HipRight].direction = bodyData.joint[(int)KinectInterop.JointType.HipRight].position -
                bodyData.joint[(int)KinectInterop.JointType.SpineBase].position;
        }

        if ((bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].trackingState == KinectInterop.TrackingState.NotTracked &&
            bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].trackingState != KinectInterop.TrackingState.NotTracked &&
            bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].trackingState != KinectInterop.TrackingState.NotTracked))
        {
            bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].trackingState = KinectInterop.TrackingState.Inferred;

            bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].position = bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].position +
                (bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].position - bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].position);
            bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].direction = bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].position -
                bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].position;
        }

        if ((bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].trackingState == KinectInterop.TrackingState.NotTracked &&
            bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].trackingState != KinectInterop.TrackingState.NotTracked &&
            bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].trackingState != KinectInterop.TrackingState.NotTracked))
        {
            bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].trackingState = KinectInterop.TrackingState.Inferred;

            bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].position = bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].position +
                (bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].position - bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].position);
            bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].direction = bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].position -
                bodyData.joint[(int)KinectInterop.JointType.SpineShoulder].position;
        }

        // calculate special directions
        if (bodyData.joint[(int)KinectInterop.JointType.HipLeft].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.HipRight].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 posRHip = bodyData.joint[(int)KinectInterop.JointType.HipRight].position;
            Vector3 posLHip = bodyData.joint[(int)KinectInterop.JointType.HipLeft].position;

            bodyData.hipsDirection = posRHip - posLHip;
            bodyData.hipsDirection -= Vector3.Project(bodyData.hipsDirection, Vector3.up);
        }

        if (bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 posRShoulder = bodyData.joint[(int)KinectInterop.JointType.ShoulderRight].position;
            Vector3 posLShoulder = bodyData.joint[(int)KinectInterop.JointType.ShoulderLeft].position;

            bodyData.shouldersDirection = posRShoulder - posLShoulder;
            bodyData.shouldersDirection -= Vector3.Project(bodyData.shouldersDirection, Vector3.up);

            Vector3 shouldersDir = bodyData.shouldersDirection;
            shouldersDir.z = -shouldersDir.z;

            Quaternion turnRot = Quaternion.FromToRotation(Vector3.right, shouldersDir);
            bodyData.bodyTurnAngle = turnRot.eulerAngles.y;
        }

        if (bodyData.joint[(int)KinectInterop.JointType.ElbowLeft].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.WristLeft].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 pos1 = bodyData.joint[(int)KinectInterop.JointType.ElbowLeft].position;
            Vector3 pos2 = bodyData.joint[(int)KinectInterop.JointType.WristLeft].position;

            bodyData.leftArmDirection = pos2 - pos1;
        }

        if (allowHandRotations && bodyData.leftArmDirection != Vector3.zero &&
           bodyData.joint[(int)KinectInterop.JointType.WristLeft].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.ThumbLeft].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 pos1 = bodyData.joint[(int)KinectInterop.JointType.WristLeft].position;
            Vector3 pos2 = bodyData.joint[(int)KinectInterop.JointType.ThumbLeft].position;

            Vector3 armDir = bodyData.leftArmDirection;
            armDir.z = -armDir.z;

            bodyData.leftThumbDirection = pos2 - pos1;
            bodyData.leftThumbDirection.z = -bodyData.leftThumbDirection.z;
            bodyData.leftThumbDirection -= Vector3.Project(bodyData.leftThumbDirection, armDir);

            bodyData.leftThumbForward = Quaternion.AngleAxis(bodyData.bodyTurnAngle, Vector3.up) * Vector3.forward;
            bodyData.leftThumbForward -= Vector3.Project(bodyData.leftThumbForward, armDir);

            if (bodyData.leftThumbForward.sqrMagnitude < 0.01f)
            {
                bodyData.leftThumbForward = Vector3.zero;
            }
        }
        else
        {
            bodyData.leftThumbDirection = Vector3.zero;
            bodyData.leftThumbForward = Vector3.zero;
        }

        if (bodyData.joint[(int)KinectInterop.JointType.ElbowRight].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.WristRight].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 pos1 = bodyData.joint[(int)KinectInterop.JointType.ElbowRight].position;
            Vector3 pos2 = bodyData.joint[(int)KinectInterop.JointType.WristRight].position;

            bodyData.rightArmDirection = pos2 - pos1;
        }

        if (allowHandRotations && bodyData.rightArmDirection != Vector3.zero &&
           bodyData.joint[(int)KinectInterop.JointType.WristRight].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.ThumbRight].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 pos1 = bodyData.joint[(int)KinectInterop.JointType.WristRight].position;
            Vector3 pos2 = bodyData.joint[(int)KinectInterop.JointType.ThumbRight].position;

            Vector3 armDir = bodyData.rightArmDirection;
            armDir.z = -armDir.z;

            bodyData.rightThumbDirection = pos2 - pos1;
            bodyData.rightThumbDirection.z = -bodyData.rightThumbDirection.z;
            bodyData.rightThumbDirection -= Vector3.Project(bodyData.rightThumbDirection, armDir);

            bodyData.rightThumbForward = Quaternion.AngleAxis(bodyData.bodyTurnAngle, Vector3.up) * Vector3.forward;
            bodyData.rightThumbForward -= Vector3.Project(bodyData.rightThumbForward, armDir);

            if (bodyData.rightThumbForward.sqrMagnitude < 0.01f)
            {
                bodyData.rightThumbForward = Vector3.zero;
            }
        }
        else
        {
            bodyData.rightThumbDirection = Vector3.zero;
            bodyData.rightThumbForward = Vector3.zero;
        }

        if (bodyData.joint[(int)KinectInterop.JointType.KneeLeft].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.AnkleLeft].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.FootLeft].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 vFootProjected = Vector3.Project(bodyData.joint[(int)KinectInterop.JointType.FootLeft].direction, bodyData.joint[(int)KinectInterop.JointType.AnkleLeft].direction);

            bodyData.joint[(int)KinectInterop.JointType.AnkleLeft].kinectPos += vFootProjected;
            bodyData.joint[(int)KinectInterop.JointType.AnkleLeft].position += vFootProjected;
            bodyData.joint[(int)KinectInterop.JointType.FootLeft].direction -= vFootProjected;
        }

        if (bodyData.joint[(int)KinectInterop.JointType.KneeRight].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.AnkleRight].trackingState != KinectInterop.TrackingState.NotTracked &&
           bodyData.joint[(int)KinectInterop.JointType.FootRight].trackingState != KinectInterop.TrackingState.NotTracked)
        {
            Vector3 vFootProjected = Vector3.Project(bodyData.joint[(int)KinectInterop.JointType.FootRight].direction, bodyData.joint[(int)KinectInterop.JointType.AnkleRight].direction);

            bodyData.joint[(int)KinectInterop.JointType.AnkleRight].kinectPos += vFootProjected;
            bodyData.joint[(int)KinectInterop.JointType.AnkleRight].position += vFootProjected;
            bodyData.joint[(int)KinectInterop.JointType.FootRight].direction -= vFootProjected;
        }

        // calculate world orientations of the body joints
        CalculateJointOrients();

    }


    // Public Bool to determine whether to use only the tracked joints (and ignore the inferred ones)
    public bool ignoreInferredJoints = true;

    private DepthSensorInterface depthSensorInterface = null; // (goe): I dont know if this is correct 

    // calculates joint orientations of the body joints
    public void CalculateJointOrients()
    {
        if (depthSensorInterface == null)
            depthSensorInterface = new Kinect2Interface(); // (goe): I dont know if this is a correct initialization

        int jointCount = bodyData.joint.Length;

        for (int j = 0; j < jointCount; j++)
        {
            int joint = j;

            KinectInterop.JointData jointData = bodyData.joint[joint];
            bool bJointValid = ignoreInferredJoints ? jointData.trackingState == KinectInterop.TrackingState.Tracked : jointData.trackingState != KinectInterop.TrackingState.NotTracked;

            if (bJointValid)
            {
                int nextJoint = (int)depthSensorInterface.GetNextJoint((KinectInterop.JointType)joint); // (goe): I dont know if this call is posible
                if (nextJoint != joint && nextJoint >= 0 && nextJoint < bodyData.joint.Length)
                {
                    KinectInterop.JointData nextJointData = bodyData.joint[nextJoint];
                    bool bNextJointValid = ignoreInferredJoints ? nextJointData.trackingState == KinectInterop.TrackingState.Tracked : nextJointData.trackingState != KinectInterop.TrackingState.NotTracked;

                    if (bNextJointValid)
                    {
                        Vector3 baseDir = KinectInterop.JointBaseDir[nextJoint];
                        Vector3 jointDir = nextJointData.direction;
                        jointDir.z = -jointDir.z;

                        if ((joint == (int)KinectInterop.JointType.ShoulderLeft) ||
                           (joint == (int)KinectInterop.JointType.ShoulderRight))
                        {
                            float angle = -bodyData.bodyTurnAngle;
                            Vector3 axis = jointDir;
                            Quaternion armTurnRotation = Quaternion.AngleAxis(angle, axis);

                            jointData.normalRotation = armTurnRotation * Quaternion.FromToRotation(baseDir, jointDir);
                        }
                        else if ((joint == (int)KinectInterop.JointType.ElbowLeft) ||
                                (joint == (int)KinectInterop.JointType.WristLeft) ||
                                (joint == (int)KinectInterop.JointType.HandLeft))
                        {
                            bool bRotated = false;

                            if (allowHandRotations && (joint != (int)KinectInterop.JointType.ElbowLeft))
                            {
                                KinectInterop.JointData handData = bodyData.joint[(int)KinectInterop.JointType.HandLeft];
                                KinectInterop.JointData handTipData = bodyData.joint[(int)KinectInterop.JointType.HandTipLeft];
                                KinectInterop.JointData thumbData = bodyData.joint[(int)KinectInterop.JointType.ThumbLeft];

                                if (handData.trackingState != KinectInterop.TrackingState.NotTracked &&
                                   handTipData.trackingState != KinectInterop.TrackingState.NotTracked &&
                                   thumbData.trackingState != KinectInterop.TrackingState.NotTracked)
                                {
                                    Vector3 rightDir = -(handData.direction + handTipData.direction);
                                    rightDir.z = -rightDir.z;

                                    Vector3 fwdDir = thumbData.direction;
                                    fwdDir.z = -fwdDir.z;

                                    if (rightDir.sqrMagnitude >= 0.01f && fwdDir.sqrMagnitude >= 0.01f)
                                    {
                                        Vector3 upDir = Vector3.Cross(fwdDir, rightDir);
                                        fwdDir = Vector3.Cross(rightDir, upDir);

                                        Quaternion handRotation = Quaternion.LookRotation(fwdDir, upDir);
                                        jointData.normalRotation = handRotation;
                                        //bRotated = true;
                                    }
                                }

                                bRotated = true;
                            }

                            if (!bRotated)
                            {
                                Quaternion armTurnRotation = Quaternion.identity;

                                if (bodyData.leftThumbDirection != Vector3.zero &&
                                   bodyData.leftThumbForward != Vector3.zero) // &&
                                //Vector3.Angle(bodyData.leftThumbForward, bodyData.leftThumbDirection) <= 90f)
                                {
                                    armTurnRotation = Quaternion.FromToRotation(bodyData.leftThumbForward, bodyData.leftThumbDirection);
                                }
                                else
                                {
                                    float angle = -bodyData.bodyTurnAngle;
                                    Vector3 axis = jointDir;
                                    armTurnRotation = Quaternion.AngleAxis(angle, axis);
                                }

                                jointData.normalRotation = armTurnRotation * Quaternion.FromToRotation(baseDir, jointDir);
                            }
                        }
                        else if ((joint == (int)KinectInterop.JointType.ElbowRight) ||
                                (joint == (int)KinectInterop.JointType.WristRight) ||
                                (joint == (int)KinectInterop.JointType.HandRight))
                        {
                            bool bRotated = false;

                            if (allowHandRotations && (joint != (int)KinectInterop.JointType.ElbowRight))
                            {
                                KinectInterop.JointData handData = bodyData.joint[(int)KinectInterop.JointType.HandRight];
                                KinectInterop.JointData handTipData = bodyData.joint[(int)KinectInterop.JointType.HandTipRight];
                                KinectInterop.JointData thumbData = bodyData.joint[(int)KinectInterop.JointType.ThumbRight];

                                if (handData.trackingState != KinectInterop.TrackingState.NotTracked &&
                                   handTipData.trackingState != KinectInterop.TrackingState.NotTracked &&
                                   thumbData.trackingState != KinectInterop.TrackingState.NotTracked)
                                {
                                    Vector3 rightDir = handData.direction + handTipData.direction;
                                    rightDir.z = -rightDir.z;

                                    Vector3 fwdDir = thumbData.direction;
                                    fwdDir.z = -fwdDir.z;

                                    if (rightDir.sqrMagnitude >= 0.01f && fwdDir.sqrMagnitude >= 0.01f)
                                    {
                                        Vector3 upDir = Vector3.Cross(fwdDir, rightDir);
                                        fwdDir = Vector3.Cross(rightDir, upDir);

                                        Quaternion handRotation = Quaternion.LookRotation(fwdDir, upDir); // Vector3.up);
                                        jointData.normalRotation = handRotation;
                                        //bRotated = true;
                                    }
                                }

                                bRotated = true;
                            }

                            if (!bRotated)
                            {
                                Quaternion armTurnRotation = Quaternion.identity;

                                if (bodyData.rightThumbDirection != Vector3.zero &&
                                   bodyData.rightThumbForward != Vector3.zero) // &&
                                //Vector3.Angle(bodyData.rightThumbForward, bodyData.rightThumbDirection) <= 90f)
                                {
                                    armTurnRotation = Quaternion.FromToRotation(bodyData.rightThumbForward, bodyData.rightThumbDirection);
                                }
                                else
                                {
                                    float angle = -bodyData.bodyTurnAngle;
                                    Vector3 axis = jointDir;
                                    armTurnRotation = Quaternion.AngleAxis(angle, axis);
                                }

                                jointData.normalRotation = armTurnRotation * Quaternion.FromToRotation(baseDir, jointDir);
                            }
                        }
                        else
                        {
                            jointData.normalRotation = Quaternion.FromToRotation(baseDir, jointDir);
                        }

                        if ((joint == (int)KinectInterop.JointType.SpineBase) || (joint == (int)KinectInterop.JointType.SpineMid) ||
                           (joint == (int)KinectInterop.JointType.SpineShoulder) || (joint == (int)KinectInterop.JointType.Neck) ||
                           (joint == (int)KinectInterop.JointType.HipLeft) || (joint == (int)KinectInterop.JointType.HipRight) ||
                           (joint == (int)KinectInterop.JointType.KneeLeft) || (joint == (int)KinectInterop.JointType.KneeRight) ||
                           (joint == (int)KinectInterop.JointType.AnkleLeft) || (joint == (int)KinectInterop.JointType.AnkleRight))
                        {
                            baseDir = Vector3.right;
                            jointDir = bodyData.shouldersDirection;
                            jointDir.z = -jointDir.z;

                            jointData.normalRotation *= Quaternion.FromToRotation(baseDir, jointDir);
                        }

                        // (goe): This is erased because the stored data has not face tracking data
                        //						if(joint == (int)KinectInterop.JointType.Neck && 
                        //						   sensorData != null && 
                        //						   depthSensorInterface != null)
                        //						{
                        //							if(sensorData.sensorInterface.IsFaceTrackingInitialized() && 
                        //							   sensorData.sensorInterface.IsFaceTracked(bodyData.liTrackingID))
                        //							{
                        //								Quaternion headRotation = Quaternion.identity;
                        //								
                        //								if(sensorData.sensorInterface.GetHeadRotation(bodyData.liTrackingID, ref headRotation))
                        //								{
                        //									Vector3 rotAngles = headRotation.eulerAngles;
                        //									rotAngles.x = -rotAngles.x;
                        //									rotAngles.y = -rotAngles.y;
                        //									
                        //									jointData.normalRotation = Quaternion.Euler(rotAngles);
                        //								}
                        //							}
                        //						}

                        Vector3 mirroredAngles = jointData.normalRotation.eulerAngles;
                        mirroredAngles.y = -mirroredAngles.y;
                        mirroredAngles.z = -mirroredAngles.z;

                        jointData.mirroredRotation = Quaternion.Euler(mirroredAngles);
                    }

                }
                else
                {
                    jointData.normalRotation = Quaternion.identity;
                    jointData.mirroredRotation = Quaternion.identity;
                }
            }

            bodyData.joint[joint] = jointData;
            
            if (joint == (int)KinectInterop.JointType.SpineBase)
            {
                bodyData.normalRotation = jointData.normalRotation;
                bodyData.mirroredRotation = jointData.mirroredRotation;
            }
        }
    }

    private void draw_skeleton()
    {
        
        
        line_1.SetPosition(0, _bones[0].position);
        line_1.SetPosition(1, _bones[1].position);
        line_1.SetPosition(2, _bones[3].position);
        line_1.SetPosition(3, _bones[4].position);             
        
        
        
        line_2.SetPosition(0, _bones[7].position);
        line_2.SetPosition(1, _bones[6].position);
        line_2.SetPosition(2, _bones[5].position);
        line_2.SetPosition(3, _bones[3].position);
        line_2.SetPosition(4, _bones[11].position);
        line_2.SetPosition(5, _bones[12].position);
        line_2.SetPosition(6, _bones[13].position);

        line_3.SetPosition(0, _bones[19].position);
        line_3.SetPosition(1, _bones[18].position);
        line_3.SetPosition(2, _bones[17].position);
        line_3.SetPosition(3, _bones[0].position);
        line_3.SetPosition(4, _bones[21].position);
        line_3.SetPosition(5, _bones[22].position);
        line_3.SetPosition(6, _bones[23].position);

        //line_1.SetPosition(2, _bones[2].position);
		/*
        line_2.SetPosition(0, Joint_LeftHand.position);
        line_2.SetPosition(1, LeftElbow.position);
        line_2.SetPosition(2, LeftUpperArm.position);
        line_2.SetPosition(3, Neck.position);
        line_2.SetPosition(4, RightUpperArm.position);
        line_2.SetPosition(5, RightElbow.position);
        line_2.SetPosition(6, Joint_RightHand.position);
		
        line_3.SetPosition(0, Joint_LeftToe.position);
        line_3.SetPosition(1, Joint_LeftFoot.position);
        line_3.SetPosition(2, LeftKnee.position);
        line_3.SetPosition(3, LeftThigh.position);
        line_3.SetPosition(4, Hips.position);
        line_3.SetPosition(5, RightThigh.position);
        line_3.SetPosition(6, RightKnee.position);
        line_3.SetPosition(7, Joint_RightFoot.position);
        line_3.SetPosition(8, Joint_RightToe.position);*/
        

    }
    void TransformSkeletonBone(int boneIndex, bool flip, Quaternion jointRotation)
    {
        Transform boneTransform = _bones[boneIndex];
        if (boneTransform == null)
            return;

        if (jointRotation == Quaternion.identity)
            return;

        // Apply the new rotation.
        Quaternion newRotation = jointRotation * initialRotations[boneIndex];

        //If an offset node is specified, combine the transform with its
        //orientation to essentially make the skeleton relative to the node
        /*
        if (offsetNode != null)
        {
            // Grab the total rotation by adding the Euler and offset's Euler.
            Vector3 totalRotation = newRotation.eulerAngles + offsetNode.transform.rotation.eulerAngles;
            // Grab our new rotation.
            newRotation = Quaternion.Euler(totalRotation);
        }
         */
        //boneTransform.rotation = newRotation; //Direct apply without smoothing transformation
        // Smoothly transition to our new rotation.
        boneTransform.rotation = Quaternion.Slerp(boneTransform.rotation, newRotation, Time.deltaTime * SmoothFactor);
    }


    // If the _bones to be mapped have been declared, map that bone to the model.
    private void Map_bones()
    {
        /* DATA FROM CERTH EXPORTER
                        0		SpineBase
                        1		SpineMid	
                        2		Neck
                        3		Head
                        4		ShoulderLeft
                        5		ElbowLeft
                        6		WristLeft
                        7		HandLeft
                        8		ShoulderRight
                        9		ElbowRight
                        10		WristRight
                        11		HandRight
                        12		HipLeft
                        13		KneeLeft
                        14		AnkleLeft
                        15		FootLeft
                        16		HipRight
                        17		KneeRight
                        18		AnkleRight
                        19		FootRight
                        20      SpineShoulder
                        21      HandTipLeft
                        22      ThumbLeft
                        23      HandTipRight
                        24      ThumbRight
                            
        _bones[0] = SpineBase;
        _bones[1] = SpineMid;
        _bones[2] = Neck;
        _bones[3] = Head;

        _bones[4] = ShoulderLeft;
        _bones[5] = ElbowLeft;
        _bones[6] = WristLeft;
        _bones[7] = HandLeft;

        _bones[8] = ShoulderRight;
        _bones[9] = ElbowRight;
        _bones[10] = WristRight;
        _bones[11] = HandRight;

        _bones[12] = HipLeft;
        _bones[13] = KneeLeft;
        _bones[14] = AnkleLeft;
        _bones[15] = FootLeft;

        _bones[16] = HipRight;
        _bones[17] = KneeRight;
        _bones[18] = AnkleRight;
        _bones[19] = FootRight;

        /*
         _bones[24] = ToesRight;


        _bones[20] = ToesLeft;
        _bones[8] = FingersLeft;
        _bones[9] = FingerTipsLeft;
        _bones[10] = ThumbLeft;

        _bones[14] = FingersRight;
        _bones[15] = FingerTipsRight;
        _bones[16] = ThumbRight;

        // special bones
        //_bones[25] = ClavicleLeft;
        //_bones[26] = ClavicleRight;
        */
        // body root and offset
        bodyRoot = Root;
    }
    // If the bones to be mapped have been declared, map that bone to the model.
    protected virtual void Map_Bones_auto()
    {
        // make OffsetNode as a parent of model transform.
        offsetNode = new GameObject(name + "Ctrl") { layer = transform.gameObject.layer, tag = transform.gameObject.tag };
        offsetNode.transform.position = transform.position;
        offsetNode.transform.rotation = transform.rotation;

        // take model transform as body root
        transform.parent = offsetNode.transform;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        bodyRoot = transform;

        // get bone transforms from the animator component
        var animatorComponent = GetComponent<Animator>();

        for (int boneIndex = 0; boneIndex < _bones.Length; boneIndex++)
        {
            if (!boneIndex2MecanimMap.ContainsKey(boneIndex))
                continue;

            _bones[boneIndex] = animatorComponent.GetBoneTransform(boneIndex2MecanimMap[boneIndex]);
        }
    }
	
    // Set _bones to initial position.
    public void RotateToInitialPosition()
    {
        if (_bones == null)
            return;

        if (offsetNode != null)
        {
            // Set the offset's rotation to 0.
            offsetNode.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        // For each bone that was defined, reset to initial position.
        for (int i = 0; i < _bones.Length; i++)
        {
            if (_bones[i] != null)
            {
                _bones[i].rotation = initialRotations[i];
            }
        }

        if (Root != null && Root.parent != null)
        {
            Root.parent.localPosition = Vector3.zero;
        }

        if (offsetNode != null)
        {
            // Restore the offset's rotation
            offsetNode.transform.rotation = originalRotation;
        }
    }

    // Capture the initial rotations of the model.
    void GetInitialRotations()
    {
        if (offsetNode != null)
        {
            // Store the original offset's rotation.
            originalRotation = offsetNode.transform.rotation;
            // Set the offset's rotation to 0.
            offsetNode.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        for (int i = 0; i < _bones.Length; i++)
        {
            if (_bones[i] != null)
            {
                initialRotations[i] = _bones[i].rotation;
            }
        }

        if (offsetNode != null)
        {
            // Restore the offset's rotation
            offsetNode.transform.rotation = originalRotation;
        }
    }



}
