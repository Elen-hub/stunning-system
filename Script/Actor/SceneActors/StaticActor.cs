using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Pathfinding;

public class StaticActor : MonoBehaviour, IActor, IInstallable, IPacket
{
    #region Event Callback
    public event System.Action OnDestroyCallback;
    #endregion
    public Fusion.NetworkRunner MyRunner { get; }
    [SerializeField] protected int _index;
    public int Index { get => _index; set => _index = value; }
    protected Data.ObjectData GetData => DataManager.Instance.ObjectTable[_index];
    ulong _worldID;
    public ulong WorldID => _worldID;
    public ulong SetWorldID { set => _worldID = value; }
    public Vector2 Direction { get; }
    public Vector2 Position {
        get {
            if(_attachment != null)
                return _attachment.GetAttachmentElement(eAttachmentTarget.Pivot).Position;

            return transform.position;
        }
    }
    protected ActorStat _actorStat;
    public ActorStat ActorStat => _actorStat;
    protected ActorBuff _actorBuff;
    public ActorBuff ActorBuff => _actorBuff;
    public ActorController Controller { get; }
    Attachment _attachment;
    public Attachment Attachment => _attachment;
    public eActorType ActorType => eActorType.StaticActor;
    [SerializeField] int _rewardSection;
    InOutState _inOutState;
    public InOutState InOutState => _inOutState;
    Collider2D _hitBoxCollider;
    public float CurrentHP { get => _currentHP; set => _currentHP = value; }
    [SerializeField] protected float _currentHP;
    public bool IsAlive => !_isInstallMode && CurrentHP > 0f;
    bool _isInstallMode;
    public bool IsExist { get; }
    public int AllyNumber { get; set; }
    public eAllyType GetAllyType(int toAllyNumber)
    {
        if (toAllyNumber < 0)
            return eAllyType.Nature;

        return AllyNumber == toAllyNumber ? eAllyType.Ally : eAllyType.Enemy;
    }
    public string OwnerGUID;
    protected DynamicGridObstacle _obstacle;
    Collider2D _obstacleCollider;

#region StaticActor BakeData
    [SerializeField] protected List<Vector2Int> _tileBlockMask;
    public List<Vector2Int> GetTileBlockCoordList => _tileBlockMask;
#endregion
#region Initialize Methods
    public virtual void Initialize(int index)
    {
        Index = index;
        _obstacle = GetComponent<Pathfinding.DynamicGridObstacle>();
#if !UNITY_SERVER
        new ObjectSkinComponent(this);
#endif
        _actorBuff = new ActorBuff(this);
        _inOutState = new InOutState();
        _hitBoxCollider = transform.Find("Collider_Hitbox")?.GetComponent<Collider2D>();
        _obstacleCollider = GetComponent<Collider2D>();
        _attachment = GetComponent<Attachment>();
        if(_attachment != null)
            _attachment.Initialize();
        SetColliderAcitveState(false);
    }
    protected virtual void OnSpawn()
    {
        ActorManager.Instance.RegistWorld(this);
        GridManager.Instance.ModifyTileState(this);

        SetColliderAcitveState(true);
#if !UNITY_SERVER
        SendComponentMessage(eComponentEvent.SetLayerOrder, Position);
#endif
    }
    public virtual void OnSpawnClient(BaseEventReceiver eventReceiver)
    {
        _worldID = eventReceiver.GetUInt64();
        transform.position = eventReceiver.GetVector3();
        OwnerGUID = eventReceiver.GetString();
        CurrentHP = eventReceiver.GetFloat();

        if(GetData.RewardIndex != 0 && DataManager.Instance.RewardTable[GetData.RewardIndex].DropPeriodHP != 0)
        {
            float periodHP = 1f / DataManager.Instance.RewardTable[GetData.RewardIndex].DropPeriodHP;
            _rewardSection = Mathf.CeilToInt((1f - CurrentHP / GetData.HP) * periodHP);
        }
        OnSpawn();
    }
    public virtual void OnSpawnLoad(ulong worldID, SaveController saveController)
    {
        _worldID = worldID;
        transform.position = saveController.GetVector3();
        OwnerGUID = saveController.GetString();
        CurrentHP = saveController.GetFloat();
        if (GetData.RewardIndex != 0 && DataManager.Instance.RewardTable[GetData.RewardIndex].DropPeriodHP != 0)
        {
            float periodHP = 1f / DataManager.Instance.RewardTable[GetData.RewardIndex].DropPeriodHP;
            _rewardSection = Mathf.CeilToInt((1f - CurrentHP / GetData.HP) * periodHP);
        }
        OnSpawn();
    }
    public virtual void OnSpawnServer(ulong worldID)
    {
        _worldID = worldID;
        _currentHP = GetData.HP;
        OnSpawn();
    }
    public void InstallMode(bool isInstallMode, Vector3 position)
    {
        _isInstallMode = isInstallMode;
        if (isInstallMode)
            OnStartInstallMode();
        else
            OnEndInstallMode();
    }
    protected virtual void OnStartInstallMode()
    {
        SetColliderAcitveState(false);
    }
    protected virtual void OnEndInstallMode()
    {
        gameObject.SetActive(false);
        ActorManager.Instance.RegistObjectMemoryPool(this);
    }
#endregion
    protected virtual void Update()
    {
        OnUpdateComponent();
        if (IsAlive)
            _actorBuff?.Update(TimeManager.DeltaTime);
    }
#region Component Pattern
    Dictionary<eComponentEvent, List<BaseComponent>> _componentListDictionary = new Dictionary<eComponentEvent, List<BaseComponent>>();

