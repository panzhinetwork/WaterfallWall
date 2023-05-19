using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AppData
{
    public static AppData s_instance;
    public static AppData instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = StorageUtility.Load<AppData>("AppData");
                if (s_instance == null)
                {
                    s_instance = new AppData();
                }
            }

            return s_instance;
        }
    }

    public bool paid
    {
        get
        {
            if (!_paid)
            {
                string text = AppSettings.instance.appId + Utils.GetMachineID() + timeStamp + "1";
                _paid = key == Utils.CreateMD5Hash(text);
            }

            return _paid;
        }
    }

    public string key;
    public string timeStamp;

    [NonSerialized]
    private bool _paid;

    public void Init()
    {
    }

    public static void Save()
    {
        StorageUtility.Save(instance, "AppData");
    }
}