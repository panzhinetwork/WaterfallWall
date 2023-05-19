using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

public class HttpWraper:MonoBehaviour
{
    private static HttpWraper _sInstance;

    public static HttpWraper instance
    {
        get
        {
            if (_sInstance == null)
            {
                GameObject go = new GameObject("HttpWraperHolder");
                DontDestroyOnLoad(go);
                _sInstance = go.AddComponent<HttpWraper>();
            }
            return _sInstance;
        }
    }

    public void StartPut(string username, string password, string serverPath, string filePath, Action<string> callback)
    {
        Put(username, password, serverPath, filePath, callback);
    }

    public void StartGet(string serverPath, Action<bool, string> callback)
    {
        StartCoroutine(Get(serverPath, callback));
    }

    public void StartPost(string serverPath, Action<bool, string> callback)
    {
        StartCoroutine(Post(serverPath, callback));
    }

    IEnumerator Get(string serverPath, Action<bool, string> callback)
    {
        bool isError = false;
        string content = string.Empty;
        UnityWebRequest webRequest = UnityWebRequest.Get(serverPath);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            isError = true;
            content = webRequest.error;
        }
        else
        {
            content = webRequest.downloadHandler.text;
        }

        callback.Invoke(isError, content);
    }

    IEnumerator Post(string serverPath, Action<bool, string> callback)
    {
        bool isError = false;
        string content = string.Empty;
        WWWForm form = new WWWForm();

        //form.AddField("key", "value");
        //form.AddField("name", "mafanwei");
        //form.AddField("blog", "qwe25878");

        UnityWebRequest webRequest = UnityWebRequest.Post(serverPath, form);
        yield return webRequest.SendWebRequest();
        if (webRequest.isHttpError || webRequest.isNetworkError)
        {
            isError = true;
            content = webRequest.error;
        }
        else
        {
            content = webRequest.downloadHandler.text;
        }

        callback.Invoke(isError, content);
    }

    public void Put(string username, string password, string serverPath, string filePath, Action<string> callback)
    {
        string content = string.Empty;
        WebClient client = new WebClient();
        Uri uri = new Uri(serverPath + new FileInfo(filePath).Name);

        client.UploadProgressChanged += (object sender, UploadProgressChangedEventArgs e) =>
        {

        };

        client.UploadFileCompleted += (object sender, UploadFileCompletedEventArgs e) =>
        {
            if (e.Error != null)
            {
                content = e.Error.ToString();
            }
            else
            {
                content = System.Text.Encoding.UTF8.GetString(e.Result);
            }

            if (string.IsNullOrEmpty(content))
            {
                content = "success";
            }

            callback.Invoke(content);
        };

        client.Credentials = new NetworkCredential(username, password);
        client.UploadFileAsync(uri, "STOR", filePath);
    }
}
