using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelExp
{
    public int level;
    public int exp;
}

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : BaseData
{
    public List<LevelExp> expList;
}
