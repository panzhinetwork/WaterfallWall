using UnityEngine;

namespace UIFramework {

    public class ShowDarkBackgroundEvent : BaseEvent<IUIPopup, Color> { }
    public class HideDarkBackgroundEvent : BaseEvent<IUIPopup> { }

    public interface IUIPopup : IUIView {
        bool NeedsDarkBackground { get; }
        Color DarkColor { get; }
    }

    public class UIAbstractPopup : UIAbstractPopup<IUIData> { }

    public class UIAbstractPopup<TData> : UIAbstractView<TData>, IUIPopup where TData : IUIData {
    
        [Header("Popup")]
        [SerializeField]
        private bool _needsDarkBackground = true;

        [SerializeField]
        private Color _darkColor = new Color(0, 0, 0, 0.8f);

        public bool NeedsDarkBackground { get { return _needsDarkBackground; } }

        public Color DarkColor { get { return _darkColor; } }

        protected override void LayoutOnShow() {
            base.LayoutOnShow();
            if (_needsDarkBackground) {
                Events.Get<ShowDarkBackgroundEvent>().Raise(this, _darkColor);
            }
        }

        protected override void WhileAnimatingOut() {
            if (_needsDarkBackground) {
                Events.Get<HideDarkBackgroundEvent>().Raise(this);
            }
        }
    }

}