    Dictionary<eComponent, BaseComponent> _componentDic = new Dictionary<eComponent, BaseComponent>();
    public void AddComponentEvent(eComponentEvent componentEvent, BaseComponent component)
    {
        if (!_componentListDictionary.ContainsKey(componentEvent))
            _componentListDictionary.Add(componentEvent, new List<BaseComponent>());

        _componentListDictionary[componentEvent].Add(component);
    }
    public void SendComponentMessage(eComponentEvent eventType, params object[] messageArr)
    {
        if (_componentListDictionary.ContainsKey(eventType))
        {
            foreach (var component in _componentListDictionary[eventType])
                component.ReceiveComponentMessage(eventType, messageArr);
        }
    }
    public bool IsContainsComponent(eComponent componentType) => _componentDic.ContainsKey(componentType);
    public BaseComponent GetComponent(eComponent componentType) => _componentDic.ContainsKey(componentType) ? _componentDic[componentType] : null;
    public T GetComponent<T>(eComponent componentType) where T : BaseComponent => _componentDic.ContainsKey(componentType) ? _componentDic[componentType] as T : null;
    public void AddComponent(BaseComponent component)
    {
        if (_componentDic.ContainsKey(component.ComponentType))
        {
            _componentDic[component.ComponentType].Cleanup();
            _componentDic[component.ComponentType].OnTerminate();
            _componentDic[component.ComponentType] = component;
        }
        else
            _componentDic.Add(component.ComponentType, component);
    }
    void OnUpdateComponent()
    {
        foreach (var component in _componentDic)
            component.Value.NextFrame(TimeManager.DeltaTime);
    }
    #endregion
    protected void SetColliderAcitveState(bool isActive)
    {
        if (_obstacle) _obstacle.enabled = isActive;
        if (_obstacleCollider) _obstacleCollider.enabled = isActive;
        if (_hitBoxCollider) _hitBoxCollider.enabled = isActive;
    }
    public void Hit(AttackProperty attackProperty)
    {
        CurrentHP -= attackProperty.Damage;
        if (CurrentHP <= 0f)
        {
            CurrentHP = 0f;
            OnDeath();
        }
        OnHitDropProcess();
        NetworkManager.Instance.ObjectEventSender.NotifyObjectHit(_worldID, attackProperty.DamageType, CurrentHP);
    }
    void OnHitDropProcess()
    {
        if (DataManager.Instance.RewardTable[GetData.RewardIndex] == null)
            return;

        if (DataManager.Instance.RewardTable[GetData.RewardIndex].DropPeriodHP != 0)
        {
            float periodHP = 1f / DataManager.Instance.RewardTable[GetData.RewardIndex].DropPeriodHP;
            int rewardSection = Mathf.CeilToInt((1f - CurrentHP / GetData.HP) * periodHP);
            if (_rewardSection != rewardSection)
            {
                int rewardCount = rewardSection - _rewardSection;
                for (int i = 0; i < rewardCount; ++i)
                    foreach(var dropTable in DataManager.Instance.RewardTable[GetData.RewardIndex].DropPeriodStructures)
                        if (ClientMath.ProcessRandom(dropTable.Probability))
                        {
                            ItemObject itemObject = ItemManager.Instance.SpawnItem(Item.NewItem(dropTable.Index, dropTable.Count), Position);
                            NetworkManager.Instance.ItemEventSender.NotifySpawnItem(itemObject);
                        }

                _rewardSection = rewardSection;
            }
        }
    }
    public void Hit(eDamageType type, float hp)
    {
        CurrentHP = hp;
        if(hp > 0)
            SendComponentMessage(eComponentEvent.HitRender);
        else
            Destroy();
    }
    protected virtual void OnDeath()
    {
        SetColliderAcitveState(false);
        foreach (var dropTable in DataManager.Instance.RewardTable[GetData.RewardIndex].DropStructures)
            if (ClientMath.ProcessRandom(dropTable.Probability))
            {
                ItemObject itemObject = ItemManager.Instance.SpawnItem(Item.NewItem(dropTable.Index, dropTable.Count), Position);
                NetworkManager.Instance.ItemEventSender.NotifySpawnItem(itemObject);
            }

        Destroy();
    }
    public void Destroy()
    {
        OnDestroyCallback?.Invoke();
        gameObject.SetActive(false);
        ActorManager.Instance.RemoveWorld(_worldID);
        ActorManager.Instance.RegistObjectMemoryPool(this);
        GridManager.Instance.RemoveTileState(this);
    }
#region Network
    public virtual int GetByteSize => ReliableHelper.IntSize + ReliableHelper.ULongSize + ReliableHelper.Vector3Size + ReliableHelper.StringSize(OwnerGUID.Length) + ReliableHelper.FloatSize;
    public virtual void EnqueueByte()
    {
        BaseEventSender.CopyBytes(_index);
        BaseEventSender.CopyBytes(_worldID);
        BaseEventSender.CopyBytes(transform.position);
        BaseEventSender.CopyBytes(OwnerGUID);
        BaseEventSender.CopyBytes(CurrentHP);
    }
#endregion
#region Save&Load
    protected SaveController _saveController;
    protected virtual void ProcessSaveByte()
    {
        _saveController.CopyBytes(_index);
        _saveController.CopyBytes(_worldID);
        _saveController.CopyBytes(transform.position);
        _saveController.CopyBytes(OwnerGUID);
        _saveController.CopyBytes(CurrentHP);
    }
    public byte[] GetSaveByte()
    {
        if (_saveController == null) _saveController = new SaveController(GetByteSize);
        else _saveController.SetCapacity(GetByteSize);
        ProcessSaveByte();
        return _saveController.GetByteArray;
    }
#endregion
#if UNITY_EDITOR
    public enum eEdtiorState
    {
        None,
        TileSet,
    }
    [HideInInspector] public eEdtiorState EditorState;
    [HideInInspector] bool _isEditMode;
    public bool IsEditMode { get => _isEditMode; set => _isEditMode = value; }
    [HideInInspector] bool _isTileSetMode;
    public bool IsTileSetMode { get => _isTileSetMode; set => _isTileSetMode = value; }
    [HideInInspector] public Vector2Int TilePosition;
    void OnDrawTileBlockMask()
    {
        if(_tileBlockMask != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            for (int i = 0; i< _tileBlockMask.Count; ++i)
            {
                Gizmos.DrawCube((Vector3Int)_tileBlockMask[i] + Vector3.one * 0.5f + transform.position, Vector3.one);
            }
        }
    }
    public void OnClickTileBlock(Vector2Int tilePos)
    {
        if (_tileBlockMask == null)
            _tileBlockMask = new List<Vector2Int>();

        for(int i = 0; i<_tileBlockMask.Count; ++i)
        {
            if(_tileBlockMask[i].Equals(tilePos))
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
            for (int i = -10; i <= 10; ++i)
                Gizmos.DrawLine(transform.position + new Vector3(i, -10), transform.position + new Vector3(i, 10, 0.5f));
            for (int i = -10; i <= 10; ++i)
                Gizmos.DrawLine(transform.position + new Vector3(-10, i), transform.position + new Vector3(10, i, 0.5f));
        }
    }
#endif
}
