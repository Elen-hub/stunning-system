using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : BaseCamera
{
    Dictionary<eWeatherType, IWeather> _weatherDic = new Dictionary<eWeatherType, IWeather>();
    public Dictionary<eWeatherType, IWeather> GetWeather => _weatherDic;
    Transform _followTarget;
    public Transform SetActor {
        set {
            _followTarget = value;
            transform.SetParent(_followTarget);
            transform.localPosition = Vector3.forward * -5f;
            // transform.localEulerAngles = Vector3.right * 90;
            // _targetPosition = transform.position;
        }
    }
    public Transform SetTagActor {
        set => _followTarget = value;
    }
    Vector3 _targetPosition;
    float _currentLerpAmount;
    public override void Initialize()
    {
        base.Initialize();

        _weatherDic.Add(eWeatherType.Rain, GetComponentInChildren<Rain>());
        _weatherDic.Add(eWeatherType.Snow, GetComponentInChildren<Snow>());
    }
}
