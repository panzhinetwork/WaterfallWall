using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UIFramework;
using System.Runtime.InteropServices;

public class AppManager : MonoBehaviour
{
    [SerializeField]
    AppSettings _appSettings = default;

    [SerializeField]
    UISettings _uiSettings = default;

    [SerializeField]
    private UIState _playState = default;

    private UIPack _uiPack;
    private UIState _currentState;

    [SerializeField] private bool _setResolution = false;

    private void ChangeStage(UIState stage)
    {
        if (_currentState != stage)
        {
            if (_currentState != null)
            {
                _currentState.ExitState();
            }
            _currentState = stage;
            _currentState.EnterState();
        }
    }

    private void Init()
    {
        ConfigFiles.ReadJsonConfigs();
        AppSettings.Initialize(_appSettings);
        AppData.instance.Init();
    }

    private void Awake()
    {
        CustomLogHandler.Init();
        Debug.Log("game start...");
        DontDestroyOnLoad(gameObject);

        Init();

        _uiPack = _uiSettings.CreatePackInstance();
        _playState.Init(_uiPack);
        ChangeStage(_playState);

        if (_setResolution)
        {
            SetResolution();
        }
    }

    private void OnApplicationQuit()
    {
        CustomLogHandler.Release();
        AppData.Save();
    }

    private void SetResolution()
    {
        Vector2 resolution = _uiPack.GetResolution();
        Screen.SetResolution((int)resolution.x, (int)resolution.y, true);
    }
}