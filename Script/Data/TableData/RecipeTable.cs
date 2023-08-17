using System.Collections.Generic;
public class RecipeTable : TableBase
{
    Dictionary<int, Data.RecipeData> _dataDictionary = new Dictionary<int, Data.RecipeData>();
    public List<Data.RecipeData> GetDataDictionary(int objectIndex) => _recipeData[objectIndex];
    HashSet<int> _containMaterialIndexHash;
    Dictionary<int, List<Data.RecipeData>> _recipeData = new Dictionary<int, List<Data.RecipeData>>();
    public bool IsContainsMaterialNeedable(int index) => _containMaterialIndexHash.Contains(index);
    public Data.RecipeData this[int index] {
        get {
            if (_dataDictionary.ContainsKey(index))
                return _dataDictionary[index];

            return null;
        }
    }
    protected override void OnLoad()
    {
        LoadData(_tableName);
        _containMaterialIndexHash = new HashSet<int>();
        foreach (var contents in _dataDic)
        {
            Data.RecipeData data = new Data.RecipeData(contents.Key, contents.Value);
            int[] targetObjectIndexArr = System.Array.ConvertAll(contents.Value["CraftPossibleObject"].Split('|'), v => int.Parse(v));
            for (int i = 0; i< targetObjectIndexArr.Length; ++i)
            {
                if (!_recipeData.ContainsKey(targetObjectIndexArr[i]))
                    _recipeData.Add(targetObjectIndexArr[i], new List<Data.RecipeData>());

                _recipeData[targetObjectIndexArr[i]].Add(data);
            }
            _dataDictionary.Add(contents.Key, data);
            foreach (var element in data.MaterialDictionary)
                if (!_containMaterialIndexHash.Contains(element.Key))
                    _containMaterialIndexHash.Add(element.Key);
        }
        _dataDic = null;
    }
}
