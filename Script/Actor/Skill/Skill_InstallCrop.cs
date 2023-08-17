using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Skill_InstallCrop : BaseSkill
{
    public struct InstallData
    {
        public int CrobObjectIndex;
    }
    [JsonRequired]
    public InstallData _installData;
    protected override void OnProcessInformationText()
    {
        base.OnProcessInformationText();

        Data.ObjectData objectData = DataManager.Instance.ObjectTable[_installData.CrobObjectIndex];
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(LocalizingManager.Instance.GetLocalizing(8001, LocalizingManager.Instance.GetLocalizing(objectData.NameKey)));
        _informationText = stringBuilder.ToString();
    }
    public override bool IsUseable(IActor caster)
    {
        Vector2Int worldPosition =(Vector2Int)GridManager.Instance.GetTilePosition;
        if (WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[worldPosition].GetOverrideTileData != null)
        {
            if (WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[worldPosition].GetOverrideTileData.TileType != eTileType.Farm)
                return false;
        }
        else
        {
            if (WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[worldPosition].GetTileData.TileType != eTileType.Farm)
                return false;
        }

        if (caster.Controller.IsBlockType(eActionBlockType.Action))
            return false;

        if (!base.IsUseable(caster))
            return false;

        if (GridManager.Instance.IsPossibleInstallAtRuntime(caster.Position))
            return !(GridManager.Instance.IsOverlapObject(worldPosition, DataManager.Instance.ObjectTable[_installData.CrobObjectIndex].TileList));

        return false;
    }
    protected override IEnumerator IEStartSequenceProcess(IActor caster, Vector2 direction)
    {
        caster.Controller.LookAtTarget();
        caster.Controller.SetBlockState(eActionBlockType.Action | eActionBlockType.Move | eActionBlockType.Direction, _defaultData.DurationTime);
        Vector3Int worldPosition = GridManager.Instance.GetTilePosition;
        yield return CoroutineUtility.Wait(_defaultData.ReadyTime);
        NetworkManager.Instance.ObjectEventSender.RequestInstallObject(_installData.CrobObjectIndex, new Vector2Int(worldPosition.x, worldPosition.y), PlayerManager.Instance.Me.Guid);
        yield break;
    }
#if UNITY_EDITOR
    public override void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
        base.OnGUIEditor(maxWidth, ref height, ref errorList);

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Install Crop object index:");
        _installData.CrobObjectIndex = UnityEditor.EditorGUI.IntField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _installData.CrobObjectIndex);
        height += 20f;
    }
    public override void SetDefaultData()
    {
        base.SetDefaultData();

        _installData = new InstallData();
    }
#endif
}
