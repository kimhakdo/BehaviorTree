
using System;
using System.Collections.Generic;

namespace Ironcow.BT
{
    [Serializable]
    public abstract class BTData
    {
        public string funcName;

        public abstract void Add<T>(T data) where T : BTData;
        public abstract void Add<T>(string funcName) where T : BTData;
    }

    [Serializable]
    public class BTRootData : BTData
    {
        public BTData root;
        public override void Add<T>(T data)
        {
            root = data;
        }

        public override void Add<T>(string funcName)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class BTSelectorData : BTData
    {
        public List<BTData> childs;
        public BTSelectorData(List<BTNode> childs)
        {
            foreach (var node in childs)
            {
                if (node.GetRealType() == typeof(SelectorNode))
                {
                    this.childs.Add(new BTSelectorData(((SelectorNode)node).childs));
                }
                else if (node.GetRealType() == typeof(SequenceNode))
                {
                    this.childs.Add(new BTSequenceData(((SequenceNode)node).childs));
                }
                else
                {
                    this.childs.Add(new BTActionData(((ActionNode)node).funcName));
                }
            }
        }

        public override void Add<T>(T data)
        {
            childs.Add(data);
        }

        public override void Add<T>(string funcName)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class BTSequenceData : BTData
    {
        public List<BTData> childs;
        public BTSequenceData(List<BTNode> childs)
        {
            foreach (var node in childs)
            {
                if (node.GetRealType() == typeof(SelectorNode))
                {
                    this.childs.Add(new BTSelectorData(((SelectorNode)node).childs));
                }
                else if (node.GetRealType() == typeof(SequenceNode))
                {
                    this.childs.Add(new BTSequenceData(((SequenceNode)node).childs));
                }
                else
                {
                    this.childs.Add(new BTActionData(((ActionNode)node).funcName));
                }
            }
        }

        public override void Add<T>(T data)
        {
            childs.Add(data);
        }

        public override void Add<T>(string funcName)
        {
            throw new NotImplementedException();
        }
    }

    [Serializable]
    public class BTActionData : BTData
    {
        public BTActionData(string funcName)
        {
            this.funcName = funcName;
        }

        public override void Add<T>(T data)
        {
            throw new NotImplementedException();
        }

        public override void Add<T>(string funcName)
        {
            this.funcName = funcName;
        }
    }

}