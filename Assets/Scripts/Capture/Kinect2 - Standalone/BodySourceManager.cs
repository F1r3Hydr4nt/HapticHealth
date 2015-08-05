using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Kinect = Windows.Kinect;
using Windows.Kinect;

public class BodySourceManager : MonoBehaviour 
{
	private Windows.Kinect.KinectSensor _Sensor;
    private BodyFrameReader _Reader;
    private Body[] _Data = null;    
	private TimeSpan currentTime = new TimeSpan();
	private Windows.Kinect.Vector4 currentFloor = new Windows.Kinect.Vector4();
	public bool isActive = false;

	public TimeSpan CurrentTime {
		get { return currentTime; }
	}
	public Windows.Kinect.Vector4 CurrentFloor {
		get { return currentFloor; }
	}
	public Body[] GetData()
    {
        return _Data;
    }
    
    void Start () 
    {
		_Sensor = Windows.Kinect.KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }

			isActive = true;

			// Floor
			_Reader.FrameArrived += (object sender, BodyFrameArrivedEventArgs e) =>
			{
				using(var frame = e.FrameReference.AcquireFrame())
				{
					this.currentTime = frame.RelativeTime;
					this.currentFloor = frame.FloorClipPlane;
				}
			};
        } 
    }
    
    void Update () 
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
			{	
				currentTime = frame.RelativeTime;

                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }
                
                frame.GetAndRefreshBodyData(_Data);               

                frame.Dispose();
                frame = null;
            }
        }    
    }
    
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
