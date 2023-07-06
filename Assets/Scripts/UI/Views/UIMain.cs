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
    [SerializeField] GameObject _activePanel;
    [SerializeField] GameObject _stayPanel;
    [SerializeField] VideoPlayer _bgVideo;
    [SerializeField] VideoPlayer _stayVideo;
    [SerializeField] GameObject _paopaoLayer;
    [SerializeField] GameObject _contentLayer;
    [SerializeField] Topic[] _topics;
    [SerializeField] PaoPao _paopaoPrefab;
    [SerializeField] Topic _topicPerfab;
    [SerializeField] Transform[] _upPoints;
    [SerializeField] Transform[] _downPoints;
    [SerializeField] Transform[] _leftPoints;
    [SerializeField] Transform[] _RightPoints;
    [SerializeField] GameObject _loading;
    [SerializeField] Text _tips;

    UDPServer udpServer = new UDPServer();
    bool _stay = true;
    bool _handle = false;
    List<TopicConfig> _topicConfigs = new List<TopicConfig>();
    int _topicIndex = 0;

    private void Start()
    {
        Events.Get<TimeoutEvent>().AddListener(OnTimeout);
        TimeoutChecker.instance.SetTimeOut(ConfigFiles.configs.timeout);
        TimeoutChecker.instance.enabled = false;
        _bgVideo.targetTexture.Release();
        _bgVideo.url = Application.streamingAssetsPath + "/Videos/bg.mp4";
        _stayVideo.targetTexture.Release();
        _stayVideo.url = Application.streamingAssetsPath + "/Videos/stay.mp4";
        StartCoroutine(LoadTopicConfigs());
        udpServer.Start(ConfigFiles.configs.port, (string data) => 
        {
            if (data == ConfigFiles.configs.stay)
            {
                _stay = true;
                _handle = true;
            }
            else if (data == ConfigFiles.configs.active)
            {
                _stay = false;
                _handle = true;
            }
        });
    }

    private void Update()
    {
        if (_handle)
        {
            _handle = false;
            if (_stay)
            {
                OnTimeout();
            }
            else
            {
                OnActive();
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("x=" + Input.mousePosition.x);
        }
#endif
    }

    public void OnActive()
    {
        TimeoutChecker.instance.enabled = true;
        _activePanel.SetActive(true);
        _stayPanel.SetActive(false);
        _stayVideo.targetTexture.Release();
        _stayVideo.Stop();
        _bgVideo.Play();
    }

    void OnTimeout()
    {
        TimeoutChecker.instance.enabled = false;
        foreach (var topic in _topics)
        {
            topic.Show(false);
        }

        _bgVideo.targetTexture.Release();
        _bgVideo.Stop();
        _activePanel.SetActive(false);
        _stayPanel.SetActive(true);
        _stayVideo.Play();
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
                            _tips.text = config.title + name;
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
            StartCoroutine(CreatePaoPaoLR());
        }

        _loading.SetActive(false);
        _activePanel.SetActive(false);
        _stayPanel.SetActive(true);
        _stayVideo.Play();
    }

    void CreatePaoPaoUD()
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
                Tween tw = paopao.transform.DOMove(_downPoints[i].position, Random.Range(ConfigFiles.configs.mint, ConfigFiles.configs.maxt));
                tw.onStepComplete = () => 
                {
                    paopao.gameObject.SetActive(true);
                    paopao.SetTopic(_topicConfigs[_topicIndex % _topicConfigs.Count]);
                    _topicIndex++;
                    if (_topicIndex < 0)
                        _topicIndex = 0;
                    
                };
                tw.SetLoops(-1);
            }
        }
    }

    IEnumerator CreatePaoPaoLR()
    {
        for (int i = 0; i < _leftPoints.Length; i++)
        {
            PaoPao paopao = GameObject.Instantiate<PaoPao>(_paopaoPrefab, _paopaoLayer.transform);
            if (paopao != null)
            {
                paopao.Init(-1, (int index, TopicConfig config) =>
                {
                    TimeoutChecker.instance.enabled = true;
                    paopao.gameObject.SetActive(false);

                    int ix = 0;
                    if (Input.mousePosition.x < ConfigFiles.configs.dx)
                        ix = 0;
                    else
                        ix = 1;
                    _topics[ix].SetConfig(config);
                    _topics[ix].Show(true);
                });

                paopao.SetTopic(_topicConfigs[_topicIndex % _topicConfigs.Count]);
                _topicIndex++;

                paopao.transform.position = _leftPoints[i].position;
                Tween tw = paopao.transform.DOMove(_RightPoints[i].position, Random.Range(ConfigFiles.configs.mint, ConfigFiles.configs.maxt));
                tw.onStepComplete = () =>
                {
                    paopao.gameObject.SetActive(true);
                    paopao.SetTopic(_topicConfigs[_topicIndex % _topicConfigs.Count]);
                    _topicIndex++;
                    if (_topicIndex < 0)
                        _topicIndex = 0;

                };
                tw.SetLoops(-1);
                yield return new WaitForSeconds(5.0f);
            }
        }
    }
}