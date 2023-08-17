using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : TSingletonMono<GridManager>
{
    ObjectMemoryPool<SpriteRenderer> _tileEffectMemory;
    List<SpriteRenderer> _currentSpawnedTileEffectList = new List<SpriteRenderer>(8);
    bool _isSpawnedTileEffect;
    bool _isProcessTilePosition = false;
    Vector3Int _processTilePosition;
    public DelegateUtility.IsOverlapCondition OverlapCondition;
    [SerializeField] HashSet<Vector2Int> _blockingTileHashSet;
    IInstallable _lastTileTarget;
    IActor _installCaster;
    public Vector3Int GetTilePosition {
        get
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Event e = Event.current;
                Vector3 pos = UnityEditor.SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(e.mousePosition);
                return new Vector3Int()
                {
                    x = Mathf.FloorToInt(pos.x),
                    y = Mathf.FloorToInt(UnityEditor.SceneView.lastActiveSceneView.camera.transform.position.y * 2f - pos.y),
                };
            }
#endif
            if (_isProcessTilePosition)
                return _processTilePosition;

            _isProcessTilePosition = true;
            Vector3 position = CameraManager.Instance.GetCamera(eCameraType.MainCamera).Camera.ScreenToWorldPoint(Input.mousePosition);
            _processTilePosition = new Vector3Int
            {
                x = Mathf.FloorToInt(position.x),
                y = Mathf.FloorToInt(position.y),
            };
            return _processTilePosition;
        }
    }
    public bool IsPossibleInstallAtRuntime(Vector2 pivot)
    {
        Vector2Int tilePosition = (Vector2Int)GetTilePosition;
        foreach (var contents in _lastTileTarget.GetTileBlockCoordList)
        {
            Vector2 tileWorldPosition = tilePosition + contents + Vector2.one * 0.5f;
            if ((tileWorldPosition - pivot).sqrMagnitude <= 4f)
            {
                if(OverlapCondition != null)
                    return OverlapCondition(tilePosition);

                return true;
            }
        }
        return false;
    }
    public bool IsOverlapObject(Vector2Int pos) => _blockingTileHashSet.Contains(pos);
    public bool IsOverlapObject(StaticActor staticActor)
    {
        Vector2Int intPos = new Vector2Int(ClientMath.RoundToInt(staticActor.transform.position.x), ClientMath.RoundToInt(staticActor.transform.position.y));
        if (staticActor.GetTileBlockCoordList != null)
        {
            for (int i = 0; i < staticActor.GetTileBlockCoordList.Count; ++i)
            {
                Vector2Int tilePos = intPos + staticActor.GetTileBlockCoordList[i];
                if (_blockingTileHashSet.Contains(tilePos))
                    return true;
            }
        }
        return false;
    }
    public bool IsOverlapObject(Vector2Int pivot, List<Vector2Int> tileList)
    {
        if (tileList != null)
        {
            int count = tileList.Count;
            for (int i = 0; i < count; ++i)
            {
                Vector2Int tilePos = pivot + tileList[i];
                if (_blockingTileHashSet.Contains(tilePos))
                    return true;
            }
        }
        return false;
    }
    protected override void OnInitialize()
    {
        _blockingTileHashSet = new HashSet<Vector2Int>();
        _tileEffectMemory = new ObjectMemoryPool<SpriteRenderer>(16);
    }
    public void ModifyTileState(ITileSetable tileSet)
    {
        Vector2Int pivot = new Vector2Int(ClientMath.RoundToInt(tileSet.transform.position.x), ClientMath.RoundToInt(tileSet.transform.position.y));
        if (tileSet.GetTileBlockCoordList != null)
            for (int j = 0; j < tileSet.GetTileBlockCoordList.Count; ++j)
                _blockingTileHashSet.Add(pivot + tileSet.GetTileBlockCoordList[j]);
    }
    public void RemoveTileState(ITileSetable tileSet)
    {
        Vector2Int pivot = new Vector2Int(ClientMath.RoundToInt(tileSet.transform.position.x), ClientMath.RoundToInt(tileSet.transform.position.y));
        if (tileSet.GetTileBlockCoordList != null)
            for (int j = 0; j < tileSet.GetTileBlockCoordList.Count; ++j)
            {
                Vector2Int pos = pivot + tileSet.GetTileBlockCoordList[j];
                if (_blockingTileHashSet.Contains(pos))
                    _blockingTileHashSet.Remove(pos);
            }
    }
    SpriteRenderer SpawnTile()
    {
        SpriteRenderer instance = Instantiate(Resources.Load<SpriteRenderer>("Tiles/TileEffect"));
        return instance;
    }
    public void CreateGridEffect(IActor installCaster, IInstallable staticActor)
    {
        _installCaster = installCaster;
        _isSpawnedTileEffect = true;
        int i = 0;
        foreach (var tile in staticActor.GetTileBlockCoordList)
        {
            if (_currentSpawnedTileEffectList.Count > i)
            {
                _currentSpawnedTileEffectList[i].transform.SetParent(staticActor.transform);
                _currentSpawnedTileEffectList[i].transform.localPosition = new Vector3(tile.x, tile.y, 0);
                _currentSpawnedTileEffectList[i].enabled = true;
            }
            else
            {
                SpriteRenderer renderer = _tileEffectMemory.GetItem();
                if (renderer == null)
                    renderer = SpawnTile();

                renderer.transform.SetParent(staticActor.transform);
                renderer.transform.localPosition = new Vector3(tile.x, tile.y, 0);
                renderer.enabled = true;
                _currentSpawnedTileEffectList.Add(renderer);
            }
            ++i;
        }
        for(int j = _currentSpawnedTileEffectList.Count; j > i; --j)
        {
            _currentSpawnedTileEffectList[j].transform.SetParent(transform);
            _currentSpawnedTileEffectList[j].enabled = false;
            _tileEffectMemory.Register(_currentSpawnedTileEffectList[j]);
            _currentSpawnedTileEffectList.RemoveAt(j);
        }
        _lastTileTarget = staticActor;
    }
    public void DeleteGridEffect()
    {
        if (!_isSpawnedTileEffect) 
            return;
        
        for(int i = 0; i< _currentSpawnedTileEffectList.Count; ++i)
        {
            _currentSpawnedTileEffectList[i].transform.SetParent(transform);
            _currentSpawnedTileEffectList[i].enabled = false;
            _tileEffectMemory.Register(_currentSpawnedTileEffectList[i]);
        }
        _lastTileTarget = null;
        _currentSpawnedTileEffectList.Clear();
    }
    void OnUpdateTileEffect()
    {
        if (_lastTileTarget == null) return;
        int currentCount = 0;
        Vector2Int tilePosition = (Vector2Int)GetTilePosition;
        foreach(var tile in _lastTileTarget.GetTileBlockCoordList)
        {
            if (IsPossibleInstallAtRuntime(_installCaster.Position))
                _currentSpawnedTileEffectList[currentCount].color = IsOverlapObject(tile + tilePosition) ? Color.red : Color.green;
            else
                _currentSpawnedTileEffectList[currentCount].color = Color.red;

            ++currentCount;
        }
    }
    private void Update()
    {
        _isProcessTilePosition = false;
        OnUpdateTileEffect();
    }
}
