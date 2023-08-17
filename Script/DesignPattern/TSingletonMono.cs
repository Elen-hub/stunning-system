using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TSingletonMono<T> : MonoBehaviour where T : TSingletonMono<T>
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

            GameObject obj = new GameObject(typeof(T).ToString(), typeof(T));
#if UNITY_EDITOR
            if(Application.isPlaying)
#endif
                DontDestroyOnLoad(obj);
            _instance = obj.GetComponent<T>();
            _instance.Initialize();
            return _instance;
        }
    }
}
