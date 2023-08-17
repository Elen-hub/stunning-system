using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcess : MonoBehaviour
{
    public Bloom Bloom { get; private set; }
    public Vignette Vignette { get; private set; }
    public SCPE.Overlay OverlayFog { get; private set; }
    private void Awake()
    {
        Volume postProcessVolume = GetComponent<Volume>();

        Bloom bloom;
        if (postProcessVolume.profile.TryGet(out bloom))
            Bloom = bloom;

        Vignette vignette;
        if (postProcessVolume.profile.TryGet(out vignette))
            Vignette = vignette;

        SCPE.Overlay overlayFog;
        if (postProcessVolume.profile.TryGet(out overlayFog))
            OverlayFog = overlayFog;
    }
}
