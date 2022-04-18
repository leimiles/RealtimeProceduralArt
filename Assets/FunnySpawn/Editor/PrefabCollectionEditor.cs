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
            DrawDragPrefabSection();
            if (!prefab_DataCollection) {
                GUILayout.Label("Data Error");
            }
            if (prefab_DataCollection.prefabs == null || prefab_DataCollection.prefabs.Count == 0) {
                GUILayout.Label("No Data");
                return;
            }

            //base.OnInspectorGUI();
        }

        void DrawDragPrefabSection() {
            GUILayout.Box("wazzup", GUILayout.Width(EditorGUIUtility.currentViewWidth), GUILayout.Height(300));
        }
    }
}
