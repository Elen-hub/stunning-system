using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstOrderInputHandler : BaseInputHandler
{
    public bool BlockInput;
    void OnOpenOption()
    {
        if (IsKeyDown(eInputType.Option))
        {

        }
    }
    void OnChattingInput()
    {
        if (IsKeyDown(eInputType.Chatting))
        {
            if(UIManager.Instance.IsOpen(eUIName.UChatting))
            {
                if (UIManager.Instance.Get<UChatting>(eUIName.UChatting).ActiveTextField)
                {
                    BlockInput = false;
                    UIManager.Instance.Get<UChatting>(eUIName.UChatting).SendMessage();
                    UIManager.Instance.Get<UChatting>(eUIName.UChatting).ActiveTextField = false;
                }
                else
                {
                    BlockInput = true;
                    UIManager.Instance.Get<UChatting>(eUIName.UChatting).Enable();
                    UIManager.Instance.Get<UChatting>(eUIName.UChatting).ActiveTextField = true;
                }
            }
        }
    }
    protected override void OnUpdate()
    {
        _currentInputHash.Clear();
        OnOpenOption();
        OnChattingInput();
    }
}
