using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [CreateAssetMenu(menuName = "Assets/AI/Conditions/NOT Condition")]
    public class ScriptableNotCondition : ScriptableCondition
    {
        public ScriptableCondition Negate;

        public override void AddCondition(ICondition condition)
        {
            Negate = condition as ScriptableCondition;
        }

        public override bool Eval(int[] worldState)
        {
            return !Negate.Eval( worldState );
        }

        public override ICondition GetChildCondition(int i)
        {
            return Negate;
        }

        public override int NumChildren()
        {
            return 1;
        }

        public override bool RuntimeEval(IRuntimeContext context)
        {
            return true;
        }
    }
}
