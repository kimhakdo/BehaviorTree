
using System;
using Unity.Profiling;

namespace Ironcow.BT
{
    public enum eNodeState
    {
        running,
        success,
        failure,
    }

    [Serializable]
    public abstract class BTNode
    {
        public string className;
        public BTNode parent;
        public abstract eNodeState Evaluate();
        public abstract BTNode AddAction(Func<eNodeState> func);
        public abstract BTNode AddSequence();
        public abstract BTNode AddSelector();

        public abstract Type GetRealType();
    }
}