using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeTileObject : StaticActor
{
    public int OverrideTile;
    Vector2Int _tilePosition;
    protected override void OnStartInstallMode()
    {
        base.OnStartInstallMode();

        SendComponentMessage(eComponentEvent.ActivateRender, true);
    }
    protected override void OnSpawn()
    {
        base.OnSpawn();

        SendComponentMessage(eComponentEvent.ActivateRender, false);
        _tilePosition = new Vector2Int()
        {
            x = Mathf.RoundToInt(transform.position.x),
            y = Mathf.RoundToInt(transform.position.y),
        };
        if(OverrideTile != 0)
        {
            Debug.Log(_tilePosition);
            WorldManager.Instance.TileChunkSystem.ModifyTileCollider(_tilePosition, false);
            WorldManager.Instance.TileChunkSystem.SetOverrideTile(_tilePosition, OverrideTile);
        }
    }
    protected override void OnDeath()
    {
        base.OnDeath();

        WorldManager.Instance.TileChunkSystem.ModifyTileCollider(_tilePosition, true);
        WorldManager.Instance.TileChunkSystem.SetDefaultTile(_tilePosition);
    }
}
