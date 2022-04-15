using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miles {
    [CreateAssetMenu(fileName = "PlacementInfo", menuName = "FunnySpawn/PlacementInfo", order = 1)]
    public class PlacementInfoManager : ScriptableObject {
        public string Name;
        public List<PlacementInfo> placementInfos;
    }
}