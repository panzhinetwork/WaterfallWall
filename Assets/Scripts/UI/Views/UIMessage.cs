using EventDef;
using UIFramework;
using UnityEngine;

/// <summary>
/// ui提示信息界面
/// </summary>
public class UIMessage : UIAbstractView<UIMessageData> {
    [SerializeField] Message _message = default;

    protected override void OnDataSet () {
        _message.Show (Data.MessageContent);
    }

    public static void Show (string message) {
        UIMessageData uiMessageData = new UIMessageData {
            MessageContent = message
        };
        Events.Get<UIMessageOpenEvent> ().Raise (uiMessageData);
    }
}