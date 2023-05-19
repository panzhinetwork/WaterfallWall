using EventDef;
using UIFramework;
using UnityEngine;

public class PlayState : UIState
{
    public override void EnterState()
    {
        Pack.ShowView<UIMain>();
        OnUIWaterMaskOpen();
    }

    public override void ExitState()
    {
    }

    private void Awake()
    {
        Events.Get<UIWaterMaskOpenEvent>().AddListener(OnUIWaterMaskOpen);
        Events.Get<UIWaterMaskCloseEvent>().AddListener(OnUIWaterMaskClose);

        Events.Get<UIMessageOpenEvent>().AddListener(OnUIMessageOpen);
        Events.Get<UIMessageCloseEvent>().AddListener(OnUIMessageClose);
    }

    private void OnDestroy()
    {
        Events.Get<UIWaterMaskOpenEvent>().RemoveListener(OnUIWaterMaskOpen);
        Events.Get<UIWaterMaskCloseEvent>().RemoveListener(OnUIWaterMaskClose);

        Events.Get<UIMessageOpenEvent>().RemoveListener(OnUIMessageOpen);
        Events.Get<UIMessageCloseEvent>().RemoveListener(OnUIMessageClose);
    }

    private void OnUIWaterMaskOpen()
    {
        Pack.ShowView<UIWaterMask>();
    }

    private void OnUIWaterMaskClose()
    {
        Pack.HideView<UIWaterMask>();
    }

    private void OnUIMessageOpen(UIMessageData data)
    {
        Pack.ShowView<UIMessage, UIMessageData>(data);
    }

    private void OnUIMessageClose()
    {
        Pack.HideView<UIMessage>();
    }
}