using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRegistFieldUI
{
    eActorType ActorType { get; }
    string Name { get; }
    Attachment Attachment { get; }
    event System.Action OnDisableFieldUIEvent;
    event System.Action<bool> OnVisibleEvent;
}
