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
                Matrix4x4 translate;
                translate = Matrix4x4.Translate(clone.transform.position);
                translate = translate * Matrix4x4.Translate(brushSettings.target);
                clone.transform.position = translate.GetColumn(3);
                placementInfo.position = clone.transform.position;

                Matrix4x4 rotate;
                rotate = Matrix4x4.Rotate(clone.transform.rotation);
                rotate = Matrix4x4.Rotate(brushSettings.Final_Quaternion) * rotate;
                clone.transform.eulerAngles = rotate.rotation.eulerAngles;
                placementInfo.eulerAngles = clone.transform.eulerAngles;

                Matrix4x4 scale;
                scale = Matrix4x4.Scale(clone.transform.localScale);
                scale = scale * Matrix4x4.Scale(Vector3.one * brushSettings.unitformScaleOffset);
                clone.transform.localScale = scale.lossyScale;
                placementInfo.scale = clone.transform.localScale;

                placementInfo.Name = obj.name;

                EditorCoroutineUtility.StartCoroutineOwnerless(ScaleAnim2(clone.transform, clone.transform.localScale));

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
