using Ironcow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CustomEditor(typeof(SheetInfoSO))]
public class SheetInfoSOEditor : Editor
{
    [MenuItem("Ironcow/DataTool")]
    public static void Edit()
    {
        SheetInfoSO.Edit();
    }

    SheetInfoSO instance;
    public override void OnInspectorGUI()
    {
        instance = (SheetInfoSO)target;

        base.OnInspectorGUI();

        if(GUILayout.Button("Download Datas"))
        {
            DownloadData(instance.datas);
        }
    }


    public async void DownloadData(List<SheetData> sheets)
    {
        foreach (var sheet in sheets)
        {
            var url = $"{instance.url}export?format=tsv&gid={sheet.sheetId}";
            var req = UnityWebRequest.Get(url);
            var op = req.SendWebRequest();
            Debug.Log($"{sheet.className}");
            await op;
            var res = req.downloadHandler.text;
            Debug.Log(res);
            sheet.datas = TSVParser.TsvToDic(res);
        }
        ImportDatas(sheets);
    }

    protected void ImportDatas(List<SheetData> sheets)
    {
        foreach (var sheet in sheets)
        {
            ImportData(sheet);
        }
    }

    protected void ImportData(SheetData sheet)
    {
        Assembly assembly = typeof(BaseData).Assembly;
        var type = assembly.GetType(sheet.className);
        GetDatas(type, sheet.datas);
    }

    public void GetDatas(Type type, List<Dictionary<string, string>> datas)
    {
            foreach (var data in datas)
            {
                var path = instance.OutPath + "/" + data["rcode"] + ".asset";
                var dt = (ScriptableObject)AssetDatabase.LoadAssetAtPath(path, type);
                if (dt == null)
                {
                    dt = DicToClass(type, data);
                }
                else
                {

                    dt = TSVParser.DicToSOData(type, dt, data);
                }

                EditorUtility.SetDirty(dt);
                AssetDatabase.SaveAssets();
            }
    }

    public ScriptableObject DicToClass(Type type, Dictionary<string, string> data)
    {
        var dt = CreateInstance(type);
        AssetDatabase.CreateAsset(dt, instance.OutPath + "/" + data["rcode"] + ".asset");
        return TSVParser.DicToSOData(type, dt, data);
    }
}
