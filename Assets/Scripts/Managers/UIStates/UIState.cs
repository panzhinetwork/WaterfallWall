using UnityEngine;
using UIFramework;

public abstract class UIState : MonoBehaviour {

    private UIPack _uiPack;

    protected UIPack Pack {
        get { return _uiPack; }
    }

    public virtual void Init(UIPack uiPack) {
        _uiPack = uiPack;
    }
    
    public abstract void EnterState();
    public abstract void ExitState();
}
