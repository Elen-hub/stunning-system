using System.Collections.Generic;

namespace Data
{
    public class EnvironmentData
    {
        public readonly int Index;
        public float SunriseTime;
        public float SunSetTime;
        public float MinimumTemp;
        public float MaximumTemp;
        public float FallProbability;
        public float[] FallAmount;
        public float SnowFallProbability;
        public float[] SnowFallAmount;
        public EnvironmentData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            float[] sunriseTime = System.Array.ConvertAll(dataPair["SunriseTime"].Split(':'), v => float.Parse(v));
            SunriseTime = sunriseTime[0] * 60f + sunriseTime[1];
            float[] sunsetTime = System.Array.ConvertAll(dataPair["SunSetTime"].Split(':'), v => float.Parse(v));
            SunSetTime = sunsetTime[0] * 60f + sunsetTime[1];
            MinimumTemp = float.Parse(dataPair["MinimumTemp"]);
            MaximumTemp = float.Parse(dataPair["MaximumTemp"]);
            FallProbability = float.Parse(dataPair["FallProbability"]);
            FallAmount = System.Array.ConvertAll(dataPair["FallAmount"].Split('~'), v => float.Parse(v));
            SnowFallProbability = float.Parse(dataPair["SnowFallProbability"]);
            SnowFallAmount = System.Array.ConvertAll(dataPair["SnowFallAmount"].Split('~'), v => float.Parse(v));
        }
    }
}