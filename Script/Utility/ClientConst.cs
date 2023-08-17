using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ClientConst
{
    public static string WriteSession;
    public static readonly string SkillDataJsonPath = "Database/Json/SkillData/";
    public static readonly string DialogStructureJsonPath = "Database/Json/Dialog/";
#if UNITY_EDITOR
    public static readonly string CacheDirectoryPath = "Cache/";
    public static readonly string LogDirectoryPath = CacheDirectoryPath + "/Log/";
#else
    public static readonly string CacheDirectoryPath = UnityEngine.Application.persistentDataPath + "/Cache/";
    public static readonly string LogDirectoryPath = CacheDirectoryPath + "/Log/";
#endif
    public static int KEY_ANIMATION_ISALIVE = Animator.StringToHash("IsAlive");
    public static int KEY_ANIMATION_STATE = Animator.StringToHash("State");
    public static int KEY_ANIMATION_MOVETYPE = Animator.StringToHash("MoveType");
    public static int KEY_ANIMATION_HITSTATE = Animator.StringToHash("IsHit");
    public static int KEY_ANIMATION_HITTRIGGER = Animator.StringToHash("HitTrigger");
    public const float InteractRadius = 1f;
    public const int MaxLevel = 10;
    public const int MaxRelicLevel = 10;
    public static readonly int[] RequireExp = new int[] { 0, 100, 110, 120, 130, 140 };
    public const float InterestAreaRange = 20f;
    public const int DefaultBagCapacity = 24;
    public static readonly Dictionary<int, float> RelicCountProbability = new Dictionary<int, float>() {
        { 1, 0.7f },{ 2, 0.2f },{ 3, 0.1f },
    };
    public const int EquipSlotCount = 6;
    public static JsonSerializerSettings SerializingSetting = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.Objects,
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };
    #region Inventory
    public const int MaxPickupColliderCapacity = 20;
    public const float PickupItemOverlapRadius = 3f;
    #endregion
}
