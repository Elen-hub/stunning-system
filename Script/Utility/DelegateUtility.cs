using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class DelegateUtility
{
    public delegate IEnumerator CombatUsingDelegate(IActor actor, Vector2 direction);
    public delegate float GetFloatDelegate();
    public delegate bool GetBoolDelegate();
    public delegate bool PossibleUsingDelegate(IActor actor);
    public delegate bool IsOverlapCondition(Vector2Int tilePosition);
}
