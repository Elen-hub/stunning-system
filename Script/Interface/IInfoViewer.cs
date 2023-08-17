using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInfoViewer
{
    Transform FollowInfoTarget { get; }
    event System.Action OnDisableInfoEvent;
}
