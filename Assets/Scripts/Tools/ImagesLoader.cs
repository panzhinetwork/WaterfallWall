using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ImagesLoader : MonoBehaviour
{
    private static Dictionary<string, List<Sprite>> _sIamgesDict = new Dictionary<string, List<Sprite>>();
    public string[] dirs;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(LoadAllImages());
    }

    IEnumerator LoadAllImages()
    {
        foreach (var dir in dirs)
        {
            List<Sprite> sprites = new List<Sprite>();
            string path = Application.streamingAssetsPath + "/" + dir;
            string[] files = null;
            try
            {
                files = Directory.GetFiles(path);
            }
            catch
            {
                Debug.Log(path + "不存在");
            }

            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.EndsWith(".png") || file.EndsWith(".jpg"))
                    {
                        string name = file.Substring(path.Length);
#if UNITY_EDITOR
                        Debug.Log(name);
#endif
                        Sprite sprite = LoadOneImage(file, name);
                        sprites.Add(sprite);
                        yield return null;
                    }
                }
            }

            _sIamgesDict.Add(dir, sprites);
        }

        SceneManager.LoadScene("Main");
    }

    IEnumerator LoadImages(string path)
    {
        List<Sprite> sprites = new List<Sprite>();
        string[] files = null;
        string[] dirs = null;
        try
        {
            files = Directory.GetFiles(path);
            dirs = Directory.GetDirectories(path);
        }
        catch
        {
            Debug.Log(path + "不存在");
        }

        if (dirs != null)
        {
            foreach (var dir in dirs)
            {
                StartCoroutine(LoadImages(dir));
#if UNITY_EDITOR
                Debug.Log(dir);
#endif
            }
        }

        if (files != null)
        {
            foreach (var file in files)
            {
                if (file.EndsWith(".png") || file.EndsWith(".jpg"))
                {
                    string name = file.Substring(path.Length);
#if UNITY_EDITOR
                    Debug.Log(name);
#endif
                    Sprite sprite = LoadOneImage(file, name);
                    sprites.Add(sprite);
                    yield return null;
                }
            }
        }

        _sIamgesDict.Add(path, sprites);
    }

    public static Sprite LoadOneImage(string path, string name)
    {
        byte[] data = File.ReadAllBytes(path);
        TextureFormat textureFormat = TextureFormat.RGBA32;
        Texture2D tex = new Texture2D(1, 1, textureFormat, false, false);
        tex.name = name;
        tex.LoadImage(data);
        tex.Apply();
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        return sprite;
    }

    public static List<Sprite> GetImages(string dir)
    {
        if (_sIamgesDict.ContainsKey(dir))
        {
            return _sIamgesDict[dir];
        }

        return null;
    }

    public static Sprite GetImage(string name)
    {
        foreach(var dir in _sIamgesDict)
        {
            List<Sprite> sprites = dir.Value;
            foreach(var sprite in sprites)
            {
                if (sprite.texture.name == name)
                    return sprite;
            }
        }

        return null;
    }

    public static Sprite GetImage(string dir, string name)
    {
        if (_sIamgesDict.ContainsKey(dir))
        {
            List<Sprite> sprites = _sIamgesDict[dir];
            foreach (var sprite in sprites)
            {
                if (sprite.texture.name == name)
                    return sprite;
            }
        }

        return null;
    }
}
