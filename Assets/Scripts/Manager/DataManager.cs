using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<string, BaseData> datas = new Dictionary<string, BaseData> ();
    public bool isInit = false;

    protected override void Awake()
    { 
        base.Awake();
        StartCoroutine(init()); 
    }
    protected IEnumerator init()
    {
        yield return new WaitUntil(() => ResourceManager.Instance.isInit);
        var datas = ResourceManager.Instance.LoadAssets<BaseData>(ResourceType.Data);
        foreach(var data in datas)
        {
            this.datas.Add(data.name, data);
        }
        isInit = true;
    }

    public List<BaseData> GetMonsterDatas()
    {
        var datas = new List<BaseData>(this.datas.Values);
        return datas.FindAll(obj => obj.rcode.Contains("ENE"));
    }

    public T GetData<T>(string rcode) where T : BaseData
    {
        return (T)datas[rcode];
    }

    public List<BaseData> GetDatas<T>() where T : BaseData
    {
        var datas = new List<BaseData>(this.datas.Values);
        return datas.FindAll(obj => obj.GetType().IsAssignableFrom(typeof(T)));
    }
}
