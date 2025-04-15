using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eItemType
{
    equip,
    usable,
    material,
}

public class ItemData : BaseData
{
    public eItemType type;
    public bool canStack;
    public int maxStackAmount;
    public int hp;
    public string equipPrefab;

    public string desc
    {
        get
        {
            if (!string.IsNullOrEmpty(equipPrefab))
            {
                return DataManager.Instance.GetData<EquipData>(equipPrefab).desc;
            }
            return hp == 99999 ? "Hp 최대 회복" : $"Hp {(Mathf.Sign(hp) > 0 ? "+" : "-")}{Mathf.Abs(hp)}";
        }
    }
}
