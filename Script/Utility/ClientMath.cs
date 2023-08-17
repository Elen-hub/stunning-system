using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientMath
{
    public static int GetLayerOrder(Vector2 position) => -Mathf.CeilToInt(position.y * 10) + Mathf.CeilToInt(position.x);
    public static Vector2 GetDirectionFromOriginPoint(Vector2 axis, float angle)
    {
        float r = Mathf.Deg2Rad * angle;

        float x = (axis.x) * Mathf.Cos(r) - (axis.y) * Mathf.Sin(r);
        float y = (axis.x) * Mathf.Sin(r) + (axis.y) * Mathf.Cos(r);

        return new Vector2(x, y);
    }
    public static float GetAngleFromAxis(Vector2 axis)
    {
        return Vector2.Angle(Vector2.right, axis) * Mathf.Sign(axis.y);
    }
    public static Vector2 GetDirectionFromAxis(Vector2 axis)
    {
        float angle = GetAngleFromAxis(axis);
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    public static Vector3 GetDirectionFromAxis(float angle)
    {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    public static Vector2 GetDirection(float angle)
    {
        float r = Mathf.Deg2Rad * angle;

        float x = Mathf.Cos(r) - Mathf.Sin(r);
        float y = Mathf.Sin(r) +Mathf.Cos(r);

        return new Vector2(x, y);
    }
    public static Vector2 GetRandomPositionInCircle(float radius)
    {
        float randomRadian = Random.Range(0f, 2 * Mathf.PI);
        float randomRadius = Random.Range(-radius, radius);
        return new Vector2(Mathf.Cos(randomRadian), Mathf.Sin(randomRadian)) * randomRadius;
    }
    public static bool ProcessRandom(float percent) => Random.Range(0f, 1f) < percent;
    #region Round, RoundToInt (float ~ decimal)
    // https://docs.microsoft.com/ko-kr/dotnet/csharp/language-reference/builtin-types/numeric-conversions#explicit-numeric-conversions 
    public static int RoundToInt(float value)
    {
        if (value >= 0)
        {
            return (int)(value + 0.5d);
        }
        return (int)(value - 0.5d);
    }
    /// <summary>
    /// 소수 반올림. Value = 반올림 대상, Decimals = 몇 번째 소수점(0~)이 대상인지. 
    /// </summary>
    public static float Round(float value, int decimals)
    {

        if (decimals < 0)
            return value;

        if (value == 0f && decimals >= 0)
            return 0f;

        double roundpoint = 10d;

        for (int i = 0; i < decimals; i++)
        {
            roundpoint *= 10d;
        }

        value = (float)global::System.Math.Truncate(value * roundpoint);

        float targetDecimal = value % 10;

        if (targetDecimal >= 5)
            value += 10 - targetDecimal;
        else
            value -= targetDecimal;


        roundpoint = 0.1d;
        for (int i = 0; i < decimals; i++)
        {
            roundpoint *= 0.1d;
        }

        return (float)(value * roundpoint);
    }


    public static float FixedFloatBoundary(float value)
    {
        return value + 0.000001f;
    }

    public static Vector3Int FloorToInt(Vector3 v3)
    {
        return Vector3Int.FloorToInt(new Vector3(FixedFloatBoundary(v3.x), FixedFloatBoundary(v3.y), FixedFloatBoundary(v3.z)));
    }

    public static Vector2Int FloorToInt(Vector2 v2)
    {
        return Vector2Int.FloorToInt(new Vector2(FixedFloatBoundary(v2.x), FixedFloatBoundary(v2.y)));
    }

    //for(float i = 0.15f; i<=3f; i+=0.2f)
    //{
    //    Zelter.EditorDebug.LogError($" {i}f = "+ CLMath.Round(i, 1)+$" RoundToInt = {CLMath.RoundToInt(i)}");
    //
    //    if(i>=1f)
    //    Zelter.EditorDebug.LogError($"value%1 = {i % 1}");
    //}
    public static float Round(float Value)
    {
        return (float)global::System.Math.Round(Value, global::System.MidpointRounding.AwayFromZero);
    }

    public static double Round(double Value)
    {
        return global::System.Math.Round(Value, global::System.MidpointRounding.AwayFromZero);
    }

    public static double Round(double Value, int decimals)
    {
        return global::System.Math.Round(Value, decimals, global::System.MidpointRounding.AwayFromZero);
    }


    #endregion
}
