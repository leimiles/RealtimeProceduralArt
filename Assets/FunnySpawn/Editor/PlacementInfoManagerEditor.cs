using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace miles {
    [CustomEditor(typeof(PlacementInfoManager))]
    public class PlacementInfoManagerEditor : Editor {
        PlacementInfoManager placementInfoManager;
        public void OnEnable() {
            placementInfoManager = target as PlacementInfoManager;
        }
        public override void OnInspectorGUI() {
            if (!placementInfoManager) {
                GUILayout.Label("Data Error");
                return;
            }
            if (placementInfoManager.placementInfos == null || placementInfoManager.placementInfos.Count == 0) {
                GUILayout.Label("No Data");
                return;
            }
            if (!MyWindowGUI.is_Init) {
                MyWindowGUI.Init();
            }
            if (GUILayout.Button("Export")) {
                Reorder();
                ExportTxt();
            }
            if (GUILayout.Button("Clear")) {
                placementInfoManager.placementInfos.Clear();
            }
            placementInfoManager.Name = GUILayout.TextField(placementInfoManager.Name);
            GUILayout.Label("File Name: " + placementInfoManager.Name + ".txt");
            GUILayout.Label("Spawn Count: " + placementInfoManager.placementInfos.Count);
            GUILayout.Label("Spawn Lists: ");
            foreach (PlacementInfo info in placementInfoManager.placementInfos) {
                GUILayout.Label("    " + GetOutputString(info));
            }
            //base.OnInspectorGUI();

        }

        float GetLow2Float(float number) {
            return (float)System.Math.Round(number, 2);
        }

        string GetOutputString(PlacementInfo info) {
            string result = "";
            result += info.Name;
            result += "|";
            result += GetLow2Float(info.position.x) + ",";
            result += GetLow2Float(info.position.y) + ",";
            result += GetLow2Float(info.position.z);
            result += "|";
            result += GetLow2Float(info.eulerAngles.x) + ",";
            result += GetLow2Float(info.eulerAngles.y) + ",";
            result += GetLow2Float(info.eulerAngles.z);
            result += "|";
            result += GetLow2Float(info.scale.x) + "|";
            result += GetLow2Float(info.scale.y);
            return result;
        }

        void ExportTxt() {
            if (placementInfoManager.Name == null || placementInfoManager.Name == "") {
                Debug.Log("Need a file name.");
                return;
            }
            string pathFolder = Application.dataPath + "/FunnySpawn/Export/";
            string path = pathFolder + placementInfoManager.Name + ".txt";

            if (Directory.Exists(pathFolder)) {
                StreamWriter streamWriter = File.CreateText(path);
                foreach (PlacementInfo info in placementInfoManager.placementInfos) {
                    streamWriter.WriteLine(GetOutputString(info));
                }
                streamWriter.Close();

            } else {
                Debug.Log("Can't find folder");
            }

        }



        void Reorder() {
            placementInfoManager.placementInfos.Sort(
                (e1, e2) => {
                    return e2.prefab_ID.CompareTo(e1.prefab_ID);
                }
            );
        }
    }

}
