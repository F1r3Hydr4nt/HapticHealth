#region Description
/*	
KinectIntrinsicsProvider is a centralized point for different Kinect Intrinsic files loading
    which atm only supports the intrinsics produced from the Kinect Calibration Toolbox
    in opencv .xml array files

IntrinsicsLoader is the abstract definition of each different loader

OpenCVLoader is the Kinect Calibration Toolbox files loader
    the files produced are :
        -   IntrinsicsRGB0.xml ( 3x3 kinect color camera matrix containing focal length and principal point )
        -   IntrinsicsIR0.xml ( 3x3 kinect depth camera matrix containing focal length and principal point )
        -   DistortionRGB0.xml ( 1x5 kinect color camera distortion parameters , radial and tangential )
        -   DistortionIR0.xml ( 1x4 kinect depth camera distortion parameters , radial and tangential , only 2 params for radial distortion compared to color's 3 )
        -   dc_a0.xml ( disparity depth correction parameters a , 1x2 )
        -   dc_b0.xml ( disparity depth correction parameters b , 640x480 per pixel specific parameter )
        -   dc0.xml ( disparity depth correction parameters c , 1x2 )
        -   R0.xml ( relative pose rotation between Kinect's color and depth camera )
        -   T0.xml ( relative pose translation between Kinect's color and depth camera )

The axes X Y Z are according to the Kinect specification

TODO:
        -   Create new , more appropriate format combining the params into one file

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
	*/
#endregion
#region Namespaces
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using VclKinectBridge;

using Utilities;

using FileLoadAction = System.Action<string, Intrinsics.KinectIntrinsics>;
using FileMapperActions = System.Collections.Generic.List<
	System.Action<string, Intrinsics.KinectIntrinsics>>;
#endregion

namespace Intrinsics
{
    internal class KinectIntrinsicsProvider
    {
        internal static KinectIntrinsics GetKinectIntrinsics(string folder)
        {
            KinectIntrinsics intrinsics;
            try
            {
                intrinsics = IntrinsicsRepository.FromDirectory(folder);
                Console.Log("Intrinsics Loaded from " + folder);
            } catch (IOException e)
            {
                intrinsics = KinectIntrinsics.Burrus;
                Debugging.Error("Error loading intrinsics from " + folder + " ! Switching to Default ( Burrus )." + " Exception = " + e.ToString());
            }
            return intrinsics;
        }
		#region Fields
        private static readonly IntrinsicsLoader IntrinsicsRepository = new OpenCVLoader();		                          
        // Constants
        private const string IntrinsicsLocation = @"D:\Data\KinectIntrinsics";
        private const string IntrinsicsFileExtension = @".xml";
		#endregion
        private abstract class IntrinsicsLoader
        {
            internal abstract KinectIntrinsics FromFile(string filename);
            internal abstract KinectIntrinsics FromDirectory(string directory);
			
