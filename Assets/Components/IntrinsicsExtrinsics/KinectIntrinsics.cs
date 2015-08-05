#region Description
/*
KinectIntrinsics is a simple Class object containing the combined afformentioned 
    Kinect calibration parameters written for Unity3D 
    to be used with the scanzip file format ( compressed Kinect scan images )

	Kinect Intrinsics Components
        Basic Coefficient Container Structures
            -   Radial Distortion
            -   Tangential Distortion
            -   Disparity Correction
        Kinect Intrinsics
            -   Camera Intrinsic Matrices ( Color and Depth )
            -   Depth & Color Relative External Position
            -   Depth & Color Distortions
            -   Depth Disparity Estimation
        Default Value : Burrus Kinect Default Parameters ( http://nicolas.burrus.name/index.php/Research/KinectCalibration#tocLink6 )

These parameters are extracted using the Kinect Calibration Toolbox
        by Daniel Herrera C. 
        Center for Machine Vision Research
        University of Oulu
This is the toolbox accompanying our PAMI 2012 paper:
- Herrera C., D., Kannala, J., Heikkilä, J., "Joint depth and color camera calibration with distortion correction", TPAMI, 2012.

Matlab code found @ http://www.ee.oulu.fi/~dherrera/kinect/

The axes X Y Z are according to the Kinect specification

TODO: 
        -   Refactor Distortion/Intrinsics into general Camera Parameters
        -   Confirm/Investigate Transformation compatibility

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

//using ScanImageZipFile;
#endregion

namespace Intrinsics
{
    public struct RadialDistortion
    {
        public RadialDistortion(float k1, float k2, float k3)
        {
            this.K1 = k1;
            this.K2 = k2;
            this.K3 = k3;
        }
        public readonly float K1, K2, K3;
    }

    public struct TangentialDistortion
    {
        public TangentialDistortion(float k4, float k5)
        {
            this.K4 = k4;
            this.K5 = k5;
        }
        public readonly float K4, K5;
    }

    public struct DisparityConversionCoeffs
    {
        public DisparityConversionCoeffs(float a0, float a1, float c0, float c1)
        {
            this.A0 = a0;
            this.A1 = a1;
            this.C0 = c0;
            this.C1 = c1;
        }
        public readonly float A0, A1, C0, C1;
    }

    public class KinectIntrinsics
    {
        public Matrix4x4 DepthCameraIntrinsics { get; private set; }
        public Matrix4x4 RgbCameraIntrinsics { get; private set; }
        public RadialDistortion DepthRadialDistortion { get; private set; }
        public TangentialDistortion DepthTangentialDistortion { get; private set; }
        public RadialDistortion RgbRadialDistortion { get; private set; }
        public TangentialDistortion RgbTangentialDistortion { get; private set; }
        public DisparityConversionCoeffs DisparityConversionCoeffs { get; private set; }
        public Matrix4x4 ExtrinsicTransformation { get; private set; }
        public float[] DisparityCorrectionMap { get; private set; }
		
        internal static KinectIntrinsics Burrus
        {
            get
            {
                var intrinsics = new KinectIntrinsics();
                // rgb
                intrinsics.RgbCameraIntrinsics = new Matrix4x4()//TODO: Column/Row major?
								{ 
								m00 = RgbFocalX , m01 = 0.0f , m02 = 0.0f , m03 = RgbPrincipalX, 
								m10 = 0.0f, m11 = RgbFocalY , m12 = 0.0f , m13 = RgbPrincipalY,
								m20 = 0.0f , m21 = 0.0f , m22 = 1.0f , m23 = 0.0f,
								m30 = 0.0f, m31 = 0.0f , m32 = 0.0f , m33 = 1.0f
								};
                intrinsics.RgbRadialDistortion = new RadialDistortion(
									RgbRadialDistortionCoeff1, RgbRadialDistortionCoeff2, RgbRadialDistortionCoeff3);
                intrinsics.RgbTangentialDistortion = new TangentialDistortion(
									RgbTangentialDistortionCoeff1, RgbTangentialDistortionCoeff2);
                // depth
                intrinsics.DepthCameraIntrinsics = new Matrix4x4()//TODO: Column/Row major?
								{ 
									m00 = DepthFocalX , m01 = 0.0f , m02 = 0.0f , m03 = DepthPrincipalX, 
									m10 = 0.0f, m11 = DepthFocalY , m12 = 0.0f , m13 = DepthPrincipalY,
									m20 = 0.0f , m21 = 0.0f , m22 = 1.0f , m23 = 0.0f,
									m30 = 0.0f, m31 = 0.0f , m32 = 0.0f , m33 = 1.0f
								};
                intrinsics.DepthRadialDistortion = new RadialDistortion(
									DepthRadialDistortionCoeff1, DepthRadialDistortionCoeff2, DepthRadialDistortionCoeff3);
                intrinsics.DepthTangentialDistortion = new TangentialDistortion(
									DepthTangentialDistortionCoeff1, DepthTangentialDistortionCoeff2);
                // disparity
                intrinsics.DisparityConversionCoeffs = new DisparityConversionCoeffs();
                intrinsics.DisparityCorrectionMap = null;
                // extrinsics
                intrinsics.ExtrinsicTransformation = new Matrix4x4()//TODO: Column/Row major?
								{
										m00 = DepthToRgbRotation [0] , m01 = DepthToRgbRotation [1] , m02 = DepthToRgbRotation [2] , m03 = 0.0f,
										m10 = DepthToRgbRotation [3] , m11 = DepthToRgbRotation [4] , m12 = DepthToRgbRotation [5] , m13 = 0.0f,
										m20 = DepthToRgbRotation [6] , m21 =  DepthToRgbRotation [7] , m22 = DepthToRgbRotation [8] , m23 = 0.0f,
										m30 = DepthToRgbTranslation [0] , m31 = DepthToRgbTranslation [1] , m32 = DepthToRgbTranslation [2] , m33 = 1.0f
								};
                return intrinsics;
            }
        }

        private KinectIntrinsics()
        {
			
        }

        internal KinectIntrinsics(float[] dcA, float[] dcB, float[] dcC,
		                          float[] DistortD, float[] DistortRGB, float[] IntrinsicsD,
		                          float[] IntrinsicsRGB, float[] Rot, float[] Trans)
        {
            // rgb
            this.RgbCameraIntrinsics = new Matrix4x4()//TODO: Column/Row major?
						{ 
							m00 = IntrinsicsRGB [0] , m01 = 0.0f , m02 = 0.0f , m03 = IntrinsicsRGB [2], 
							m10 = 0.0f, m11 = IntrinsicsRGB [4] , m12 = 0.0f , m13 = IntrinsicsRGB [5],
							m20 = 0.0f , m21 = 0.0f , m22 = 1.0f , m23 = 0.0f,
							m30 = 0.0f, m31 = 0.0f , m32 = 0.0f , m33 = 1.0f
						};
            this.RgbRadialDistortion = new RadialDistortion(DistortRGB [0], DistortRGB [1], DistortRGB [4]);
            this.RgbTangentialDistortion = new TangentialDistortion(DistortRGB [2], DistortRGB [3]);
            // depth
            this.DepthCameraIntrinsics = new Matrix4x4()//TODO: Column/Row major?
						{ 
							m00 = IntrinsicsD [0] , m01 = 0.0f , m02 = 0.0f , m03 = IntrinsicsD [2], 
							m10 = 0.0f, m11 = IntrinsicsD [4] , m12 = 0.0f , m13 = IntrinsicsD [5],
							m20 = 0.0f , m21 = 0.0f , m22 = 1.0f , m23 = 0.0f,
							m30 = 0.0f, m31 = 0.0f , m32 = 0.0f , m33 = 1.0f
						};
            this.DepthRadialDistortion = new RadialDistortion(DistortD [0], DistortD [1], DistortD [4]);
            this.DepthTangentialDistortion = new TangentialDistortion(DistortD [2], DistortD [3]);
            // disparity
            this.DisparityConversionCoeffs = new DisparityConversionCoeffs(dcA [0], dcA [1], dcC [0], dcC [1]);
            this.DisparityCorrectionMap = dcB;
            // extrinsics
            this.ExtrinsicTransformation = new Matrix4x4()//TODO: Column/Row major?
						{
							m00 = Rot [0] , m01 = Rot [1] , m02 = Rot [2] , m03 = 0.0f,
							m10 = Rot [3] , m11 = Rot [4] , m12 = Rot [5] , m13 = 0.0f,
							m20 = Rot [6] , m21 =  Rot [7] , m22 = Rot [8] , m23 = 0.0f,
							m30 = Trans [0] , m31 = Trans [1] , m32 = Trans [2] , m33 = 1.0f
						};
        }

        public float[] ToFloat()
        {
            float[] data = new float[307244];
            // rgb 2d transform 3x3 matrix
            data [0] = this.RgbCameraIntrinsics.m00;	// xx - focalX
            data [1] = 0.0f;											// xy
            data [2] = this.RgbCameraIntrinsics.m03;	// Tx - principalX
            data [3] = 0.0f;											// yx
            data [4] = this.RgbCameraIntrinsics.m11;	// yy - focalY
            data [5] = this.RgbCameraIntrinsics.m13;	// Ty - principalY
            data [6] = 0.0f;
            data [7] = 0.0f;
            data [8] = 1.0f;
            // depth 2d transform 3x3 matrix
            data [9] = this.DepthCameraIntrinsics.m00;	// xx - focalX
            data [10] = 0.0f;											// xy
            data [11] = this.DepthCameraIntrinsics.m03;	// Tx - principalX
            data [12] = 0.0f;											// yx
            data [13] = this.DepthCameraIntrinsics.m11;	// yy - focalY
            data [14] = this.DepthCameraIntrinsics.m13;	// Ty - principalY
            data [15] = 0.0f;
            data [16] = 0.0f;
            data [17] = 1.0f;
            // rgb distortion 1x5 
            data [18] = this.RgbRadialDistortion.K1;
            data [19] = this.RgbRadialDistortion.K2;
            data [20] = this.RgbRadialDistortion.K3;
            data [21] = this.RgbTangentialDistortion.K4;
            data [22] = this.RgbTangentialDistortion.K5;
            // depth distortion 1x5
            data [23] = this.DepthRadialDistortion.K1;
            data [24] = this.DepthRadialDistortion.K2;
            data [25] = 0.0f;
            data [26] = this.DepthTangentialDistortion.K4;
            data [27] = this.DepthTangentialDistortion.K5;
            // rgb 2 depth rotation 3x3 matrix
            data [28] = this.ExtrinsicTransformation.m00;
            data [29] = this.ExtrinsicTransformation.m01;
            data [30] = this.ExtrinsicTransformation.m02;
            data [31] = this.ExtrinsicTransformation.m10;
            data [32] = this.ExtrinsicTransformation.m11;
            data [33] = this.ExtrinsicTransformation.m12;
            data [34] = this.ExtrinsicTransformation.m20;
            data [35] = this.ExtrinsicTransformation.m21;
            data [36] = this.ExtrinsicTransformation.m22;
            // rgb 2 depth translation 1x3 vector
            data [37] = this.ExtrinsicTransformation.m30;
            data [38] = this.ExtrinsicTransformation.m31;
            data [39] = this.ExtrinsicTransformation.m32;
            // disparity correction coeffs 2  [ 1 x 2 ] vectors
            data [40] = this.DisparityConversionCoeffs.A0;
            data [41] = this.DisparityConversionCoeffs.A1;
            data [42] = this.DisparityConversionCoeffs.C0;
            data [43] = this.DisparityConversionCoeffs.C1;
            // disparity correction map
            Buffer.BlockCopy(this.DisparityCorrectionMap, 0, data, 44 * sizeof(float), this.DisparityCorrectionMap.Length * sizeof(float));
            return data;
        }
		
        private const int FloatDataLength = 307244;
		
		#region Burrus Constants
        //http://nicolas.burrus.name/index.php/Research/KinectCalibration#tocLink6
		
        // Disparity Distortion Model
        // UndistortedDisparity(u,v) = Depth(u,v) + dc_b(u,v) * exp( dc_a(0) - dc_a(1) * Depth(u,v) )
		
        // Disparity to Depth 
        // UndistoredDepth(u,v) = 1000 / ( dc_c(0) * UndistortedDisparity(u,v) + dc_c(1) )
		
        // Depth to Disparity 
        // Disparity(u,v) = ( ( 1000/Depth(u,v) ) - bita ) / alpha
        private const float alpha = -0.00307F;
        private const float bita = 3.331F;
		
        // RGB PARAMS
        // fx_rgb
        private const float RgbFocalX = 5.2921508098293293e+02F;
        // fy_rgb
        private const float RgbFocalY = 5.2556393630057437e+02F;
        // cx_rgb
        private const float RgbPrincipalX = 3.2894272028759258e+02F;
        // cy_rgb
        private const float RgbPrincipalY = 2.6748068171871557e+02F;
        // k1_rgb
        private const float RgbRadialDistortionCoeff1 = 2.6451622333009589e-01F;
        // k2_rgb
        private const float RgbRadialDistortionCoeff2 = -8.3990749424620825e-01F;
        // p1_rgb
        private const float RgbTangentialDistortionCoeff1 = -1.9922302173693159e-03F;
        // p2_rgb
        private const float RgbTangentialDistortionCoeff2 = 1.4371995932897616e-03F;
        // k3_rgb
        private const float RgbRadialDistortionCoeff3 = 9.1192465078713847e-01F;
		
        // DEPTH PARAMS
        // fx_depth
        private const float DepthFocalX = 5.9421434211923247e+02F;
        // fy_depth
        private const float DepthFocalY = 5.9104053696870778e+02F;
        // cx_depth
        private const float DepthPrincipalX = 3.3930780975300314e+02F;
        // cy_depth
        private const float DepthPrincipalY = 2.4273913761751615e+02F;
        // k1_depth
        private const float DepthRadialDistortionCoeff1 = -2.6386489753128833e-01F;
        // k2_depth
        private const float DepthRadialDistortionCoeff2 = 9.9966832163729757e-01F;
        // p1_depth
        private const float DepthTangentialDistortionCoeff1 = -7.6275862143610667e-04F;
        // p2_depth
        private const float DepthTangentialDistortionCoeff2 = 5.0350940090814270e-03F;
        // k3_depth
        private const float DepthRadialDistortionCoeff3 = -1.3053628089976321e+00F;
		
        // EXTERNAL PARAMS
        // Rotation
        private static float[] DepthToRgbRotation = new float[]
		{
			9.9984628826577793e-01F, 1.2635359098409581e-03F, -1.7487233004436643e-02F,
			-1.4779096108364480e-03F, 9.9992385683542895e-01F, -1.2251380107679535e-02F,
			1.7470421412464927e-02F, 1.2275341476520762e-02F, 9.9977202419716948e-01F 
		};
        // Translation
        private static float[] DepthToRgbTranslation = new float[] {
						1.9985242312092553e-02F,
						-7.4423738761617583e-04F,
						-1.0916736334336222e-02F
				};
		
		#endregion
    }
}
