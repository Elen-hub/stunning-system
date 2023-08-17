using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterSkinComponent : SkinComponent
{
    bool _isFlip;
    Vector2 _prevPosition;
    SpriteRenderer[] _rendererArr;
    MaterialPropertyBlock[] _materialPropertyBlock;

    Coroutine _colorBlinkingCoroutine;
    int _colorID;
    public CharacterSkinComponent(IActor actor) : base(actor)
    {
        Transform render = actor.transform.Find("Render");
        _rendererArr = render.GetComponentsInChildren<SpriteRenderer>(true);
        _materialPropertyBlock = new MaterialPropertyBlock[_rendererArr.Length];
        _colorID = Shader.PropertyToID("_Color");
        for (int i = 0; i < _rendererArr.Length; ++i)
        {
            _materialPropertyBlock[i] = new MaterialPropertyBlock();
            _rendererArr[i].GetPropertyBlock(_materialPropertyBlock[i]);
        }
        AddEventMethods(eComponentEvent.HitRender, OnReceiveEventHitRender);
    }
    void OnReceiveEventHitRender(params object[] messageArr)
    {
        if (_colorBlinkingCoroutine != null)
            _owner.StopCoroutine(_colorBlinkingCoroutine);

        _colorBlinkingCoroutine = _owner.StartCoroutine(IEColorBlinking());
    }
    protected override void OnSetLayerOrder()
    {
        for(int i = 0; i < _rendererArr.Length; ++i)
            _rendererArr[i].sortingOrder = _targetLayer;
    }
    protected override void OnClientUpdate(float deltaTime)
    {
        base.OnClientUpdate(deltaTime);

        Vector2 position = _owner.Position;
        if(_prevPosition != position)
        {
            _prevPosition = position;
            OnReceiveEventSetLayerOrder(_prevPosition);
        }
        bool isFlip = _owner.Direction.x > 0;
        if (isFlip != _isFlip)
        {
            _isFlip = isFlip;
            for (int i = 0; i < _rendererArr.Length; ++i)
                _rendererArr[i].flipX = _isFlip;
        }
    }
    IEnumerator IEColorBlinking()
    {
        float deltaTime;
        for (int i = 0; i < 3; ++i)
        {
            deltaTime = 0f;
            while (deltaTime < 1f)
            {
                deltaTime += TimeManager.DeltaTime * 15f;
                Color lerpColor = Color.Lerp(Color.white, Color.red, deltaTime);
                for (int j = 0; j < _rendererArr.Length; ++j)
                {
                    _rendererArr[j].color = lerpColor;
                }
                //for (int j = 0; j < _materialPropertyBlock.Length; ++j)
                //{
                //    _materialPropertyBlock[j].SetColor(_colorID, lerpColor);
                //    _rendererArr[j].SetPropertyBlock(_materialPropertyBlock[j]);
                //}
                yield return null;
            }
            deltaTime = 0f;
            while (deltaTime < 1f)
            {
                deltaTime += TimeManager.DeltaTime * 15f;
                Color lerpColor = Color.Lerp(Color.red, Color.white, deltaTime);
                for (int j = 0; j < _rendererArr.Length; ++j)
                {
                    _rendererArr[j].color = lerpColor;
                }
                //for (int j = 0; j < _materialPropertyBlock.Length; ++j)
                //{
                //    _materialPropertyBlock[j].SetColor(_colorID, lerpColor);
                //    _rendererArr[j].SetPropertyBlock(_materialPropertyBlock[j]);
                //}
                yield return null;
            }
        }
        _colorBlinkingCoroutine = null;
    }
}
