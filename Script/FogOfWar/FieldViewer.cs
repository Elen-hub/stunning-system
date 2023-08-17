using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldViewer
{
    IActor _actor;
    Transform transform;
    public FieldViewer(IActor viewer)
    {
        _actor = viewer;
        transform = viewer.transform;
    }
    public void Scan()
    {

    }
}
