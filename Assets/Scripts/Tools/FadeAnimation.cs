using DG.Tweening;
using UnityEngine;

public class FadeAnimation : MonoBehaviour
{
    private Tween _tween;

    protected void FadeIn(float duration, System.Action callback = null)
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        var canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0;
        _tween = canvasGroup.DOFade(1, duration);
        _tween.onComplete = () =>
        {
            _tween = null;
            callback?.Invoke();
        };
    }

    protected void FadeOut(float duration, System.Action callback = null)
    {
        if (_tween != null)
        {
            _tween.Kill();
            _tween = null;
        }

        var canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        _tween = canvasGroup.DOFade(0, duration);
        _tween.onComplete = () =>
        {
            _tween = null;
            callback?.Invoke();
        };
    }

    public static void In(GameObject target, float duration = 0.4f, System.Action callback = null)
    {
        var fadeAnimation = target.GetComponent<FadeAnimation>();
        if (fadeAnimation == null)
        {
            fadeAnimation = target.AddComponent<FadeAnimation>();
        }
        fadeAnimation.FadeIn(duration, callback);
    }

    public static void Out(GameObject target, float duration = 0.4f, System.Action callback = null)
    {
        var fadeAnimation = target.GetComponent<FadeAnimation>();
        if (fadeAnimation == null)
        {
            fadeAnimation = target.AddComponent<FadeAnimation>();
        }
        fadeAnimation.FadeOut(duration, callback);
    }
}