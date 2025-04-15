using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableHandler
{
    public bool isInit = false;

    public string GetPath(string key, string type)
    {
        return Catalog.instance.Find(key, type);
    }

    public void Init()
    {
        Addressables.DownloadDependenciesAsync("InitDownload").Completed += (oper) => {
            Debug.Log("다운로드 완료");
            isInit = true;
            Addressables.Release(oper);
        };
    }

    public string GetExtension(string type)
    {
        switch(type)
        {
            case ResourceType.Data:
                {
                    return Extension.Asset;
                }
            case ResourceType.Prefab:
                {
                    return Extension.Prefab;
                }
            case ResourceType.Thumbnail:
                {
                    return Extension.Png;
                }
            case ResourceType.UI:
                {
                    return Extension.Prefab;
                }
        }
        return "";
    }

    public T LoadAsset<T>(string key, string type) where T : UnityEngine.Object
    {
        //var path = $"{type}/{key}{GetExtension(type)}";
        var path = GetPath(key, type);
        try
        {
            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var obj = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
                return obj.GetComponent<T>();
            }
            else
                return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return default;
    }

    public List<T> LoadAssets<T>(string label) where T : UnityEngine.Object
    {
        var list = Addressables.LoadAssetsAsync<T>(label, (obj) => {
            Debug.Log(obj.ToString());
        }).WaitForCompletion();
        List<T> assets = list != null ? new List<T>(list) : new List<T>(); 
        return assets;
    }
}
