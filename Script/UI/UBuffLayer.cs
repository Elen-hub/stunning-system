using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UBuffLayer : MonoBehaviour
{
    int _prevCount;
    ActorBuff _actorBuff;
    BuffElement[] _buffElementArr;
    public void Initialize()
    {
        _buffElementArr = GetComponentsInChildren<BuffElement>(true);
        for (int i = 0; i < _buffElementArr.Length; ++i)
            _buffElementArr[i].Initialize();
    }
    public void Enable(Character character)
    {
        _actorBuff = character.ActorBuff;
        Refresh();
        gameObject.SetActive(true);
    }
    void Refresh()
    {
        _actorBuff.IsModify = false;
        int count = _actorBuff.GetBuffList.Count;
        for (int i = 0; i < count; ++i)
            _buffElementArr[i].Enable(_actorBuff.GetBuffList[i]);

        for (int i = count; i < _prevCount; ++i)
            _buffElementArr[i].Disable();

        _prevCount = count;
    }
    private void LateUpdate()
    {
        if (_actorBuff == null)
            return;

        if (_actorBuff.IsModify)
            Refresh();
    }
}
