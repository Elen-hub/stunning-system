using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LightPair
{
    public static LightPair Blank = new LightPair()
    {
        IsUsed = true,
        LightColor = Color.clear,
        Intensity = 0f,
    };
    [SerializeField] public bool IsUsed;
    [SerializeField] public Color LightColor;
    [SerializeField] public float Intensity;
}
