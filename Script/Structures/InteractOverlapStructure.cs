using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InteractOverlapStructure
{
    public readonly Collider2D Collider;
    public readonly IInteractable InteractObject;
    public InteractOverlapStructure(Collider2D collider, IInteractable interactObject)
    {
        Collider = collider;
        InteractObject = interactObject;
    }
}
