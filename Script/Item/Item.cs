using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

[System.Serializable]
public class Item : IPacket
{
    public Data.ItemData ItemData => DataManager.Instance.ItemTable[Index];
    public int Index;
    public int Count;
    public virtual string GetInformationText() => LocalizingManager.Instance.GetLocalizing(ItemData.DescriptionKey);
    public bool IsIncludeItemType(eItemType type) => (type & ItemData.Type) != 0;
    public Item()
    {

    }
    public virtual void OnEnterSlot(IComponent actor)
    {

    }
    public virtual void OnExitSlot(IComponent actor)
    {

    }

    #region Operator
    public static Item operator + (Item item1, Item item2)
    {
        if (item2 == null) 
            return item1;

        item1.Count += item2.Count;
        return item1;
    }
    #endregion
    #region Malloc
    public static Item NewItem(int index) => NewItem(index, 1);
    public static Item NewItem(int index, int count)
    {
        switch (DataManager.Instance.ItemTable[index].Type)
        {
            case eItemType.Weapon:
                switch (DataManager.Instance.ItemTable[index].SubType)
                {
                    case 0:
                        return new WeaponItem()
                        {
                            Index = index,
                            Count = count,
                        };
                    case 1:
                        return new AutomaticGunItem()
                        {
                            Index = index,
                            Count = count,
                        };
                }
                break;
            case eItemType.Magazine:
                return new BulletMagazineItem()
                {
                    Index = index,
                    Count = count,
                };
            case eItemType.Material:
                return new Item()
                {
                    Index = index,
                    Count = count,
                };
            case eItemType.Install:
                return new InstallItem()
                {
                    Index = index,
                    Count = count,
                };
            case eItemType.Crop:
                return new CropItem()
                {
                    Index = index,
                    Count = count,
                };
            case eItemType.Food:
                return new FoodItem()
                {
                    Index = index,
                    Count = count,
                };
            case eItemType.Armor:
            case eItemType.Hat:
            case eItemType.Shoes:
            case eItemType.Ring:
            case eItemType.Necklace:
                return new EquipItem()
                {
                    Index = index,
                    Count = count,
                };
        }
        return null;
    }
    public virtual Item Clone()
    {
        return MemberwiseClone() as Item;
    }
    #endregion
    #region Network
    public virtual int GetByteSize => ReliableHelper.IntSize * 2;
    public virtual void EnqueueByte()
    {
        BaseEventSender.CopyBytes(Index);
        BaseEventSender.CopyBytes(Count);
    }
    #endregion
    #region Save & Load
    public static Item LoadItem(SaveController saveController)
    {
        int index = saveController.GetInt32();
        int count = saveController.GetInt32();
        Item item = NewItem(index, count);
        item.OnLoadItem(saveController);
        return item;
    }
    public virtual void OnLoadItem(SaveController saveController)
    {

    }
    public virtual void EnqueueByte(SaveController saveController)
    {
        saveController.CopyBytes(Index);
        saveController.CopyBytes(Count);
    }
    #endregion
}
