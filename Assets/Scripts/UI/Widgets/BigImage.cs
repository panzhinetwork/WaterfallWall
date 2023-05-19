using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigImage : MonoBehaviour
{
    [SerializeField] Image _image;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetImage(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
