using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AI
{
    /**/
    public class ScriptableEffect : ScriptableObject, IEffect
    {
        public WorldStateDefinition WorldContext;

        public string Variable;

        public EffectOperation Op;

        public int Value;

        public string SourceVariable = null;

        private int previous;

        public void Apply(int[] ws)
        {
            switch (Op)
            {
                case EffectOperation.ADD_CONSTANT:
                    add(ws, Variable, Value);
                    break;
                case EffectOperation.SET_CONSTANT:
                    set(ws, Variable, Value);
                    break;
                case EffectOperation.SET_VARIABLE:
                    set(ws, Variable, SourceVariable);
                    break;
                default:
                    break;
            }
        }

        private void add(int[] ws, string name, int value)
        {
            int idx = WorldContext.FindIndex(name);
            if (idx >= 0)
            {
                previous = ws[idx];
                ws[idx] = previous + value;
            }
        }

        private void set(int[] ws, string name, int value)
        {
            int idx = WorldContext.FindIndex(name);
            if (idx >= 0)
            {
                previous = ws[idx];
                ws[idx] = value;
            }
        }

        private void set(int[] ws, string name, string source)
        {
            int destinationIdx = WorldContext.FindIndex(name);
            if (destinationIdx >= 0)
            {
                int sourceIdx = WorldContext.FindIndex(source);
                previous = ws[destinationIdx];
                ws[destinationIdx] = ws[sourceIdx];
            }
        }

        public virtual void ApplyRuntimeEffects(IRuntimeContext runtime)
        {

        }

        public void UnApply(int[] ws)
        {
            int idx = WorldContext.FindIndex(Variable);
            if (idx >= 0)
            {
                ws[idx] = previous;
            }
        }

        public void SetEffectOperation(EffectOperation op)
        {
            Op = op;
        }

        public void SetVariable(int variable)
        {
            throw new System.NotImplementedException();
        }

        public void SetValue(int value)
        {
            Value = value;
        }
    }
}

#if UNITY_EDITOR



#endif