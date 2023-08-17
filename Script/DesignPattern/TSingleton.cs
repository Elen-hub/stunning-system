using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TSingleton<T> where T : TSingleton<T>, new()
{
    static T _instance;
    public bool IsLoad;
    bool _isFirstLoad;
    public void Initialize()
    {
        if (!_isFirstLoad)
        {
            _isFirstLoad = true;
            OnInitialize();
        }
    }
    protected abstract void OnInitialize();
    public static T Instance
    {
        get
        {
            if (_instance != null)
                return _instance;

            _instance = new T();
            _instance.Initialize();
            return _instance;
        }
    }
}
