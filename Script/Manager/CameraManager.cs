using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FoW;
using UnityEngine.Rendering.UI;
public enum eCameraType
{
    MainCamera,
    UICamera,
}
public class CameraManager : TSingletonMono<CameraManager>
{
    FogOfWarTeam _fowTeam;
    eCameraType _currentCameraType;
    Dictionary<eCameraType, BaseCamera> _cameraDic;
    public bool IsFading => _cameraDic[eCameraType.MainCamera].IsFading;
    public Transform SetActor {
        set {
            GetCamera<MainCamera>(eCameraType.MainCamera).SetActor = value;
        }
    }
    protected override void OnInitialize()
    {
        _cameraDic = new Dictionary<eCameraType, BaseCamera>();
        CreateCamera(eCameraType.MainCamera);
        CreateCamera(eCameraType.UICamera);
        GameObject obj = Instantiate(Resources.Load<GameObject>("System/FogOfWar"), transform);
        _fowTeam = obj.GetComponent<FogOfWarTeam>();
        IsLoad = true;
    }
    BaseCamera CreateCamera(eCameraType type)
    {
        BaseCamera cam = Resources.Load<BaseCamera>("Camera/" + type.ToString());
        cam = Instantiate(cam, transform); 
        cam.Initialize();
        _cameraDic.Add(type, cam);
        return cam;
    }
    public BaseCamera GetCamera(eCameraType cameraType)
    {
        if (!_cameraDic.ContainsKey(cameraType))
            CreateCamera(cameraType);

        return _cameraDic[cameraType];
    }
    public T GetCamera<T>(eCameraType cameraType) where T : BaseCamera
    {
        if (!_cameraDic.ContainsKey(cameraType))
            _cameraDic.Add(cameraType, CreateCamera(cameraType));

        return _cameraDic[cameraType] as T;
    }
    public void FadeOn(bool useUI, eCameraFadeType type, float durationTime, System.Action action)
    {
        _cameraDic[eCameraType.MainCamera].FadeOn(type, durationTime, action);
        if (useUI) _cameraDic[eCameraType.UICamera].AssignEffect(type);
        else _cameraDic[eCameraType.UICamera].SetNullFade();
    }
    public void FadeOff(bool useUI, eCameraFadeType type, float durationTime, System.Action action)
    {
        _cameraDic[eCameraType.MainCamera].FadeOff(type, durationTime, action);
        if (useUI) _cameraDic[eCameraType.UICamera].AssignEffect(type);
        else _cameraDic[eCameraType.UICamera].SetNullFade();
    }
    public void FadeOff(bool useUI, System.Action action)
    {
        _cameraDic[eCameraType.MainCamera].FadeOff(action);
        if (useUI) _cameraDic[eCameraType.UICamera].AssignEffect(_cameraDic[eCameraType.MainCamera].CurrentFadeType);
        else _cameraDic[eCameraType.UICamera].SetNullFade();
    }
}
