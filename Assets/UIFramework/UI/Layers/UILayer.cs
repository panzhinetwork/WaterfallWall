using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {

    public class UILayer : MonoBehaviour {

        public UILayerType layerType;

        public Canvas Canvas { get; private set; }
        public CanvasScaler CanvasScaler { get; private set; }
        public GraphicRaycaster GraphicRaycaster { get; private set; }

        //private bool _active;
        private List<GameObject> _viewObjects = new List<GameObject>();

        private HashSet<IUIView> _animatingViews = new HashSet<IUIView>();
        private Action _blockScreenInput;
        private Action _unblockScreenInput;

        public void Init() {
            Canvas = GetComponent<Canvas>();
            CanvasScaler = GetComponent<CanvasScaler>();
            GraphicRaycaster = GetComponent<GraphicRaycaster>();
        }

        public void SetScreenBlockHandlers(Action blockScreen, Action unblockScreen) {
            _blockScreenInput = blockScreen;
            _unblockScreenInput = unblockScreen;
        }

        public void AddView(IUIView view) {
            view.ViewTransform.SetParent(transform, false);
            _viewObjects.Add(view.ViewTransform.gameObject);

            view.OnViewWillDestroy += OnViewDestroyed;
            view.OnAnimationStarted += AddAnimating;
            view.OnAnimationCompleted += RemoveAnimating;
        }

        //private void RefreshState() {
        //    int count = _viewObjects.Count;
        //    for (int i = 0; i < count; i++) {
        //        var go = _viewObjects[i];
        //        if (go != null && go.activeSelf) {
        //            SetActive(true);
        //            return;
        //        }
        //    }

        //    SetActive(false);
        //}

        //private void SetActive(bool active) {
        //    _active = active;
        //    Canvas.enabled = active;
        //    CanvasScaler.enabled = active;
        //    GraphicRaycaster.enabled = active;
        //}

        private void OnViewDestroyed(IUIView view) {
            view.OnViewWillDestroy -= OnViewDestroyed;
            view.OnAnimationStarted -= AddAnimating;
            view.OnAnimationCompleted -= RemoveAnimating;
        }

        private bool IsViewAnimationInProgress() {
            return _animatingViews.Count != 0;
        }

        private void AddAnimating(IUIView view, UIAnimationType type) {
            _animatingViews.Add(view);
            if (_blockScreenInput != null) {
                _blockScreenInput();
            }
        }

        private void RemoveAnimating(IUIView view, UIAnimationType type) {
            _animatingViews.Remove(view);
            if (!IsViewAnimationInProgress()) {
                if (_unblockScreenInput != null) {
                    _unblockScreenInput();
                }
            }
        }

        #region -- Create GameObject --

        public static UILayer CreateLayerGameObject(Transform parent, UILayerType layerType, Camera camera, Vector2 resolution) {
            var go = new GameObject(string.Format("UILayer_{0}", layerType)) {
                transform = { parent = parent },
                layer = LayerMask.NameToLayer("UI")
            };
            var layer = go.AddComponent<UILayer>();
            layer.layerType = layerType;
            layer.CreateCanvasOnLayer(camera, resolution);

            return layer;
        }

        private void CreateCanvasOnLayer(Camera cam, Vector2 resolution) {
            var rectTransform = gameObject.AddComponent<RectTransform>();
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, 0f);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, 0f);
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;

            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.pixelPerfect = false;
            canvas.sortingOrder = GetSortOrderByLayerType(layerType);
            canvas.worldCamera = cam;

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = resolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            gameObject.AddComponent<GraphicRaycaster>();
        }

        private static int GetSortOrderByLayerType(UILayerType layerType) {
            return 10 * (int)layerType;
        }

        #endregion
    }

}