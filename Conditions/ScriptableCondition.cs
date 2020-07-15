using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

namespace AI
{
    [System.Serializable]
    public class ConditionList : ReorderableArray<ScriptableCondition> {}

    /**/
    public abstract class ScriptableCondition : ScriptableObject, ICondition
    {
        public WorldDefinition WorldContext;

        public abstract void AddCondition(ICondition condition);

        public abstract bool Eval(int[] worldState);
        public abstract ICondition GetChildCondition(int i);

        public string GetName()
        {
            return name;
        }

        public abstract int NumChildren();
        public abstract bool RuntimeEval(IRuntimeContext context);

        public void SetName(string name)
        {
            
        }
    }
}
