using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Attachment))]
public class AttachmentEditor : Editor
{
    Attachment _attachment;
    Attachment Attachment {
        get {
            if (_attachment == null)
            {
                _attachment = target as Attachment;
                _attachment.Initialize();
            }
            return _attachment; 
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        GUILayout.Label("Attachment Element List");
        GUILayout.Space(5f);
        for (eAttachmentTarget i = 0; i<eAttachmentTarget.End; ++i)
        {
            OnGUIAttachmentElement(i);
            GUILayout.Space(3f);
        }
    }
    void OnGUIAttachmentElement(eAttachmentTarget attachmentTarget)
    {
        AttachmentElement element = Attachment.GetAttachmentElement(attachmentTarget);
        if (element == null)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f)))
            {
                CreateAttachment(attachmentTarget).AttachmentTarget = attachmentTarget;
                EditorUtility.SetDirty(Attachment);
                Attachment.Initialize();
                // string path = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().assetPath;
                return;
            }
            GUILayout.Label(attachmentTarget.ToString());
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("¥ø", GUILayout.MinWidth(20f), GUILayout.MaxWidth(20f)))
            {
                Selection.activeGameObject = element.gameObject;
            }
            GUILayout.Label(attachmentTarget.ToString());
            GUILayout.EndHorizontal();
        }
    }
    AttachmentElement CreateAttachment(eAttachmentTarget target)
    {
        if (Attachment.GetAttachmentElement(target) != null)
            return null;

        GameObject obj = new GameObject("Attachment_" + target.ToString());
        obj.transform.SetParent(Attachment.transform);
        AttachmentElement attach = obj.AddComponent<AttachmentElement>();
        attach.AttachmentTarget = target;
        return attach;
    }
}
