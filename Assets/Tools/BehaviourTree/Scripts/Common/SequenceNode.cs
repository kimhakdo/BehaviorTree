using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ironcow.BT
{
    [Serializable]
    public sealed class SequenceNode : BTNode
    {

        [SerializeReference] public List<BTNode> childs = new List<BTNode>();

        public SequenceNode(BTNode parent)
        {
            className = GetType().Name;
            childs = new List<BTNode>();
            this.parent = parent;
        }

        public override BTNode AddAction(Func<eNodeState> func)
        {
            childs.Add(new ActionNode(func, this));
            return this;
        }

        public override BTNode AddSequence()
        {
            var newSequence = new SequenceNode(this);
            childs.Add(newSequence);
            return newSequence;
        }

        public override BTNode AddSelector()
        {
            var newSelector = new SelectorNode(this);
            childs.Add(newSelector);
            return newSelector;
        }


        public override eNodeState Evaluate()
        {
            if (childs == null || childs.Count == 0)
                return eNodeState.failure;

            foreach (var child in childs)
            {
                switch (child.Evaluate())
                {
                    case eNodeState.running:
                        return eNodeState.running;
                    case eNodeState.success:
                        continue;
                    case eNodeState.failure:
                        return eNodeState.failure;
                }
            }

            return eNodeState.success;
        }

        public override Type GetRealType()
        {
            return GetType();
        }
    }
}