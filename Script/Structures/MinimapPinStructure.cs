using UnityEngine;

public class MinimapPinStructure
{
    public bool IsActive = true;
    public Transform Transform;
    public ePinType PinType;
    public MinimapPinStructure(Transform target, ePinType type)
    {
        Transform = target;
        PinType = type;
    }
}
