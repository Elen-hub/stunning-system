using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Tilemaps;

public class Skill_OverrideTile : BaseSkill
{
    #region Data definitions
    public struct CombatData
    {
        public eTileType TileType;
        public float Range;
    }
    #endregion
    [JsonRequired]
    protected CombatData _combatData;

    const string _plowingTileName = "Dirt1";
    public override bool IsUseable(IActor caster)
    {
        if (caster.Controller.IsBlockType(eActionBlockType.Action))
            return false;

        if (!base.IsUseable(caster))
            return false;

        Vector2Int worldPosition = (Vector2Int)GridManager.Instance.GetTilePosition;
        if (GridManager.Instance.IsOverlapObject(worldPosition))
            return false;

        if (_combatData.TileType != eTileType.Default)
        {
            if (!WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[worldPosition].GetTileData.OverrideTileData.ContainsKey(_combatData.TileType))
                return false;

            if (WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[worldPosition].GetTileData.OverrideTileData[_combatData.TileType] == WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[worldPosition].OverrideTileKey)
                return false;
        }
        else
        {
            if (WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[worldPosition].OverrideTileKey == 0)
                return false;
        }
        return true;
    }
    protected override IEnumerator IEStartSequenceProcess(IActor caster, Vector2 direction)
    {
        caster.Controller.LookAtTarget();
        caster.Controller.SetBlockState(eActionBlockType.Action | eActionBlockType.Move | eActionBlockType.Direction, _defaultData.DurationTime);
        yield return CoroutineUtility.Wait(_defaultData.ReadyTime);
        Vector2Int worldPosition = (Vector2Int)GridManager.Instance.GetTilePosition;
        NetworkManager.Instance.StageEventSender.RequestOverrideTile(worldPosition, _combatData.TileType);
    }
#if UNITY_EDITOR
    public override void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
        base.OnGUIEditor(maxWidth, ref height, ref errorList);

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "OverrieTileType:");
        _combatData.TileType = (eTileType)UnityEditor.EditorGUI.EnumPopup(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _combatData.TileType);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Range:");
        _combatData.Range = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _combatData.Range);
        height += 20f;
    }
    public override void SetDefaultData()
    {
        base.SetDefaultData();

        _combatData = new CombatData();
    }
#endif
}
