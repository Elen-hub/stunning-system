using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSkinComponent : SkinComponent
{
    static int OutlineBlendID = Shader.PropertyToID("_AlphaOutlineBlend");

    bool _isOutlineActive;
    eObjectRenderState _currentState;
    Dictionary<eObjectRenderState, GameObject> _renderStateDictionary;
    Dictionary<eObjectRenderState, Renderer[]> _renderArrDictionary;
    Dictionary<eObjectRenderState, int[]> _layerOrderArrDictionary;
    public ObjectSkinComponent(IActor actor) : base(actor)
    {
        OnInitializeRenderer();
        AddEventMethods(eComponentEvent.ActivateOutline, OnReceiveEventActivateOutliner);
        AddEventMethods(eComponentEvent.SetObjectRender, OnReceiveEventSetObjectRender);
        AddEventMethods(eComponentEvent.HitRender, OnReceiveEventHitRender);
    }
    void OnInitializeRenderer()
    {
        _renderStateDictionary = new Dictionary<eObjectRenderState, GameObject>();
        _renderArrDictionary = new Dictionary<eObjectRenderState, Renderer[]>();
        _layerOrderArrDictionary = new Dictionary<eObjectRenderState, int[]>();
        Transform trs = _owner.transform.Find("IdleRender");
        if (trs != null)
        {
            _renderStateDictionary.Add(eObjectRenderState.Idle, trs.gameObject);
            _renderArrDictionary.Add(eObjectRenderState.Idle, trs.GetComponentsInChildren<Renderer>(true));
            _layerOrderArrDictionary.Add(eObjectRenderState.Idle, new int[_renderArrDictionary[eObjectRenderState.Idle].Length]);
            for (int i = 0; i < _layerOrderArrDictionary[eObjectRenderState.Idle].Length; ++i)
                _layerOrderArrDictionary[eObjectRenderState.Idle][i] = _renderArrDictionary[eObjectRenderState.Idle][i].sortingOrder;
        }
        trs = _owner.transform.Find("InteractRender");
        if (trs != null)
        {
            _renderStateDictionary.Add(eObjectRenderState.Interact, trs.gameObject);
            _renderArrDictionary.Add(eObjectRenderState.Interact, trs.GetComponentsInChildren<Renderer>(true));
            _layerOrderArrDictionary.Add(eObjectRenderState.Interact, new int[_renderArrDictionary[eObjectRenderState.Interact].Length]);
            for (int i = 0; i < _layerOrderArrDictionary[eObjectRenderState.Interact].Length; ++i)
                _layerOrderArrDictionary[eObjectRenderState.Interact][i] = _renderArrDictionary[eObjectRenderState.Interact][i].sortingOrder;
        }
        trs = _owner.transform.Find("ActionRender");
        if (trs != null)
        {
            _renderStateDictionary.Add(eObjectRenderState.Action, trs.gameObject);
            _renderArrDictionary.Add(eObjectRenderState.Action, trs.GetComponentsInChildren<Renderer>(true));
            _layerOrderArrDictionary.Add(eObjectRenderState.Action, new int[_renderArrDictionary[eObjectRenderState.Action].Length]);
            for (int i = 0; i < _layerOrderArrDictionary[eObjectRenderState.Action].Length; ++i)
                _layerOrderArrDictionary[eObjectRenderState.Action][i] = _renderArrDictionary[eObjectRenderState.Action][i].sortingOrder;
        }
    }
    void OnReceiveEventActivateOutliner(params object[] messageArr)
    {
        bool isActive = (bool)messageArr[0];
        if (_isOutlineActive != isActive)
        {
            _isOutlineActive = isActive;
            ActiveOutline(_currentState, isActive);
        }
    }
    void OnReceiveEventSetObjectRender(params object[] messageArr)
    {
        eObjectRenderState state = (eObjectRenderState)messageArr[0];
        if (_currentState == state)
            return;

        if (_renderStateDictionary.ContainsKey(state))
        {
            if (_isOutlineActive)
                ActiveOutline(_currentState, false);

            _renderStateDictionary[_currentState].gameObject.SetActive(false);
            _currentState = state;
            _renderStateDictionary[_currentState].gameObject.SetActive(true);
            OnSetLayerOrder();
            if (_isOutlineActive)
                ActiveOutline(_currentState, true);
        }
    }
    void OnReceiveEventHitRender(params object[] messageArr)
    {
        _owner.StartCoroutine(
            CoroutineUtility.ScaleLerp(_owner.transform, Vector3.one, new Vector3(1f, 0.8f, 1), 0.1f,
            CoroutineUtility.ScaleLerp(_owner.transform, _owner.transform.localScale, Vector3.one, 0.12f)));
    }
    protected override void OnSetLayerOrder()
    {
        if (!_renderArrDictionary.ContainsKey(_currentState))
            return;

        for (int i = 0; i < _renderArrDictionary[_currentState].Length; ++i)
            _renderArrDictionary[_currentState][i].sortingOrder = _layerOrderArrDictionary[_currentState][i] + _targetLayer;
    }
    void ActiveOutline(eObjectRenderState renderState, bool isActive)
    {
        if (!_renderArrDictionary.ContainsKey(renderState))
            return;

        MaterialPropertyBlock[] materialBlockArr = new MaterialPropertyBlock[_renderArrDictionary[renderState].Length];
        if (materialBlockArr != null)
        {
            for (int i = 0; i < materialBlockArr.Length; ++i)
            {
                materialBlockArr[i] = new MaterialPropertyBlock();
                _renderArrDictionary[renderState][i].GetPropertyBlock(materialBlockArr[i]);
                materialBlockArr[i].SetFloat(OutlineBlendID, isActive ? 1 : 0);
                _renderArrDictionary[renderState][i].SetPropertyBlock(materialBlockArr[i]);
            }
        }
    }
}
