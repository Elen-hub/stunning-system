using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoroutineUtility
{
    public static IEnumerator Wait(float targetTime)
    {
        if (targetTime == 0f)
            yield break;

        float deltaTime = 0f;
        while (deltaTime < targetTime)
        {
            yield return null;
            deltaTime += TimeManager.DeltaTime;
        }
    }
    public static IEnumerator IESetThrust(Transform trs, Vector3 direction, float speed, float maxDistance)
    {
        float distance = 0f;
        while(distance < maxDistance)
        {
            float currentSpeed = speed * TimeManager.DeltaTime;
            trs.position += direction * currentSpeed;
            distance += currentSpeed;
            yield return null;
        }
    }
    public static IEnumerator UIColorLerp(MaskableGraphic graphic, Color startColor, Color endColor, float time, IEnumerator chainEnumerator = null)
    {
        float elapsedTime = 0f;
        while (time >= elapsedTime)
        {
            elapsedTime +=TimeManager.DeltaTime;
            graphic.color = Color.Lerp(startColor, endColor, elapsedTime / time);
            yield return null;
        }
        if (chainEnumerator != null)
            yield return chainEnumerator;
    }
    public static IEnumerator UIAnchoredPositionLerp(RectTransform rectTransform, Vector2 startPosition, Vector2 endPosition, float time, IEnumerator chainEnumerator = null)
    {
        float elapsedTime = 0f;
        while (time >= elapsedTime)
        {
            elapsedTime +=TimeManager.DeltaTime;
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / time);
            yield return null;
        }
        if (chainEnumerator != null)
            yield return chainEnumerator;
    }
    public static IEnumerator LocalPositionLerp(Transform transform, Vector2 startPosition, Vector2 endPosition, float time, IEnumerator chainEnumerator = null)
    {
        float elapsedTime = 0f;
        while (time >= elapsedTime)
        {
            elapsedTime +=TimeManager.DeltaTime;
            transform.localPosition = Vector2.Lerp(startPosition, endPosition, elapsedTime / time);
            yield return null;
        }
        if (chainEnumerator != null)
            yield return chainEnumerator;
    }
    public static IEnumerator WorldPositionLerp(Transform transform, Vector2 startPosition, Vector2 endPosition, float time, IEnumerator chainEnumerator = null)
    {
        float elapsedTime = 0f;
        while (time >= elapsedTime)
        {
            elapsedTime +=TimeManager.DeltaTime;
            transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / time);
            yield return null;
        }
        if (chainEnumerator != null)
            yield return chainEnumerator;
    }
    public static IEnumerator ScaleLerp(Transform transform, Vector3 startSize, Vector3 endSize, float time, IEnumerator chainEnumerator = null)
    {
        float elapsedTime = 0f;
        while (time >= elapsedTime)
        {
            elapsedTime += TimeManager.DeltaTime;
            transform.localScale = Vector3.Lerp(startSize, endSize, elapsedTime / time);
            yield return null;
        }
        if (chainEnumerator != null)
            yield return chainEnumerator;
    }
    public static IEnumerator UIScaleLerp(RectTransform rectTransform, Vector3 startLocalScale, Vector2 endLocalScale, float time, IEnumerator chainEnumerator = null)
    {
        float elapsedTime = 0f;
        while (time >= elapsedTime)
        {
            elapsedTime +=TimeManager.DeltaTime;
            rectTransform.localScale = Vector3.Lerp(startLocalScale, endLocalScale, elapsedTime / time);
            yield return null;
        }
        if (chainEnumerator != null)
            yield return chainEnumerator;
    }

    public static IEnumerator UIRotationLerp(RectTransform rectTransform, Vector3 startLocalRotation, Vector3 endLocalRotation, float time, IEnumerator chainEnumerator = null)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = Quaternion.Euler(startLocalRotation);
        Quaternion endRotation = Quaternion.Euler(endLocalRotation);

        while (time >= elapsedTime)
        {
            elapsedTime +=TimeManager.DeltaTime;
            rectTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / time);
            yield return null;
        }
        if (chainEnumerator != null)
            yield return chainEnumerator;
    }
    public static IEnumerator UIRotationLerp_Lamda(RectTransform rectTransform, Vector3 startLocalRotation, Vector3 endLocalRotation, float time, global::System.Action OnCompleted)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = Quaternion.Euler(startLocalRotation);
        Quaternion endRotation = Quaternion.Euler(endLocalRotation);

        while (time >= elapsedTime)
        {
            elapsedTime +=TimeManager.DeltaTime;
            rectTransform.localRotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / time);
            yield return null;
        }

        if (OnCompleted != null)
            OnCompleted.Invoke();
    }

}