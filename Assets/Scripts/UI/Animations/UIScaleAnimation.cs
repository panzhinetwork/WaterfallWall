using System;
using UnityEngine;
using UIFramework;
using DG.Tweening;


public class UIScaleAnimation : UIAnimation {

    [SerializeField] protected float _duration = 0.4f;
    [SerializeField] protected Ease _ease = Ease.InOutBack;
    [SerializeField] protected bool _splitScale = false;
    [Range(0f, 1f)]
    [SerializeField] protected float _xyDurationSplit = 0.25f;

    [Header("Fade")]
    [SerializeField] protected bool _doFade;
    [SerializeField] protected float _fadeDurationPercent = 0.5f;

    private UIFadeHelper _fadeHelper = null;
    private Tween _tweenScale;

    private static readonly Vector3 kMaxScale = Vector3.one;
    private static readonly Vector3 kMinScale = Vector3.zero;

    public override void StartAnimation(Transform target, Action onCompleted) {
        var rTransform = target as RectTransform;

        bool isOutAnimation = (type == UIAnimationType.Out);

        if (_doFade) {
            if (_fadeHelper == null) {
                _fadeHelper = new UIFadeHelper(rTransform.gameObject, _duration * _fadeDurationPercent);
            }
            _fadeHelper.DoFadeAnimation(isOutAnimation);
        }

        if (_tweenScale != null) {
            _tweenScale.Kill(false);
        }

        if (isOutAnimation) {
            rTransform.localScale = kMaxScale;
        } else {
            rTransform.localScale = kMinScale;
        }
        if (_splitScale) {
            var scaleSequence = DOTween.Sequence();

            var xScale = rTransform.DOScaleX(isOutAnimation ? 0 : 1, _duration * _xyDurationSplit).SetEase(_ease);
            var yScale = rTransform.DOScale(isOutAnimation ? 0 : 1, _duration * (1 - _xyDurationSplit)).SetEase(_ease);
            scaleSequence.Append(xScale)
                .Append(yScale)
                .SetUpdate(true)
                .OnComplete(() => OnFinished(onCompleted, rTransform));

            _tweenScale = scaleSequence;
        } else {
            _tweenScale = rTransform.DOScale(isOutAnimation ? kMinScale : kMaxScale, _duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .OnComplete(() => OnFinished(onCompleted, rTransform));
        }
    }

    public override void Kill() {
        if (_tweenScale != null) {
            _tweenScale.Kill(true);
            _tweenScale = null;
        }
    }

    private void OnFinished(Action onComplete, RectTransform rectTransform) {
        onComplete();
        rectTransform.localScale = Vector3.one;
        if (_fadeHelper != null) {
            _fadeHelper.ResetAlpha();
        }
    }

}
