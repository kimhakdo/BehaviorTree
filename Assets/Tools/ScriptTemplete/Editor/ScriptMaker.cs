using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptMaker : Editor
{
    [MenuItem("Assets/Create/Script/Test Script", false, 1)]
    static void CreateMonoScript()
    {
        string templetePath = Application.dataPath.Replace("Assets", "Assets/Tools/ScriptTemplete/Template/");
        var template = templetePath + "TestTemplete.cs.txt";
        var dest = "Test.cs";
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
    }
    [MenuItem("Assets/Create/Script/UIBase Script", false, 1)]
    static void CreateUIBaseScript()
    {
        string templetePath = Application.dataPath.Replace("Assets", "Assets/Tools/ScriptTemplete/Template/");
        var template = templetePath + "UIBaseTemplete.cs.txt";
        var dest = "UI.cs";
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(template, dest);
    }
}
