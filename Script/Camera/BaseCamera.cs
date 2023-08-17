using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalSalmon.Fade;

public enum eCameraFadeType
{
    BandWipe,
    BarnDoorOpenVertical,
    BarnDoorStretch,
    BattleWipe,
    BattleWipeExtreme,
    Blur,
    Fade,
    Rubicks,
    RubicksHorizontal,
    RubicksVertical,
    Zoom,
    ZoomCustom,
    ZoomSmall,
    End,
}
public class BaseCamera : MonoBehaviour
{
    public bool IsFading => _fadeEffectProcess.IsFading;
    Dictionary<eCameraFadeType, FadeEffect> _fadeEffectDic = new Dictionary<eCameraFadeType, FadeEffect>();
    eCameraFadeType _currentType;
    public eCameraFadeType CurrentFadeType => _currentType;
    protected FadePostProcess _fadeEffectProcess;
    public FadePostProcess FadeProcess => _fadeEffectProcess;
    Camera _camera;
    public Camera Camera => _camera;
    public bool Enabled
    {
        get{ return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }
    protected Vector3 _shakeCurrentPosition;
    public virtual void Initialize()
    {
        _camera = GetComponentInChildren<Camera>();
        _fadeEffectProcess = GetComponent<FadePostProcess>();
        for(eCameraFadeType i = 0; i< eCameraFadeType.End; ++i)
            _fadeEffectDic.Add(i, Instantiate(Resources.Load<FadeEffect>($"FadeEffect/{i.ToString()}")));

        _currentType = 0;

        // transform.position = Vector3.right * 8f;

        //float targetRate = 1080f / 1920f;
        //float screenRate = (float)Screen.height / Screen.width;
        //Camera.orthographicSize = 6.4f * screenRate / targetRate;
    }
    public void FadeOn(eCameraFadeType type, float durationTime, System.Action action)
    {
        _currentType = type;
        _fadeEffectProcess.AssignEffect(_fadeEffectDic[type], true);
        _fadeEffectProcess.effectDuration = durationTime;
        _fadeEffectProcess.FadeDown(false, action);
    }
    public void AssignEffect(eCameraFadeType type)
    {
        _currentType = type;
        _fadeEffectProcess.AssignEffect(_fadeEffectDic[type], false);
    }
    public void SetNullFade()
    {
        _currentType = eCameraFadeType.End;
        _fadeEffectProcess.AssignEffect(null, false);
    }
    public void FadeOff(eCameraFadeType type, float durationTime, System.Action action)
    {
        _currentType = type;
        _fadeEffectProcess.AssignEffect(_fadeEffectDic[type], true);
        _fadeEffectProcess.effectDuration = durationTime;
        _fadeEffectProcess.FadeUp(false, action);
    }
    public void FadeOff(System.Action action)
    {
        _fadeEffectProcess.FadeUp(false, action);
    }
}
