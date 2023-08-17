using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum eTableName
{
    CharacterTable = 1<<0,
    MonsterTable = 1<<1,
    StatTable = 1 <<2,
    StatusWeightTable = 1 << 3,
    ObjectTable = 1 << 4,
    EnvironmentTable = 1 << 5,
    ItemTable = 1 << 6,
    LocalizingTable = 1 << 7,
    RecipeTable = 1 << 8,
    FoodTable = 1 << 9,
    PatternTable =  1 << 10,
    FSMConfigureTable = 1 << 11,
    SkillTable = 1 << 12,
    RewardTable = 1 << 13,
    ItemStatTable = 1 << 14,
    TileTable = 1 << 15,
    EffectTable = 1 << 16,
    BuffTable = 1 << 17,
    CropTable = 1 << 18,
}
public class DataManager : TSingleton<DataManager>
{
    Dictionary<eTableName, TableBase> _tableDic = new Dictionary<eTableName, TableBase>();
    public CharacterTable CharacterTable;
    public MonsterTable MonsterTable;
    public PatternTable PatternTable;
    public SkillTable SkillTable;
    public StatTable StatTable;
    public StatusWeightTable StatusWeightTable;
    public ObjectTable ObjectTable;
    public EnvironmentTable EnvironmentTable;
    public ItemTable ItemTable;
    public LocalizingTable LocalizingTable;
    public RecipeTable RecipeTable;
    public FSMConfigureTable FSMConfigureTable;
    public RewardTable RewardTable;
    public ItemStatTable ItemStatTable;
    public TileTable TileTable;
    public EffectTable EffectTable;
    public HouseTable HouseTable;
    public BuffTable BuffTable;
    public CropTable CropTable;
    protected override void OnInitialize()
    {
        SkillTable = LoadTable<SkillTable>(eTableName.SkillTable);
        SkillTable.Reload();
        PatternTable = LoadTable<PatternTable>(eTableName.PatternTable);
        PatternTable.Reload();
        CharacterTable = LoadTable<CharacterTable>(eTableName.CharacterTable);
        CharacterTable.Reload();
        StatTable = LoadTable<StatTable>(eTableName.StatTable);
        StatTable.Reload();
        StatusWeightTable = LoadTable<StatusWeightTable>(eTableName.StatusWeightTable);
        StatusWeightTable.Reload();
        ObjectTable = LoadTable<ObjectTable>(eTableName.ObjectTable);
        ObjectTable.Reload();
        EnvironmentTable = LoadTable<EnvironmentTable>(eTableName.EnvironmentTable);
        EnvironmentTable.Reload();
        ItemTable = LoadTable<ItemTable>(eTableName.ItemTable);
        ItemTable.Reload();
        LocalizingTable = LoadTable<LocalizingTable>(eTableName.LocalizingTable);
        LocalizingTable.Reload();
        RecipeTable = LoadTable<RecipeTable>(eTableName.RecipeTable);
        RecipeTable.Reload();
        FSMConfigureTable = LoadTable<FSMConfigureTable>(eTableName.FSMConfigureTable);
        FSMConfigureTable.Reload();
        RewardTable = LoadTable<RewardTable>(eTableName.RewardTable);
        RewardTable.Reload();
        ItemStatTable = LoadTable<ItemStatTable>(eTableName.ItemStatTable);
        ItemStatTable.Reload();
        MonsterTable = LoadTable<MonsterTable>(eTableName.MonsterTable); 
        MonsterTable.Reload();
        TileTable = LoadTable<TileTable>(eTableName.TileTable);
        TileTable.Reload();
        EffectTable = LoadTable<EffectTable>(eTableName.EffectTable);
        EffectTable.Reload();
        BuffTable = LoadTable<BuffTable>(eTableName.BuffTable);
        BuffTable.Reload();
        CropTable = LoadTable<CropTable>(eTableName.CropTable);
        CropTable.Reload();
        HouseTable = Resources.Load<ScriptableObject>("Database/ScriptableObject/HouseTable") as HouseTable;
        //EffectTable = LoadTable<EffectTable>(eTableName.EffectTable);
        //EffectTable.Reload();
        //StageTable = LoadTable<StageTable>(eTableName.StageTable);
        //StageTable.Reload();
        //LocalizingTable = LoadTable<LocalizingTable>(eTableName.LocalizingTable);
        //LocalizingTable.Reload();
        //StaticNPCTable = LoadTable<StaticNPCTable>(eTableName.StaticNPCTable);
        //StaticNPCTable.Reload();
        //ItemTable = LoadTable<ItemTable>(eTableName.ItemTable);
        //ItemTable.Reload();
        //EquipItemTable = LoadTable<EquipItemTable>(eTableName.EquipItemTable);
        //EquipItemTable.Reload();
        //UseItemTable = LoadTable<UseItemTable>(eTableName.UseItemTable);
        //UseItemTable.Reload();
        //StatTable = LoadTable<StatTable>(eTableName.StatTable);
        //StatTable.Reload();
    }
    public T LoadTable<T>(eTableName name, bool isReload = false) where T : TableBase, new()
    {
        if (_tableDic.ContainsKey(name))
        {
            if (isReload) _tableDic.Remove(name);
            else return null;
        }
        T t = new T();
        t.SetTableName = name.ToString();
        _tableDic.Add(name, t);
        return t;
    }
}
