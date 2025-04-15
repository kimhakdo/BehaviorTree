using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.Progress;
using UnityEngine.Events;

public class PopupShop : UIBase
{
    [SerializeField] private ItemShop itemPrefab;
    [SerializeField] private Transform listParent;

    private List<ItemShop> items = new List<ItemShop>();

    public override void Opened(object[] param)
    {
        SetList();
    }

    public override void HideDirect()
    {
        UIManager.Hide<PopupShop>();
    }

    public void SetList()
    {
        ClearList();
        var shopDatas = DataManager.Instance.GetDatas<ShopData>();
        for (int i = 0; i < shopDatas.Count; i++)
        {
            ItemShop item = AddItem();
            var key = shopDatas[i].rcode;
            item.Init((ShopData)shopDatas[i], OnClickItem);
        }
    }

    public ItemShop AddItem()
    {
        var item = Instantiate(itemPrefab, listParent);
        items.Add(item);
        return item;
    }

    public void ClearList()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
    }

    public void OnClickItem(ShopData data)
    {
        UnityAction act = () => UIManager.Hide<PopupAlert>();
        if (data == null)
        {
            UIManager.Show<PopupAlert>("경고", "아이템을 선택하세요.", act);
            return;
        }
        if (GameManager.Instance.userCharacter.info.gold.Value > data.price)
        {
            GameManager.Instance.userCharacter.info.gold.Value -= data.price;
            GameManager.Instance.userCharacter.info.AddItem(data.target);
        }
        else
        {
            UIManager.Show<PopupAlert>("경고", "골드가 부족합니다.", act);
        }
    }
}
