using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RPCGenerator : NetworkBehaviour
{
    public static RPCGenerator Instance;
    public override void Spawned()
    {
        base.Spawned();

        Instance = this;
    }
    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeLocal = false)]
    public void RPC_SpawnEffect(ulong worldID, int index, Vector2 position, Vector2 direction, float durationTime, float speed, float maxDistance)
    {
        EffectManager.Instance.SpawnEffect(worldID, index, position, direction, durationTime, speed, maxDistance);
    }
    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeLocal = false)]
    public void RPC_SpawnEffect(ulong worldID, int index, Vector2 position, Vector2 direction, float durationTime)
    {
        EffectManager.Instance.SpawnEffect(worldID, index, position, direction, durationTime);
    }
    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeLocal = false)]
    public void RPC_SpawnEffect(ulong worldID, int index, ulong trackingTarget, eAttachmentTarget attachmentTarget, float durationTime)
    {
        IActor actor = ActorManager.Instance.GetActor(trackingTarget);
        if(actor != null)
            EffectManager.Instance.SpawnEffect(worldID, index, actor, attachmentTarget, durationTime);
    }
    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Unreliable, InvokeLocal = true)]
    public void RPC_RemoveEffect(ulong worldID)
    {
        EffectManager.Instance.RemoveEffect(worldID); 
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.Proxies, Channel = RpcChannel.Unreliable, InvokeLocal = false)]
    public void RPC_SetEmoticon(ulong worldID, eEmoticonType type, eAttachmentTarget target, bool isFollow, float durationTime = 2.5f)
    {
        DynamicActor dynamicActor = ActorManager.Instance.GetActor<DynamicActor>(worldID);
        if (dynamicActor != null)
            Emoticon.SetEmoticon(dynamicActor, type, target, isFollow, durationTime);
    }
}
