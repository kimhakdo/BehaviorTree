using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceType
{
    public const string Data = "Data";
    public const string Prefab = "Prefab";
    public const string Thumbnail = "Thumbnail";
    public const string UI = "UI";
}

public class Extension
{
    public const string Prefab = ".prefab";
    public const string Asset = ".asset";
    public const string Png = ".png";
    public const string Jpg = ".jpg";

}

public class ResourceManager : Singleton<ResourceManager>
{
    public Dictionary<string, Object> pool = new Dictionary<string, Object>();
#if !USE_ADDRESSABLE
    public ResourcesHandler handler = new ResourcesHandler();
#else
    public AddressableHandler handler = new AddressableHandler();
#endif
    public bool isInit => handler.isInit;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    public void Init()
    {
        handler.Init();
    }

    public T LoadAsset<T>(string key, string type) where T : UnityEngine.Object
    {
        if (pool.TryGetValue(key, out var obj))
        {
            return (T)obj;
        }
        else
        {
            var asset = handler.LoadAsset<T>(key, type);
            if (asset != null)
                pool.Add(key, asset);
            return asset;
        }
    }

    public List<T> LoadAssets<T>(string type) where T : UnityEngine.Object
    {
        List<T> retList = new List<T>();
        var keys = pool.Keys.ToList().FindAll(obj => obj.Contains(type));
        if(keys.Count > 0)
        {
            foreach(var newKey in keys)
            {
                retList.Add((T)pool[newKey]);
            }
        }
        else
        {
            retList = handler.LoadAssets<T>(type);
        }
        return retList;
    }

    public T InstantiateAsset<T>(string key, string type, Transform parent = null) where T : Object
    {
        var asset = Instantiate(LoadAsset<T>(key, type), parent);
        return asset;
    }
}
