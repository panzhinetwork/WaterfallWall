using System;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework {

    /// <summary>
    /// 负责动态创建 UI 的所有分层结构。
    /// </summary>
    public class UIViewPresenter : MonoBehaviour {

        [SerializeField]
        private Vector2 _resolution = new Vector2(1280, 720);

        private UILayer[] _layers;

        public Vector2 resolution
        {
            get { return _resolution; }
        }

        public void Init(Camera uiCamera, bool dontDestroyOnLoad) {
            // Make all layers at root.
            _layers = CreateAllLayerGameObjects(transform, uiCamera, _resolution);
            foreach(var layer in _layers) {
                layer.SetScreenBlockHandlers(BlockScreenInput, UnblockScreenInput);
            }

            if (dontDestroyOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
        }
        
        public UILayer GetLayer(UILayerType layerType) {
            var index = (int)layerType;
            return _layers[index];
        }

        public void SetInteractionEnabled(bool enabled) {
            for (int i=0; i<_layers.Length; i++) {
                var raycaster = _layers[i].GraphicRaycaster;
                if (raycaster != null) {
                    raycaster.enabled = enabled;
                }
            }
        }

        #region -- View --

        public void AnchorViewToLayer(IUIView view) {
            GetLayer(view.LayerType).AddView(view);

        }

        public void ShowView(IUIView view) {
            view.Show();
        }

        public void ShowView<TData>(IUIView view, TData data) where TData : IUIData {
            view.Show(data);
        }

        public void HideView(IUIView view) {
            view.Hide();
        }


        #endregion

        #region -- Block Screen Input --

        private void BlockScreenInput() {
            SetInteractionEnabled(false);
        }

        private void UnblockScreenInput() {
            SetInteractionEnabled(true);
        }

        #endregion

        private void Awake() {
            for (int i=0; i<_layers.Length; i++) {
                _layers[i].Init();
            }
        }

        public static UILayer[] CreateAllLayerGameObjects(Transform parent, Camera camera, Vector2 resolution) {
            int layerCount = System.Enum.GetNames(typeof(UILayerType)).Length;
            var layers = new UILayer[layerCount];
            for (int i=0; i<layerCount; i++) {
                layers[i] = UILayer.CreateLayerGameObject(parent, (UILayerType)i, camera, resolution);
            }
            return layers;
        }
    }

}