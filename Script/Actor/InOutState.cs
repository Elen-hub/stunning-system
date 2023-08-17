using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InOutState
{
    [SerializeField] HouseAreaChecker _currHouse;
    public HouseAreaChecker InBuild {
        set {
            if (_currHouse != value)
            {
                _currHouse = value;
                State = HouseAreaChecker.eHouseAreaType.In;
            }
        }
    }
    public HouseAreaChecker OutBuild {
        set {
            if (_currHouse == value)
            {
                _currHouse = null;
                State = HouseAreaChecker.eHouseAreaType.Out;
            }
        }
    }
    public HouseAreaChecker.eHouseAreaType State = HouseAreaChecker.eHouseAreaType.Out;
    public int GetHouseNumber => _currHouse != null ? _currHouse.HouseNumber : -1;
    public void Reset()
    {
        _currHouse = null;
        State = HouseAreaChecker.eHouseAreaType.Out;
    }
}