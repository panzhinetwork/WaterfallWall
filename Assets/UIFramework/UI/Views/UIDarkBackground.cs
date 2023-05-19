using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework {

    public class UIDarkBackground : UIAbstractView {

        [Header ("Dark")]
        [SerializeField]
        private Image _background = default;

        private List<IUIView> _views = new List<IUIView> ();

        protected override void AddListeners () {
            Events.Get<ShowDarkBackgroundEvent> ().AddListener (StartBackground);
        }

        protected override void RemoveListeners () {
            Events.Get<ShowDarkBackgroundEvent> ().RemoveListener (StartBackground);
        }

        private void StartBackground (IUIPopup view, Color darkColor) {
            if (_views.Contains (view)) {
                return;
            }
            _views.Add (view);
            view.OnAnimationCompleted += OnViewOut;
            _background.color = darkColor;
            Show ();
        }

        private void OnViewOut (IUIView view, UIAnimationType animationType) {
            if (animationType == UIAnimationType.Out && _views.Contains (view)) {
                _views.Remove (view);
                view.OnAnimationCompleted -= OnViewOut;
                if (_views.Count > 0) {
                    var popup = (IUIPopup) _views[_views.Count - 1];
                    _background.color = popup.DarkColor;
                    var sublingIndex = Mathf.Max (FindLastActiveObjectIndex () - 1, 0);
                    transform.SetSiblingIndex (sublingIndex);
                } else {
                    Hide ();
                }
            }
        }

        protected override void LayoutOnShow () {
            var sublingIndex = Mathf.Max (transform.parent.childCount - 2, 0);
            transform.SetSiblingIndex (sublingIndex);
        }

        private int FindLastActiveObjectIndex () {
            var parent = transform.parent;
            var childCount = parent.childCount;
            for (int i = childCount - 1; i >= 0; --i) {
                var child = parent.GetChild (i);
                if (child != transform && child.gameObject.activeSelf) {
                    return i;
                }
            }
            return -1;
        }
    }

}