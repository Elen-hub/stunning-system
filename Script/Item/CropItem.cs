using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropItem : UseItem
{
    public Data.ObjectData ObjectData => DataManager.Instance.ObjectTable[_installKey];
    Skill_InstallCrop _installSkill;
    public Skill_InstallCrop GetInstallSkill
    {
        get {
            if (_installSkill == null)
                _installSkill = GetSkill as Skill_InstallCrop;

            return _installSkill;
        }
    }
    int _installKey;
    public override string GetInformationText()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(base.GetInformationText());

        return stringBuilder.ToString();
    }
    public override void OnEnterSlot(IComponent actor)
    {
        base.OnEnterSlot(actor);

        if (_installKey == 0)
        {
            Skill_InstallCrop installSkill = DataManager.Instance.SkillTable[ItemData.ReferenceIndex].Skill as Skill_InstallCrop;
            _installKey = installSkill._installData.CrobObjectIndex;
        }
        GridManager.Instance.OverlapCondition += IsPossibleInstall;
        actor.SendComponentMessage(eComponentEvent.InstallMode, true, ObjectData.Index);
    }
    public override void OnExitSlot(IComponent actor)
    {
        base.OnExitSlot(actor);

        GridManager.Instance.OverlapCondition -= IsPossibleInstall;
        actor.SendComponentMessage(eComponentEvent.InstallMode, false, 0);
    }
    bool IsPossibleInstall(Vector2Int tilePosition)
    {
        if (!WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary.ContainsKey(tilePosition))
            return false;

        if(WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[tilePosition].GetOverrideTileData != null)
        {
            if (WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[tilePosition].GetOverrideTileData.TileType != eTileType.Farm)
                return false;
        }
        else
        {
            if (WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary[tilePosition].GetTileData.TileType != eTileType.Farm)
                return false;
        }
        return true;
    }
    public override bool IsUseable(IActor actor)
    {
        if (!base.IsUseable(actor))
            return false;

        return GetSkill.IsUseable(actor);
    }
    public override void OnUseStart(IActor actor)
    {
        GetSkill.StartSequence(actor, actor.Direction);
        --Count;
    }
}
