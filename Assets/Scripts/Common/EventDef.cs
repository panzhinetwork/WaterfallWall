
using UIFramework;

namespace EventDef
{
    public class UIMessageOpenEvent : BaseEvent<UIMessageData> { };
    public class UIMessageCloseEvent : BaseEvent { };
    public class UIWaterMaskOpenEvent : BaseEvent { };
    public class UIWaterMaskCloseEvent : BaseEvent { };
    public class TimeoutEvent : BaseEvent { };
}