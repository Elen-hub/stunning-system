using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour, IWeather
{
    ParticleSystem _particleSystem;
    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
    public void StartWeather()
    {
        _particleSystem.Play();
    }
    public void EndWeather()
    {
        _particleSystem.Stop();
    }
}
