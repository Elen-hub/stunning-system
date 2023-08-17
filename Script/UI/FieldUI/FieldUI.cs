using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EFieldUIType
{
    DamageText,
    MessageBox,
    NameTag,
    FieldText,
}
public class FieldUI : BaseUI
{
    Transform _cameraTransform;
    Dictionary<EFieldUIType, ObjectMemoryPool<BaseFieldUI>> _ObjectMemoryPoolDic = new Dictionary<EFieldUIType, ObjectMemoryPool<BaseFieldUI>>(2);
    Dictionary<int, FieldUI_MessageBox> _messageBoxDic = new Dictionary<int, FieldUI_MessageBox>();
    //public void SetHPBar(Actor target)
    //{
    //    FindFieldUI<FieldUI_HPBar>(EFieldUIType.HPBar).Enabled(target);
    //}
    // Dictionary<int, >
    protected override void InitReference()
    {
        base.InitReference();

        PlayerEventReceiver.MessageUIEvent += SetMessageBox;
        _ObjectMemoryPoolDic.Add(EFieldUIType.DamageText, new ObjectMemoryPool<BaseFieldUI>(20));
        _ObjectMemoryPoolDic.Add(EFieldUIType.MessageBox, new ObjectMemoryPool<BaseFieldUI>(20));
        _ObjectMemoryPoolDic.Add(EFieldUIType.NameTag, new ObjectMemoryPool<BaseFieldUI>(20));
        _ObjectMemoryPoolDic.Add(EFieldUIType.FieldText, new ObjectMemoryPool<BaseFieldUI>(10));
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = CameraManager.Instance.GetCamera(eCameraType.MainCamera).Camera;
        canvas.planeDistance = 1;
        canvas.sortingLayerName = "FieldUI";
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = Screen.width / Screen.height;
        _cameraTransform = CameraManager.Instance.GetCamera(eCameraType.MainCamera).transform;
    }
    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }
    void OnRemoveMessageBoxHash(int id)
    {
        if (_messageBoxDic.ContainsKey(id))
            _messageBoxDic.Remove(id);
    }
    public void SetDamageText(Vector3 pos, double damage, bool isCritical)
    {
        FindFieldUI<FieldUI_DamageText>(EFieldUIType.DamageText).Enabled(pos, damage, isCritical);
    }
    public void SetMessageBox(MessageStructure message)
    {
        if (message.Type != eMessageType.Normal) return;
        FieldUI_MessageBox messageBox = null;
        if (_messageBoxDic.ContainsKey(message.PlayerID))
        {
            messageBox = _messageBoxDic[message.PlayerID];
        }
        else
        {
            messageBox = FindFieldUI<FieldUI_MessageBox>(EFieldUIType.MessageBox);
            messageBox.DisableCallback += OnRemoveMessageBoxHash;
            _messageBoxDic.Add(message.PlayerID, messageBox);
        }
        messageBox.Enable(message);
    }
    public void SetNameTag(IRegistFieldUI owner)
    {
        FieldUI_NameTag nameTagUI = FindFieldUI<FieldUI_NameTag>(EFieldUIType.NameTag);
        nameTagUI.Enable(owner);
    }
    public void SetFieldText(IActor owner, int localizingKey, params object[] parsingParameters)
    {
        FieldUI_FieldText fieldTextUI = FindFieldUI<FieldUI_FieldText>(EFieldUIType.FieldText);
        fieldTextUI.Enable(owner, localizingKey, parsingParameters);
    }
    public void SetFieldText(IActor owner, string log)
    {
        FieldUI_FieldText fieldTextUI = FindFieldUI<FieldUI_FieldText>(EFieldUIType.FieldText);
        fieldTextUI.Enable(owner, log);
    }
    T FindFieldUI<T>(EFieldUIType type) where T : BaseFieldUI
    {
        T ui = _ObjectMemoryPoolDic[type].GetItem() as T;
        if (ui != null)
            return ui;

        ui = Instantiate(Resources.Load<T>("UI/FieldUI/" + type.ToString()), transform);
        ui.Init(_ObjectMemoryPoolDic[type].Register);
        return ui;
    }
    private void Update()
    {
        // transform.position = _cameraTransform.position;
    }
}
