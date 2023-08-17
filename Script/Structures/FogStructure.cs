using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FogStructure
{
    public float FogProbability { get; private set; }
    public float FogIntencity { get; private set; }
    public float FogDurationTime { get; private set; }

    public static FogStructure Default = new FogStructure()
    {
        FogProbability = 1,
        FogIntencity = 0.9f,
    };
}
