using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupAlert : UIBase
{
    public Text title;
    public Text desc;
    public Button button;
    public UnityAction okAction;

    public override void Opened(params object[] param)
    {
        title.text = (string)param[0];
        desc.text = (string)param[1];
        okAction = (UnityAction)param[2];
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupAlert>();
    }

    public void OnClickClose()
    {
        okAction?.Invoke();
    }
}
