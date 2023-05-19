using System;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;
using DG.Tweening;

public class UIColorAnimation : UIAnimation {

    public Graphic graphic;
    public Color color = Color.clear;
    public Ease ease = Ease.InOutCubic;
    public float duration = 0.4f;

    private Color _fromColor;
    private Color _toColor;
    private Tween _tweenColor;

    private void Awake() {
        if (type == UIAnimationType.In) {
            _fromColor = color;
            _toColor = graphic.color;
        } else {
            _fromColor = graphic.color;
            _toColor = color;
        }
    }

    public override void StartAnimation(Transform target, Action onCompleted) {
        if (_tweenColor != null) {
            _tweenColor.Kill(false);
        }
        graphic.color = _fromColor;
        _tweenColor = graphic.DOColor(_toColor, duration)
            .SetEase(ease)
            .SetUpdate(true)
            .OnComplete(() => {
            onCompleted();
            _tweenColor = null;
        });
    }

    public override void Kill() {
        if (_tweenColor != null) {
            _tweenColor.Kill(true);
            _tweenColor = null;
        }
    }
}
