#region Description
/*
SkeletonTextImporter imports Kinect Skeletons exported data in text format.
    The frames can be enumerated by using NextFrame() or requested as an array through AllFrames
    
TODO:
    - Check/Refactor MasterDetails implementation ( allFrames currently in memory all the time even when enumerating )
    
@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
*/
#endregion
#region Namespaces
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FileHelpers;
using FileHelpers.MasterDetail;

using VclKinectBridge;
#endregion

namespace SkeletonOne
{
    public sealed class SkeletonTextImporter
    {
        #region Constructors
        public SkeletonTextImporter(string filename)
        {
            this.engine = new MasterDetailEngine(
                    typeof(FrameContainer), typeof(SkeletonContainer),
                    CommonSelector.MasterIfBegins, "Frame");
            this.engine.HeaderText = SkeletonTextExporter.TextHeader;
            this.allFrames = this.engine.ReadFile(filename);
        }
        #endregion
        #region Properties
        public SkeletonFrame[] AllFrames
        {
            get
            {
                return this.allFrames.Select((md) => ConstructSkeletonFrame(md)).ToArray();
            }
        }
        #endregion
        #region Methods
        public IEnumerable<SkeletonFrame> NextFrame()
        {
            foreach (MasterDetails md in this.allFrames)
            {
                yield return this.ConstructSkeletonFrame(md);
            }
        }
            
        private SkeletonFrame ConstructSkeletonFrame(MasterDetails md)
        {
            FrameContainer frame = md.Master as FrameContainer;
            SkeletonContainer skeleton = md.Details.Cast<SkeletonContainer>().FirstOrDefault();
            return new SkeletonFrame(ExtractJoints(skeleton), frame.TimeStamp);
        }

        private SkeletonJoint[] ExtractJoints(SkeletonContainer skeleton)
        {
            return new SkeletonJoint[]
            {
                new SkeletonJoint(skeleton.HipCenter),
                new SkeletonJoint(skeleton.Spine),
                new SkeletonJoint(skeleton.ShoulderCenter),
                new SkeletonJoint(skeleton.Head),
                new SkeletonJoint(skeleton.ShoulderLeft),
                new SkeletonJoint(skeleton.ElbowLeft),
                new SkeletonJoint(skeleton.WristLeft),
                new SkeletonJoint(skeleton.HandLeft),
                new SkeletonJoint(skeleton.ShoulderRight),
                new SkeletonJoint(skeleton.ElbowRight),
                new SkeletonJoint(skeleton.WristRight),
                new SkeletonJoint(skeleton.HandRight),
                new SkeletonJoint(skeleton.HipLeft),
                new SkeletonJoint(skeleton.KneeLeft),
                new SkeletonJoint(skeleton.AnkleLeft),
                new SkeletonJoint(skeleton.FootLeft),
                new SkeletonJoint(skeleton.HipRight),
                new SkeletonJoint(skeleton.KneeRight),
                new SkeletonJoint(skeleton.AnkleRight),
                new SkeletonJoint(skeleton.FootRight)
            };
        }
        #endregion
            
        #region Fields
        private MasterDetailEngine engine;
        private MasterDetails[] allFrames;
        #endregion         
    }
}

