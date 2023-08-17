using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct PlayerInput : INetworkInput
{
    public static PlayerInput Empty = new PlayerInput() { };

    public const uint Sprint = 1 << 0;
    public const uint Walk = 1 << 1;

    public uint ButtonDown;
    public Vector2 MoveAxis;
    public Vector2 Direction;
    public bool IsDown(uint button) => (ButtonDown & button) == button;
    public bool IsUp(uint button, PlayerInput prevInput) =>
        prevInput.IsDown(button) && !IsDown(button);

    public static bool operator == (PlayerInput a , PlayerInput b)
    {
        if (a.ButtonDown != b.ButtonDown ||
            a.MoveAxis != b.MoveAxis ||
            a.Direction != b.Direction
            )
            return false;

        return true;
    }
    public static bool operator !=(PlayerInput a, PlayerInput b)
    {
        if (a.ButtonDown != b.ButtonDown ||
            a.MoveAxis != b.MoveAxis ||
            a.Direction != b.Direction
            )
            return true;

        return false;
    }
    public override int GetHashCode()
    {
        return GetHashCode();
    }
    public override bool Equals(object o)
    {
        if (o.GetType() == typeof(PlayerInput))
            return this == (PlayerInput)o;

        return false;
    }
}