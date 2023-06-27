using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaoPao : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Text _topic;

    int _index;
    TopicConfig _config;
    System.Action<int, TopicConfig> _onClick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int index, System.Action<int, TopicConfig> onClick)
    {
        _index = index;
        _onClick = onClick;
    }

    public void SetTopic(TopicConfig config)
    {
        _config = config;
        _topic.text = _config.title;
        if (_config.images.Count > 0)
            _image.sprite = _config.images[0];
        if (_config.title.Length >= 19)
            _topic.fontSize = 14;
        else if (_config.title.Length >= 14)
            _topic.fontSize = 16;
        else
            _topic.fontSize = 18;
    }

    public TopicConfig GetTopic()
    {
        return _config;
    }

    public void OnClick()
    {
        _onClick?.Invoke(_index, _config);
    }
}
