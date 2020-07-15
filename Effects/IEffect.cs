using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /**/
    public interface IEffect
    {
        void SetEffectOperation( EffectOperation op );
        void SetVariable(int variable);
        void SetValue(int value);

        void Apply( int[] ws );
        void UnApply( int[] ws );
        void ApplyRuntimeEffects(IRuntimeContext runtime);
    }


}
