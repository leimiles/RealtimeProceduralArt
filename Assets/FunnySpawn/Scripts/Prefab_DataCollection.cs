using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miles {
    [CreateAssetMenu(fileName = "PrefabCollection", menuName = "FunnySpawn/PrefabCollection", order = 1)]
    public class Prefab_DataCollection : ScriptableObject {
        public string Name;
        public int category_ID;
        public List<GameObject> prefabs;
    }
}
