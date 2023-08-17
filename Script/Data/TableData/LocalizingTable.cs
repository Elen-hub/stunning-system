using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizingTable : TableBase
{
    Dictionary<int, Dictionary<eLanguage, string>> _localizingDic = new Dictionary<int, Dictionary<eLanguage, string>>();
    public string this[int index] {
        get {
            if (_localizingDic.ContainsKey(index))
                return _localizingDic[index][RuntimePreference.Preference.Language];

            return null;
        }
    }
    protected override void OnLoad()
    {
        LoadData(_tableName);
        foreach (var contents in _dataDic)
        {
            if (!_localizingDic.ContainsKey(contents.Key))
                _localizingDic.Add(contents.Key, new Dictionary<eLanguage, string>());

            _localizingDic[contents.Key].Add(eLanguage.Korean, contents.Value["KR"].Replace("\\n", "\n").Replace("P!0",","));
            _localizingDic[contents.Key].Add(eLanguage.English, contents.Value["EN"].Replace("\\n", "\n").Replace("P!0", ","));
        }
    }
}
