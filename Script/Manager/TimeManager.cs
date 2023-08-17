using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : TSingleton<TimeManager>
{
    public static float DeltaTime => Time.deltaTime;
    protected override void OnInitialize()
    {
        
    }
}
