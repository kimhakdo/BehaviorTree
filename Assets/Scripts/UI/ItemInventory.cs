using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private Text qty;
    [SerializeField] private GameObject equipMark;

    private ItemInfo itemData;
    private Action<ItemInfo> action;

    public void Init(ItemInfo itemData, int qty, Action<ItemInfo> action)
    {
        this.itemData = itemData;
        this.action = action;
        thumbnail.sprite = ResourceManager.Instance.LoadAsset<Sprite>(itemData.rcode, ResourceType.Thumbnail);
        this.qty.text = qty.ToString();
        gameObject.SetActive(true);
        equipMark.SetActive(itemData.isEquipped);
    }

    public void OnClickItem(bool isOn)
    {
        action?.Invoke(this.itemData);
    }
}
