using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class SpriteAnimationSequence 
{
    private List<SpriteAnimation> _list = new List<SpriteAnimation>();

    public Action OnCompleted;

    public Action OnOneCompleted;

    private bool _isPlaying;
    private SpriteAnimation _last;

    public static SpriteAnimationSequence Create()
    {
        return new SpriteAnimationSequence();
    }

    public void Append(SpriteAnimation spriteAnimation)
    {
        _list.Add(spriteAnimation);
        spriteAnimation.gameObject.SetActive(false);
    }

    public void Play()
    {
        if (!_isPlaying)
        {
            _last = _list[_list.Count - 1];
            _isPlaying = true;
            PlayNext();
        }
    }

    private void PlayNext()
    {
        if (_list.Count > 0)
        {
            SpriteAnimation spriteAnimation = _list[0];
            _list.RemoveAt(0);

            spriteAnimation.gameObject.SetActive(true);
            spriteAnimation.Play();
            spriteAnimation.OnCompleted = () =>
            {
                OnOneCompleted?.Invoke();
                if (spriteAnimation != _last)
                {
                    spriteAnimation.gameObject.SetActive(false);
                }
                PlayNext();
            };
        }
        else
        {
            _isPlaying = false;
            OnCompleted?.Invoke();
        }
    }

    public void Reset()
    {
        _isPlaying = false;
        _list.Clear();
    }
}