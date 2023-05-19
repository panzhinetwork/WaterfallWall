using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

namespace UIFramework.Editor {

    public static class UITools {

        [MenuItem("Assets/Create/UIFramework/UI Pack Prefab", priority = 1)]
        public static void CreateUIPackPrefab() {
            var uiPack = CreateUIPack();

            string prefabPath = EditorUtility.SaveFilePanel("UI Pack Prefab", GetCurrentPath(), "UIPack", "prefab");

            // Convert absolute path to relative path.
            if (prefabPath.StartsWith(Application.dataPath, System.StringComparison.Ordinal)) {
                prefabPath = prefabPath.Replace(Application.dataPath, "Assets");
            }

            if (!string.IsNullOrEmpty(prefabPath)) {
                PrefabUtility.SaveAsPrefabAsset(uiPack, prefabPath);
            }

            Object.DestroyImmediate(uiPack);
        }

        [MenuItem("GameObject/UIFramework/UI Pack in Scene", priority = 1)]
        public static void CreateUIPackInScene() {
            CreateUIPack();
        }

        [MenuItem("GameObject/UIFramework/UI Devolop Layer", priority = 2)]
        public static void CreateUIDevolopEnvironment() {
            var uiLayer = LayerMask.NameToLayer("UI");
            var root = new GameObject("UIRoot") {
                layer = uiLayer,
            };

            var cam = CreateUICamera(root.transform, uiLayer);

            CreateEventSystem(root.transform);

            // Create UILayer
            var layer = UILayer.CreateLayerGameObject(root.transform, UILayerType.Normal, cam, new Vector2(720, 1280));
            layer.gameObject.name = "UILayer_dev";
        }

        private static GameObject CreateUIPack() {
            var uiLayer = LayerMask.NameToLayer("UI");

            var root = new GameObject("UIPack") {
                layer = uiLayer,
            };
            var uiPack = root.AddComponent<UIPack>();

            // Create UICamera
            var cam = CreateUICamera(root.transform, uiLayer);

            // Create EventSystem
            CreateEventSystem(root.transform);

            // Create UILayers
            var layerManager = CreateUILayerManager(root.transform, uiLayer, cam);

            SetPrivateField(uiPack, layerManager, "_layerManager");

            return root;
        }

        private static Camera CreateUICamera(Transform parent, int uiLayer) {
            var cam = new GameObject("UICamera") {
                layer = uiLayer,
                transform = { 
                    parent = parent, 
                    localPosition = new Vector3(0, 0, -100) 
                }
            }.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.cullingMask = LayerMask.GetMask("UI");
            cam.orthographic = true;
            cam.depth = 5f;
            cam.nearClipPlane = -50f;
            cam.farClipPlane = 200f;

            return cam;
        }

        private static void CreateEventSystem(Transform parent) {
            var eventSystem = new GameObject("EventSystem") {
                transform = { parent = parent }
            };
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static UIViewPresenter CreateUILayerManager(Transform parent, int uiLayer, Camera uiCamera) {
            var go = new GameObject("UILayerManager") {
                transform = { parent = parent },
                layer = uiLayer,
            };
            var uiLayers = go.AddComponent<UIViewPresenter>();
            var layerArray = UIViewPresenter.CreateAllLayerGameObjects(go.transform, uiCamera, new Vector2(720, 1280));

            SetPrivateField(uiLayers, layerArray, "_layers");

            return uiLayers;
        }

        private static string GetCurrentPath() {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "") {
                path = "Assets";
            } else if (Path.GetExtension(path) != "") {
                path = Path.GetDirectoryName(path);
            }

            return path;
        }

        private static void SetPrivateField(object target, object element, string fieldName) {
            var field = target.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic
                | System.Reflection.BindingFlags.Instance);
            field.SetValue(target, element);
        }
    }

}