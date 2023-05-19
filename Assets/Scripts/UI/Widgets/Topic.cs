using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TopicConfig
{
    public string title;
    public List<Sprite> images;
}

public class Topic : MonoBehaviour
{
    [SerializeField] Text _title;
    [SerializeField] SmallImage[] _images;
    [SerializeField] GameObject _turnLeft;
    [SerializeField] GameObject _turnRight;
    [SerializeField] BigImageBar _bigImageBar;

    TopicConfig _config;
    int _indexImage = 0; // 案例图片索引
    int _indexSmallImage = 0; //缩略图索引
    int _curPage = 0;
    int _totalPage = 0;
    int _switchTime = 10;

    private void Awake()
    {
        _switchTime += Random.Range(0, 5);
        _bigImageBar.Init((int indexImage) => 
        {
            if (indexImage == _indexImage)
                return;
            _indexImage = indexImage;
            ChangeContent(indexImage);
        });

        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].gameObject.SetActive(false);
            _images[i].Select(false);
            _images[i].Init((int indexImage, Sprite image) =>
            {
                if (indexImage == _indexImage)
                    return;
                _indexImage = indexImage;
                ChangeContent(indexImage, image);
                _bigImageBar.SetImage(indexImage);
            });
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetConfig(TopicConfig config)
    {
        _config = config;
        _title.text = config.title;
        _indexImage = 0;
        _curPage = 0;
        _indexSmallImage = 0;
        _bigImageBar.CreateImage(_config.images);

        if (_config.images.Count > 0)
        {
            ShowPage(0);
            _images[0].Select(true);
        }

        _totalPage = _config.images.Count / _images.Length;
        if (_config.images.Count % _images.Length != 0)
            _totalPage++;
        if (_totalPage > 1)
        {
            _turnLeft.SetActive(true);
            _turnRight.SetActive(true);
        }
        else
        {
            _turnLeft.SetActive(false);
            _turnRight.SetActive(false);
        }
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
        if (show)
            FadeAnimation.In(gameObject, 0.5f);
    }

    void ChangeContent(int indexImage, Sprite image)
    {
        _images[_indexSmallImage].Select(false);
        _indexSmallImage = indexImage % _images.Length;
        _images[_indexSmallImage].Select(true);
    }

    void ChangeContent(int indexImage)
    {
        int lastPage = _curPage;

        _curPage = indexImage / _images.Length;

        if (_config.images.Count > _images.Length &&
            lastPage != _curPage)
        {
            ShowPage(_curPage);
        }

        _images[_indexSmallImage].Select(false);
        _indexSmallImage = indexImage % _images.Length;
        _images[_indexSmallImage].Select(true);
    }

    public void OnNextPage()
    {
        if (_totalPage < 2)
            return;
        int curPage = _curPage;
        curPage++;
        if (curPage < _totalPage)
        {
            ShowPage(curPage);
            _curPage = curPage;
        }
    }

    public void OnPrevPage()
    {
        if (_totalPage < 2)
            return;
        int curPage = _curPage;
        curPage--;
        if (curPage >= 0)
        {
            ShowPage(curPage);
            _curPage = curPage;
        }
    }

    void ShowPage(int page)
    {
        int index = page * _images.Length;
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i].Select(false);
            if (index < _config.images.Count)
            {
                _images[i].gameObject.SetActive(true);
                _images[i].SetImage(index, _config.images[index]);
                index++;
            }
            else
            {
                _images[i].gameObject.SetActive(false);
            }
        }
    }
}
