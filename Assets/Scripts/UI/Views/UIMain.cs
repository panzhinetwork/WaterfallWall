using EventDef;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using DG.Tweening;

public class UIMain : UIAbstractView
{
    [SerializeField] VideoPlayer _bgVideo;
    [SerializeField] GameObject _paopaoLayer;
    [SerializeField] GameObject _contentLayer;
    [SerializeField] Topic[] _topics;
    [SerializeField] PaoPao _paopaoPrefab;
    [SerializeField] Topic _topicPerfab;
    [SerializeField] Transform[] _upPoints;
    [SerializeField] Transform[] _downPoints;
    [SerializeField] GameObject _loading;
    [SerializeField] Text _tips;

    List<TopicConfig> _topicConfigs = new List<TopicConfig>();
    int _topicIndex = 0;

    private void Start()
    {
        Events.Get<TimeoutEvent>().AddListener(OnTimeout);
        TimeoutChecker.instance.SetTimeOut(ConfigFiles.configs.timeout);
        TimeoutChecker.instance.enabled = true;
        _bgVideo.url = Application.streamingAssetsPath + "/Videos/bg.mp4";
        StartCoroutine(LoadTopicConfigs());
    }

    void OnTimeout()
    {
        foreach (var topic in _topics)
        {
            topic.Show(false);
        }
    }

    IEnumerator LoadTopicConfigs()
    {
        string[] dirs = Directory.GetDirectories(Application.streamingAssetsPath + "/Images");
        foreach (var dir in dirs)
        {
            TopicConfig config = new TopicConfig();
            if (config != null)
            {
                string[] str = dir.Split(')');
                if (str.Length == 2)
                {
                    config.title = str[1];
                }
                else
                {
                    config.title = str[0];
                }

                _tips.text = config.title;

                List<Sprite> sprites = new List<Sprite>();
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(dir);
                }
                catch
                {
                    Debug.Log(dir + "不存在");
                }

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.EndsWith(".png", System.StringComparison.CurrentCultureIgnoreCase) || file.EndsWith(".jpg", System.StringComparison.CurrentCultureIgnoreCase))
                        {
                            string name = file.Substring(dir.Length);
                            Sprite sprite = ImagesLoader.LoadOneImage(file, name);
                            sprites.Add(sprite);
                            yield return null;
                        }
                    }
                }
                if (sprites.Count == 0)
                    Debug.Log(config.title + "没加载图片");
                config.images = sprites;
                _topicConfigs.Add(config);
            }
        }

        if (_topicConfigs.Count > 0)
        {
            CreatePaoPao();
        }

        _loading.SetActive(false);
        _bgVideo.Play();
    }

    void CreatePaoPao()
    {
        for (int i=0; i< _upPoints.Length; i++)
        {
            PaoPao paopao = GameObject.Instantiate<PaoPao>(_paopaoPrefab, _paopaoLayer.transform);
            if (paopao != null)
            {
                int ix = 0;

                if (i >= 0 && i < 3)
                    ix = 0;
                else if (i >= 3 && i < 7)
                    ix = 1;
                else if (i >= 7 && i < 10)
                    ix = 2;
                paopao.Init(ix, (int index, TopicConfig config) =>
                {
                    TimeoutChecker.instance.enabled = true;
                    paopao.gameObject.SetActive(false);
                    _topics[index].SetConfig(config);
                    _topics[index].Show(true);
                });

                paopao.SetTopic(_topicConfigs[_topicIndex % _topicConfigs.Count]);
                _topicIndex++;

                paopao.transform.position = _upPoints[i].position;
                Tween tw = paopao.transform.DOMove(_downPoints[i].position, Random.Range(10.0f, 20.0f));
                tw.onStepComplete = () => 
                {
                    paopao.gameObject.SetActive(true);
                    paopao.SetTopic(_topicConfigs[_topicIndex % _topicConfigs.Count]);
                    _topicIndex++;
                    
                };
                tw.SetLoops(-1);
            }
        }
    }
}