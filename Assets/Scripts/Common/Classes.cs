
using System.Collections.Generic;
using UniRx;
using UnityEditor.PackageManager;
using UnityEngine;

public class BaseInfo
{
    protected BaseData data;
    public string rcode => data.rcode;
}

public class CharacterInfo : BaseInfo
{
    public CharacterData Data => (CharacterData)data;

    public string title;

    public int maxHp;
    public ReactiveProperty<int> hp = new ReactiveProperty<int>();
    public ReactiveProperty<int> atk = new ReactiveProperty<int>();
    public ReactiveProperty<int> def = new ReactiveProperty<int>();
    public ReactiveProperty<int> cri = new ReactiveProperty<int>();

    public ReactiveProperty<int> lv = new ReactiveProperty<int>();
    public ReactiveProperty<int> exp = new ReactiveProperty<int>();
    public int maxExp;

    public ReactiveProperty<int> gold = new ReactiveProperty<int>();

    public List<ItemInfo> inventory = new List<ItemInfo>();
    public Dictionary<int, EquipInfo> equips = new Dictionary<int, EquipInfo>();
    public CharacterInfo(CharacterData data)
    {
        this.data = data;
        maxHp = data.hp;
        this.hp.Value = data.hp;
        atk.Value = data.atk;
        def.Value = data.def;
        cri.Value = 10;
        SetLevel(1);

        inventory.Add(new ItemInfo(DataManager.Instance.GetData<ItemData>("ITE00003")));
        inventory.Add(new ItemInfo(DataManager.Instance.GetData<ItemData>("ITE00006")));
    }

    public void Equip(ItemInfo item)
    {
        var equip = new EquipInfo(DataManager.Instance.GetData<EquipData>(item.Data.equipPrefab));
        equip.itemInfo = item;
        var pos = (int)equip.Data.equipType;
        Unequip(pos);
        this.atk.Value += equip.Data.atk;
        this.def.Value += equip.Data.def;
        item.isEquipped = true;
        if (equips.ContainsKey(pos))
        {
            equips[pos] = equip;
        }
        else
        {
            equips.Add(pos, equip);
        }
    }

    public void Unequip(int pos)
    {
        if(equips.TryGetValue(pos, out var equip))
        {
            equip.itemInfo.isEquipped = false;
            this.atk.Value -= equip.Data.atk;
            this.def.Value -= equip.Data.def;
            equips.Remove(pos);
        }
    }
    
    public void UseItem(ItemInfo item)
    {
        item.qty--;
        hp.Value = Mathf.Min(hp.Value + item.Data.hp, maxHp);
        if (item.qty <= 0)
        {
            inventory.Remove(item);
        }
    }

    public void AddItem(string rcode)
    {
        var data = DataManager.Instance.GetData<ItemData>(rcode);
        if(data.canStack)
        {
            var items = inventory.FindAll(obj => obj.Data.rcode == rcode);
            bool isAdd = false;
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].qty >= items[i].Data.maxStackAmount)
                {
                    continue;
                }
                else
                {
                    items[i].qty++;
                    isAdd = true;
                    break;
                }
            }
            if (!isAdd)
            {
                inventory.Add(new ItemInfo(data));
            }
        }
        else
        {
            inventory.Add(new ItemInfo(data));
        }
    }

    public void SetLevel(int level)
    {
        var dif = level - lv.Value;
        if (lv.Value == 0) dif = 0;
        lv.Value = level;
        var dt = DataManager.Instance.GetData<LevelData>("LevelData").expList.Find(obj => obj.level == lv.Value);
        maxExp = dt.exp;
        atk.Value += Data.atkPerLv * dif;
        def.Value += Data.atkPerLv * dif;
        maxHp += Data.hpPerLv * dif;
        this.hp.Value = maxHp;
    }

    public void AddExp(int exp)
    {
        this.exp.Value += exp;
        if(this.exp.Value >= maxExp && maxExp > 0)
        {
            this.exp.Value -= maxExp;
            SetLevel(lv.Value + 1);
        }
    }
}


public class EnemyInfo : BaseInfo
{
    public EnemyData Data => (EnemyData)data;
    public int hp;

    public EnemyInfo(EnemyData data)
    {
        this.data = data;
        this.hp = data.hp;
    }
}


public class ItemInfo : BaseInfo
{
    public ItemData Data => (ItemData)data;
    public bool isEquipped;
    public int qty;
    public ItemInfo(ItemData data)
    {
        this.data = data;
        qty = 1;
    }
}

public class EquipInfo : BaseInfo
{
    public EquipData Data => (EquipData)data;
    public ItemInfo itemInfo;
    public EquipInfo(EquipData data)
    {
        this.data = data;
    }
}