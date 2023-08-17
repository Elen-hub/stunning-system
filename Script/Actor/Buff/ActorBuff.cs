using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorBuff
{
    public bool IsModify { get; set; }
    IActor _owner;
    protected uint _allocatedKey = 0;
    List<Buff> _buffList;
    public List<Buff> GetBuffList => _buffList;
    public ActorBuff(IActor owner)
    {
        _owner = owner;
        _buffList = new List<Buff>(10);
    }
    public uint ApplyBuff(int key)
    {
        for(int i = _buffList.Count - 1; i >= 0; --i)
        {
            if(_buffList[i].Index == key || _buffList[i].Data.FirstStackIndex == key)
            {
                if (_buffList[i].Data.NextStackIndex == 0)
                {
                    _buffList[i].ResetTime();
                    return _buffList[i].BuffKey;
                }
                else
                {
                    _buffList[i].End();
                    Buff nextBuff = Buff.NewBuff(_buffList[i].Data.NextStackIndex);
                    _buffList[i] = nextBuff;
                    nextBuff.Start(_owner, ++_allocatedKey);
                    IsModify = true;
                    return _allocatedKey;
                }
            }
        }

        Buff buff = Buff.NewBuff(key);
        _buffList.Add(buff);
        buff.Start(_owner, ++_allocatedKey);
        IsModify = true;
        return _allocatedKey;
    }
    public void Update(float deltaTime)
    {
        for(int i = _buffList.Count - 1; i >= 0; --i)
        {
            _buffList[i].Update(deltaTime);
            if (_buffList[i].IsEnd)
            {
                _buffList.RemoveAt(i);
                IsModify = true;
            }
        }
    }
}