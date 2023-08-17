using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UBulletMagazineUI : BaseUI
{
    Image _bulletImage;
    TextMeshProUGUI _bulletCountText;
    BulletMagazineItem _usingBulletMagzine;
    protected override void InitReference()
    {
        _bulletImage = transform.Find("Img_Bullet").GetComponent<Image>();
        _bulletCountText = transform.Find("Text_BulletCount").GetComponent<TextMeshProUGUI>();
    }
    public void SetReloadable(BulletMagazineItem bulletMagazine)
    {
        _usingBulletMagzine = bulletMagazine;
        _usingBulletMagzine.OnChangedBullet += OnChangedData;
        _bulletImage.sprite = DataManager.Instance.ItemTable[bulletMagazine.GetReloadSkill._combatData.RequireItemIndex].Icon;
        OnChangedData();
    }
    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }
    void OnChangedData()
    {
        _bulletCountText.text = $"{_usingBulletMagzine.ReloadedCount}/{_usingBulletMagzine.GetReloadSkill._combatData.MagCapacity}";
    }
}
