using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : BaseInputHandler
{
    Camera _camera;
    NetworkController _controller;
    public CharacterInputHandler(NetworkController networkController)
    {
        _controller = networkController;
        _camera = CameraManager.Instance.GetCamera(eCameraType.MainCamera).Camera;
    }
    void OnUpdateMoveAxis()
    {
        Vector2 moveAxis = Vector2.zero;
        if (IsKeyInput(eInputType.MoveUp))
            moveAxis.y += 1f;
        if (IsKeyInput(eInputType.MoveDown))
            moveAxis.y -= 1f;
        if (IsKeyInput(eInputType.MoveLeft))
            moveAxis.x -= 1f;
        if (IsKeyInput(eInputType.MoveRight))
            moveAxis.x += 1f;

        _networkInput.MoveAxis = moveAxis;
    }
    void OnUpdateMouseAxis()
    {
        Vector2 worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        _networkInput.Direction = worldPosition - _controller.Owner.Position;
        _networkInput.Direction.Normalize();
    }
    void OnUpdateNetworkAction()
    {
        if (IsKeyDown(eInputType.Sprint))
            SetButton = PlayerInput.Sprint;

        if(IsKeyUp(eInputType.Sprint))
            RemoveButton = PlayerInput.Sprint;
    }
    void OnUpdateQuickslot()
    {
        if (IsKeyDown(eInputType.QuickSlot1))
            _controller.InputQuickSlot(0);
        if (IsKeyDown(eInputType.QuickSlot2))
            _controller.InputQuickSlot(1);
        if (IsKeyDown(eInputType.QuickSlot3))
            _controller.InputQuickSlot(2);
        if (IsKeyDown(eInputType.QuickSlot4))
            _controller.InputQuickSlot(3);
        if (IsKeyDown(eInputType.QuickSlot5))
            _controller.InputQuickSlot(4);
        if (IsKeyDown(eInputType.QuickSlot6))
            _controller.InputQuickSlot(5);
        if (IsKeyDown(eInputType.QuickSlot7))
            _controller.InputQuickSlot(6);
        if (IsKeyDown(eInputType.QuickSlot8))
            _controller.InputQuickSlot(7);
    }
    void OnUpdateMouseInput()
    {
        if (IsKeyDown(eInputType.MouseLeft))
            _controller.Owner.SendComponentMessage(eComponentEvent.MouseLeftDown);

        if (IsKeyUp(eInputType.MouseLeft))
            _controller.Owner.SendComponentMessage(eComponentEvent.MouseLeftUp);

        if (IsKeyDown(eInputType.MouseRight))
            _controller.Owner.SendComponentMessage(eComponentEvent.MouseRightDown);

        if (IsKeyUp(eInputType.MouseRight))
            _controller.Owner.SendComponentMessage(eComponentEvent.MouseRightUp);
    }
    void OnUpdateClientAction()
    {
        if (IsKeyDown(eInputType.QuickSlotSwap))
            _controller.InputSwapQuickSlot();

        if (IsKeyDown(eInputType.PickupItem))
            _controller.PickupItem();

        if (IsKeyDown(eInputType.OpenInventory))
        {
            UInventory inventory = UIManager.Instance.Get<UInventory>(eUIName.UInventory);
            if (inventory.SubInventoryIndex == 0)
            {
                inventory.Open();
                UIManager.Instance.Open<UEquipmentUI>(eUIName.UEquipmentUI);
            }
        }

        if (IsKeyDown(eInputType.Reload))
            _controller.Owner.SendComponentMessage(eComponentEvent.Reload);

        if (IsKeyDown(eInputType.Interact))
            _controller.Owner.SendComponentMessage(eComponentEvent.InteractInput);
    }
    protected override void OnUpdate()
    {
        // Network Process
        OnUpdateMoveAxis();
        OnUpdateMouseAxis();
        OnUpdateNetworkAction();

        // Client Process
        OnUpdateQuickslot();
        OnUpdateMouseInput();
        OnUpdateClientAction();
    }
}
