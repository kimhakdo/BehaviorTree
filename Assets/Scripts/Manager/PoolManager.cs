using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PoolData
{
    public string key;
    public PoolBase gameObject;
    public int capacity;
    public int interval;
}

public class PoolManager : Singleton<PoolManager>
{
    public List<PoolData> poolDatas;
    private Dictionary<string, Queue<PoolBase>> poolDic = new Dictionary<string, Queue<PoolBase>>();
 
    private void Start() 
    {
        foreach (var poolData in poolDatas)
        {
            CreateAsset(poolData.key, poolData.capacity, poolData.gameObject);
        }
    }

    public void CreateAsset(string key, int count, PoolBase prefab)
    {
        if (!poolDic.ContainsKey(key))
        {
            poolDic.Add(key, new Queue<PoolBase>());
        }
        var parent = transform.Find($"{key}Parent");
        if (parent == null)
        {
            parent = new GameObject($"{key}Parent").transform;
        }
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            poolDic[key].Enqueue(obj);
        }
    }

    public T Spawn<T>(Transform parent = null) where T : PoolBase
    {
        var key = typeof(T).ToString();
        if (poolDic[key].Count == 0)
        {
            var data = poolDatas.Find(obj => obj.key == key);
            CreateAsset(key, data.interval, data.gameObject);
        }
        var obj = poolDic[key].Dequeue();
        obj.transform.parent = parent;
        obj.gameObject.SetActive(true);
        return (T)obj;
    }

    public void Release<T>(T obj) where T : PoolBase
    {
        var key = typeof(T).ToString();
        var parent = transform.Find($"{key}Parent");
        obj.transform.parent = parent;
        obj.gameObject.SetActive(false);
        poolDic[key].Enqueue(obj);
    }
}
