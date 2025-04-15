using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : BaseData
{
    public string title;
    public int hp;
    public int atk;
    public int def;
    public int atkPerLv;
    public int defPerLv;
    public int hpPerLv;
}
