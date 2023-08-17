using System.Collections;
using System.Collections.Generic;

public class InventoryConstraint
{
    bool _isPushable = true;
    public eItemType ItemTypeConstraint;
    HashSet<int> _indexConstraintHash;
    public InventoryConstraint(bool isPushable)
    {
        _isPushable = isPushable;
    }
    public void SetIndexConstraint(IEnumerator<int> indexArr)
    {
        if (_indexConstraintHash == null)
            _indexConstraintHash = new HashSet<int>();

        while (indexArr.MoveNext())
        {
            if (!_indexConstraintHash.Contains(indexArr.Current))
                _indexConstraintHash.Add(indexArr.Current);
        }
    }
    public void SetIndexConstraint(params int[] indexArr)
    {
        if (_indexConstraintHash == null)
            _indexConstraintHash = new HashSet<int>();

        for (int i = 0; i < indexArr.Length; ++i)
            if (!_indexConstraintHash.Contains(indexArr[i]))
                _indexConstraintHash.Add(indexArr[i]);
    }
    public bool IsPushable(Item item)
    {
        if (!_isPushable)
            return false;

        if(_indexConstraintHash != null)
        {
            if (!_indexConstraintHash.Contains(item.Index))
                return false;
        }
        if(ItemTypeConstraint != eItemType.None)
        {
            if (!item.IsIncludeItemType(ItemTypeConstraint))
                return false;
        }
        return true;
    }
}
