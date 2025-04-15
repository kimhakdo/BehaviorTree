using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "BTEditor", menuName = "ScriptableObjects/BTEditor")]
public class BTEditor : SOSingleton<BTEditor>
{
	public Object savePath;

#if UNITY_EDITOR
    public static string ParentPath { get => AssetDatabase.GetAssetPath(instance.savePath); }
	public static string SavePath { get => Path.Combine(AssetDatabase.GetAssetPath(instance.savePath), "BTSaveData/"); }
#endif
}