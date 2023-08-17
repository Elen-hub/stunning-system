using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
public enum eLanguage
{
    None,
    Korean,
    English,
}
public class LocalizingManager : TSingletonMono<LocalizingManager>
{
    Dictionary<int, Dictionary<eLanguage, string[]>> _parsingSourceArray = new Dictionary<int, Dictionary<eLanguage, string[]>>();
    protected override void OnInitialize()
    {
        
    }
    public string GetLocalizing(int key)
    {
        if (DataManager.Instance.LocalizingTable[key] != null)
            return DataManager.Instance.LocalizingTable[key];

        return null;
    }
    public string GetLocalizing(int key, params object[] parsingParameters)
    {
        return string.Format(GetLocalizing(key), parsingParameters);
    }
}
