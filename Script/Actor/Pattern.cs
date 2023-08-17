public class Pattern
{
    int _index;
    public Data.PatternData Data => DataManager.Instance.PatternTable[_index];
    DynamicActor _owner;
    float _coolTime;
    public Pattern(int index, DynamicActor owner)
    {
        _owner = owner;
        _index = index;
    }
    public bool IsContainsState(eFSMState state)
    {
        return (Data.State & state) != 0;
    }
    public bool IsMatchCondition()
    {
        if (_coolTime < Data.CoolTime)
            return false;

        for (int i = 0; i < Data.PatternConditionArray.Length; ++i)
        {
            if (!Data.PatternConditionArray[i].IsMatchCondition(_owner))
                return false;
        }
        return true;
    }
    public void ResetCooltime()
    {
        _coolTime = 0f;
    }
    public void OnUpdate(float deltaTime)
    {
        if (_coolTime < Data.CoolTime)
            _coolTime += deltaTime;
    }
}