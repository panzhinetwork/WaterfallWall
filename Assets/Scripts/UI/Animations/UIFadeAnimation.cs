using System;
using UnityEngine;
using UIFramework;
using DG.Tweening;

public class UIFadeAnimation : UIAnimation {

    [SerializeField] protected float _duration = 0.4f;

    private UIFadeHelper _fadeHelper = null;

    public override void StartAnimation(Transform target, Action onCompleted) {
        bool isOutAnimation = (type == UIAnimationType.Out);

        if (_fadeHelper == null) {
            _fadeHelper = new UIFadeHelper(target.gameObject, _duration);
        }
        _fadeHelper.DoFadeAnimation(isOutAnimation)
            .SetUpdate(true)
            .OnComplete(() => {
                onCompleted();
                _fadeHelper.ResetAlpha();
            });

    }

    public override void Kill() {
        if (_fadeHelper != null) {
            _fadeHelper.Kill();
        }
    }
}
