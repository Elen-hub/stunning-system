using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class BuffData
    {
        public readonly int Index;
        public readonly int NameKey;
        public readonly int DescriptionKey;
        readonly int _iconHash = 0;
        public Sprite Icon => _iconHash != 0 ? ResourceManager.Instance.GetSprite(_iconHash) : null;
        public readonly bool IsStatusType;
        public readonly eStatusType StatusType;
        public readonly bool IsMul;
        public readonly eCustomBuffType CustomType;
        public readonly float Value;
        public readonly bool IsInterval;
        public readonly float IntervalTime;
        public readonly float TargetTime;
        public readonly int FirstStackIndex;
        public readonly int NextStackIndex;
        public readonly int EffectIndex;
        public readonly eAttachmentTarget AttachmentTarget;
        public BuffData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            NameKey = int.Parse(dataPair["NameKey"]);
            DescriptionKey = int.Parse(dataPair["DescriptionKey"]);
            if (Application.isPlaying)
            {
                if(!string.IsNullOrEmpty(dataPair["IconPath"]))
                    _iconHash = ResourceManager.Instance.AddSpriteHash(dataPair["IconPath"]);
            }
            string buffType = dataPair["BuffType"];
            if (EnumConverter.StatusTypeConvert.ContainsKey(buffType))
            {
                IsStatusType = true;
                StatusType = EnumConverter.StatusTypeConvert[buffType];
                IsMul = bool.Parse(dataPair["IsMul"]);
            }
            else
                CustomType = EnumConverter.CustomBuffConvert[buffType];

            IntervalTime = float.Parse(dataPair["IntervalTime"]);
            TargetTime = float.Parse(dataPair["TargetTime"]);
            IsInterval = IntervalTime > 0f;
            FirstStackIndex = int.Parse(dataPair["FirstStackIndex"]);
            NextStackIndex = int.Parse(dataPair["NextStackIndex"]);
            EffectIndex = int.Parse(dataPair["EffectIndex"]);
            if(EffectIndex != 0)
            {
                if (EnumConverter.AttachmentTargetConvert.ContainsKey(dataPair["EffectAttachpoint"]))
                    AttachmentTarget = EnumConverter.AttachmentTargetConvert[dataPair["EffectAttachpoint"]];
            }
        }
    }
}