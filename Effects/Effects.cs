using System;

namespace AI
{
    public enum EffectOperation
    {
        SET_CONSTANT,
        ADD_CONSTANT,
        SET_VARIABLE,
        ADD_VARIABLE
    }

    public static class DefaultEffects
    {
        public static void Apply(int[] ws, int idx, EffectOperation op, int value)
        {
            switch (op)
            {
                case EffectOperation.ADD_CONSTANT:
                    ws[idx] += value;
                    break;
                case EffectOperation.SET_CONSTANT:
                    ws[idx] = value;
                    break;
                case EffectOperation.SET_VARIABLE:
                    ws[idx] = ws[value];
                    break;
                case EffectOperation.ADD_VARIABLE:
                    ws[idx] += ws[value];
                    break;
                default:
                    break;
            }
        }
    }

    public class SimpleEffect : IEffect
    {
        public EffectOperation Op;

        public int Index;

        public int Value;

        private int previous;

        public SimpleEffect() { }

        public SimpleEffect(int index, EffectOperation op, int value)
        {
            Index = index;
            Op = op;
            Value = value;
        }

        public void Apply(int[] ws)
        {
            previous = ws[Index];
            DefaultEffects.Apply(ws, Index, Op, Value);
        }

        public virtual void ApplyRuntimeEffects(IRuntimeContext runtime)
        {
            
        }

        public void SetEffectOperation(EffectOperation op)
        {
            Op = op;
        }

        public void SetValue(int value)
        {
            Value = value;
        }

        public void SetVariable(int variable)
        {
            Index = variable;
        }

        public void UnApply(int[] ws)
        {
            ws[Index] = previous;
        }
    }

    public class Effect : IEffect
    {
        public WorldStateDefinition WorldContext;

        public string Variable;

        public EffectOperation Op;

        public int Value;

        public string SourceVariable = null;

        private int previous;

        public Effect() { }

        public Effect(WorldStateDefinition world, string variable, EffectOperation op, int value)
        {
            WorldContext = world;
            Variable = variable;
            Op = op;
            Value = value;
        }

        public Effect(WorldStateDefinition world, string variable, EffectOperation op, string source)
        {
            WorldContext = world;
            Variable = variable;
            Op = op;
            SourceVariable = source;
        }


        public void Apply(int[] ws)
        {
            int idx = WorldContext.FindIndex(Variable);
            if (idx >= 0)
            {
                previous = ws[idx];
                if (SourceVariable != null)
                {
                    int sourceIdx = WorldContext.FindIndex(SourceVariable);
                    DefaultEffects.Apply(ws, idx, Op, sourceIdx);
                }
                else
                {
                    DefaultEffects.Apply(ws, idx, Op, Value);
                }
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
            Variable = WorldContext.order[variable];
        }

        public void SetValue(int value)
        {
            Value = value;
        }
    }

}
