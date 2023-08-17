using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class FieldLight : MonoBehaviour
{
    [SerializeField] Light2D _light;
    [SerializeField] protected eTimeTag _timeTag;
    [SerializeField] protected LightPair DawnLightPair;
    [SerializeField] protected LightPair TwilightLightPair;
    [SerializeField] protected LightPair MorningLightPair;
    [SerializeField] protected LightPair NoonLightPair;
    [SerializeField] protected LightPair EveningLightPair;
    [SerializeField] protected LightPair NightLightPair;
    [SerializeField] protected LightPair MidnightLightPair;
    Dictionary<eTimeTag, LightPair> _timeLightDictionary;
    public void Initialize()
    {
        _light = GetComponent<Light2D>();
        _timeLightDictionary = new Dictionary<eTimeTag, LightPair>()
        {
            { eTimeTag.Dawn, DawnLightPair },
            { eTimeTag.Twilight, TwilightLightPair },
            { eTimeTag.Morning, MorningLightPair },
            { eTimeTag.Noon, NoonLightPair },
            { eTimeTag.Evening, EveningLightPair },
            { eTimeTag.Night, NightLightPair },
            { eTimeTag.Midnight, MidnightLightPair },
        };
#if UNITY_SERVER
        _light.enabled = false;
#endif
    }
    public void SetTime(eTimeTag time)
    {
        _timeTag = time;
        SetTime();
    }
    public void Lerp(eTimeTag before, eTimeTag after, float amount)
    {
        LightPair beforeLgiht = _timeLightDictionary[before].IsUsed ? _timeLightDictionary[before] : LightPair.Blank;
        LightPair afterLight = _timeLightDictionary[after].IsUsed ? _timeLightDictionary[after] : LightPair.Blank;
        _light.intensity = Mathf.Lerp(beforeLgiht.Intensity, afterLight.Intensity, amount);
        _light.color = Color.Lerp(beforeLgiht.LightColor, afterLight.LightColor, amount);
    }
    public void SetTime()
    {
        _light.enabled = _timeLightDictionary[_timeTag].IsUsed;
        if (_timeLightDictionary[_timeTag].IsUsed)
        {
            _light.intensity = _timeLightDictionary[_timeTag].Intensity;
            _light.color = _timeLightDictionary[_timeTag].LightColor;
        }
    }
}
