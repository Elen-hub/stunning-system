using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCollision : MonoBehaviour
{
    [SerializeField]
    public IInteractable Owner;
    public BoxCollider2D Collider;
    #region Unity API
    private void Awake()
    {
        if (Owner == null)
            Owner = GetComponentInParent<IInteractable>();
    }
    #endregion
#if UNITY_EDITOR
    public void CreateCollider()
    {
        if (Collider != null) return;

        gameObject.layer = LayerMask.NameToLayer("Interact");
        Collider = gameObject.AddComponent<BoxCollider2D>();
        Collider.isTrigger = true;
    }
#endif
}
