using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class DynamicActor : NetworkBehaviour, IActor
{
    #region Event Callback
    public event System.Action OnDestroyCallback;
    #endregion
    public virtual string Name { get; set; }
    public NetworkRunner MyRunner => Runner;
    [Networked(OnChanged = nameof(Initialize), OnChangedTargets = OnChangedTargets.All)]
    public int Index { get; set; }
    [Networked(OnChanged = nameof(Spawn), OnChangedTargets = OnChangedTargets.All)]
    public ulong WorldID { get; set; }
    [Networked] public Vector2 Direction { get; set; }
    public Vector2 Position => transform.position;
    public virtual eActorType ActorType { get; }
    public bool IsAlive => CurrentHP > 0;
    bool IActor.IsExist { get; }
    [SerializeField] InOutState _inOutState;
    public InOutState InOutState => _inOutState;
    Collider2D _hitBoxCollider;
    protected ActorController _controller;
    public ActorController Controller { get => _controller; set => _controller = value; }
    Attachment _attachment;
    public Attachment Attachment => _attachment;
    protected ActorStat _actorStat;
    public ActorStat ActorStat => _actorStat;
    protected ActorBuff _actorBuff;
    public ActorBuff ActorBuff => _actorBuff;
    NetworkMecanimAnimator _animator;
    public NetworkMecanimAnimator Animator => IsProxy ? null : _animator;
    [Networked] public int AllyNumber { get; set; }
    public eAllyType GetAllyType(int toAllyNumber)
    {
        if (toAllyNumber < 0)
            return eAllyType.Nature;

        return AllyNumber == toAllyNumber ? eAllyType.Ally : eAllyType.Enemy;
    }
    [Networked] public float CurrentHP { get; set; }
    float _elapsedRecoveryTime;
    public Vector2 SpawnPosition;
    protected bool _isHit;
    public bool IsHit => _isHit;
    protected float _hitTime;
    protected bool _isSleep;
    public bool IsSleep => _isSleep;
    float _weatherElapsedTime = 0f;
    const float _weatherTargetTime = 0.1f;
    #region Initialize Methods
    static void Initialize(Changed<DynamicActor> changed)
    {
        changed.Behaviour.OnInitialize();
    }
    protected virtual void OnInitialize()
    {
        //_transform = transform;
        //StatComponent statComponent = new StatComponent(this);
        //CurrentHP = statComponent[eStatusType.HP];
        _animator =  transform.Find("Render").GetComponent<NetworkMecanimAnimator>();
        _hitBoxCollider = transform.Find("Collider_Hitbox").GetComponent<Collider2D>();
#if !UNITY_SERVER
        new CharacterSkinComponent(this);
        _attachment = GetComponent<Attachment>();
        _attachment.Initialize();
#endif
        _inOutState = new InOutState();
    }
    static void Spawn(Changed<DynamicActor> changed)
    {
        changed.Behaviour.OnSpawn();
    }
    protected virtual void OnSpawn()
    {
        //#if !UNITY_SERVER
        //        UIManager.Instance.FieldUI.SetNameTag(this);
        //        UIManager.Instance.FieldUI.SetHPBar(this);
        //#endif
        ActorManager.Instance.RegistWorld(this);
        if (Object.HasInputAuthority)
        {
            new InteractComponent(this);
            new BuildComponent(this);
            UIManager.Instance.Open<UQuickSlot>(eUIName.UQuickSlot).SetItemComponent = new ItemComponent(this);
            NetworkController controller = new NetworkController(this);
            _controller = controller;
            InputManager.Instance.SetInputHandler = new CharacterInputHandler(controller);
            CameraManager.Instance.SetActor = transform;
            SendComponentMessage(eComponentEvent.SetMainCharacter);
            WorldManager.Instance.SetMainCharacter(this);
        }
        if (Object.InputAuthority != PlayerRef.None)
            PlayerManager.Instance.GetPlayer(Object.InputAuthority).SetMainCharacter(this);

        SpawnPosition = Position;
        Animator?.Animator.SetBool(ClientConst.KEY_ANIMATION_ISALIVE, true);
        _hitBoxCollider.enabled = true;
    }
    public virtual void DefaultAttack()
    {

    }
    protected virtual void OnUpdateRecoveryTime()
    {
        if (Object.HasStateAuthority)
            CurrentHP = Mathf.Clamp(CurrentHP + _actorStat[eStatusType.HPRecovery], 0f, _actorStat[eStatusType.HP]);
    }
    void OnUpdateWeather()
    {
        const int rainKey = 1;
        const int snowKey = 2;

        if (InOutState.State == HouseAreaChecker.eHouseAreaType.In)
            return;

        _weatherElapsedTime += TimeManager.DeltaTime;
        if (_weatherElapsedTime > _weatherTargetTime)
        {
            _weatherElapsedTime = 0f;
            switch (EnvironmentManager.Instance.WeatherController.CurrentWeahterHandler.WeahterType)
            {
                case eWeatherType.Rain:
                    _actorBuff.ApplyBuff(rainKey);
                    break;
                case eWeatherType.Snow:
                    _actorBuff.ApplyBuff(snowKey);
                    break;
            }
        }
    }
    protected virtual void Update()
    {
        OnUpdateComponent();
        if (IsAlive)
        {
            _elapsedRecoveryTime += TimeManager.DeltaTime;
            if (_elapsedRecoveryTime > 1f)
            {
                OnUpdateRecoveryTime();
                _elapsedRecoveryTime = 0f;
            }
            _actorBuff.Update(TimeManager.DeltaTime);

            OnUpdateWeather();
        }
        if (_isHit)
        {
            _hitTime -= TimeManager.DeltaTime;
            if (_hitTime <= 0f)
            {
                _isHit = false;
                Animator?.Animator.SetBool(ClientConst.KEY_ANIMATION_HITSTATE, false);
            }
        }
        if (_controller != null)
        {
            if (_isSleep) 
                _controller.SetBlockState(eActionBlockType.All, TimeManager.DeltaTime);

            _controller.Update(TimeManager.DeltaTime);
        }
    }
#endregion
#region Component
    Dictionary<eComponentEvent, List<BaseComponent>> _componentListDictionary = new Dictionary<eComponentEvent, List<BaseComponent>>();
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
            foreach(var component in _componentListDictionary[eventType])
                component.ReceiveComponentMessage(eventType, messageArr);
        }
    }
    Dictionary<eComponent, BaseComponent> _componentDic = new Dictionary<eComponent, BaseComponent>();
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
    public void SetSleep(bool isSleep)
    {
        _isSleep = isSleep;
    }
    public virtual void Hit(AttackProperty attackProperty)
    {
        CurrentHP -= attackProperty.Damage;
        if (CurrentHP <= 0f)
            OnDeath();

        _isHit = true;
        _hitTime = 0.5f;
        RPC_Hit();
        Animator?.Animator.SetBool(ClientConst.KEY_ANIMATION_HITSTATE, true);
        Animator?.SetTrigger(ClientConst.KEY_ANIMATION_HITTRIGGER);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeLocal = false)]
    protected virtual void RPC_Hit()
    {
        SendComponentMessage(eComponentEvent.HitRender);
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeLocal = true)]
    protected virtual void RPC_Death()
    {
        _hitBoxCollider.enabled = false;
    }
    protected virtual void OnDeath()
    {
        RPC_Death();
        Destroy();
        if (_controller != null)
            _controller.Terminate();

        Animator?.Animator.SetBool(ClientConst.KEY_ANIMATION_ISALIVE, false);
        StartCoroutine(IEDeath());
    }
    protected virtual IEnumerator IEDeath()
    {
        yield return null;
    }
    public virtual void Destroy()
    {
        OnDestroyCallback?.Invoke();
        ActorManager.Instance.RemoveWorld(WorldID);
    }
}
