using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : DynamicActor
{
    Data.MonsterData GetData => DataManager.Instance.MonsterTable[Index];
    public override eActorType ActorType => eActorType.Monster;
    protected override void OnInitialize()
    {
        base.OnInitialize();

        _actorStat = new MonsterStat(this);
        _actorBuff = new ActorBuff(this);
#if UNITY_SERVER
        FSMController fsmController = new FSMController(this);
        fsmController.GenerateFSMSystem();
        fsmController.GeneratePattern();
        _controller = fsmController;
#endif
    }
    protected override void OnSpawn()
    {
        base.OnSpawn();

#if UNITY_SERVER
        CurrentHP = _actorStat[eStatusType.HP];
#endif
    }
    protected override IEnumerator IEDeath()
    {
        foreach (var dropTable in DataManager.Instance.RewardTable[GetData.RewardIndex].DropStructures)
            if (ClientMath.ProcessRandom(dropTable.Probability))
            {
                ItemObject itemObject = ItemManager.Instance.SpawnItem(Item.NewItem(dropTable.Index, dropTable.Count), Position);
                NetworkManager.Instance.ItemEventSender.NotifySpawnItem(itemObject);
            }

        yield return CoroutineUtility.Wait(2f);
        NetworkManager.Instance.Runner.Despawn(Object);
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (_controller != null)
            _controller.NetworkUpdate(Runner.DeltaTime);
    }
}
