#region Description
/*
MinimapColorRendering Behaviour
    Displays Kinect Color Frames using a Quad Primitive as a Video Surface
        and an orthographic camera in a minimap like style

	TODO:
        - Refactor to allow for more Kinect minimaps?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using Utilities;
#endregion
namespace VclUnityKinect
{
    public sealed  class MinimapColorRendering : MonoBehaviour
    {
		#region Properties
        internal Texture2D ColorTexture
        {
            get { return this.colorTexture;}
        }
		#endregion	
		#region Unity
        void Awake()
        {
            this.transform.position = CameraPosition;
            this.surfaceTransform = GetComponentInChildren<Transform>();
            this.surfaceTransform.localPosition += ChildOffsetPosition;
            this.camera.transform.LookAt(this.surfaceTransform.position);
            this.colorTexture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
            this.surfaceRenderer = GetComponentInChildren<Renderer>();
            this.surfaceRenderer.material.mainTexture = this.colorTexture;
            DontDestroyOnLoad(this.gameObject);
        }

        void OnEnable()
        {
            this.camera.enabled = true;
        }

        void OnDisable()
        {
            this.camera.enabled = false;
        }
		#endregion
		#region Fields
        private Transform surfaceTransform;
        private Renderer surfaceRenderer;
        private Texture2D colorTexture;

        private static Vector3 CameraPosition = new Vector3(-500, -500, 500);
        private static Vector3 ChildOffsetPosition = new Vector3(0, 0, -10);
		#endregion
    }
}
