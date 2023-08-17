using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class USmartWatch_TimeLayer : USmartWatchSubUI
{
    Image _weatherIcon;
    TextMeshProUGUI _timeText;
    TextMeshProUGUI _coordXText;
    TextMeshProUGUI _coordYText;
    int _currentHour;
    int _currentMiniute;

    int _currentCoordX;
    int _currentCoordY;

    Dictionary<eWeatherType, Sprite> _weatherIconDicrionary;
    protected override void InitReference()
    {
        base.InitReference();

        _weatherIcon = transform.Find("Img_Weather").GetComponent<Image>();
        _timeText = transform.Find("Text_Time").GetComponent<TextMeshProUGUI>();
        _coordXText = transform.Find("Text_CoordX").GetComponent<TextMeshProUGUI>();
        _coordYText = transform.Find("Text_CoordY").GetComponent<TextMeshProUGUI>();

        _weatherIconDicrionary = new Dictionary<eWeatherType, Sprite>((int)eWeatherType.End);
        _weatherIconDicrionary.Add(eWeatherType.None, Resources.Load<Sprite>("Sprite/Weather/Sun"));
        _weatherIconDicrionary.Add(eWeatherType.Rain, Resources.Load<Sprite>("Sprite/Weather/Rain"));
        _weatherIconDicrionary.Add(eWeatherType.Snow, Resources.Load<Sprite>("Sprite/Weather/Snow"));
        _weatherIconDicrionary.Add(eWeatherType.Cloudy, Resources.Load<Sprite>("Sprite/Weather/Cloud"));
        _weatherIconDicrionary.Add(eWeatherType.Fog, Resources.Load<Sprite>("Sprite/Weather/Water"));
    }
    public override void Enable()
    {
        EnvironmentManager.Instance.WeatherController.OnChagnedWeatherEvent += OnChangedWeatherCallback;
        if (EnvironmentManager.Instance.WeatherController.CurrentWeahterHandler != null)
            OnChangedWeatherCallback(EnvironmentManager.Instance.WeatherController.CurrentWeahterHandler.WeahterType);
        else
            OnChangedWeatherCallback(eWeatherType.None);

        EnvironmentManager.Instance.OnChangedHour += OnChangedHourCallback;
        EnvironmentManager.Instance.OnChangedMinute += OnChangedMinuteCallback;

        OnChangedHourCallback(EnvironmentManager.Instance.GetHour);
        OnChangedMinuteCallback(EnvironmentManager.Instance.GetMinute);

        gameObject.SetActive(true);
    }
    public override void Disable()
    {
        EnvironmentManager.Instance.WeatherController.OnChagnedWeatherEvent -= OnChangedWeatherCallback;
        EnvironmentManager.Instance.OnChangedHour -= OnChangedHourCallback;
        EnvironmentManager.Instance.OnChangedMinute -= OnChangedMinuteCallback;

        gameObject.SetActive(false);
    }
    public void OnChangedWeatherCallback(eWeatherType weather)
    {
        _weatherIcon.sprite = _weatherIconDicrionary[weather];
    }
    void OnChangedHourCallback(int hour)
    {
        _currentHour = hour;
        _timeText.text = string.Format("{0:00}:{1:00}", _currentHour, _currentMiniute);
    }
    void OnChangedMinuteCallback(int minitue)
    {
        _currentMiniute = minitue;
        _timeText.text = string.Format("{0:00}:{1:00}", _currentHour, _currentMiniute);
    }
    void OnUpdateCoord()
    {
        if (PlayerManager.Instance.Me.Character == null)
            return;

        Vector2 characterPos = PlayerManager.Instance.Me.Character.Position;
        int roundX = ClientMath.RoundToInt(characterPos.x);
        int roundY = ClientMath.RoundToInt(characterPos.y);

        if (_currentCoordX != roundX)
        {
            _currentCoordX = roundX;
            _coordXText.text = string.Format("X: {0}", _currentCoordX);
        }
        if (_currentCoordY != roundY)
        {
            _currentCoordY = roundY;
            _coordYText.text = string.Format("Y: {0}", _currentCoordY);
        }
    }
    private void LateUpdate()
    {
        OnUpdateCoord();
    }
}
