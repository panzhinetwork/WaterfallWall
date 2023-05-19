using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// 序列帧播放
/// </summary>
[RequireComponent(typeof(Image))]
public class SpriteAnimation : MonoBehaviour
{
#if UNITY_EDITOR
    public string spriteFramesEditorDirectory;
#endif

    public Action OnCompleted;

    public Sprite[] spriteFrames = default;
    [SerializeField] private float _fps = 24;
    [SerializeField] private bool _loop = false;
    [SerializeField] private bool _autoPlay = false;
    [SerializeField] private float  _delay = 0;
    [SerializeField] private Image _image = default;

    private bool _isPlaying = false;
    private bool _foward = true;
    private int _curFrame = 0;
    private float _delta = 0;

    /// <summary>
    /// 总帧数
    /// </summary>
    public int frameCount
    {
        get
        {
            return spriteFrames.Length;
        }
    }

    public int currentFrame
    {
        get
        {
            return _curFrame;
        }
    }

    /// <summary>
    /// 帧率
    /// </summary>
    public float fps
    {
        get { return _fps; }
    }

    void Awake()
    {
        _isPlaying = false;
    }

    private void Start()
    {
        if (_autoPlay)
        {
            StartCoroutine(DelayAutoPlay());
        }
    }

    IEnumerator DelayAutoPlay()
    {
        yield return new WaitForSeconds(_delay);
        Play();
    }

    private void SetSprite(int idx)
    {
        _image.sprite = spriteFrames[idx];
        _image.SetNativeSize();
    }

    /// <summary>
    /// 播放
    /// </summary>
    public void Play()
    {
        _isPlaying = true;
        _foward = true;
    }

    /// <summary>
    /// 倒序播放
    /// </summary>
    public void PlayReverse()
    {
        _isPlaying = true;
        _foward = false;
    }

    void Update()
    {
        if (!_isPlaying || 0 == frameCount)
        {
            return;
        }

        _delta += Time.deltaTime;
        if (_delta > 1 / _fps)//时间到，显示下一帧
        {
            _delta = 0;
            if (_foward)
            {
                _curFrame++;
            }
            else
            {
                _curFrame--;
            }
            if (_curFrame >= frameCount)//播完
            {
                if (OnCompleted != null)
                {
                    OnCompleted();
                }

                if (_loop)//循环
                {
                    _curFrame = 0;
                }
                else//停止
                {
                    _curFrame = frameCount - 1;
                    _isPlaying = false;
                    return;
                }
            }
            else if (_curFrame < 0)
            {
                if (OnCompleted != null)
                {
                    OnCompleted();
                }

                if (_loop)
                {
                    _curFrame = frameCount - 1;
                }
                else
                {
                    _curFrame = 0;
                    _isPlaying = false;
                    return;
                }
            }
            SetSprite(_curFrame);
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        _isPlaying = false;
    }

    /// <summary>
    /// 恢复
    /// </summary>
    public void Resume()
    {
        if (!_isPlaying)
        {
            _isPlaying = true;
        }
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        _curFrame = 0;
        SetSprite(_curFrame);
        _isPlaying = false;
    }

    public bool isPlaying
    {
        get
        {
            return _isPlaying;
        }
    }
}