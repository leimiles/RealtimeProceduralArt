using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace miles {
    public class Prefab_Tool_Editor : EditorWindow {
        Brush_Instantiate_Mode brush_Instantiate_Mode = Brush_Instantiate_Mode.Click_Single;
        Brush_Delete_Mode brush_Delete_Mode = Brush_Delete_Mode.Delete_All;
        PlacementInfoManager placementInfoManager;
        static string placementInfoFilePath = "Assets/" + "FunnySpawn/Data/" + "PlacementInfo.asset";
        static string[] layerMaskNames = { "miles" };
        static EditorWindow main_Window;
        private Toolbar_Tabs active_Tab = Toolbar_Tabs.Draw;
        [MenuItem("Window/Funny Spawn By Miles")]
        public static void ShowWindow() {
            main_Window = GetWindow(typeof(Prefab_Tool_Editor), false, "Funny Spawn", true);
        }

        // called everytime when the custom window is focused
        public void OnFocus() {
#if UNITY_2018_4
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
#else
            SceneView.duringSceneGui -= this.OnSceneGUI;
            SceneView.duringSceneGui += this.OnSceneGUI;
#endif
        }

        private void LoadResources() {
            Prefab_Data.Init(0);
        }

        // called once the window is created
        public void Awake() {
            LoadResources();
        }

        private void SetGUIStyles() {
            if (!MyWindowGUI.is_Init) {
                // must be called in OnGUI function
                MyWindowGUI.Init();

            }
        }

        public void OnGUI() {
            SetGUIStyles();
            EditorGUILayout.BeginVertical();
            DrawHeaders();
            DrawTips();
            EditorGUILayout.BeginHorizontal();
            DrawPlacementMangerField();
            DrawSceneRootField();
            DrawPrefabCollectionField();
            EditorGUILayout.EndHorizontal();
            placementInfoManager = AssetDatabase.LoadAssetAtPath<PlacementInfoManager>(placementInfoFilePath);
            if (!placementInfoManager) {
                GUILayout.Label("Add PlacementInfoManger Please...");
                return;
            } else {
                if (placementInfoManager.placementInfos == null) {
                    placementInfoManager.placementInfos = new List<PlacementInfo>();
                }
            }

            DrawToolbars();
            EditorGUILayout.Space();
            switch (active_Tab) {
                case Toolbar_Tabs.Draw:
                    EditorGUILayout.BeginHorizontal();
                    DrawClearPrefabDataButton();
                    DrawSavePrefabDataButton();
                    DrawExportSpawnListButton();
                    EditorGUILayout.EndHorizontal();
                    DrawBrushIntantiateMode();
                    DrawPrefabSection();
                    if (Prefab_Data.resources_Pool != null && Prefab_Data.resources_Pool.Count > 0) {
                        foreach (int key in Prefab_Data.resources_Pool.Keys) {
                            if (Prefab_Data.resources_Pool[key].is_Activated) {
                                MyWindowGUI.scrollViewPos_PlacementParameters = EditorGUILayout.BeginScrollView(MyWindowGUI.scrollViewPos_PlacementParameters, false, false, GUILayout.Height(main_Window.position.height - (MyWindowGUI.prefabs_Section_Height + 120)));
                                DrawPlacementParameter(Prefab_Data.resources_Pool[key].Name, Prefab_Data.resources_Pool[key].brushSettings);
                                EditorGUILayout.EndScrollView();
                            }
                        }
                    }
                    break;
                case Toolbar_Tabs.Delete:
                    DrawBrushDeleteMode();
                    //DrawPrefabSection();
                    DrawTempTips();

                    break;
                case Toolbar_Tabs.Repaint:
                    DrawTempTips();
                    break;

            }
            GUI.color = Color.white;
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
            //Debug.Log(active_Tab.ToString());

        }

        void DrawClearPrefabDataButton() {
            if (GUILayout.Button("Clear Prefabs Data")) {
                if (Prefab_Data.resources_Pool != null && Prefab_Data.resources_Pool.Count > 0) {
                    Prefab_Data.resources_Pool.Clear();
                }
            }

        }

        void DrawSavePrefabDataButton() {
            if (GUILayout.Button("Save Prefabs Data")) {
                if (prefab_DataCollection == null || prefab_DataCollection.prefabs == null) {
                    return;
                }

                foreach (Prefab_Data data in Prefab_Data.resources_Pool.Values) {
                    if (!prefab_DataCollection.prefabs.Contains(data.instance)) {
                        prefab_DataCollection.prefabs.Add(data.instance);
                    }
                }

            }

        }

        void DrawExportSpawnListButton() {
            if (GUILayout.Button("Export Spawn Data")) {
                if (placementInfoManager != null && placementInfoManager.placementInfos != null) {
                    Debug.Log("Find PlacementInfo.asset file that helps!");
                    EditorGUIUtility.PingObject(placementInfoManager);
                }

            }

        }

        void SetPrefabDataButtonColor(int ID) {
            if (!Prefab_Data.resources_Pool.ContainsKey(ID)) {
                Debug.Log("No Such ID: " + ID);
                return;
            }
            if (Prefab_Data.resources_Pool[ID].is_Activated == true) {
                GUI.color = MyWindowGUI.prefab_Data_Button_Activated_Colr;
                //MyWindowGUI.prefab_Data_Button_Style.normal.background = Texture2D.blackTexture;
            } else {
                GUI.color = MyWindowGUI.prefab_Data_Button_Deactivated_Color;
                //MyWindowGUI.prefab_Data_Button_Style.normal.background = Texture2D.whiteTexture;
            }
        }

        void SetupSpawnerIDs() {
            if (Spawner.IDs == null) {
                Spawner.IDs = new List<int>();
            } else {
                Spawner.IDs.Clear();
            }
            foreach (int key in Prefab_Data.resources_Pool.Keys) {
                if (Prefab_Data.resources_Pool[key].is_Activated) {
                    Spawner.IDs.Add(key);
                }
            }
            //Debug.Log(Spawner.IDs.Count);
        }

        static Mesh preview_Mesh;
        void SetupPreviewMesh_Combined2(int ID) {
            MeshFilter[] meshFilters = Prefab_Data.resources_Pool[ID].instance.GetComponentsInChildren<MeshFilter>();
            SkinnedMeshRenderer[] skinnedMeshRenderers = Prefab_Data.resources_Pool[ID].instance.GetComponentsInChildren<SkinnedMeshRenderer>();
            List<CombineInstance> combineInstances = new List<CombineInstance>();
            foreach (MeshFilter meshFilter in meshFilters) {
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = meshFilter.sharedMesh;
                //combineInstance.transform = Matrix4x4.identity;
                combineInstance.transform = meshFilter.transform.localToWorldMatrix;
                combineInstances.Add(combineInstance);
            }
            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers) {
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = skinnedMeshRenderer.sharedMesh;
                combineInstance.transform = skinnedMeshRenderer.transform.localToWorldMatrix;
                combineInstances.Add(combineInstance);
            }

            preview_Mesh = new Mesh();
            preview_Mesh.CombineMeshes(combineInstances.ToArray());
        }

        void SetPrefabData_Active(int ID, Brush_Instantiate_Mode mode = Brush_Instantiate_Mode.Click_Single) {
            switch (mode) {
                case Brush_Instantiate_Mode.Click_Single:
                    if (Prefab_Data.resources_Pool[ID].is_Activated == true) {
                        Prefab_Data.resources_Pool[ID].is_Activated = false;

                    } else {
                        foreach (int key in Prefab_Data.resources_Pool.Keys) {
                            Prefab_Data.resources_Pool[key].is_Activated = false;
                        }
                        Prefab_Data.resources_Pool[ID].is_Activated = true;
                        SetupPreviewMesh_Combined2(ID);
                    }
                    break;
                default:
                    break;
            }
            SetupPreviewMaterial();
            SetupSpawnerIDs();

        }

        static Material preview_Material;
        void SetupPreviewMaterial() {
            if (preview_Material == null) {
                preview_Material = new Material(Shader.Find("Sofunny/shd_Preview_V1"));
            }
        }

        bool IsBrushHit(Vector3 origin, Vector3 direction, out RaycastHit hit) {
            // this method return all raycast hit object if 
            return Physics.Raycast(origin, direction, out hit, Mathf.Infinity, LayerMask.GetMask(layerMaskNames));
        }

        void KeyboardRecation(BrushSettings brushSettings) {
            Event current = Event.current;
            if (current.type != EventType.KeyDown) {
                return;
            }
            switch (current.keyCode) {
                case KeyCode.LeftBracket:
                    brushSettings.radiusScaler -= 0.05f;
                    break;
                case KeyCode.RightBracket:
                    brushSettings.radiusScaler += 0.05f;
                    break;
                case KeyCode.Equals:
                    brushSettings.heightOffset += 0.1f;
                    break;
                case KeyCode.Minus:
                    brushSettings.heightOffset -= 0.1f;
                    break;
                case KeyCode.Comma:
                    if (brushSettings.use_FixedRotation) {
                        brushSettings.angleOffset += 15.0f;
                    }
                    break;
                case KeyCode.Period:
                    if (brushSettings.use_FixedRotation) {
                        brushSettings.angleOffset -= 15.0f;
                    }
                    break;
                case KeyCode.Semicolon:
                    if (!brushSettings.use_RandomScale) {
                        brushSettings.unitformScaleOffset -= 0.1f;
                    }
                    break;
                case KeyCode.Quote:
                    if (!brushSettings.use_RandomScale) {
                        brushSettings.unitformScaleOffset += 0.1f;
                    }
                    break;
                default:
                    break;
            }
        }

        void AddPrefabData(Object[] objects) {
            for (int i = 0; i < objects.Length; i++) {
                if (objects[i].GetType().Name == "GameObject") {
                    GameObject obj = (GameObject)objects[i];
                    if (Prefab_Data.resources_Pool.ContainsKey(obj.GetInstanceID())) {
                        Debug.Log("有了啊");
                        continue;
                    } else {
                        int ID = obj.GetInstanceID();
                        Prefab_Data.indexToID[Prefab_Data.count] = ID;
                        Prefab_Data.resources_Pool[ID] = new Prefab_Data(obj, AssetPreview.GetAssetPreview(objects[i]));
                    }
                }
            }
        }

        #region GUI

        void DrawPlacementMangerField() {
            GUILayout.Label("Recorder: ", GUILayout.Width(60));
            placementInfoManager = (PlacementInfoManager)EditorGUILayout.ObjectField(placementInfoManager, typeof(PlacementInfoManager), false);
        }
        static GameObject sceneRoot;
        void DrawSceneRootField() {
            GUILayout.Label("Scene Root: ", GUILayout.Width(75));
            sceneRoot = (GameObject)EditorGUILayout.ObjectField(sceneRoot, typeof(GameObject), true);
        }
        static Prefab_DataCollection prefab_DataCollection;
        void DrawPrefabCollectionField() {
            GUILayout.Label("Prefabs Datas: ", GUILayout.Width(80));
            prefab_DataCollection = (Prefab_DataCollection)EditorGUILayout.ObjectField(prefab_DataCollection, typeof(Prefab_DataCollection), false);
            if (GUILayout.Button("Load", GUILayout.Width(80))) {
                /*
                if (prefab_DataCollection == null || prefab_DataCollection.prefabs == null) {
                    return;
                }
                int count = prefab_DataCollection.prefabs.Count;
                Object[] objects = new Object[count];
                for (int i = 0; i < count; i++) {
                    objects[i] = prefab_DataCollection.prefabs[i];
                }
                */
                //need new load function

                //AddPrefabData(objects);
            }
        }

        void DrawBrushIntantiateMode() {
            brush_Instantiate_Mode = (Brush_Instantiate_Mode)EditorGUILayout.EnumPopup("Brush Mode: ", brush_Instantiate_Mode);
        }
        void DrawBrushDeleteMode() {
            brush_Delete_Mode = (Brush_Delete_Mode)EditorGUILayout.EnumPopup("Brush Mode: ", brush_Delete_Mode, GUILayout.Width(MyWindowGUI.brushModePopupWidth));
        }
        void DrawPlacementParameter(string name, BrushSettings brushSettings) {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Foldout(true, name);
            // zero line
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(MyWindowGUI.foldoutIndentWidth));
            brushSettings.use_WorldY = GUILayout.Toggle(brushSettings.use_WorldY, "Use World Y", GUILayout.Width(200));
            brushSettings.use_AlignedDirection = GUILayout.Toggle(brushSettings.use_AlignedDirection, "Align Direction", GUILayout.Width(200));
            EditorGUILayout.EndHorizontal();

            // second line
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(MyWindowGUI.foldoutIndentWidth));
            brushSettings.heightOffset = EditorGUILayout.FloatField("Height Offset: ", brushSettings.heightOffset, GUILayout.Width(200));

            brushSettings.unitformScaleOffset = EditorGUILayout.FloatField("Scale Offset: ", brushSettings.unitformScaleOffset, GUILayout.Width(200));
            if (brushSettings.unitformScaleOffset < 0.1f) {
                brushSettings.unitformScaleOffset = 0.1f;
            }

            EditorGUILayout.EndHorizontal();

            // first line
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(MyWindowGUI.foldoutIndentWidth));
            brushSettings.use_FixedRotation = EditorGUILayout.BeginToggleGroup("Use Fixed Rotation", brushSettings.use_FixedRotation);
            brushSettings.angleOffset = EditorGUILayout.FloatField("Angle Offset: ", brushSettings.angleOffset, GUILayout.Width(200));
            if (Mathf.Abs(brushSettings.angleOffset) > 360.0f) {
                brushSettings.angleOffset = brushSettings.angleOffset % 360.0f;
            }
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            // third line
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(MyWindowGUI.foldoutIndentWidth));

            brushSettings.use_RandomScale = EditorGUILayout.BeginToggleGroup("Use Random Scale", brushSettings.use_RandomScale);
            brushSettings.scale_Min = EditorGUILayout.FloatField("Scale Offset Min: ", brushSettings.scale_Min, GUILayout.Width(200));
            if (brushSettings.scale_Min >= brushSettings.scale_Max) {
                brushSettings.scale_Min = brushSettings.scale_Max;
            }
            if (brushSettings.scale_Min < 0.1f) {
                brushSettings.scale_Min = 0.1f;
            }
            brushSettings.scale_Max = EditorGUILayout.FloatField("Scale Offset Max: ", brushSettings.scale_Max, GUILayout.Width(200));
            if (brushSettings.scale_Max < brushSettings.scale_Min) {
                brushSettings.scale_Max = brushSettings.scale_Min;
            }
            if (brushSettings.scale_Max < 0.1f) {
                brushSettings.scale_Max = 0.1f;
            }
            EditorGUILayout.EndToggleGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

        }

        void DrawTips() {
            GUILayout.Label("Tips: \"[\" or \"]\" to adjust bursh size. \"-\" or \"+\" to adjust height offset. \" ; \" or \" ' \" to adjust size offset.\" < \" or \" > \" to adjust angle offset.");
        }
        void DrawTempTips() {
            GUILayout.Label("施工中...");
        }

        static bool isMouseDrag = false;
        static bool isMouseDown = false;
        static bool isMouseUp = false;
        static bool isMouseLeaveWindow = false;

        static void SetMouseAndKeyState() {
            isMouseDrag = Event.current.type == EventType.MouseDrag && Event.current.button == 0;
            isMouseDown = Event.current.type == EventType.MouseDown && Event.current.button == 0;
            isMouseUp = Event.current.type == EventType.MouseUp && Event.current.button == 0;
            isMouseLeaveWindow = Event.current.type == EventType.MouseLeaveWindow;
        }

        void ShowDrawBrush() {
            if (Prefab_Data.resources_Pool == null || Spawner.IDs == null) {
                return;
            }
            if (Prefab_Data.resources_Pool.Count > 0 && Spawner.IDs.Count > 0) {
                Ray brushRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit brushHitOn;
                if (IsBrushHit(brushRay.origin, brushRay.direction, out brushHitOn)) {
                    SceneView.RepaintAll();
                    SetMouseAndKeyState();
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                    if (Event.current.modifiers == EventModifiers.Alt) {
                    } else {
                        switch (brush_Instantiate_Mode) {
                            case Brush_Instantiate_Mode.Click_Single:
                                int ID = Spawner.IDs[0];
                                KeyboardRecation(Prefab_Data.resources_Pool[ID].brushSettings);
                                DrawPreview(ID, Prefab_Data.resources_Pool[ID].brushSettings);
                                UpdateBrushSettings(Prefab_Data.resources_Pool[ID].brushSettings, brushHitOn);
                                DrawBrush_ClickSingle(Prefab_Data.resources_Pool[ID].brushSettings);
                                if (isMouseDown) {
                                    Spawner.InstantiateByIDs(placementInfoManager, sceneRoot);
                                    UpdateBrushSettings(Prefab_Data.resources_Pool[ID].brushSettings);
                                }
                                break;
                            case Brush_Instantiate_Mode.Drag_Single:
                                break;
                            case Brush_Instantiate_Mode.Click_Group:
                                break;
                            case Brush_Instantiate_Mode.Drag_Group:
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
        }

        void UpdateBrushSettings(BrushSettings brushSettings, RaycastHit hit) {
            brushSettings.origin = hit.point;
            brushSettings.target = hit.point;
            if (brushSettings.use_WorldY) {
                brushSettings.target.y += brushSettings.heightOffset;
            } else {
                brushSettings.target += hit.normal * brushSettings.heightOffset;
            }
            brushSettings.label = brushSettings.target.ToString() + " | " + brushSettings.direction.ToString();
            if (brushSettings.use_WorldY) {
                brushSettings.direction = Vector3.up;
            } else {
                brushSettings.direction = hit.normal;
            }

        }
        void UpdateBrushSettings(BrushSettings brushSettings) {
            if (brushSettings.use_RandomScale) {
                brushSettings.unitformScaleOffset = UnityEngine.Random.Range(brushSettings.scale_Min, brushSettings.scale_Max);
            }
            if (!brushSettings.use_FixedRotation) {
                brushSettings.angleOffset = UnityEngine.Random.Range(0.0f, 360.0f);
            }
        }

        void DrawPreview(int ID, BrushSettings brushSettings) {
            if (preview_Material && preview_Mesh) {
                preview_Material.SetPass(0);
                Graphics.DrawMeshNow(preview_Mesh, brushSettings.GetTransform());
            }

        }

        void DrawBrush_ClickSingle(BrushSettings brushSettings) {
            Handles.color = brushSettings.color;
            Handles.DrawWireDisc(brushSettings.origin, brushSettings.direction, brushSettings.radius * brushSettings.radiusScaler);
            Handles.color = Color.grey;
        }


        void DrawPrefabSection() {
            Repaint();
            if (Prefab_Data.resources_Pool == null) {
                return;
            }
            MyWindowGUI.scrollViewPos_PrefabDatas = EditorGUILayout.BeginScrollView(MyWindowGUI.scrollViewPos_PrefabDatas, false, false, GUILayout.Height(MyWindowGUI.prefabs_Section_Height));
            int column = Mathf.FloorToInt(main_Window.position.width / MyWindowGUI.prefab_Data_Area_Size);
            int row;
            if (column > Prefab_Data.resources_Pool.Count) {
                row = 1;
            } else {
                row = Mathf.CeilToInt(Prefab_Data.resources_Pool.Count * 1.0f / column * 1.0f);
            }
            // this is just background
            Rect background_Rect = new Rect(0, 0, main_Window.position.width, MyWindowGUI.prefab_Data_Area_Size * (row + 1) + 10 * row);
            GUI.Box(background_Rect, "");
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < row; i++) {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < column; j++) {
                    int index = i * column + j;
                    if (index < Prefab_Data.resources_Pool.Count) {
                        DrawPrefabDataUI(Prefab_Data.GetIDFromIndex(index), i, j);
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
                AddPrefabData(DragAndDrop.objectReferences);
            }
            EditorGUILayout.EndScrollView();
            GUI.color = Color.white;

        }

        void DrawPrefabDataUI(int ID, int row, int column) {
            if (!Prefab_Data.resources_Pool.ContainsKey(ID)) {
                Debug.Log("No such ID: " + ID);
                return;
            }
            // render the box just because the scroll view can't work via guilayout.Area
            GUILayout.Box("", MyWindowGUI.prefab_Data_Area_Style, GUILayout.Width(MyWindowGUI.prefab_Data_Area_Size), GUILayout.Height(MyWindowGUI.prefab_Data_Area_Size));
            Rect PrefabData_Rect = new Rect(
                (column * MyWindowGUI.prefab_Data_Area_Size + MyWindowGUI.prefab_Data_Area_Style.margin.left * column) + MyWindowGUI.prefab_Data_Area_Style.margin.left,
                (row * MyWindowGUI.prefab_Data_Area_Size + MyWindowGUI.prefab_Data_Area_Style.margin.top * row) + MyWindowGUI.prefab_Data_Area_Style.margin.left,
                MyWindowGUI.prefab_Data_Area_Size,
                MyWindowGUI.prefab_Data_Area_Size);
            using (var prefab_Section_Scope = new GUILayout.AreaScope(PrefabData_Rect)) {
                SetPrefabDataButtonColor(ID);
                GUIContent content = new GUIContent(Prefab_Data.resources_Pool[ID].Name, Prefab_Data.resources_Pool[ID].Thumbnail);
                if (GUILayout.Button(content, MyWindowGUI.prefab_Data_Button_Style, GUILayout.ExpandHeight(true))) {
                    SetPrefabData_Active(ID, brush_Instantiate_Mode);
                }
            }
        }


        void DrawHeaders() {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Funny Spawn By Miles", MyWindowGUI.header_Style);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void DrawToolbars() {
            EditorGUILayout.BeginHorizontal();
            SetTabColor(Toolbar_Tabs.Draw);
            if (GUILayout.Button("Draw")) {
                SetTabActive(Toolbar_Tabs.Draw);
            }
            SetTabColor(Toolbar_Tabs.Delete);
            if (GUILayout.Button("Delete")) {
                SetTabActive(Toolbar_Tabs.Delete);
            }
            SetTabColor(Toolbar_Tabs.Repaint);
            if (GUILayout.Button("Repaint")) {
                SetTabActive(Toolbar_Tabs.Repaint);
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion
        void SetTabColor(Toolbar_Tabs tab) {
            if (tab == active_Tab) {
                GUI.color = Color.gray;
            } else {
                GUI.color = Color.white;
            }
        }

        void SetTabActive(Toolbar_Tabs tab) {
            active_Tab = tab;
        }


        // this is not a default callback function, then
        void OnSceneGUI(SceneView sceneView) {
            ShowDrawBrush();
        }

        // called one the window is closed
        void OnDestroy() {
            Prefab_Data.Clear();
#if UNITY_2018_4
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
#else
            SceneView.duringSceneGui -= this.OnSceneGUI;
#endif
        }

    }
}