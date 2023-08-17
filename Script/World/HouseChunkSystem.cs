using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseChunkSystem
{
    Transform _parent; 
    HashSet<House> _houseRoomChecker;
    public void AddRoomChecker(House checker) => _houseRoomChecker.Add(checker);
    public HouseChunkSystem(Transform parent)
    {
        _parent = parent;
        _houseRoomChecker = new HashSet<House>();
    }
    public void SetHouseData(TextAsset textAsset)
    {
        byte[] byteArr = textAsset.bytes;
        SaveController controller = new SaveController(byteArr);
        while (controller.IsPossibleParsing)
        {
            int index = controller.GetInt32();
            Vector2 pos = new Vector2(controller.GetFloat(), controller.GetFloat());
            GameObject houseObject = DataManager.Instance.HouseTable.HouseIndexList[index];
            House house = Object.Instantiate(houseObject, pos, Quaternion.identity, _parent).GetComponent<House>();
            house.Initialize();
        }
    }
    public void ProcessHosueChecher(DynamicActor dynamicActor)
    {
        foreach(var element in _houseRoomChecker)
            element.ProcessHouseChecker(dynamicActor);
    }
}
