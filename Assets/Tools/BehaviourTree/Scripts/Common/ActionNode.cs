using System;

namespace Ironcow.BT
{
    [Serializable]
    public class ActionNode : BTNode
    {
        public Func<eNodeState> onUpdate = null;
        public string funcName;
        public ActionNode(Func<eNodeState> onUpdate, BTNode parent)
        {
            className = GetType().Name;
            this.onUpdate = onUpdate;
            this.parent = parent;
        }

        public ActionNode(BTNode parent)
        {
            className = GetType().Name;
            this.parent = parent;
        }

        public void SetFunc(Func<eNodeState> onUpdate)
        {
            this.onUpdate = onUpdate;
            funcName = onUpdate.Method.Name;
        }

        public override BTNode AddAction(Func<eNodeState> state)
        {
            return this;
        }

        public override BTNode AddSequence()
        {
            return this;
        }
        public override BTNode AddSelector()
        {
            return this;
        }


        public override eNodeState Evaluate() => onUpdate?.Invoke() ?? eNodeState.failure;

        public override Type GetRealType()
        {
            return GetType();
        }
    }
}