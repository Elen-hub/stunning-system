using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BuildComponent : BaseComponent
{
    IInstallable _installTarget;
    public BuildComponent(IActor master) : base(master, eComponent.BuildComponent)
    {
        AddEventMethods(eComponentEvent.InstallMode, OnReceiveEventEditMode);
    }
    void OnReceiveEventEditMode(params object[] messageArr)
    {
        bool isOn = (bool)messageArr[0];
        if (isOn)
        {
            int index = (int)messageArr[1];
            ActorManager.Instance.InstallStaticActor(index).ContinueWith(result =>
            {
                _installTarget = result;
                _installTarget.InstallMode(true, GridManager.Instance.GetTilePosition);
                GridManager.Instance.CreateGridEffect(_owner, _installTarget);
            });
        }
        else
        {
            GridManager.Instance.DeleteGridEffect();
            if (_installTarget != null)
            {
                _installTarget.InstallMode(false, Vector2.zero);
                _installTarget = null;
            }
        }
    }
    protected override void OnUpdate(float deltaTime)
    {
        if (_installTarget != null)
            _installTarget.transform.position = GridManager.Instance.GetTilePosition;
    }
}
