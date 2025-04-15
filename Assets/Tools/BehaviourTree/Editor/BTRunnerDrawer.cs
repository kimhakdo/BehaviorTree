using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Ironcow.BT;
using System.Reflection;
using System.Linq.Expressions;
using System.IO;
using Ironcow;


[CustomPropertyDrawer(typeof(BTRunner))]
public class BTRunnerDrawer : PropertyDrawer
{
    BTRunner instance;
    bool isOpened;
    List<MethodInfo> methods;
    SerializedProperty property;
    BTSaveData data;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (this.property == null)
        {
            this.property = property;
        }
        if (instance == null)
        {
            instance = (BTRunner)this.fieldInfo.GetValue(property.serializedObject.targetObject);
            if (instance == null)
            {
                instance = new BTRunner();
                this.fieldInfo.SetValue(property.serializedObject.targetObject, instance);
            }
            else
            {
                var fieldInfo = instance.GetType().GetField("data", BindingFlags.NonPublic | BindingFlags.Instance);
                //data = (BTSaveData)fieldInfo.GetValue(instance);
            }
        }
        if (methods == null)
        {
            methods = this.fieldInfo.DeclaringType.GetMethods().ToList();
            methods.RemoveAll(obj => obj.ReturnType != typeof(eNodeState));
        }
        if (data == null)
        {
            LoadData();
        }
        //EditorGUI.PropertyField(position, property, label, true);
        isOpened = EditorGUI.Foldout(position, isOpened, "BTRunner");
        if (isOpened)
        {
            DrawNode(instance.root, position);
        }
        property.serializedObject.ApplyModifiedProperties();
    }

    public void SaveData()
    {
        if (Application.isPlaying) return;
        if (instance.root != null)
            data.SaveData(instance.root);
        EditorUtility.SetDirty(data);
    }

    bool isDialog = false;
    public void LoadData()
    {
        if (isDialog) return;
        if (BTEditor.instance.savePath == null)
        {
            Selection.activeObject = BTEditor.instance;
            isDialog = EditorUtility.DisplayDialog("경고", "BTEditor의 SavePath를 등록해주세요", "ok");
            return;
        }
        var name = $"{property.serializedObject.targetObject.name.Replace("(Clone)", "")}Data.asset";
        var path = Path.Combine(BTEditor.SavePath, name);
        data = AssetDatabase.LoadAssetAtPath<BTSaveData>(path);
        if (data == null)
        {
            if (!AssetDatabase.IsValidFolder(BTEditor.SavePath))
            {
                AssetDatabase.CreateFolder(BTEditor.ParentPath, "BTSaveData");
            }
            var dt = ScriptableObject.CreateInstance<BTSaveData>();
            AssetDatabase.CreateAsset(dt, path);
            Debug.Log("Create Complete");
            data = AssetDatabase.LoadAssetAtPath<BTSaveData>(path);
        }

        if (!Application.isPlaying)
            instance.root = JsonUtility.FromJson<RootNode>(data.data);
        if (instance.root == null)
            instance.root = new RootNode();
    }

    public ReorderableList DrawNode(BTNode node, Rect position)
    {
        if (node == null) return null;
        ReorderableList rl = null;
        var nodes = new List<BTNode>();
        var type = node.GetType().UnderlyingSystemType;
        EditorGUI.indentLevel++;
        if (type == typeof(SelectorNode))
        {
            var nd = (SelectorNode)node;
            rl = SetList(type.Name.ToString(), nd, nd.childs, type, position);
        }
        else if (type == typeof(SequenceNode))
        {
            var nd = (SequenceNode)node;
            rl = SetList(type.Name.ToString(), nd, nd.childs, type, position);
        }
        else if (type == typeof(RootNode))
        {
            var nd = (RootNode)node;
            rl = SetList(type.Name.ToString(), nd, new List<BTNode>() { nd.node }, type, position);
            EditorGUILayout.Space(rl.GetHeight());
        }
        else if (type == typeof(ActionNode))
        {
            DrawAction(type.Name.ToString(), (ActionNode)node, position);
        }
        EditorGUI.indentLevel--;
        return rl;
    }

    public float DrawAction(string label, ActionNode node, Rect position)
    {
        int width = 60;
        EditorGUI.LabelField(new Rect(position.x, position.y, width, position.height), label.Replace("Node", ""));
        var content = new GUIContent(node.onUpdate == null ? "<none>" : node.onUpdate.Method.Name);
        if (node.onUpdate == null && !string.IsNullOrEmpty(node.funcName))
        {
            var method = methods.Find(obj => obj.Name == node.funcName);
            node.SetFunc(Expression.Lambda<Func<eNodeState>>(
                Expression.Call(Expression.Constant(property.serializedObject.targetObject), method), method.Name, true, null).Compile());
        }
        if (EditorGUI.DropdownButton(new Rect(position.x + width, position.y, position.width - width, position.height), content, FocusType.Passive))
        {
            GenericMenu menuClass = new GenericMenu();
            foreach (var method in methods)
            {
                var isEquip = node.onUpdate != null && node.onUpdate.Method == method;
                menuClass.AddItem(new GUIContent(method.Name), isEquip, (dt) =>
                {
                    node.SetFunc(Expression.Lambda<Func<eNodeState>>(
                        Expression.Call(Expression.Constant(property.serializedObject.targetObject), method), method.Name, true, null).Compile());
                    SaveData();
                }, method.Name);
            }
            menuClass.ShowAsContext();
        }
        return 30;
    }

    public ReorderableList SetList(string label, BTNode node, List<BTNode> nodes, Type type, Rect position)
    {
        ReorderableList rlist = new ReorderableList(nodes, type, false, true, true, true);
        rlist.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, label.Replace("Node", ""));
        };
        rlist.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            var list = DrawNode(nodes[index], rect);
            if (list != null)
            {
                rlist.elementHeight += list.GetHeight();
            }
        };
        rlist.elementHeightCallback += index =>
        {
            return GetHeight(nodes[index]);
        };
        rlist.onAddCallback = lt =>
        {
            var type = node.GetType().UnderlyingSystemType;
            RootNode rn = null;
            if (type == typeof(RootNode))
            {
                rn = (RootNode)node;

            }
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Selector"), false, (nm) =>
            {
                if (rn != null) rn.node = new SelectorNode(rn);
                else nodes.Add(new SelectorNode(node));
                SaveData();
            }, "Selector");
            menu.AddItem(new GUIContent("Sequence"), false, (nm) =>
            {
                if (rn != null) rn.node = new SequenceNode(rn);
                else nodes.Add(new SequenceNode(node));
                SaveData();
            }, "Sequence");
            menu.AddItem(new GUIContent("Action"), false, (nm) =>
            {
                if (rn != null) rn.node = new ActionNode(rn);
                else nodes.Add(new ActionNode(node));
                SaveData();
            }, "Action");
            menu.ShowAsContext();
        };
        rlist.onRemoveCallback = lt =>
        {
            nodes.RemoveAt(nodes.Count - 1);
            SaveData();
        };
        rlist.DoList(position);
        return rlist;
    }

    public float GetHeight(List<BTNode> nodes)
    {
        var height = 0f;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i] == null) break;
            var type = nodes[i].GetType().UnderlyingSystemType;
            if (type == typeof(SelectorNode))
            {
                var nd = (SelectorNode)nodes[i];
                if (nd.childs.Count == 0) height += 70;
                else height += GetHeight(nd.childs) + 60;
            }
            else if (type == typeof(SequenceNode))
            {
                var nd = (SequenceNode)nodes[i];
                if (nd.childs.Count == 0) height += 70;
                else height += GetHeight(nd.childs) + 60;
            }
            else if (type == typeof(ActionNode))
            {
                var nd = (ActionNode)nodes[i];
                height += 15;
            }
        }
        if (height == 0) height = 20;
        return height;
    }

    public float GetHeight(BTNode node)
    {
        var height = 0f;
        if (node == null) return 20;
        var type = node.GetType().UnderlyingSystemType;
        if (type == typeof(SelectorNode))
        {
            var nd = (SelectorNode)node;
            if (nd.childs.Count == 0) height += 70;
            else height += GetHeight(nd.childs) + 60;
        }
        else if (type == typeof(SequenceNode))
        {
            var nd = (SequenceNode)node;
            if (nd.childs.Count == 0) height += 70;
            else height += GetHeight(nd.childs) + 60;
        }
        else if (type == typeof(ActionNode))
        {
            var nd = (ActionNode)node;
            height += 15;
        }
        if (height == 0) height = 20;
        return height;
    }
}