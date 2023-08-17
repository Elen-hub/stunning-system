using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UBaseSubUI : MonoBehaviour
{
    protected eLanguage _currentLanguage;
    public UBaseSubUI Initialize()
    {
        InitReference();
        return this;
    }
    protected virtual void InitReference()
    {

    }
    public void RefreshLocalizing()
    {
        if (_currentLanguage != RuntimePreference.Preference.Language)
        {
            _currentLanguage = RuntimePreference.Preference.Language;
            OnRefreshLocalizing();
        }
    }
    protected virtual void OnRefreshLocalizing()
    {

    }
}
