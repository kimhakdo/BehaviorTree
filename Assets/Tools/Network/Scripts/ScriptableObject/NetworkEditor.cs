using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NetworkEditorSO : SOSingleton<NetworkEditorSO>
{
	public string base_url;
	public int base_port;

	public Object targetFolder;
#if UNITY_EDITOR
	public static string TargetPath { get => AssetDatabase.GetAssetPath(instance.targetFolder); }
#endif
	public string apiName;
	public string clsasName;
    public Dictionary<string, string> parameters = new Dictionary<string, string>();
    public Dictionary<string, string> receieves = new Dictionary<string, string>();
}