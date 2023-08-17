using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour, ITileSetable
{
    public int Index;
    public int HouseNumber;
    HouseAreaChecker[] _roomArr;
    [SerializeField] protected List<Vector2Int> _tileBlockMask;
    public List<Vector2Int> GetTileBlockCoordList => _tileBlockMask;
    Dictionary<HouseAreaChecker.eHouseAreaType, Vector4[]> _houseAreaChecker;
    Dictionary<HouseAreaChecker.eHouseAreaType, int> _houseAreaCheckerCount;
    GameObject _outSide;

    SpriteRenderer[] _wallArr;
    public void Initialize()
    {
        OnInitializeExterior();
        OnInitializeChecker();
        GridManager.Instance.ModifyTileState(this);
        WorldManager.Instance.HouseChunkSystem.AddRoomChecker(this);
    }
    void OnInitializeExterior()
    {
#if !UNITY_SERVER
        Transform layout = transform.Find("Exterior");
       _wallArr = layout.Find("Wall").GetComponentsInChildren<SpriteRenderer>(true);
        int count = _wallArr.Length;
        for (int i = 0; i < count; ++i)
        {
            _wallArr[i].sortingOrder = ClientMath.GetLayerOrder(_wallArr[i].transform.position);
            _wallArr[i].enabled = false;
        }
        Transform outside = layout.Find("Outside");
        if(outside != null)
        {
            _outSide = outside.gameObject;
            SpriteRenderer[] rendererArr = outside.GetComponentsInChildren<SpriteRenderer>(true);
            count = rendererArr.Length;
            for (int i = 0; i < count; ++i)
                rendererArr[i].sortingOrder = ClientMath.GetLayerOrder(rendererArr[i].transform.position) + rendererArr[i].sortingOrder;
        }
#endif
    }
    void OnInitializeChecker()
    {
        Transform layout = transform.Find("Checker");
        _roomArr = layout.GetComponentsInChildren<HouseAreaChecker>(true);
        if (_roomArr != null)
        {
            _houseAreaChecker = new Dictionary<HouseAreaChecker.eHouseAreaType, Vector4[]>();
            _houseAreaCheckerCount = new Dictionary<HouseAreaChecker.eHouseAreaType, int>();
        }
        for (int i = 0; i < _roomArr.Length; ++i)
        {
            _roomArr[i].Initialize(this);
            if (!_houseAreaChecker.ContainsKey(_roomArr[i].AreaType))
            {
                _houseAreaChecker.Add(_roomArr[i].AreaType, new Vector4[6]);
                _houseAreaCheckerCount.Add(_roomArr[i].AreaType, 0);
            }
            _houseAreaCheckerCount[_roomArr[i].AreaType] = _houseAreaCheckerCount[_roomArr[i].AreaType] + 1;
            _houseAreaChecker[_roomArr[i].AreaType][i] = new Vector4(_roomArr[i].AreaBounds.center.x, _roomArr[i].AreaBounds.center.y,
                _roomArr[i].AreaBounds.extents.x, _roomArr[i].AreaBounds.extents.y);
        }
    }
    public void ProcessHouseChecker(DynamicActor dynamicActor)
    {
        if (_roomArr != null)
        {
            HouseAreaChecker inArea = null;
            HouseAreaChecker outArea = null;
            for (int i = 0; i < _roomArr.Length; ++i)
            {
                if(_roomArr[i].AreaBounds.Contains(dynamicActor.Position))
                {
                    if (inArea != null)
                        continue;

                    if (_roomArr[i].IsEnterState(dynamicActor))
                        inArea = _roomArr[i];
                }
                else
                {
                    if (outArea != null)
                        continue;

                    if (_roomArr[i].IsExitState(dynamicActor))
                        outArea = _roomArr[i];
                }
            }
#if !UNITY_SERVER
            if(outArea != null && inArea != null)
                if (outArea.AreaType == inArea.AreaType)
                    return;

            if (outArea != null)
            {
                switch (outArea.AreaType)
                {
                    case HouseAreaChecker.eHouseAreaType.In:
                        Shader.DisableKeyword("PARTICLE_SHADER_BLOCK");
                        _outSide?.SetActive(true);
                        int length = _wallArr.Length;
                        for (int i = 0; i < length; ++i)
                            _wallArr[i].enabled = false;
                        break;
                    case HouseAreaChecker.eHouseAreaType.Back:
                        Shader.SetGlobalInteger("_targetCount", _houseAreaChecker[HouseAreaChecker.eHouseAreaType.Back].Length);
                        Shader.DisableKeyword("BACKFACE_FADE");
                        break;
                }
            }
            if (inArea != null)
            {
                switch (inArea.AreaType)
                {
                    case HouseAreaChecker.eHouseAreaType.In:
                        Shader.EnableKeyword("PARTICLE_SHADER_BLOCK");
                        Shader.SetGlobalInteger("_targetCount", _houseAreaChecker[HouseAreaChecker.eHouseAreaType.In].Length);
                        Shader.SetGlobalVectorArray("_blockBounds", _houseAreaChecker[HouseAreaChecker.eHouseAreaType.In]);
                        _outSide?.SetActive(false);
                        int length = _wallArr.Length;
                        for (int i = 0; i < length; ++i)
                            _wallArr[i].enabled = true;
                        break;
                    case HouseAreaChecker.eHouseAreaType.Back:
                        Shader.EnableKeyword("BACKFACE_FADE");
                        Shader.SetGlobalInteger("_targetCount", _houseAreaChecker[HouseAreaChecker.eHouseAreaType.Back].Length);
                        Shader.SetGlobalVectorArray("_fadeBounds", _houseAreaChecker[HouseAreaChecker.eHouseAreaType.Back]);
                        break;
                }
            }
#endif
        }
    }
#if UNITY_EDITOR
    public enum eEdtiorState
    {
        None,
        TileSet,
    }
    [HideInInspector] public eEdtiorState EditorState;
    [HideInInspector] bool _isEditMode;
    public bool IsDebug = true;
    public bool IsEditMode { get => _isEditMode; set => _isEditMode = value; }
    bool _isTileSetMode;
    public bool IsTileSetMode { get => _isTileSetMode; set => _isTileSetMode = value; }
    [HideInInspector] public Vector2Int TilePosition;
    void OnDrawTileBlockMask()
    {
        if (_tileBlockMask != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            for (int i = 0; i < _tileBlockMask.Count; ++i)
            {
                Gizmos.DrawCube(transform.position + (Vector3Int)_tileBlockMask[i] + Vector3.one * 0.5f, Vector3.one);
            }
        }
    }
    public void OnClickTileBlock(Vector2Int tilePos)
    {
        if (_tileBlockMask == null)
            _tileBlockMask = new List<Vector2Int>();

        for (int i = 0; i < _tileBlockMask.Count; ++i)
        {
            if (_tileBlockMask[i].Equals(tilePos))
            {
                _tileBlockMask.RemoveAt(i);
                if (_tileBlockMask.Count == 0)
                    _tileBlockMask = null;

                return;
            }
        }
        _tileBlockMask.Add(tilePos);
    }
    [ExecuteInEditMode]
    public void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        if (IsTileSetMode)
        {
            OnDrawTileBlockMask();
            if (_tileBlockMask != null)
            {
                for (int i = 0; i < _tileBlockMask.Count; ++i)
                {
                    if (_tileBlockMask[i].Equals(TilePosition))
                    {
                        Gizmos.color = new Color(1f, 1f, 0f, 1f);
                        goto DrawCube;
                    }
                }
            }

            Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
            DrawCube:
            Gizmos.DrawCube((Vector3Int)TilePosition + new Vector3(0.5f, 0.5f), Vector3.one);
            Gizmos.color = Color.white;
            for (int i = -30; i <= 30; ++i)
                Gizmos.DrawLine(transform.position + new Vector3(i, -30), transform.position + new Vector3(i, 30, 0.5f));
            for (int i = -30; i <= 30; ++i)
                Gizmos.DrawLine(transform.position + new Vector3(-30, i), transform.position + new Vector3(30, i, 0.5f));
        }
    }
#endif
}
