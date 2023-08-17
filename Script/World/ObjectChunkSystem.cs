using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class ObjectChunkSystem
{
    public ObjectChunkSystem()
    {

    }
    public async UniTaskVoid SpawnObjectAsync(TextAsset text)
    {
        byte[] byteArr = text.bytes;
        SaveController controller = new SaveController(byteArr);
        while (controller.IsPossibleParsing)
            await ActorManager.Instance.SpawnLoadObject(controller.GetInt32(), controller);
    }
}
