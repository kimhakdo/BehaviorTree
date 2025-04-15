using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Ironcow.BT;
using System.Reflection;
using System.Linq.Expressions;
using Ironcow;

[Serializable]
public class BTRunner
{
    [SerializeReference] public RootNode root;
    private BTSaveData data;
#if UNITY_EDITOR
    public BTSaveData Data => data;
#endif
    public BTRunner()
    {
        if(root == null)
            root = new RootNode();

    }

    public BTRunner(string parentName)
    { 
        data = ResourceManager.Instance.LoadAsset<BTSaveData>($"{parentName}Data", ResourceType.Data);
        root = JsonUtility.FromJson<RootNode>(data.data);
    }

    public BTRunner SetActions<T>(T parent)
    {
        var methods = typeof(T).GetMethods();
        methods.ToList().RemoveAll(obj => obj.ReturnType != typeof(eNodeState));
        FindAction<T>(root.node, parent, methods);
        return this;
    }

    public void FindAction<T>(BTNode node, T parent, MethodInfo[] methods)
    {
        if (node.GetRealType() == typeof(SelectorNode))
        {
            foreach (var nod in ((SelectorNode)node).childs)
            {
                FindAction(nod, parent, methods);
            }
        }
        else if (node.GetRealType() == typeof(SequenceNode))
        {
            foreach (var nod in ((SequenceNode)node).childs)
            {
                FindAction(nod, parent, methods);
            }
        }
        else if (node.GetRealType() == typeof(ActionNode))
        {
            var actionNode = (ActionNode)node;
            actionNode.SetFunc(GetMethod(actionNode.funcName, methods, parent));
        }
    }

    public Func<eNodeState> GetMethod<T>(string funcName, MethodInfo[] methods, T parent)
    {
        var method = methods.ToList().Find(obj => obj.Name == funcName);
        return Expression.Lambda<Func<eNodeState>>(
            Expression.Call(Expression.Constant(parent), method), method.Name, true, null).Compile();
    }

    public BTNode AddSelector()
    {
        return root.AddSelector();
    }

    public BTNode AddAction(Func<eNodeState> func)
    {
        return root.AddAction(func);
    }

    public BTNode AddSequence()
    {
        return root.AddSequence();
    }

    public void Operate()
    {
        root.Evaluate();
    }
}