using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Data
{
    public class ObjectData
    {
        public readonly int Index;
        public readonly string Name;
        public readonly int NameKey;
        public readonly string ResourcePath;
        public readonly eStaticActorType StaticActorType;
        public readonly eIngredientType IngredientType;
        public readonly float HP;
        public readonly int RewardIndex;
        public List<Vector2Int> TileList;
        public ObjectData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
#if UNITY_EDITOR
            Name = dataPair["Name"];
#endif
            NameKey = int.Parse(dataPair["NameKey"]);
            ResourcePath = dataPair["ResourcePath"];
            SetTileDataAsync().Forget();
            StaticActorType = EnumConverter.StaticActorTypeConvert[dataPair["StaticActorType"]];
            HP = float.Parse(dataPair["HP"]);
            string ingredient = dataPair["Ingredient"];
            if(!string.IsNullOrEmpty(ingredient))
            {
                eIngredientType[] ingredientArr = System.Array.ConvertAll(ingredient.Split('|'), v => EnumConverter.IngredientTypeConvert[v]);
                for (int i = 0; i < ingredientArr.Length; ++i)
                    IngredientType |= ingredientArr[i];
            }
            if (!string.IsNullOrEmpty(dataPair["RewardIndex"]))
                RewardIndex = int.Parse(dataPair["RewardIndex"]);
        }
        async UniTaskVoid SetTileDataAsync()
        {
            StaticActor staticActor = await Resources.LoadAsync<StaticActor>(ResourcePath) as StaticActor;
            TileList = staticActor.GetTileBlockCoordList;
        }
    }
}