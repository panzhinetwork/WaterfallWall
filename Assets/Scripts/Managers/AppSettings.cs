using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AppSettings", menuName = "App/AppSettings")]
public class AppSettings : ScriptableObject
{
    [SerializeField]
    private string _appId = "CONTROL";

    public static AppSettings instance { get; private set; }

    public string appId
    {
        get { return _appId; }
    }

    public static void Initialize(AppSettings database)
    {
        if (!instance) DestroyImmediate(instance);
        instance = Instantiate(database);
        instance.hideFlags = HideFlags.HideAndDontSave;
    }
}