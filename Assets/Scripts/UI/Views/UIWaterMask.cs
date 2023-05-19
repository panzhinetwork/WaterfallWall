using EventDef;
using System;
using System.Collections;
using System.IO;
using UIFramework;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class UIWaterMask : UIAbstractView
{
#if !NEW_AUTHORIZATION
    [DllImport("Auth64")]
    public static extern int Permission(string mac, string key);
#endif

    protected override void OnDataSet()
    {
#if !UNITY_EDITOR
#if !NEW_AUTHORIZATION
        if (Permission(Utils.GetMachineID(), AppSettings.instance.appId) == 1)
        {
            Events.Get<UIWaterMaskCloseEvent>().Raise();
        }
#else
        if (AppData.instance.paid)
        {
            Events.Get<UIWaterMaskCloseEvent>().Raise();
        }
        else
        {
            StartCheck();
        }
#endif
#endif
    }

    private string GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    private bool Check(string key, string timeStamp)
    {
        string text = AppSettings.instance.appId + Utils.GetMachineID() + timeStamp + "1";
        string md5 = Utils.CreateMD5Hash(text);
        return key == md5;
    }

    private void StartCheck()
    {
        string timeStamp = GetTimeStamp();
        string url = string.Format("http://www.dudutushu.com/auth/getauth?mac={0}&key={1}&t={2}", Utils.GetMachineID(), AppSettings.instance.appId, timeStamp);
        HttpWraper.instance.StartGet(url, (error, content) =>
        {
            if (error)
            {
                UIMessage.Show("网络异常，请重试");
                return;
            }

            if (!string.IsNullOrEmpty(content) && content != "0")
            {
                if (Check(content, timeStamp))
                {
                    AppData.instance.key = content;
                    AppData.instance.timeStamp = timeStamp;
                    Events.Get<UIWaterMaskCloseEvent>().Raise();
                }
#if UNITY_EDITOR
                Debug.Log(content);
#endif
            }
        });
    }
}