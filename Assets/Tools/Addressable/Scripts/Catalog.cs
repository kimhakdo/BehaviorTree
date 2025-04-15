using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CatalogData
{
    public string key;
    public string type;
    public string address;
}

public class Catalog : SOSingleton<Catalog>
{
    public List<CatalogData> catalogs = new List<CatalogData>();

    public void AddRange(List<CatalogData> list)
    {
        catalogs.AddRange(list);
    }

    public void Add(CatalogData category)
    {
        catalogs.Add(category);
    }

    public void Clear()
    {
        catalogs.Clear();
    }

    public string Find(string key, string type)
    {
        string path = "";
        var catalogData = catalogs.Find(obj => obj.key == key && type  == obj.type);
        path = catalogData == null ? "" : catalogData.address;
        return path;
    }

    public List<string> GetPaths()
    {
        List<string> paths = new List<string>();
        foreach(var data in catalogs)
        {
            paths.Add(data.address);
        }
        return paths;
    }
}
