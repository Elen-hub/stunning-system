using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ComponentEventMessage
{
    public eComponentEvent ComponentEvent;
    public ComponentEventMessage(eComponentEvent componentEvent)
    {
        ComponentEvent = componentEvent;
    }
}
