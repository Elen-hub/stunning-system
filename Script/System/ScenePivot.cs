using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePivot : MonoBehaviour
{
    Vector3 _size = new Vector3(50, 50);
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _size);
    }
}
