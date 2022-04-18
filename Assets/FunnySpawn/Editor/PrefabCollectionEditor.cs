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
            if (!prefab_DataCollection) {
                GUILayout.Label("Data Error");
                return;
            }
            DisplayDragSection();
            if (GUILayout.Button("Clear")) {
                if (prefab_DataCollection.prefabs != null || prefab_DataCollection.prefabs.Count > 0) {
                    prefab_DataCollection.prefabs.Clear();
                }
            }
            DisplayPreview();
            base.OnInspectorGUI();
        }

        void DisplayPreview() {
        }

        void DisplayDragSection() {

            MyWindowGUI.scrollViewPos_PrefabDatas = EditorGUILayout.BeginScrollView(MyWindowGUI.scrollViewPos_PrefabDatas, false, false, GUILayout.Height(MyWindowGUI.prefabs_Section_Height));
            // this is just background
            Rect background_Rect = new Rect(0, 0, EditorGUIUtility.currentViewWidth, MyWindowGUI.Prefab_Data_Collection_DragArea_Height);
            GUI.Box(background_Rect, "Drag Prefabs Here", MyWindowGUI.prefab_Data_Collection_Style);


            if (Event.current.type == EventType.DragUpdated && background_Rect.Contains(Event.current.mousePosition)) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                Event.current.Use();
            }

            if (Event.current.type == EventType.DragPerform && background_Rect.Contains(Event.current.mousePosition)) {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
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
    }
}
