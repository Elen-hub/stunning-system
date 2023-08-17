using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : ActorController
{
    public event DelegateUtility.GetBoolDelegate IsPossibleSprintEvent;
    PlayerInput _prevInput = PlayerInput.Empty;
    public PlayerInput PlayerInput => _prevInput;
    public NetworkController(DynamicActor actor) : base(actor)
    {

    }
    public override void NetworkUpdate(float deltaTime)
    {
        base.NetworkUpdate(deltaTime);

        if (_owner.GetInput(out PlayerInput input))
        {
            if (!_owner.IsSleep)
            {
                if (IsBlockType(eActionBlockType.Move))
                {
                    MoveType = eMoveType.None;
                }
                else
                {
                    if (input.MoveAxis.x != 0 || input.MoveAxis.y != 0)
                    {
                        MoveType = eMoveType.Run;
                        if (MoveType != eMoveType.None)
                        {
                            OnUpdateSprint(input);
                            MovePosition(input.MoveAxis);
                        }
                    }
                    else
                        MoveType = eMoveType.None;
                }
                if (!IsBlockType(eActionBlockType.Direction))
                    _owner.Direction = input.Direction;
            }
            _prevInput = input;
        }
    }
    void OnUpdateSprint(PlayerInput input)
    {
        if (input.IsDown(PlayerInput.Sprint))
        {
            if (input.MoveAxis.x != 0 || input.MoveAxis.y != 0)
            {
                if (IsPossibleSprintEvent())
                    MoveType = eMoveType.Sprint;
            }
        }
        if (MoveType == eMoveType.Sprint)
        {
            if (input.IsUp(PlayerInput.Sprint, _prevInput))
                MoveType = eMoveType.Run;
        }
    }
    public virtual void InputQuickSlot(int slotNumber)
    {
        _owner.SendComponentMessage(eComponentEvent.SelectQuickSlot, slotNumber);
    }
    public void InputSwapQuickSlot()
    {
        ItemComponent itemComponent = _owner.GetComponent<ItemComponent>(eComponent.ItemComponent);
        if(itemComponent != null)
            itemComponent.SwapQuickSlot();
    }
    public void PickupItem()
    {
        ItemComponent itemComponent = _owner.GetComponent<ItemComponent>(eComponent.ItemComponent);
        if (itemComponent != null)
            itemComponent.PickupItem();
    }
}
