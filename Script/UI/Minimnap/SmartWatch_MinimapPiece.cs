using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmartWatch_MinimapPiece : Image
{
    Image _fogImage;
    Vector2Int _offset;
    public Vector2Int Offset => _offset;
    public void Initialize(Vector2Int offset)
    {
        _offset = offset;
        rectTransform.anchoredPosition = _offset * 2;
        _fogImage = transform.Find("Img_Fog").GetComponent<Image>();
    }
    public void SetFogSprite(Sprite sprite)
    {
        _fogImage.sprite = sprite;

    }
}
