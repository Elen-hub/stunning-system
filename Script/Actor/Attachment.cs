using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum eAttachmentTarget
{
    OverHead,
    HPBar,
    Head,
    Body,
    Pivot,
    Ground,
    UnderPivot,
    InfoViewer,
    NameTag,
    End,
}
[DisallowMultipleComponent]
public class Attachment : MonoBehaviour
{
    Dictionary<eAttachmentTarget, AttachmentElement> _attachmentDictionary;
    public void Initialize()
    {
        _attachmentDictionary = new Dictionary<eAttachmentTarget, AttachmentElement>();
        AttachmentElement[] attachmentElement = GetComponentsInChildren<AttachmentElement>();
        if (attachmentElement != null)
            for (int i = 0; i < attachmentElement.Length; ++i)
                _attachmentDictionary.Add(attachmentElement[i].AttachmentTarget, attachmentElement[i]);
    }
    public AttachmentElement GetAttachmentElement(eAttachmentTarget target)
    {
        if (_attachmentDictionary.ContainsKey(target))
            return _attachmentDictionary[target];

        return null;
    }
    #region Inspector Editor Methods
#if UNITY_EDITOR
    // SkeletonMecanim _skeletonMecanim;
    public bool IsEditorActiveGizmos = true;
    private void Awake()
    {
        // _skeletonMecanim = GetComponentInChildren<SkeletonMecanim>();
        Initialize();
    }
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && DebugUtility.Instance.EnableAttachmentRuntimeGizmo && IsEditorActiveGizmos)
            DebugAttachmentPoints();
    }
    private void OnDrawGizmos()
    {        
        if (Application.isPlaying) 
            return; 

        if(IsEditorActiveGizmos)
            DebugAttachmentPoints();
    }
    void DebugAttachmentPoints()
    {
        float cameraDistance = UnityEditor.SceneView.currentDrawingSceneView.cameraDistance / 4f - 1f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.05f);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-2f, 0f));
        DebugUtility.DrawString("Prefab Pivot", transform.position + new Vector3(-2f, 0.1f), cameraDistance, cameraDistance, Gizmos.color);

        if (_attachmentDictionary == null)
            return;

        foreach (var element in _attachmentDictionary)
        {
            float radius = 0f;
            Color color;
            if (UnityEditor.Selection.activeGameObject == element.Value.gameObject)
            {
                color = Color.red;
                radius = 0.1f;
            }
            else
            {
                color = Color.yellow;
                radius = 0.05f;
            }
            Gizmos.color = color;
            if (UnityEditor.PrefabUtility.IsPrefabAssetMissing(element.Value))
                continue;

            Vector3 position = element.Value.transform.position;
            Gizmos.DrawWireSphere(position, radius);
            Gizmos.DrawLine(position, position + new Vector3(-2f, 0.5f));
            DebugUtility.DrawString(element.Value.AttachmentTarget.ToString(), position + new Vector3(- 2.2f,  0.7f), cameraDistance, cameraDistance, color);
        }
    }
#endif
    #endregion
}
