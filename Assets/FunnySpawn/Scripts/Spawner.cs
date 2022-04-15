using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

namespace miles {
    public class Spawner {
        public static List<int> IDs;

        public static void InstantiateByIDs(PlacementInfoManager placementInfoManager, GameObject sceneRoot = null) {
            if (placementInfoManager == null) {
                Debug.Log("placementManager is not ready");
                return;
            }
            if (IDs == null) {
                Debug.Log("nothing in activated IDs");
                return;
            }
            foreach (int id in IDs) {
                BrushSettings brushSettings = Prefab_Data.resources_Pool[id].brushSettings;
                GameObject obj = Prefab_Data.resources_Pool[id].instance;

                GameObject clone = PrefabUtility.InstantiatePrefab(obj) as GameObject;
                PlacementInfo placementInfo = new PlacementInfo(obj.GetInstanceID(), clone.GetInstanceID());
                Undo.RecordObject(placementInfoManager, "add" + clone.name);

                placementInfo.position = brushSettings.target;
                clone.transform.position = brushSettings.target;

                placementInfo.eulerAngles = brushSettings.EulerAngles;
                clone.transform.eulerAngles = brushSettings.EulerAngles;

                placementInfo.scale = Vector3.one * brushSettings.unitformScaleOffset;
                clone.transform.localScale = Vector3.one * brushSettings.unitformScaleOffset;

                placementInfo.Name = obj.name;

                EditorCoroutineUtility.StartCoroutineOwnerless(ScaleAnim2(clone.transform, Vector3.one * brushSettings.unitformScaleOffset));

                if (sceneRoot) {
                    clone.transform.SetParent(sceneRoot.transform);
                }

                placementInfoManager.placementInfos.Add(placementInfo);

                Undo.RegisterCreatedObjectUndo(clone, "brush stroke: " + clone.name);

            }
        }

        static IEnumerator ScaleAnim(Transform transform, Vector3 targetScale) {
            transform.localScale = Vector3.one * 0.1f;
            int times = Mathf.CeilToInt(targetScale.x / transform.localScale.x);
            float scaleStep = 0.1f;
            Debug.Log(times);
            int i = 1;
            while (i < times) {
                transform.localScale += Vector3.one * scaleStep;
                i++;
                yield return null;
            }
        }

        static IEnumerator ScaleAnim2(Transform transform, Vector3 targetScale) {
            transform.localScale = Vector3.one * 0.1f;
            int times = 20;
            float scaleStep = (targetScale.x - transform.localScale.x) / times;
            int i = 0;
            while (i < times) {
                transform.localScale += Vector3.one * scaleStep;
                i++;
                yield return null;
            }

        }

    }
}
