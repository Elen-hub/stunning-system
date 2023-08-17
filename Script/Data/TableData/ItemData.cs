using UnityEngine;
using System.Collections.Generic;

namespace Data
{
    public class ItemData
    {
        public readonly int Index;
        public readonly string Name;
        public readonly eItemType Type;
        public readonly int SubType;
        public readonly int ReferenceIndex;
        public readonly int ExReferenceIndex;
        readonly int _iconHash;
        public UnityEngine.Sprite Icon => _iconHash != 0 ? ResourceManager.Instance.GetSprite(_iconHash) : null;
        public readonly int MergeCount;
        public readonly int NameKey;
        public readonly int DescriptionKey;
        public readonly int Price;
        public ItemData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
#if UNITY_EDITOR
            Name = dataPair["Name"];
#endif
            Type = EnumConverter.ItemTypeConvert[dataPair["Type"]];
            SubType = int.Parse(dataPair["SubType"]);
            NameKey = int.Parse(dataPair["NameKey"]);
            DescriptionKey = int.Parse(dataPair["DescriptionKey"]);
            ReferenceIndex = int.Parse(dataPair["ReferenceIndex"]);
            ExReferenceIndex = int.Parse(dataPair["ExReferenceIndex"]);
            if (Application.isPlaying)
            {
                if (!string.IsNullOrEmpty(dataPair["IconPath"]))
                    _iconHash = ResourceManager.Instance.AddSpriteHash(dataPair["IconPath"]);
            }
            MergeCount = int.Parse(dataPair["MergeCount"]);
            Price = int.Parse(dataPair["Price"]);
        }
    }
}