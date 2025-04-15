using Ironcow.BT;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BTSaveData : ScriptableObject
{
    public string data;

    public void SaveData(RootNode node)
    {
        data = JsonUtility.ToJson(node);
        /*this.node = new BTRootData();
        if (node.node.GetRealType() == typeof(SelectorNode))
        {
            this.node.Add(AddData<BTRootData, BTSelectorData>(this.node, (SelectorNode)node.node));
        }
        else if (node.node.GetRealType() == typeof(SequenceNode))
        {
            this.node.Add(AddData<BTRootData, BTSequenceData>(this.node, (SequenceNode)node.node));
        }
        else if (node.node.GetRealType() == typeof(ActionNode))
        {
            this.node.Add(AddData<BTRootData, BTActionData>(this.node, ((ActionNode)node.node).funcName));
        }*/
    }

    public T AddData<B, T>(B node, List<BTNode> childs) where B : BTData where T : BTData
    {
        var instance = (T)Activator.CreateInstance(typeof(T));
        foreach (var child in childs)
        {
            if (child.GetRealType() == typeof(SelectorNode))
            {
                instance.Add(AddData<T, BTSelectorData>(instance, ((SelectorNode)child).childs));
            }
            else if (child.GetRealType() == typeof(SequenceNode))
            {
                instance.Add(AddData<T, BTSequenceData>(instance, ((SequenceNode)child).childs));
            }
            else if (child.GetRealType() == typeof(ActionNode))
            {
                instance.Add(AddData<T, BTActionData>(instance, ((ActionNode)child).funcName));
            }
        }
        return instance;
    }

    public T AddData<B, T>(B node, BTNode root) where B : BTData where T : BTData
    {
        var instance = (T)Activator.CreateInstance(typeof(T));
        if (root.GetRealType() == typeof(SelectorNode))
        {
            instance.Add(AddData<T, BTSelectorData>(instance, ((SelectorNode)root).childs));
        }
        else if (root.GetRealType() == typeof(SequenceNode))
        {
            instance.Add(AddData<T, BTSequenceData>(instance, ((SequenceNode)root).childs));
        }
        else if (root.GetRealType() == typeof(ActionNode))
        {
            instance.Add(AddData<T, BTActionData>(instance, ((ActionNode)root).funcName));
        }
        return instance;
    }

    public T AddData<B, T>(B node, string funcName) where B : BTData where T : BTData
    {
        var instance = Activator.CreateInstance<T>();
        instance.Add<T>(funcName);
        return instance;
    }

    //public BTNode LoadData()
    //{

    //}
}