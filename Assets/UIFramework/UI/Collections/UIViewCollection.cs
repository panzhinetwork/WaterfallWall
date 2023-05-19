using System.Collections.Generic;
using UnityEngine;

namespace UIFramework {

    public class UIViewCollection {

        private Dictionary<string, IUIView> _registeredViews = new Dictionary<string, IUIView>();

        #region -- Register --

        public bool RegisterView(string viewId, IUIView view) {
            if (!_registeredViews.ContainsKey(viewId)) {
                ProcessViewRegister(viewId, view);
                return true;
            } else {
                Debug.LogError("[UIViewCollection] View is already registered for id: " + viewId);
            }
            return false;
        }

        public void UnregisterView(string viewId, IUIView view) {
            if (_registeredViews.ContainsKey(viewId)) {
                ProcessViewUnregister(viewId, view);
            } else {
                Debug.LogError("[UIViewCollection] View is not registered for id: " + viewId);
            }
        }

        public bool IsViewRegistered(string viewId) {
            return _registeredViews.ContainsKey(viewId);
        }

        public IUIView TryGetViewById(string viewId) {
            IUIView view;
            if (_registeredViews.TryGetValue(viewId, out view)) {
                return view;
            } else {
                Debug.LogError("[UIViewCollection] Faield to get view. The id is not registered: " + viewId);
            }
            return null;
        }

        protected virtual void ProcessViewRegister(string viewId, IUIView view) {
            view.ViewId = viewId;
            _registeredViews.Add(viewId, view);
            view.OnViewWillDestroy += OnViewDestroyed;
        }

        protected virtual void ProcessViewUnregister(string viewId, IUIView view) {
            view.OnViewWillDestroy -= OnViewDestroyed;
            _registeredViews.Remove(viewId);
        }

        private void OnViewDestroyed(IUIView view) {
            var id = view.ViewId;
            if (!string.IsNullOrEmpty(id)) {
                UnregisterView(id, view);
            }
        }

        #endregion
    }
}