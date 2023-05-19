using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SmallImage : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Image _select;
    [SerializeField] Image _mask;

    Sprite _sprite;
    System.Action<int, Sprite> _onClick;
    int _indexImage = -1; // 案例图片索引

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(System.Action<int, Sprite> onClick)
    {
        _onClick = onClick;
    }

    public void SetImage(int indexImage, Sprite image)
    {
        _indexImage = indexImage;
        _sprite = image;
        _image.sprite = image;
    }

    public void Select(bool select)
    {
        _select.gameObject.SetActive(select);
        _mask.gameObject.SetActive(!select);
        if (select)
        {
            gameObject.transform.DOScale(0.26f, 0.5f);
        }
        else
        {
            gameObject.transform.DOScale(0.245f, 0.5f);
        }
    }

    public void OnClick()
    {
        _onClick?.Invoke(_indexImage, _sprite);
        _select.gameObject.SetActive(true);
        _mask.gameObject.SetActive(false);
    }
}
