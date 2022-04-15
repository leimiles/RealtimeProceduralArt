using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miles {
    public enum Toolbar_Tabs {
        Draw, Delete, Repaint
    }

    public enum Brush_Instantiate_Mode {
        Click_Single, Drag_Single, Click_Group, Drag_Group
    }

    public enum Brush_Delete_Mode {
        Delete_All, Delete_Selected
    }
    public class Prefab_Data {
        public static Dictionary<int, Prefab_Data> resources_Pool;
        public static Dictionary<int, int> indexToID;
        public static void Init(uint number) {
            Prefab_Data.resources_Pool = new Dictionary<int, Prefab_Data>();
            Prefab_Data.indexToID = new Dictionary<int, int>();
        }
        public static int count = 0;
        public static int GetIDFromIndex(int index) {
            return indexToID[index];
        }
        public GameObject instance;
        public BrushSettings brushSettings;
        public bool is_Activated;
        public static void Clear() {
            if (resources_Pool != null) {
                resources_Pool.Clear();
            }
            if (indexToID != null) {
                indexToID.Clear();
            }
            Prefab_Data.count = 0;
        }
        string name;
        public string Name {
            get {
                return name;
            }
        }
        int id;
        public int ID {
            get {
                return id;
            }
        }
        private Texture2D thumbnail;
        public Texture2D Thumbnail {
            get {
                return thumbnail;
            }
        }
        public Prefab_Data(GameObject gameObject, Texture2D thumbnail) {
            this.instance = gameObject;
            this.thumbnail = thumbnail;
            this.name = gameObject.name;
            this.id = this.instance.GetInstanceID();
            this.brushSettings = new BrushSettings();
            Prefab_Data.count++;
        }

    }
}