using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReloadable
{
    int BulletIndex { get; }
    bool IsPossibleReload(IActor actor);
    void Reload(IActor actor);
}
