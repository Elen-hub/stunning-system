using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class SkillData
    {
        public BaseSkill Skill;
        readonly int _iconHash;
        public readonly string Name;
        public UnityEngine.Sprite Icon => ResourceManager.Instance.GetSprite(_iconHash);
        public readonly float CoolTime;
        public readonly string ClassAssembly;
        public readonly string SerializeJson;
        public readonly int NameKey;
        public readonly int ExplanationKey;
        public readonly int InformationKey;
        public SkillData(Dictionary<string, string> dataPair)
        {
#if UNITY_EDITOR
            Name = dataPair["Name"];
#endif
            if (UnityEngine.Application.isPlaying)
                _iconHash = ResourceManager.Instance.AddSpriteHash(dataPair["IconPath"]);
            CoolTime = float.Parse(dataPair["CoolTime"]);
            NameKey = int.Parse(dataPair["NameKey"]);
            ClassAssembly = dataPair["ClassAssembly"];
            SerializeJson = dataPair["SerializeJson"];
            ExplanationKey = int.Parse(dataPair["ExplanationKey"]);
            InformationKey = int.Parse(dataPair["InformationKey"]);
        }
    }
}