using System;

namespace AI
{
    [Serializable]
    public class WorldEffect : IEffect
    {
        public WorldDefinition WorldContext;

        public string Variable;

        public EffectOperation Op;

        public int Value;

        public string SourceVariable = null;

        private int previous;

        public WorldEffect() { }

        public WorldEffect(WorldDefinition world, string variable, EffectOperation op, int value)
        {
            WorldContext = world;
            Variable = variable;
            Op = op;
            Value = value;
        }

        public WorldEffect(WorldDefinition world, string variable, EffectOperation op, string source)
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
            Variable = WorldContext.World.order[variable];
        }

        public void UnApply(int[] ws)
        {
            int idx = WorldContext.FindIndex(Variable);
            if (idx >= 0)
            {
                ws[idx] = previous;
            }
        }
    }

}