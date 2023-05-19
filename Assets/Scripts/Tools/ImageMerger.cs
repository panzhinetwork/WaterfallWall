using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageMerger : MonoBehaviour
{
    private bool _doing = false;
    private System.Action<Texture2D> _onComplete;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Scale(Texture2D source, int targetWidth, int targetHeight, System.Action<Texture2D> onComplete)
    {
        if (_doing)
            return false;
        _onComplete = onComplete;
        StartCoroutine(scale(source, targetWidth, targetHeight));
        _doing = true;
        return true;
    }

    private IEnumerator scale(Texture2D source, int targetWidth, int targetHeight)
    {
        yield return new WaitForEndOfFrame();
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, false);
        for (int h = 0; h < result.height; ++h)
        {
            for (int w = 0; w < result.width; ++w)
            {
                Color color = source.GetPixelBilinear(1.0f * w / result.width, 1.0f * h / result.height);
                result.SetPixel(w, h, color);
            }
        }

        result.Apply();
        _doing = false;
        _onComplete?.Invoke(result);
    }

    public bool HorizontalFlipTexture(Texture2D source, System.Action<Texture2D> onComplete)
    {
        if (_doing)
            return false;
        _onComplete = onComplete;
        StartCoroutine(horizontalFlipTexture(source));
        _doing = true;
        return true;
    }

    private IEnumerator horizontalFlipTexture(Texture2D source)
    {
        yield return new WaitForEndOfFrame();
        //得到图片的宽高
        int width = source.width;
        int height = source.height;

        Texture2D flipTexture = new Texture2D(width, height);

        for (int i = 0; i < width; i++)
        {
            flipTexture.SetPixels(i, 0, 1, height, source.GetPixels(width - i - 1, 0, 1, height));
        }
        flipTexture.Apply();

        _doing = false;
        _onComplete?.Invoke(flipTexture);
    }

    public bool VerticalFlipTexture(Texture2D source, System.Action<Texture2D> onComplete)
    {
        if (_doing)
            return false;
        _onComplete = onComplete;
        StartCoroutine(verticalFlipTexture(source));
        _doing = true;
        return true;
    }

    private IEnumerator verticalFlipTexture(Texture2D texture)
    {
        yield return new WaitForEndOfFrame();
        //得到图片的宽高
        int width = texture.width;
        int height = texture.height;

        Texture2D flipTexture = new Texture2D(width, height);
        for (int i = 0; i < height; i++)
        {
            flipTexture.SetPixels(0, i, width, 1, texture.GetPixels(0, height - i - 1, width, 1));
        }
        flipTexture.Apply();

        _doing = false;
        _onComplete?.Invoke(flipTexture);
    }

    public bool Blend(Texture2D source, Texture2D target, System.Action<Texture2D> onComplete)
    {
        if (_doing)
            return false;
        _onComplete = onComplete;
        StartCoroutine(blend(source, target));
        _doing = true;
        return true;
    }

    private IEnumerator blend(Texture2D source, Texture2D target)
    {
        yield return new WaitForEndOfFrame();
        if (source.width == target.width && source.height == target.height)
        {
            for (int h = 0; h < target.height; ++h)
            {
                for (int w = 0; w < target.width; ++w)
                {
                    Color sc = source.GetPixel(w, h);
                    Color tc = target.GetPixel(w, h);

                    float a = Mathf.Max(1 - sc.a, 0);
                    Color c = Color.black;
                    c.a = 1;
                    c.r = sc.r * sc.a + tc.r * a;
                    c.g = sc.g * sc.a + tc.g * a;
                    c.b = sc.b * sc.a + tc.b * a;
                    target.SetPixel(w, h, c);
                }
            }

            target.Apply();
        }

        _doing = false;
        _onComplete?.Invoke(target);
    }
}
