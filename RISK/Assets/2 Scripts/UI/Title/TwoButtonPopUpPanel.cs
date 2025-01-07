using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoButtonPopupPanel : UIPopup
{
    public Button okButton;
    private bool isOk = false;

    protected override void Awake()
    {
        base.Awake();
        okButton.onClick.AddListener(OkButtonClick);
    }
    public void SetPopup(string title, string message, Action<bool> callback)
    {
        base.SetPopup(title, message, () => callback?.Invoke(isOk));
    }

    public void OkButtonClick()
    {
        isOk = true;
        callback?.Invoke();
        PanelManager.Instance.PopupClose();
    }

    protected override void CloseButtonClick()
    {
        isOk = false;
        callback?.Invoke();
        PanelManager.Instance.PopupClose();
    }
}
