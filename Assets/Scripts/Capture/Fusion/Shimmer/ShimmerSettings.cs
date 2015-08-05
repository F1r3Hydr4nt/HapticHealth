#region Description
/*
Shimmer Settings Behaviour
    allows for control over the Shimmer3 device's properties
    shows GUI to modify those

TODO:
    -   Add more uses/features
    -   Add joint selection? or mounting position?

@author Nick Zioulis , Visual Computing Lab  ITI-CERTH
@date July 2014
    */
#endregion
#region Namespaces
using UnityEngine;
using ShimmerConnect;
#endregion

namespace Shimmer
{
    public sealed class ShimmerSettings : MonoBehaviour
    {	
		#region Unity
        void Awake()
        {
            this.shimmer = GetComponent<ShimmerControlling>();
            this.skin = Resources.Load<GUISkin>(GuiSkinLocation);
        }
		
        void OnEnable()
        {
            this.enabled = this.shimmer != null;
        }
		
        void OnGUI()
        {
            GUI.skin = this.skin;
            GUI.Box(GroupRect, GroupTitle);
            GUI.BeginGroup(GroupRect);
            GUI.Label(LabelRect, LabelText);
            this.shimmer.name = GUI.TextField(TextRect, this.shimmer.name, NameMaxLength);
            GUI.EndGroup();	
        }
		
        void OnDisable()
        {
			
        }
		#endregion
		#region Fields
        private GUISkin skin;
        private ShimmerControlling shimmer;
		
        private const string GuiSkinLocation = @"GUI/Skins/BlackUI";//TODO: global gui set? or editor settable through public?
        // group panel
        private const int GroupWidth = 300;
        private const int GroupHeight = 300;
        private const int GroupY = 450 + GroupHeight;
        private static Rect GroupRect = new Rect(Screen.width - GroupWidth, Screen.height - GroupY, GroupWidth, GroupHeight);
        private static Rect BoxRect = new Rect(Screen.width - GroupWidth - 5, Screen.height - GroupY - 5, GroupWidth - 10, GroupHeight - 10);
        private const string GroupTitle = @"Shimmer Settings";
        //	name field
        private const int LabelWidth = 50;
        private const int LabelHeight = 30;
        private const string LabelText = @"Name : ";
        private const int TextWidth = 125;
        private const int TextHeight = 30;
        private const int LabelX = 10;
        private const int LabelY = 25;
        private static Rect LabelRect = new Rect(LabelX, LabelY, LabelWidth, LabelHeight);
        private const int TextX = LabelX + LabelWidth;
        private const int TextY = LabelY;
        private static Rect TextRect = new Rect(TextX, TextY, TextWidth, TextHeight);
        private const int NameMaxLength = 15;
		#endregion
    }
}
