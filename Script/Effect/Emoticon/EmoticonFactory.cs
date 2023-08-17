using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoticonFactory
{
    public Transform transform;
    Dictionary<eEmoticonType, ObjectMemoryPool<Emoticon>> _emoticonPoolDic;
    Dictionary<eEmoticonType, string> _emoticonPathDic;
    Dictionary<ulong, Emoticon> _emoticonDic;
    public EmoticonFactory()
    {
        int emoticonCatalogCount = (int)eEmoticonType.End;
        _emoticonPoolDic = new Dictionary<eEmoticonType, ObjectMemoryPool<Emoticon>>(emoticonCatalogCount);
        _emoticonPathDic = new Dictionary<eEmoticonType, string>(emoticonCatalogCount);
        for (eEmoticonType i = 0; i < eEmoticonType.End; ++i)
        {
            _emoticonPoolDic.Add(i, new ObjectMemoryPool<Emoticon>(10));
            _emoticonPathDic.Add(i, string.Format("Emoticon/Emoticon_{0}", i.ToString()));
        }
        _emoticonDic = new Dictionary<ulong, Emoticon>();
        GameObject parent = new GameObject("EmoticonFactory");
        Object.DontDestroyOnLoad(parent);
        transform = parent.transform;
    }
    Emoticon SpawnEmoticon(eEmoticonType type)
    {
        Emoticon spawnedEmoticon = _emoticonPoolDic[type].GetItem();
        if(spawnedEmoticon == null)
        {
            Emoticon memory = Resources.Load<Emoticon>(_emoticonPathDic[type]);
            spawnedEmoticon = Object.Instantiate(memory);
            spawnedEmoticon.Initialize(type);
        }
        spawnedEmoticon.OnDisableEvent += RegistPool;
        return spawnedEmoticon;
    }
    public Emoticon SetEmoticon(DynamicActor targetActor, eEmoticonType type, eAttachmentTarget attachmentType, bool isFollow, float durationTime)
    {
        if (_emoticonDic.ContainsKey(targetActor.WorldID))
            _emoticonDic[targetActor.WorldID].Disable();

        Transform targetTransform = targetActor.Attachment.GetAttachmentElement(attachmentType).Transform;
        Emoticon emoticon = SpawnEmoticon(type);
        emoticon.OnDisableEvent += (emoticon) => _emoticonDic.Remove(targetActor.WorldID);
        if (isFollow) emoticon.Enable(targetTransform, durationTime);
        else emoticon.Enable(targetTransform.position, durationTime);
        _emoticonDic.Add(targetActor.WorldID, emoticon);
        return emoticon;
    }
    void RegistPool(Emoticon emoticon)
    {
        _emoticonPoolDic[emoticon.EmoticonType].Register(emoticon);
    }
}
