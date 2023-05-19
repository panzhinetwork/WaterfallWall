using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {

    public class UIPack : MonoBehaviour {

        [Tooltip("Set this to false if you want to manually initialize this UI Pack.")]
        [SerializeField] 
        private bool _initializeOnAwake = true;

        [SerializeField] private bool _dontDestoryOnLoad = false;

        [SerializeField] private UIViewPresenter _viewPresenter = default;

        [SerializeField] private Camera _uiCamera = default;

        private UIViewCollection _viewCollection;

        public Vector2 GetResolution()
        {
            return _viewPresenter.resolution;
        }

        public Camera GetUICamera()
        {
            return _uiCamera;
        }

        public void Initialize() {
            if (_viewPresenter == null) {
                _viewPresenter = GetComponentInChildren<UIViewPresenter>();
            }
            _viewPresenter.Init(GetComponentInChildren<Camera>(), _dontDestoryOnLoad);

            if (_viewCollection == null) {
                _viewCollection = new UIViewCollection();
            }
        }

        public void ShowView<TView>() where TView : IUIView {
            string viewId = typeof(TView).ToString();
            ShowView(viewId);
        }

        /// <summary>
        /// Core show view method without data.
        /// </summary>
        /// <param name="viewId">View identifier.</param>
        public void ShowView(string viewId) {
            var view = _viewCollection.TryGetViewById(viewId);
            if (view != null) {
                _viewPresenter.ShowView(view);
            }
        }

        public void ShowView<TView, TData>(TData data) where TView : IUIView where TData : IUIData {
            string viewId = typeof(TView).ToString();
            ShowView(viewId, data);
        }

        /// <summary>
        /// Core show view method with data.
        /// </summary>
        /// <param name="viewId">View identifier.</param>
        public void ShowView<TData>(string viewId, TData data) where TData : IUIData {
            var view = _viewCollection.TryGetViewById(viewId);
            if (view != null) {
                _viewPresenter.ShowView(view, data);
            }
        }

        public void HideView<TView>() where TView : IUIView {
            string viewId = typeof(TView).ToString();
            HideView(viewId);
        }

        /// <summary>
        /// Core hide view method.
        /// </summary>
        /// <param name="viewId">View identifier.</param>
        public void HideView(string viewId) {
            var view = _viewCollection.TryGetViewById(viewId);
            if (view != null) {
                _viewPresenter.HideView(view);
            }
        }

        public bool IsViewVisible<TView>() where TView : IUIView {
            string viewId = typeof(TView).ToString();
            return IsViewVisible(viewId);
        }

        public bool IsViewVisible(string viewId) {
            var view = _viewCollection.TryGetViewById(viewId);
            if (view != null) {
                return view.IsVisible;
            }
            return false;
        }

        #region -- Register --

        public void RegisterView(string viewId, IUIView view) {
            if (_viewCollection.RegisterView(viewId, view)) {
                _viewPresenter.AnchorViewToLayer(view);
            }
        }

        public void RegisterView(IUIView view) {
            string viewId = view.GetType().ToString();
            RegisterView(viewId, view);
        }

        public void UnregisterView(string viewId, IUIView view) {
            _viewCollection.UnregisterView(viewId, view);
        }

        public void UnregisterView(IUIView view) {
            string viewId = view.GetType().ToString();
            UnregisterView(viewId, view);
        }

        #endregion

        private void Awake() {
            if (_initializeOnAwake) {
                Initialize();
            }
            if (_dontDestoryOnLoad) {
                DontDestroyOnLoad(gameObject);
            }
        }


    }

}