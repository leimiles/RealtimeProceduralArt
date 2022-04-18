using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace miles {
    [CustomEditor(typeof(Prefab_DataCollection))]
    public class PrefabCollectionEditor : Editor {
        // Start is called before the first frame update
        Prefab_DataCollection prefab_DataCollection;
        public void OnEnable() {
            prefab_DataCollection = target as Prefab_DataCollection;
        }

        public override void OnInspectorGUI() {
            if (!MyWindowGUI.is_Init) {
                MyWindowGUI.Init();
            }
            GUILayout.Label("Prefab Data Collection");
            DrawPrefabSection();
            //DrawDragPrefabSection();
            if (!prefab_DataCollection) {
                GUILayout.Label("Data Error");
                return;
            }
            if (prefab_DataCollection.prefabs == null || prefab_DataCollection.prefabs.Count == 0) {
                GUILayout.Label("No Data");
                return;
            }
            //GUILayout.Box("data", GUILayout.Width(MyWindowGUI.prefab_Data_Area_Size), GUILayout.Height(MyWindowGUI.prefab_Data_Area_Size));



            //base.OnInspectorGUI();
        }
        /*
        void DrawDragPrefabSection() {
            Event evt = Event.current;
            Rect drop_area = GUILayoutUtility.GetRect(0.0f, MyWindowGUI.Prefab_Data_Collection_DragArea_Height, GUILayout.ExpandWidth(true));
            GUI.Box(drop_area, "Drag Prefabs Here ... ", MyWindowGUI.prefab_Data_Collection_Style);
            switch (evt.type) {
                case EventType.DragUpdated:
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    break;
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition)) {
                        return;
                    }
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        Debug.Log(DragAndDrop.objectReferences.Length);
                    }
                    break;
            }
        }
        */

        void DrawPrefabSection() {
            Repaint();

            MyWindowGUI.scrollViewPos_PrefabDatas = EditorGUILayout.BeginScrollView(MyWindowGUI.scrollViewPos_PrefabDatas, false, false, GUILayout.Height(MyWindowGUI.prefabs_Section_Height));
            int column = Mathf.FloorToInt(EditorGUIUtility.currentViewWidth / MyWindowGUI.prefab_Data_Area_Size);
            int row;
            if (column > prefab_DataCollection.prefabs.Count) {
                row = 1;
            } else {
                row = Mathf.CeilToInt(prefab_DataCollection.prefabs.Count * 1.0f / column * 1.0f);
            }
            // this is just background
            Rect background_Rect = new Rect(0, 0, EditorGUIUtility.currentViewWidth, MyWindowGUI.prefab_Data_Area_Size * (row + 1) + 10 * row);
            GUI.Box(background_Rect, "Drag Prefabs Here", MyWindowGUI.prefab_Data_Collection_Style);
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < row; i++) {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < column; j++) {
                    int index = i * column + j;
                    if (index < prefab_DataCollection.prefabs.Count) {
                        DrawPrefabBox(index, i, j);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            if (Event.current.type == EventType.DragUpdated && background_Rect.Contains(Event.current.mousePosition)) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }

            if (Event.current.type == EventType.DragPerform && background_Rect.Contains(Event.current.mousePosition)) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                //AddPrefabData(DragAndDrop.objectReferences);
                AddPrefabs(DragAndDrop.objectReferences);
            }
            EditorGUILayout.EndScrollView();
            GUI.color = Color.white;

        }

        void AddPrefabs(object[] objects) {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i].GetType().Name == "GameObject") {
                    GameObject obj = (GameObject)objects[i];
                    if (prefab_DataCollection.prefabs.Contains(obj)) {
                        Debug.Log("有了啊");
                        continue;
                    } else {
                        prefab_DataCollection.prefabs.Add(obj);
                    }
                }
            }

        }

        void DrawPrefabBox(int index, int row, int column) {
            GUILayout.Box("", MyWindowGUI.prefab_Data_Area_Style, GUILayout.Width(MyWindowGUI.prefab_Data_Area_Size), GUILayout.Height(MyWindowGUI.prefab_Data_Area_Size));
            Rect Prefab_Rect = new Rect(
                (column * MyWindowGUI.prefab_Data_Area_Size + MyWindowGUI.prefab_Data_Area_Style.margin.left * column) + MyWindowGUI.prefab_Data_Area_Style.margin.left,
                (row * MyWindowGUI.prefab_Data_Area_Size + MyWindowGUI.prefab_Data_Area_Style.margin.top * row) + MyWindowGUI.prefab_Data_Area_Style.margin.left,
                MyWindowGUI.prefab_Data_Area_Size,
                MyWindowGUI.prefab_Data_Area_Size);
            using (var prefab_Section_Scope = new GUILayout.AreaScope(Prefab_Rect)) {

                GUIContent content = new GUIContent(prefab_DataCollection.prefabs[index].name, AssetPreview.GetAssetPreview(prefab_DataCollection.prefabs[index]));
                if (GUILayout.Button(content, MyWindowGUI.prefab_Data_Button_Style, GUILayout.ExpandHeight(true))) {

                }
            }

        }

    }
}
