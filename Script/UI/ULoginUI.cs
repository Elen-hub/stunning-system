using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ULoginUI : BaseUI
{
    TMP_InputField _guidField;
    TMP_InputField _ipField;
    TMP_InputField _nameField;
    protected override void InitReference()
    {
        base.InitReference();

        _guidField = transform.Find("Input_GUID").GetComponent<TMP_InputField>();
        _ipField = transform.Find("Input_IP").GetComponent<TMP_InputField>();
        _nameField = transform.Find("Input_Name").GetComponent<TMP_InputField>();
        Button btn = transform.Find("Btn_Join").GetComponent<Button>();
        btn.onClick.AddListener(OnClickJoin);
    }
    protected override void OnOpen()
    {
        _guidField.text = SystemInfo.deviceUniqueIdentifier;
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }
    void OnClickJoin()
    {
        Player player = new Player();
        player.Guid = _guidField.text;
        player.PlayerName = _nameField.text;
        player.PlayerCharacterData = new PlayerCharacterData()
        {
            Index = 1,
            Name = _nameField.text,
        };
        PlayerManager.Instance.Me = player;

        NetworkManager.Instance.JoinRoom(_ipField.text, 27015);

        InputManager.Instance.Initialize();
        UIManager.Instance.Open(eUIName.UChatting);
        Emoticon.StaticInitialize();
    }
}
