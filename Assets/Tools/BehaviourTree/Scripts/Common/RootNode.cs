using System;
using UnityEngine;

namespace Ironcow.BT
{
    [Serializable]
    public class RootNode : BTNode
    {
        [SerializeReference] public BTNode node;

        public RootNode()
        {
            className = GetType().Name;
        }

        public override BTNode AddAction(Func<eNodeState> func)
        {
            node = new ActionNode(func, this);
            return this;
        }

        public override BTNode AddSelector()
        {
            node = new SelectorNode(this);
            return node;
        }

        public override BTNode AddSequence()
        {
            node = new SequenceNode(this);
            return node;
        }

        public override eNodeState Evaluate()
        {
            return node.Evaluate();
        }

        public override Type GetRealType()
        {
            return GetType();
        }
    }
}