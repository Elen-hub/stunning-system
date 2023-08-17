using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightController : MonoBehaviour
{
    FieldLight _globalLight;
    FieldLight[] _lightHandlerArr;
    [SerializeField] protected eTimeTag _timeTag;
    public eTimeTag GetTimeTag => _timeTag;
    [SerializeField] bool _isLerp;
    [SerializeField] float _lerpCurrentTime;
    [SerializeField] float _lerpTargetTime;
    public void Initialize()
    {
        _globalLight = transform.Find("GlobalLight").GetComponent<FieldLight>();
        if (_globalLight != null)
            _globalLight.Initialize();

        //_lightHandlerArr = transform.Find("SubLight").GetComponentsInChildren<FieldLight>();
        //if (_lightHandlerArr != null)
        //    for (int i = 0; i < _lightHandlerArr.Length; ++i)
        //        _lightHandlerArr[i].Initialize();
    }
    public void UpdateMinute()
    {
        if (_isLerp)
        {
            if (_lerpCurrentTime < _lerpTargetTime)
                ++_lerpCurrentTime;
            else
                _isLerp = false;

            if (_timeTag == eTimeTag.Dawn)
                Lerp(eTimeTag.Midnight, eTimeTag.Dawn, _lerpCurrentTime / _lerpTargetTime);
            else
                Lerp(_timeTag - 1, _timeTag, _lerpCurrentTime / _lerpTargetTime);
        }
    }
    public void Lerp(eTimeTag before, eTimeTag after, float amount)
    {
 #if !UNITY_SERVER
        if (_globalLight != null)
            _globalLight.Lerp(before, after, amount);
        //if (_lightHandlerArr != null)
        //    for (int i = 0; i < _lightHandlerArr.Length; ++i)
        //        _lightHandlerArr[i].Lerp(before, after, amount);
 #endif
    }
    public void SetTime(int hour)
    {

    }
    public void SetTimeTag(eTimeTag timeTag)
    {
        _timeTag = timeTag;
        _isLerp = true;
        _lerpCurrentTime = 0f;
        switch (_timeTag)
        {
            default:
                _lerpTargetTime = (EnvironmentManager.Instance.TimeTagDictionary[_timeTag] - EnvironmentManager.Instance.TimeTagDictionary[_timeTag-1]) * 0.2f;
                break;
            case eTimeTag.Dawn:
                _lerpTargetTime = 120f;
                break;
        }
//#if !UNITY_SERVER
//        if (_globalLight != null)
//            _globalLight.SetTime(timeTag);
//        //if (_lightHandlerArr != null)
//        //    for (int i = 0; i < _lightHandlerArr.Length; ++i)
//        //        _lightHandlerArr[i].SetTime(timeTag);
//#endif
    }
}
