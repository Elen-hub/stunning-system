using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseAreaChecker : MonoBehaviour
{
    public enum eHouseAreaType
    {
        Out,
        In,
        Back,
    }
    [SerializeField] House _house;
    public int HouseNumber => _house.HouseNumber;
    [SerializeField] protected eHouseAreaType _houseAreaType;
    public eHouseAreaType AreaType => _houseAreaType;
    [SerializeField] protected Bounds _roomArea;
    public Bounds AreaBounds => _roomArea;
    HashSet<IActor> _containsHash;
    public void Initialize(House house)
    {
        _house = house;
        _roomArea.center = transform.position;
        _containsHash = new HashSet<IActor>();
    }
    public bool IsEnterState(DynamicActor dynamicActor)
    {
        if (!_containsHash.Contains(dynamicActor))
        {
            if(_houseAreaType == eHouseAreaType.In)
                dynamicActor.InOutState.InBuild = this;

            return _containsHash.Add(dynamicActor);
        }
        return false;
    }
    public bool IsExitState(DynamicActor dynamicActor)
    {
        if (_containsHash.Contains(dynamicActor))
        {
            if(_houseAreaType == eHouseAreaType.In)
                dynamicActor.InOutState.OutBuild = this;

            return _containsHash.Remove(dynamicActor);
        }
        return false;
    }
#if UNITY_EDITOR
    House _debugHouse;
    Dictionary<eHouseAreaType, Color> _colorDictionary = new Dictionary<eHouseAreaType, Color>()
    {
        { eHouseAreaType.In, new Color(0, 1, 0, 0.4f) },
        { eHouseAreaType.Back, new Color(1f, 0f, 1f, 0.4f) },
    };
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (_debugHouse == null)
            _debugHouse = GetComponentInParent<House>();

        if (_debugHouse != null)
            if (!_debugHouse.IsDebug)
                return;

        if (_roomArea.center != transform.position)
            _roomArea.center = transform.position;

        if (_houseAreaType != eHouseAreaType.Out)
        {
            float cameraDistance = UnityEditor.SceneView.currentDrawingSceneView.cameraDistance / 4f - 1f;

            Gizmos.color = _colorDictionary[_houseAreaType];
            Gizmos.DrawCube(_roomArea.center, _roomArea.size);
            DebugUtility.DrawString(_houseAreaType.ToString(), _roomArea.center, cameraDistance, cameraDistance, Color.red);
        }
    }
#endif
}
