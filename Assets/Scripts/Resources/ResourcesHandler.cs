using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourcesHandler
{
    public bool isInit;

    public void Init()
    {
        isInit = true;
    }

    public T LoadAsset<T>(string key, string type) where T : UnityEngine.Object
    {
        var asset = Resources.Load<T>($"{type}/{key}");
        return asset;
    }

    public List<T> LoadAssets<T>(string type) where T : UnityEngine.Object
    {
        return Resources.LoadAll<T>(type + "/").ToList();
    }
}
