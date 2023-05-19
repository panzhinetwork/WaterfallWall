using System;
using UnityEngine;

namespace UIFramework {

    public interface IUIData { }

    public interface IUIView {
        string ViewId { get; set; }
        bool IsVisible { get; }

        Transform ViewTransform { get; }

        UILayerType LayerType { get; }

        void Show (IUIData data = null);
        void Hide (bool animated = true);

        event Action<IUIView, UIAnimationType> OnAnimationStarted;
        event Action<IUIView, UIAnimationType> OnAnimationCompleted;
        event Action<IUIView> OnViewWillDestroy;
    }

    /// <summary>
    /// UI 视图抽象类，不带 UI 数据的。
    /// </summary>
    public abstract class UIAbstractView : UIAbstractView<IUIData> { }

    /// <summary>
    /// UI 视图抽象类，带有 UI 数据的。一般子类要override方法 OnDataSet()。
    /// </summary>
    public abstract class UIAbstractView<TData> : MonoBehaviour, IUIView
    where TData : IUIData {

        [Header ("Layer")]
        [SerializeField]
        private UILayerType _layerType = default;

        [Header ("View Animations")]
        [Tooltip ("Animation that shows the view.")]
        [SerializeField]
        private UIAnimation _animationIn = default;

        [Tooltip ("Animation that hides the view.")]
        [SerializeField]
        private UIAnimation _animationOut = default;

        [Header ("View Data")]
        [SerializeField]
        private TData _data = default;

        public UIAnimation AnimationIn {
            get { return _animationIn; }
            set { _animationIn = value; }
        }

        public UIAnimation AnimationOut {
            get { return _animationOut; }
            set { _animationOut = value; }
        }

        public TData Data {
            get { return _data; }
            set { SetData (value); }
        }

        protected UIAnimation _animationRunning;

        #region IUIView

        /// <summary>
        /// 唯一的字符串 id。
        /// </summary>
        /// <value>The view identifier.</value>
        public string ViewId { get; set; }

        /// <summary>
        /// 是否可见。
        /// </summary>
        /// <value><c>true</c> if is visible; otherwise, <c>false</c>.</value>
        public bool IsVisible {
            get { return gameObject.activeSelf; }
        }

        public Transform ViewTransform {
            get { return transform; }
        }

        public UILayerType LayerType {
            get { return _layerType; }
        }

        public event Action<IUIView, UIAnimationType> OnAnimationStarted;
        public event Action<IUIView, UIAnimationType> OnAnimationCompleted;

        public event Action<IUIView> OnViewWillDestroy;

        public void Show (IUIData data = null) {
            if (data != null) {
                if (data is TData) {
                    SetData ((TData) data);
                } else {
                    Debug.LogError ("UIViewProperties type error! Passing " +
                        data.GetType () + ", expecting " + typeof (TData));
                    return;
                }
            }

            LayoutOnShow ();
            if (OnAnimationStarted != null) {
                OnAnimationStarted (this, UIAnimationType.In);
            }
            if (!gameObject.activeSelf) {
                DoAnimation (_animationIn, CompleteAnimationIn, true);
            } else {
                CompleteAnimationIn ();
            }

            OnDataSet();
        }

        public void Hide (bool animated = true) {
            if (OnAnimationStarted != null) {
                OnAnimationStarted (this, UIAnimationType.Out);
            }
            DoAnimation (animated ? _animationOut : null, CompleteAnimationOut, false);
            WhileAnimatingOut ();
        }

        #endregion

        private void DoAnimation (UIAnimation anim, Action onCompleted, bool isVisible) {
            if (_animationRunning != null) {
                _animationRunning.Kill ();
            }

            if (anim == null) {
                gameObject.SetActive (isVisible);
                if (onCompleted != null) {
                    onCompleted ();
                }
            } else {
                if (isVisible && !gameObject.activeSelf) {
                    gameObject.SetActive (true);
                }

                _animationRunning = anim;
                anim.StartAnimation (transform, onCompleted);
            }
        }

        private void CompleteAnimationIn () {
            _animationRunning = null;
            if (OnAnimationCompleted != null) {
                OnAnimationCompleted (this, UIAnimationType.In);
            }
        }

        private void CompleteAnimationOut () {
            gameObject.SetActive (false);

            _animationRunning = null;
            if (OnAnimationCompleted != null) {
                OnAnimationCompleted (this, UIAnimationType.Out);
            }
        }

        protected virtual void Awake () {
            AddListeners ();
        }

        protected virtual void OnDestroy () {
            if (OnViewWillDestroy != null) {
                OnViewWillDestroy (this);
            }

            OnAnimationStarted = null;
            OnAnimationCompleted = null;
            OnViewWillDestroy = null;

            RemoveListeners ();
        }

        protected virtual void AddListeners () { }

        protected virtual void RemoveListeners () { }

        /// <summary>
        /// 方便子类扩展。
        /// </summary>
        /// <param name="data">Properties.</param>
        protected virtual void SetData (TData data) {
            _data = data;
        }

        protected virtual void OnDataSet () {

        }

        /// <summary>
        /// 当播放 out 动画时，这个方法会被调用。
        /// </summary>
        protected virtual void WhileAnimatingOut () { }

        /// <summary>
        /// 需要的话，在这里做特殊的行为。
        /// </summary>
        protected virtual void LayoutOnShow () {
            if (_layerType == UILayerType.Popup) {
                transform.SetAsLastSibling ();
            }
        }

    }

}