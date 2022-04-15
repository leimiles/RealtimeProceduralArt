using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miles {
    public static class MyWindowGUI {
        public static bool is_Init = false;
        public static GUIStyle header_Style;
        public static Vector2 scrollViewPos_PrefabDatas;
        public static Vector2 scrollViewPos_PlacementParameters;
        public static int prefabs_Section_Height;
        public static int prefab_Data_Area_Size;
        public static GUIStyle prefab_Data_Area_Style;
        public static GUIStyle prefab_Data_Button_Style;
        public static Color prefab_Data_Button_Activated_Colr;
        public static Color prefab_Data_Button_Deactivated_Color;
        public static GUIStyle prefab_Datas_Selection_Mode_Style;
        public static BrushSettings brush;
        public static int foldoutIndentWidth;
        public static int brushModePopupWidth;
        public static void Init() {
            header_Style = new GUIStyle(GUI.skin.GetStyle("label"));
            header_Style.fontStyle = FontStyle.BoldAndItalic;
            header_Style.fontSize = 15;
            header_Style.alignment = TextAnchor.MiddleCenter;

            scrollViewPos_PrefabDatas = Vector2.zero;
            scrollViewPos_PlacementParameters = Vector2.zero;

            prefabs_Section_Height = 256;
            prefab_Data_Area_Size = 128;

            prefab_Data_Area_Style = new GUIStyle(GUI.skin.GetStyle("box"));
            prefab_Data_Area_Style.alignment = TextAnchor.LowerCenter;
            prefab_Data_Area_Style.margin = new RectOffset(5, 0, 5, 0);
            prefab_Data_Area_Style.hover.background = Texture2D.whiteTexture;

            prefab_Data_Button_Style = new GUIStyle(GUI.skin.GetStyle("button"));
            prefab_Data_Button_Style.alignment = TextAnchor.LowerCenter;
            prefab_Data_Button_Style.fixedHeight = prefab_Data_Area_Size;
            prefab_Data_Button_Style.fixedWidth = prefab_Data_Area_Size;
            prefab_Data_Button_Style.imagePosition = ImagePosition.ImageAbove;
            prefab_Data_Button_Activated_Colr = Color.yellow;
            prefab_Data_Button_Deactivated_Color = Color.white;

            foldoutIndentWidth = 18;

            brushModePopupWidth = 350;

            prefab_Datas_Selection_Mode_Style = new GUIStyle(GUI.skin.GetStyle("toggle"));
            brush = new BrushSettings();

            is_Init = true;
        }
    }
}