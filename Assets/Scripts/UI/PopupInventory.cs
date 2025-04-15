using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum eButtonType
{
    equip,
    unequip,
    usable
}

public class PopupInventory : UIBase
{
    [SerializeField] private ItemInventory itemPrefab;
    [SerializeField] private Transform listParent;
    [SerializeField] private Text itemName;
    [SerializeField] private Text itemDesc;
    [SerializeField] private Text count;
    [SerializeField] private List<GameObject> buttons;

    private List<ItemInventory> items = new List<ItemInventory>();
    private ItemInfo selectedItemInfo;

    public override void HideDirect()
    {
        UIManager.Hide<PopupInventory>();
    }

    public override void Opened(params object[] param)
    {
        SetList();
    }

    public void SetList()
    {
        ClearList();
        var inventory = GameManager.Instance.userCharacter.inventory;
        count.text = string.Format("<color=#EEA970>{0}</color> / 120", inventory.Count);
        for (int i = 0; i < inventory.Count; i++)
        {
            ItemInventory item = AddItem();
            var key = inventory[i].Data.rcode;
            item.Init(inventory[i], inventory[i].qty, OnClickItem);
        }
    }

    public ItemInventory AddItem()
    {
        var item = Instantiate(itemPrefab, listParent);
        items.Add(item);
        return item;
    }

    public void ClearList()
    {
        for(int i = 0; i < items.Count; i++)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
        this.itemName.text = "";
        this.itemDesc.text = "";
        buttons.ForEach(obj => obj.SetActive(false));
    }

    public void OnClickItem(ItemInfo itemInfo)
    {
        selectedItemInfo = itemInfo;
        this.itemName.text = itemInfo.Data.displayName;
        this.itemDesc.text = itemInfo.Data.description;
        buttons.ForEach(obj => obj.SetActive(false));
        switch (itemInfo.Data.type)
        {
            case eItemType.equip:
                {
                    if (itemInfo.isEquipped)
                    {
                        buttons[(int)eButtonType.unequip].SetActive(true);
                    }
                    else
                    {
                        buttons[(int)eButtonType.equip].SetActive(true);
                    }
                }
                break;
            case eItemType.usable:
                {
                    buttons[(int)eButtonType.usable].SetActive(true);
                }
                break;
        }
    }

    public void OnClickEquip()
    {
        GameManager.Instance.userCharacter.info.Equip(selectedItemInfo);
        SetList();
    }

    public void OnClickUnequip()
    {
        GameManager.Instance.userCharacter.info.Unequip((int)selectedItemInfo.Data.type);
        SetList();
    }

    public void OnClickUse()
    {
        GameManager.Instance.userCharacter.info.UseItem(selectedItemInfo);
        SetList();
    }
}
