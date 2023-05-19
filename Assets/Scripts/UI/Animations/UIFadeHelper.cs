using UnityEngine;
using DG.Tweening;

public class UIFadeHelper {

    public float Duration;

    private CanvasGroup _canvasGroup = null;
    private Tween _fadeTween;

    public UIFadeHelper(GameObject target, float duration) {
        _canvasGroup = target.GetComponent<CanvasGroup>();
        if (_canvasGroup == null) {
            _canvasGroup = target.AddComponent<CanvasGroup>();
        }
        Duration = duration;
    }

    public Tween DoFadeAnimation(bool isFadeOut) {
        if (_fadeTween != null) {
            _fadeTween.Kill(false);
        }
        _canvasGroup.alpha = isFadeOut ? 1 : 0;
        _fadeTween = _canvasGroup.DOFade(isFadeOut ? 0 : 1, Duration);
        return _fadeTween;
    }

    public void Kill() {
        if (_fadeTween != null) {
            _fadeTween.Kill(true);
            _fadeTween = null;
        }
    }

    public void ResetAlpha() {
        _canvasGroup.alpha = 1;
    }

}
