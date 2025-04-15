using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;

public class UIMain : UIBase
{
    [SerializeField] private Text name;
    [SerializeField] private Text lv;
    [SerializeField] private Text desc;
    [SerializeField] private Text title;
    [SerializeField] private Text gold;
    [SerializeField] private Text exp;
    [SerializeField] private Image expFill;

    public override void Opened(params object[] param)
    {
        var info = GameManager.Instance.userCharacter.info;
        name.text = info.Data.displayName;
        desc.text = info.Data.description;
        title.text = info.Data.title;
        info.exp.Subscribe(SetExp).AddTo(gameObject);
        info.lv.Subscribe(SetLevel).AddTo(gameObject);
        info.gold.Subscribe(SetGold).AddTo(gameObject);
        InputManager.Instance.Subscribe(KeyCode.Space, () =>
        {
            info.gold.Value += 100;
        });
    }

    public void SetMonsterInfo(string name, int hp)
    {
        GameManager.Instance.userCharacter.info.AddExp(1);
    }

    public void SetGold(int gold)
    {
        this.gold.text = $"{gold:#,##0}";// string.Format("{0:#,###}", gold);
    }

    public void SetExp(int exp)
    {
        var info = GameManager.Instance.userCharacter.info;
        this.exp.text = $"{exp}/{info.maxExp}";
        this.expFill.fillAmount = (float)exp / (float)info.maxExp;
    }

    public void SetLevel(int level)
    {
        this.lv.text = $"<size=45>lv</size><b>{level}</b>";
    }

    public override void HideDirect()
    {
        UIManager.Hide<UIMain>();
    }

    public void OnClickInventory()
    {
        UIManager.Show<PopupInventory>();
    }

    public void OnClickStatus()
    {
        UIManager.Show<PopupStatus>();
    }

    public void OnClickShop()
    {
        UIManager.Show<PopupShop>();
    }

    public void OnClickNetwork()
    {
        UIManager.Show<PopupNetworkTest>();
    }
}
