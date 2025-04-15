using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class SheetData
{
    public string className;
    public string sheetId;
    public string key;
    public List<Dictionary<string, string>> datas;
}

public class SheetInfoSO : SOSingleton<SheetInfoSO>
{
    public Object outPath;
#if UNITY_EDITOR
    public string OutPath { get => AssetDatabase.GetAssetPath(outPath); }
#endif
    public string url;
    public List<SheetData> datas;
}