            protected KinectIntrinsics Return(KinectIntrinsicsContainer container)
            {			
                return new KinectIntrinsics(container.DisparityCorrectionParamA,
				                            container.DisparityCorrectionParamB, container.DisparityCorrectionParamC,
				                            container.DepthDistortionParams, container.RgbDistortionParams,
				                            container.DepthIntrinsicsParams, container.RgbIntrinsicsParams,
				                            container.RgbDepthRotationParams, container.RgbDepthTranslationParams);
            }
///<summary>
/// A Plain Old Class Object 
/// Contains the Parsed Data
///</summary>
            internal protected sealed class KinectIntrinsicsContainer
            {
                internal float[] DisparityCorrectionParamA;
                internal float[] DisparityCorrectionParamB;
                internal float[] DisparityCorrectionParamC;
                internal float[] DepthDistortionParams;
                internal float[] RgbDistortionParams;
                internal float[] DepthIntrinsicsParams;
                internal float[] RgbIntrinsicsParams;
                internal float[] RgbDepthRotationParams;
                internal float[] RgbDepthTranslationParams;
            }
        }
///<summary>
/// Loads Kinect Intrinsics from a Directory 
/// Containing the files generated from Kinect Calibration Toolbox for Matlab
///</summary>
        private sealed class OpenCVLoader : IntrinsicsLoader
        {
            internal override KinectIntrinsics FromFile(string filename)
            {
                var intrinsics = KinectIntrinsics.Burrus;
                //TODO: Raise exception instead of standard params?
                return intrinsics;
            }
            internal override KinectIntrinsics FromDirectory(string directory)
            {
                KinectIntrinsicsContainer intrinsics = new KinectIntrinsicsContainer();
                OpenCVLoader.FileToIntrinsicMapping.ForEach(
					(action) => action.Invoke(directory, intrinsics));
                return Return(intrinsics);
            }
			#region Fields
            // File names , Data Sizes 
            private const string DisparityCorrectionParameterA = @"dc_a0";
            private const int DisparityCorrectionParameterAElements = 2;
            private const string DisparityCorrectionParameterB = @"dc_b0";
            private const int DisparityCorrectionParameterBElements = 307200;
            private const string DisparityCorrectionParameterC = @"dc0";
            private const int DisparityCorrectionParameterCElements = 2;
            private const string RgbIntrinsics = @"IntrinsicsRGB0";
            private const int RgbIntrinsicsParameterElements = 9;
            private const string DepthIntrinsics = @"IntrinsicsIR0";
            private const int DepthIntrinsicsParameterElements = 9;
            private const string RgbDistortion = @"DistortionRGB0";
            private const int RgbDistortionParameterElements = 5;
            private const string DepthDistortion = @"DistortionIR0";
            private const int DepthDistortionParameterElements = 5;
            private const string RelativeRotation = @"R0";
            private const int RgbDepthRotationParameterElements = 9;
            private const string RelativeTranslation = @"T0";
            private const int RgbDepthTranslationParameterElements = 3;
            private const string FileExtension = @".xml";
            // dalexiad - OpenCV Format 
            private static readonly char[] Delimiters = new char[]
            {
                '\n',
                '\r'
            };
            private static readonly System.Globalization.NumberFormatInfo OpenCVNumberInfo =
				new System.Globalization.NumberFormatInfo()
			{
				NumberDecimalSeparator = ".",
				NumberDecimalDigits = 6,
				NumberNegativePattern = 1
			};
            // File Load Actions Mapper
			#region Mapper
            private static readonly List<Action<string,KinectIntrinsicsContainer>> FileToIntrinsicMapping
				= new List<Action<string, KinectIntrinsicsContainer>>()
						{
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Depth Camera Disparity Correction Parameters , dc_a0
										var doc = XDocument.Load (Path.Combine (dir,
					                                      OpenCVLoader.DisparityCorrectionParameterA + OpenCVLoader.FileExtension));
										intrinsics.DisparityCorrectionParamA = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.DisparityCorrectionParameterAElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Depth Camera Disparity Correction Parameters , dc_b0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.DisparityCorrectionParameterB + OpenCVLoader.FileExtension));
										intrinsics.DisparityCorrectionParamB = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.DisparityCorrectionParameterBElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Depth Camera Disparity Correction Parameters , dc0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.DisparityCorrectionParameterC + OpenCVLoader.FileExtension));
										intrinsics.DisparityCorrectionParamC = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.DisparityCorrectionParameterCElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Depth Camera Distortion Parameters , DistortionIR0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.DepthDistortion + OpenCVLoader.FileExtension));
										intrinsics.DepthDistortionParams = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.DepthDistortionParameterElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Rgb Camera Distortion Parameters , DistortionRGB0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.RgbDistortion + OpenCVLoader.FileExtension));
										intrinsics.RgbDistortionParams = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.RgbDistortionParameterElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Depth Camera Intrinsic Parameters , IntrinsicsIR0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.DepthIntrinsics + OpenCVLoader.FileExtension));
										intrinsics.DepthIntrinsicsParams = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.DepthIntrinsicsParameterElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Rgb Camera Intrinsic Parameters , IntrinsicsRGB0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.RgbIntrinsics + OpenCVLoader.FileExtension));
										intrinsics.RgbIntrinsicsParams = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.RgbIntrinsicsParameterElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>
								{// Rgb-Depth Relative Rotation , R0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.RelativeRotation + OpenCVLoader.FileExtension));
										intrinsics.RgbDepthRotationParams = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.RgbDepthRotationParameterElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								}),
				new Action<string,KinectIntrinsicsContainer>((dir,intrinsics) =>					
								{// Rgb-Depth Relative Translation , T0
										var doc = XDocument.Load (Path.Combine (dir,
						                                      OpenCVLoader.RelativeTranslation + OpenCVLoader.FileExtension));
										intrinsics.RgbDepthTranslationParams = doc.Element ("opencv_storage").Element ("MyMatrix")
							.Element ("data").Value
							.Split (OpenCVLoader.Delimiters, OpenCVLoader.RgbDepthTranslationParameterElements, StringSplitOptions.RemoveEmptyEntries)
								.Select ((s) => float.Parse (s, OpenCVNumberInfo))
								.ToArray ();
								})
						};
			#endregion
			#endregion
        }
    }
}
