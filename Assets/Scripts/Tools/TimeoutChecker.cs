using EventDef;
using UIFramework;
using UnityEngine;

public class TimeoutChecker : MonoBehaviour
{
    public static TimeoutChecker instance;

    private float _maxTimeout = 10;

    private float _timer;

    private void Awake()
    {
        instance = this;
    }

    void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
        _timer = Time.time + _maxTimeout;
    }

    public void SetTimeOut(float t)
    {
        _maxTimeout = t;
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            _timer = Time.time + _maxTimeout;
        }

        if (_timer <= Time.time)
        {
            Events.Get<TimeoutEvent>().Raise();
            enabled = false;
        }
    }
}
