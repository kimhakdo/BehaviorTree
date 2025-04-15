using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UniRx;

public class PopupStatus : UIBase
{
    [SerializeField] private Text atk;
    [SerializeField] private Text def;
    [SerializeField] private Text hp;
    [SerializeField] private Text cri;

    public override void Opened(object[] param)
	{
        var info = GameManager.Instance.userCharacter.info;
        info.atk.Subscribe(SetAtk).AddTo(gameObject);
        info.def.Subscribe(SetDef).AddTo(gameObject);
        info.hp.Subscribe(SetHp).AddTo(gameObject);
        info.cri.Subscribe(SetCri).AddTo(gameObject);
    }

	public override void HideDirect()
	{
		UIManager.Hide<PopupStatus>();
	}

    public void SetAtk(int atk)
    {
        this.atk.text = atk.ToString();
    }

    public void SetHp(int hp)
    {
        this.hp.text = hp.ToString();
    }

    public void SetDef(int def)
    {
        this.def.text = def.ToString();
    }

    public void SetCri(int cri)
    {
        this.cri.text = cri.ToString();
    }
}
