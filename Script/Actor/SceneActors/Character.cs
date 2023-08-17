using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class Character : DynamicActor, IRegistFieldUI
{
    [Networked] public override string Name { get; set; }
    [Networked] public float CurrentSP { get; set; }
    [Networked] public float CurrentHunger { get; set; }
    Dictionary<eStatusType, eStatusState> _statusStateDictionary;
    public eStatusState GetStatusState(eStatusType statusType) => _statusStateDictionary[statusType];
    [Networked] public float CurrentStress { get; set; }
    [Networked] public float CurrentSleepy { get; set; }
    [Networked] public float CurrentTemperature { get; set; }
    public override eActorType ActorType => eActorType.Character;
    public event System.Action OnDisableFieldUIEvent;
    public event System.Action<bool> OnVisibleEvent;
    protected override void OnInitialize()
    {
        base.OnInitialize();

        _actorStat = new CharacterStat(this);
        _actorBuff = new ActorBuff(this);
#if UNITY_SERVER
        NetworkController networkController = new NetworkController(this);
        _controller = networkController;
        networkController.IsPossibleSprintEvent += IsPossibleSprint;
        bool IsPossibleSprint() => CurrentSP > 10f;
#else
        // new CharacterSkinComponent(this);
#endif
        new EquipmentComponent(this);
        _statusStateDictionary = new Dictionary<eStatusType, eStatusState>();
    }
    protected override void OnSpawn()
    {
        base.OnSpawn();

        CurrentHP = _actorStat[eStatusType.HP];
        CurrentSP = _actorStat[eStatusType.SP];
        CurrentHunger = _actorStat[eStatusType.Hunger] * 0.5f;
        CurrentTemperature = _actorStat[eStatusType.Temperature];

        _statusStateDictionary.Add(eStatusType.Hunger, eStatusState.Middle);
        _statusStateDictionary.Add(eStatusType.Temperature, eStatusState.Middle);
        _statusStateDictionary.Add(eStatusType.Stress, eStatusState.Lowest);
        _statusStateDictionary.Add(eStatusType.HP, eStatusState.Highnest);
        _statusStateDictionary.Add(eStatusType.Sleepy, eStatusState.Lowest);

        if (Object.HasInputAuthority)
        {
            UIManager.Instance.Open<UStatusHUD>(eUIName.UStatusHUD).Character = this;
            UIManager.Instance.Open<USmartWatchUI>(eUIName.USmartWatchUI);
        }
#if !UNITY_SERVER
        UIManager.Instance.FieldUI.SetNameTag(this);
        MinimapPinStructure pin = null;
        if (Object.HasInputAuthority) pin = new MinimapPinStructure(transform, ePinType.Player);
        else pin = new MinimapPinStructure(transform, ePinType.Friend);
        WorldManager.Instance.MinimapSystem.AddMinimapPin(this, pin);
#endif
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
#if !UNITY_SERVER
        OnDisableFieldUIEvent?.Invoke();
        WorldManager.Instance.MinimapSystem.RemovePin(this);
#endif
        base.Despawned(runner, hasState);
    }
    #region Update Recovery
    protected override void OnUpdateRecoveryTime()
    {
        base.OnUpdateRecoveryTime();

        if (Object.HasStateAuthority)
        {
            if (_controller.MoveType != eMoveType.Sprint)
                CurrentSP = Mathf.Clamp(CurrentSP + _actorStat[eStatusType.SPRecovery], 0f, _actorStat[eStatusType.SP]);
        }
        OnUpdateHP();
        OnUpdateHunger();
        OnUpdateStress();
        OnUpdateSleepy();
        OnUpdateTemp();
    }
    void OnUpdateHP()
    {
        float maxAcmount = _actorStat[eStatusType.HP];
        if (Object.HasStateAuthority)
            CurrentHP = Mathf.Clamp(CurrentHP + _actorStat[eStatusType.HPRecovery], 0f, maxAcmount);

        if (Object.HasStateAuthority || Object.HasInputAuthority)
        {
            float percentage = CurrentHP / maxAcmount;
            if (percentage > 0.8f) _statusStateDictionary[eStatusType.HP] = eStatusState.Highnest;
            else if (percentage > 0.6f) _statusStateDictionary[eStatusType.HP] = eStatusState.High;
            else if (percentage > 0.4f) _statusStateDictionary[eStatusType.HP] = eStatusState.Middle;
            else if (percentage > 0.2f) _statusStateDictionary[eStatusType.HP] = eStatusState.Low;
            else _statusStateDictionary[eStatusType.HP] = eStatusState.Lowest;
        }
    }
    void OnUpdateHunger()
    {
        float maxAcmount = _actorStat[eStatusType.Hunger];
        if(Object.HasStateAuthority)
            CurrentHunger = Mathf.Clamp(CurrentHunger + _actorStat[eStatusType.HungerRecovery], 0f, maxAcmount);

        if (Object.HasStateAuthority || Object.HasInputAuthority)
        {
            float percentage = CurrentHunger / maxAcmount;
            if (percentage > 0.8f) _statusStateDictionary[eStatusType.Hunger] = eStatusState.Highnest;
            else if (percentage > 0.6f) _statusStateDictionary[eStatusType.Hunger] = eStatusState.High;
            else if (percentage > 0.4f) _statusStateDictionary[eStatusType.Hunger] = eStatusState.Middle;
            else if (percentage > 0.2f) _statusStateDictionary[eStatusType.Hunger] = eStatusState.Low;
            else _statusStateDictionary[eStatusType.Hunger] = eStatusState.Lowest;
        }
    }
    void OnUpdateStress()
    {
        float maxAcmount = _actorStat[eStatusType.Stress];
        if (Object.HasStateAuthority)
            CurrentStress = Mathf.Clamp(CurrentStress + _actorStat[eStatusType.StressRecovery], 0f, maxAcmount);

        if (Object.HasStateAuthority || Object.HasInputAuthority)
        {
            float percentage = CurrentStress / maxAcmount;
            if (percentage > 0.8f) _statusStateDictionary[eStatusType.Stress] = eStatusState.Highnest;
            else if (percentage > 0.6f) _statusStateDictionary[eStatusType.Stress] = eStatusState.High;
            else if (percentage > 0.4f) _statusStateDictionary[eStatusType.Stress] = eStatusState.Middle;
            else if (percentage > 0.2f) _statusStateDictionary[eStatusType.Stress] = eStatusState.Low;
            else _statusStateDictionary[eStatusType.Stress] = eStatusState.Lowest;
        }
    }
    void OnUpdateSleepy()
    {
        float maxAcmount = _actorStat[eStatusType.Sleepy];
        if (Object.HasStateAuthority && !IsSleep)
            CurrentSleepy = Mathf.Clamp(CurrentSleepy + _actorStat[eStatusType.SleepyRecovery], 0f, maxAcmount);

        if (Object.HasStateAuthority || Object.HasInputAuthority)
        {
            float percentage = CurrentSleepy / maxAcmount;
            if (percentage > 0.8f) _statusStateDictionary[eStatusType.Sleepy] = eStatusState.Highnest;
            else if (percentage > 0.6f) _statusStateDictionary[eStatusType.Sleepy] = eStatusState.High;
            else if (percentage > 0.4f) _statusStateDictionary[eStatusType.Sleepy] = eStatusState.Middle;
            else if (percentage > 0.2f) _statusStateDictionary[eStatusType.Sleepy] = eStatusState.Low;
            else _statusStateDictionary[eStatusType.Sleepy] = eStatusState.Lowest;
        }
    }
    void OnUpdateTemp()
    {
        if (Object.HasStateAuthority)
        {
            if (InOutState.State != HouseAreaChecker.eHouseAreaType.In)
                CurrentTemperature = Mathf.Lerp(CurrentTemperature, EnvironmentManager.Instance.GetTemp, TimeManager.DeltaTime);
            else
                CurrentTemperature = Mathf.Lerp(CurrentTemperature, _actorStat[eStatusType.Temperature], TimeManager.DeltaTime);
        }
    }
    #endregion
    public override void DefaultAttack()
    {

    }
    protected override void Update()
    {
        base.Update();

        if (_controller != null)
        {
            if (_controller.MoveType == eMoveType.Sprint)
            {
                if (CurrentSP <= 0f)
                {
                    _controller.MoveType = eMoveType.Run;
                    return;
                }
                CurrentSP -= TimeManager.DeltaTime * 5f;
            }
        }
    }
    private void LateUpdate()
    {
        if(!IsProxy)
            WorldManager.Instance.HouseChunkSystem.ProcessHosueChecher(this);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (Object.HasStateAuthority)
        {
            Runner.AddPlayerAreaOfInterest(Object.InputAuthority, transform.position, ClientConst.InterestAreaRange/*, Object.InputAuthority*/);
            if (_controller != null)
                _controller.NetworkUpdate(Runner.DeltaTime);
        }
    }
#if !UNITY_SERVER
    public void OnBecameVisible()
    {
        OnVisibleEvent?.Invoke(true);
    }
    public void OnBecameInvisible()
    {
        OnVisibleEvent?.Invoke(false);
    }
#endif
}
