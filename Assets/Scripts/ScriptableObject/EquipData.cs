using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eEquipType
{
    weapon,
    shield,
    armor,
    helm,
}

public class EquipData : BaseData
{
    public eEquipType equipType;
    public int atk;
    public int def;
    public string desc
    {
        get
        {
            return atk > 0 ? $"���ݷ� +{atk}" : $"���� +{def}";
        }
    }
}
