using System;
using System.Collections;
using System.Collections.Generic;
public enum eTileType
{
    Default,
    Farm,
}
public enum eTileCompressionType : byte
{
    None,
    RLE,
}
#region Components
public enum eComponent
{
    EquipComponent,
    InteractComponent,
    RelicComponent,
    SkillComponent,
    SkinComponent,
    BuildComponent,
    ItemComponent,
    EquipmentComponent,
}
public enum eComponentEvent
{
    SetMainCharacter,
    OverlapInteract,
    InstallMode,
    MouseLeftDown,
    MouseLeftUp,
    MouseRightDown,
    MouseRightUp,
    SelectQuickSlot,
    PickupItem,
    InteractInput,
    Interact,
    InteractEnd,
    SetObjectRender,
    HitRender,
    ActivateOutline,
    SetLayerOrder,
    Reload,
}
#endregion
public enum ePinType
{
    Player,
    Friend,
}
public enum eSkinEffectType
{
    Skin,
    Wall,
}
[System.Flags]
public enum eAllyType
{
    None = 0,
    Ally = 1 << 0,
    Enemy = 1 << 1,
    Nature = 1 << 2,
    All = ~0,
}
public enum eInputType
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Sprint,
    Shift,

    QuickSlot1,
    QuickSlot2,
    QuickSlot3,
    QuickSlot4,
    QuickSlot5,
    QuickSlot6,
    QuickSlot7,
    QuickSlot8,
    QuickSlotSwap,

    MouseLeft,
    MouseRight,

    Chatting,
    Option,

    PickupItem,
    Interact,
    Reload,

    Left,
    Right,

    OpenInventory,

    End,
}
[Flags]
public enum eInputState
{
    InstallMode,
}
#region UI
[System.Flags]
public enum eItemTooltipButtonType
{
    Equip = 1<<0,
    UnEquip = 1 << 1,
    Use = 1 << 2,
    QuickSlot = 1 << 3,
    Exit = 1 << 4,

    End,
}
#endregion

#region Environment
public enum eWeatherType
{
    None,
    Rain,
    Snow,
    Cloudy,
    Fog,
    End,
}
public enum eTimeTag
{
    Dawn,
    Twilight,
    Morning,
    Noon,
    Evening,
    Night,
    Midnight,
}
public enum eItemTooltipType
{
    ItemSlot,
    EquipSlot,
}
#endregion

#region Item
[System.Flags]
public enum eItemType
{
    None = 0,
    Weapon = 1 << 0,
    Medical = 1 << 1,
    Install = 1 <<2,
    Hat = 1 << 3,
    Armor = 1 << 4,
    Shoes = 1 << 5,
    Necklace = 1 << 6,
    Ring = 1 << 7,
    Material = 1 << 8,
    Food = 1 << 9,
    Scroll = 1 << 10,
    Magazine = 1 << 11,
    Crop = 1 << 12,

    EquipItem = Hat | Armor | Shoes | Necklace | Ring,
    UseItem = Weapon | Medical | Install | Food | Scroll | Magazine | Crop,
}
public enum eEquipSlotType
{
    Hat,
    Armor,
    Shoes,
    Necklace,
    Ring1,
    Ring2,
    End,
}
public enum eInventoryViewType
{
    All,
    Weapon,
    Armor,
    Potion,
    Magic,
    Etc,
}
public enum eUseItemFunction
{
    HPRecovery,
}
#endregion

#region Actor
public enum eCharacterState
{
    None,
    Move,
}
[System.Flags]
public enum eIngredientType : int
{
    None = 0,
    Flesh = 1 << 0,
    Wood = 1 << 1,
    Stone = 1 << 2,
    Metal = 1 << 3,
}
public enum eInteractState
{
    Standby,
    Interacting,
}
public enum eStatusType
{
    STR,
    DEX,
    INT,
    WIS,

    HP,
    HPRate,
    HPRecovery,
    SP,
    SPRecovery,
    Hunger,
    HungerRecovery,
    Temperature,
    Stress,
    StressRecovery,
    Sleepy,
    SleepyRecovery,

    MoveSpeed,
    WalkSpeed,
    RunSpeed,

    AttackSpeed,
    AttackDamage,
    AttackDamageRate,
    CoolTimeReduce,
}
public enum eSkillInputType
{
    Loopable,
    Single,
    Charge,
}
public enum eSpawnType
{
    Origin,     // Server instance
    Clone,      // Client instance
    Load,       // Load data
}
[System.Flags]
public enum eActionBlockType
{
    None = 0,
    Action = 1 << 0,
    Move = 1 << 1,
    Direction = 1 << 2,
    All = Action | Move | Direction,
}
public enum eActorType
{
    Character,
    Monster,
    StaticActor,
    StaticNPC,
    End,
}
public enum eStaticActorType
{
    Normal,
    Plant,
    CookObject,
    Refinement,
    Storage,
    Sleep,
}
public enum eObjectRenderState
{
    Idle,
    Interact,
    Action,
}
public enum eMoveType
{
    None,
    Walk,
    Run,
    Sprint,
}
public enum eStatusCalculateSign
{
    Add,
    Mul,
}
public enum eStatusState
{
    Highnest,
    High,
    Middle,
    Low,
    Lowest,
}
[System.Flags]
public enum eFSMState
{
    None = 0,
    Idle = 1 << 0,
    Patrol = 1 << 1,
    Chase = 1 << 2,
    Battle = 1 << 3,
    Death = 1 << 4,
    Comback = 1 << 5,
    Sleep = 1 << 6,

    End = 5,
}
public enum ePatternCondition
{
    HP,
    Distance,
    StateEvent,
    TargetType,
}
public enum eDamageType
{
    Default,
    Absolute,
}
#endregion
public enum eEmoticonType
{
    ExclamationMark,
    QuestionMark,
    SleepMark,
    End,
}
/// <summary>
/// DB 초기화용
/// </summary>
public static class EnumConverter
{
    public static Dictionary<string, eStatusType> StatusTypeConvert;
    public static Dictionary<string, eItemType> ItemTypeConvert;
    public static Dictionary<string, eUseItemFunction> UseItemFunctionConvert;
    public static Dictionary<string, eStaticActorType> StaticActorTypeConvert;
    public static Dictionary<string, eIngredientType> IngredientTypeConvert;
    public static Dictionary<string, eTileType> TileTypeConvert;
    public static Dictionary<string, eCustomBuffType> CustomBuffConvert;
    public static Dictionary<string, eAttachmentTarget> AttachmentTargetConvert;
    public static void Init()
    {
        GenerateEnumConverter(ref StatusTypeConvert);
        GenerateEnumConverter(ref ItemTypeConvert);
        GenerateEnumConverter(ref UseItemFunctionConvert);
        GenerateEnumConverter(ref StaticActorTypeConvert);
        GenerateEnumConverter(ref IngredientTypeConvert);
        GenerateEnumConverter(ref TileTypeConvert);
        GenerateEnumConverter(ref CustomBuffConvert);
        GenerateEnumConverter(ref AttachmentTargetConvert);
    }
    public static void Clear()
    {

    }
    static void GenerateEnumConverter<T>(ref Dictionary<string, T> enumContainer) where T : Enum
    {
        string[] keys = Enum.GetNames(typeof(T));
        Array values = Enum.GetValues(typeof(T));
        enumContainer = new Dictionary<string, T>(keys.Length);
        int count = 0;
        foreach (var value in values)
            enumContainer.Add(keys[count++], (T)value);
    }
}