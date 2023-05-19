using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UIFramework.UGUI_Extension.Editor {
    public static class UGUI_ExtensionTools {
        [MenuItem ("GameObject/Fancy UGUI Extension/Large Hit Rect Button", priority = 1)]
        public static void CreateLargeHitRectButton () {
            Transform parent = GetValidUIParent ();
            // Create the UI object.
            var buttonObj = new GameObject ("Button") {
                transform = { parent = parent },
                layer = LayerMask.NameToLayer ("UI"),
            };
            buttonObj.transform.localScale = Vector3.one;
            var image = buttonObj.AddComponent<Image> ();
            image.raycastTarget = true;
            var button = buttonObj.AddComponent<Button> ();
            button.targetGraphic = image;

            CreateHitRectOnParent (buttonObj.transform);

            Selection.activeGameObject = buttonObj;
        }

        [MenuItem ("GameObject/Fancy UGUI Extension/Hit Rect", priority = 2)]
        public static void CreateHitRect () {
            Transform parent = GetValidUIParent ();
            CreateHitRectOnParent (parent);
        }

        private static Transform GetValidUIParent () {
            var selectedObject = Selection.activeGameObject;
            if (selectedObject != null && selectedObject.GetComponent<RectTransform> () != null) {
                return selectedObject.transform;
            } else {
                // Find the canvas and create the UI GameObject
                var canvas = Object.FindObjectOfType<Canvas> ();
                if (canvas == null) {
                    // Crteate a new canvas.
                    var go = new GameObject ("Canvas") {
                    layer = LayerMask.NameToLayer ("UI"),
                    };
                    canvas = go.AddComponent<Canvas> ();
                    go.AddComponent<CanvasScaler> ();
                    go.AddComponent<GraphicRaycaster> ();
                    if (GameObject.FindObjectOfType<EventSystem> () == null) {
                        go = new GameObject ("EventSystem");
                        go.AddComponent<EventSystem> ();
                        go.AddComponent<StandaloneInputModule> ();
                    }
                }
                return canvas.transform;
            }
        }

        private static void CreateHitRectOnParent (Transform parentNode) {
            var areaObj = new GameObject ("LargeHitRect") {
                transform = { parent = parentNode },
                layer = LayerMask.NameToLayer ("UI")
            };
            areaObj.transform.localScale = Vector3.one;
            var emptyRect = areaObj.AddComponent<EmptyRect> ();
            emptyRect.raycastTarget = true;
            var rect = areaObj.GetComponent<RectTransform> ();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2 (-40, -10);
            rect.offsetMax = new Vector2 (40, 10);
        }
    }

}