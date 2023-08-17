using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FSMController : ActorController
{
    #region SearchHandler
    ActorSearchHandler _targetSearchHandler;
    public ActorSearchHandler SearchHandler => _targetSearchHandler;
    #endregion
    Dictionary<eFSMState, BaseState> _fsmDictionary;
    [SerializeField] eFSMState _currentState = eFSMState.Idle;
    Pattern[] _pattern;
    public AgentController Agent => _agentController;
    int _fsmConfigureIndex;
    public override eFSMState State {
        get => _currentState;
        set {
            if (_currentState == value)
                return;

            _fsmDictionary[_currentState].OnStateExit();
            _currentState = value;
            _fsmDictionary[_currentState].OnStateEnter();
            _owner.Animator?.Animator.SetInteger(ClientConst.KEY_ANIMATION_STATE, (int)_currentState);
        }
    }
    public FSMController(DynamicActor actor) : base(actor)
    {
        _agentController = new AgentController(actor);
        _fsmDictionary = new Dictionary<eFSMState, BaseState>();
        _targetSearchHandler = new ActorSearchHandler(_owner).AddConditionAllyType(eAllyType.Enemy);
    }
    public void GenerateFSMSystem()
    {
        switch(_owner.ActorType)
        {
            case eActorType.Monster:
                _fsmConfigureIndex = DataManager.Instance.MonsterTable[_owner.Index].FSMConfigureIndex;
                break;
        }
        _fsmDictionary.Add(eFSMState.Idle, new IdleState(this, _owner, _fsmConfigureIndex));
        _fsmDictionary.Add(eFSMState.Battle, new BattleState(this, _owner, _fsmConfigureIndex));
        _fsmDictionary.Add(eFSMState.Chase, new ChaseState(this, _owner, _fsmConfigureIndex));
        _fsmDictionary.Add(eFSMState.Comback, new CombackState(this, _owner, _fsmConfigureIndex));
        _agentController.SetStoppingDistance = DataManager.Instance.FSMConfigureTable[_fsmConfigureIndex].LeastDistance;
    }
    public void GeneratePattern()
    {
        int[] patternIndexArr = new int[] { 1 };
        _pattern = new Pattern[patternIndexArr.Length];
        for(int i = 0; i < _pattern.Length; ++i)
        {
            Pattern allocPattern = new Pattern(patternIndexArr[i], _owner);
            _pattern[i] = allocPattern;
        }
    }
    public bool UseMatchConditionSkill(IActor actor)
    {
        if (_pattern == null)
            return false;

        int highestPriority = 0;
        float sumProbWeight = 0;
        Queue<Pattern> patternQueue = new Queue<Pattern>();

        for (int i = 0; i < _pattern.Length; ++i)
        {
            if (!_pattern[i].IsContainsState(_currentState))
                continue;

            if (_pattern[i].IsMatchCondition())
            {
                if (_pattern[i].Data.Priority < highestPriority)
                    continue;

                if (_pattern[i].Data.Priority > highestPriority)
                {
                    sumProbWeight = 0;
                    patternQueue.Clear();
                }

                sumProbWeight += _pattern[i].Data.ProbWeight;
                patternQueue.Enqueue(_pattern[i]);
            }
        }

        if (patternQueue.Count == 0)
            return false;

        float selectProbWeight = Random.Range(0, sumProbWeight);
        Pattern selectPattern = null;
        while (patternQueue.Count > 0)
        {
            selectPattern = patternQueue.Dequeue();
            if (selectProbWeight >= sumProbWeight - selectPattern.Data.ProbWeight)
                break;

            selectProbWeight -= selectPattern.Data.ProbWeight;
        }

        BaseSkill selectSkill = DataManager.Instance.SkillTable[selectPattern.Data.SkillIndex].Skill;
        if (selectSkill.IsUseable(_owner))
        {
            selectPattern.ResetCooltime();
            selectSkill.StartSequence(_owner, _owner.Direction);
            return true;
        }
        return false;
    }
    public override void MovePosition(Vector2 position)
    {
        if (IsBlockType(eActionBlockType.Move))
            return;

        if (_agentController.SetDestination(position))
        {
            _owner.Direction = _agentController.Velocity.normalized;
            MoveType = eMoveType.Run;
            _agentController.SetSpeed = _owner.ActorStat[eStatusType.RunSpeed];
        }
    }
    public override void Update(float deltaTime)
    {
        if (!_owner.IsAlive)
            return;

        base.Update(deltaTime);

        MoveType = eMoveType.None;
        OnUpdate(deltaTime);
        OnUpdatePattern(deltaTime);
        // _agentController.MoveableUpdate();
    }
    void OnUpdate(float deltaTime)
    {
        if (_owner.IsHit) 
            return;

        _fsmDictionary[_currentState].OnStateStay(deltaTime);
    }
    void OnUpdatePattern(float deltaTime)
    {
        if (_pattern == null)
            return;

        for (int i = 0; i < _pattern.Length; ++i)
            _pattern[i].OnUpdate(deltaTime);
    }
}
