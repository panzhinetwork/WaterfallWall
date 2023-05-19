using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class BigImageBar : ScrollRect
{
    [SerializeField] GameObject _content;
    [SerializeField] BigImage _prefab;
    [SerializeField] float _pageDragValue = 100;
    [SerializeField] float _pageWidth = 945;

    System.Action<int> _onImageChange;
    int _indexImage = 0;
    int _imageCount = 0;
    float _startX = 0;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        _startX = eventData.position.x;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        int indexImage = _indexImage;
        float v = eventData.position.x - _startX;
        if (v > _pageDragValue) //上一页
        {
            indexImage--;
        }
        else if (v < -_pageDragValue) //下页
        {
            indexImage++;
        }

        if (indexImage < 0)
            indexImage = 0;
        if (indexImage >= _imageCount)
            indexImage = _imageCount - 1;

        if (_indexImage != indexImage)
        {
            _indexImage = indexImage;
            _onImageChange?.Invoke(indexImage);
        }

        //_content.transform.localPosition = new Vector3(-_indexImage * _pageWidth - _pageWidth / 2, 0, 0);
        _content.transform.DOLocalMoveX(-_indexImage * _pageWidth - _pageWidth / 2, 0.5f);
    }

    public void Init(System.Action<int> onImageChange)
    {
        _onImageChange = onImageChange;
    }

    public void CreateImage(List<Sprite> images)
    {
        DestroyImage();
        for (int i=0; i<images.Count; i++)
        {
            BigImage image = GameObject.Instantiate<BigImage>(_prefab, _content.transform);
            if (image != null)
            {
                image.SetImage(images[i]);
            }
        }

        _indexImage = 0;
        _imageCount = images.Count;
        SetImage(_indexImage);
    }

    void DestroyImage()
    {
        while (_content.transform.childCount > 0)
        {
            DestroyImmediate(_content.transform.GetChild(0).gameObject);
        }
    }

    public void SetImage(int index)
    {
        _indexImage = index;
        //_content.transform.localPosition = new Vector3(-index * _pageWidth - _pageWidth / 2, 0, 0);
        _content.transform.DOLocalMoveX(-index * _pageWidth - _pageWidth / 2, 1);
    }
}
