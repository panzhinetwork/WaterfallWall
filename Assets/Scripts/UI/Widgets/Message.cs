using System.Collections;
using System.Collections.Generic;
using DG;
using DG.Tweening;
using EventDef;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UIFramework;

/// <summary>
/// 提示信息，从下向上弹出
/// </summary>
public class Message : MonoBehaviour {
    [SerializeField] RectTransform _rectTransform = default;
    [SerializeField] Text _text = default;
    [SerializeField] float _offsetY = 300;
    [SerializeField] float _moveDuration = 0.4f;
    [SerializeField] float _duration = 2;

    private Vector2 _beginAnchorPosition;
    private Tweener _moveTweener;
    private float _waitTime;

    private void Awake () {
        _waitTime = _duration - _moveDuration;
        _beginAnchorPosition = _rectTransform.anchoredPosition;
    }

    /// <summary>
    /// 从下向上弹出文本提示
    /// </summary>
    public void Show (string text) {
        StopAllCoroutines ();

        _text.text = text;
        _rectTransform.anchoredPosition = _beginAnchorPosition;

        gameObject.SetActive (true);

        if (_moveTweener == null) {
            _moveTweener = _rectTransform.DOAnchorPosY (_offsetY, _moveDuration);
            _moveTweener.SetRelative (true);
            _moveTweener.SetAutoKill (false);
            _moveTweener.onComplete = OnMoveEnd;
        }
        _moveTweener.Restart ();
    }

    private void OnMoveEnd () {
        StartCoroutine (WaitToDisappear ());
    }

    IEnumerator WaitToDisappear () {
        yield return new WaitForSeconds (_waitTime);
        gameObject.SetActive (false);
        Events.Get<UIMessageCloseEvent> ().Raise ();
    }
}