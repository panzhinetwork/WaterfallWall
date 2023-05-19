using System;
using UnityEngine;

/// <summary>
/// 数据存储工具类。加密和校验存储的数据。
/// </summary>
public static class StorageUtility {

    /// <summary>
    /// 保存文本数据。
    /// </summary>
    /// <param name="text">Text.</param>
    /// <param name="key">Key.</param>
    public static void Save(string text, string key) {
        var data = new StorageData();
        data.data = Obfuscator.Encode(text);
        SaveToPrefs(key, data);
    }

    /// <summary>
    /// 读取文本数据，做了数据校验，校验不通过则返回 null。
    /// </summary>
    /// <returns>The load.</returns>
    /// <param name="key">Key.</param>
    public static string Load(string key) {
        var data = LoadFromPrefs(key);
        if (data != null) {
            return Obfuscator.Decode(data.data);
        }
        return null;
    }

    /// <summary>
    /// 使用 JsonUtility 把 obj 转换成 json 文本来保存，所以 obj 必须满足 JsonUtility 的约束条件。
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="key">Key.</param>
    public static void Save(object obj, string key) {
        Save(JsonUtility.ToJson(obj), key);
    }

    /// <summary>
    /// 使用了 JsonUtility 来读取出对应的 object，所以之前保存的 object 必须满足 JsonUtility 的约束条件。
    /// </summary>
    /// <returns>The load.</returns>
    /// <param name="key">Key.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T Load<T>(string key) {
        var json = Load(key);
        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// 删除 key 对应的数据。
    /// </summary>
    /// <param name="key">Key.</param>
    public static void Delete(string key) {
        if (PlayerPrefs.HasKey(key)) {
            PlayerPrefs.DeleteKey(key);
        }
    }

    private static void SaveToPrefs(string key, StorageData data) {
        if (data.check == null) {
            data.ComputeCheck();
        }
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    private static StorageData LoadFromPrefs(string key) {
        if (!PlayerPrefs.HasKey(key)) {
            return null;
        }
        var json = PlayerPrefs.GetString(key);
        var data = JsonUtility.FromJson<StorageData>(json);
        if (data.IsValid()) {
            return data;
        }
        return null;
    }



    [System.Serializable]
    public class StorageData {

        public string data;
        public string check;

        public void ComputeCheck() {
            check = DataHash.CreateCheck(data);
        }

        public bool IsValid() {
            return DataHash.CreateCheck(data).Equals(check);
        }
    }

}




