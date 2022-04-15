using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miles {
    [System.Serializable]
    public class PlacementInfo {
        public int belong_ID;
        public int prefab_ID;
        public string Name;
        public int ID;
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
        public PlacementInfo(int prefab_ID, int ID, int belong_ID = 0) {
            this.prefab_ID = prefab_ID;
            this.belong_ID = belong_ID;
            this.Name = "";
            this.ID = ID;
            this.position = Vector3.zero;
            this.eulerAngles = Vector3.zero;
            this.scale = Vector3.one;
        }
    }
}
