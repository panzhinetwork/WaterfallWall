using System;
using UnityEngine;
using UIFramework;
using DG.Tweening;

public class UISlideAnimation : UIAnimation {

    public enum Position {
        None,
        Left,
        Right,
        Top,
        Bottom,
    }

    [Tooltip("From position when In. To position when out.")]
    [SerializeField] protected Position _position = Position.Left;
    [SerializeField] protected float _duration = 0.4f;
    [SerializeField] protected Ease _ease = Ease.InOutCubic;

    [Header("Fade")]
    [SerializeField] protected bool _doFade;
    [SerializeField] protected float _fadeDurationPercent = 0.5f;

    private UIFadeHelper _fadeHelper;
    private Tween _tweenPosition;

    public override void StartAnimation(Transform target, Action onCompleted) {
        var rTransform = target as RectTransform;
        var originPosition = rTransform.anchoredPosition;
        var point = Vector3.zero;

        switch(_position) {
            case Position.Left:
                point = new Vector3(-rTransform.rect.width, 0, 0);
                break;
            case Position.Right:
                point = new Vector3(rTransform.rect.width, 0, 0);
                break;
            case Position.Top:
                point = new Vector3(0, rTransform.rect.height, 0);
                break;
            case Position.Bottom:
                point = new Vector3(0, -rTransform.rect.height, 0);
                break;
        }

        bool isSlidingOut = (type == UIAnimationType.Out);
        Vector3 startPosition = Vector3.zero;
        Vector3 endPosition = Vector3.zero;
        if (isSlidingOut) {
            startPosition = Vector3.zero;
            endPosition = point;
        } else {
            startPosition = point;
            endPosition = Vector3.zero;
        }

        if (_doFade) {
            if (_fadeHelper == null) {
                _fadeHelper = new UIFadeHelper(rTransform.gameObject, _duration * _fadeDurationPercent);
            }
            _fadeHelper.DoFadeAnimation(isSlidingOut);
        }


        if (_tweenPosition != null) {
            _tweenPosition.Kill(false);
        }

        rTransform.anchoredPosition = startPosition;
        _tweenPosition = rTransform.DOAnchorPos(endPosition, _duration, true)
            .SetEase(_ease)
            .SetUpdate(true)
            .OnComplete(() => {
                onCompleted();
                rTransform.anchoredPosition = originPosition;
                if (_fadeHelper != null) {
                    _fadeHelper.ResetAlpha();
                }
            });

    }

    public override void Kill() {
        if (_tweenPosition != null) {
            _tweenPosition.Kill(true);
            _tweenPosition = null;
        }
    }

}
