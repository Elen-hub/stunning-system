using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderingManager : TSingletonMono<RenderingManager>
{
    PostProcess _postProcess;
    protected override void OnInitialize()
    {
        _postProcess = Instantiate(Resources.Load<PostProcess>("System/PostProcess"), transform);
    }
    public void StartFog()
    {
        _postProcess.OverlayFog.intensity.value = 0f;
        _postProcess.OverlayFog.active = true;
    }
    public void SetFogValue(float value) => _postProcess.OverlayFog.intensity.value = value;
    public void EndFog()
    {
        _postProcess.OverlayFog.active = false;
    }
}
