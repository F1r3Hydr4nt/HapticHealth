#region Description
/*	
Base Kinect Behaviour that allows access to each Kinect's Handle

TODO:
    - More uses?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using KinectHandle = System.Int32;
#endregion
namespace VclUnityKinect
{
    public abstract class WithKinectHandle : MonoBehaviour
    {
		#region Properties
        internal bool IsValid { get; private set; }
        internal KinectHandle Handle
        {
            get { return this.handle; }
            set
            { //MaxValue is Invalid Handle							
                if (value != 0 && value != KinectHandle.MaxValue)
                {
                    this.handle = value;
                    this.IsValid = true;
                } else
                {
                    this.handle = 0;
                    this.IsValid = false;	
                }
            }
        }
		#endregion
		#region Unity
        protected void OnDisable()//OBSOLETE?
        {
            // disabled when passed a null ( 0 ) handle
            // usually when we have more Kinects in our scene than devices connected
            // TODO log error?
        }
		#endregion
		#region Fields
        private KinectHandle handle;
        protected const int SuccessCheck = 0;
		#endregion	
    }
}
