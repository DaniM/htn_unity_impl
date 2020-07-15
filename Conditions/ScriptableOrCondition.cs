using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AI
{
    [CreateAssetMenu(menuName = "Assets/AI/Conditions/OR Condition")]
    public class ScriptableOrCondition : ScriptableCondition
    {
#if UNITY_EDITOR

        [Reorderable]
        public ConditionList Conditions;

        private void OnEnable()
        {
            conditions = Conditions.ToArray();
        }

#endif

        [HideInInspector]
        public ScriptableCondition[] conditions;


        public override bool Eval(int[] worldState)
        {
            for (int i = 0; i < conditions.Length; i++)
            {
                if (conditions[i].Eval(worldState))
                {
                    return true;
                }
            }
            return false;
        }

        public override void AddCondition(ICondition condition)
        {
#if UNITY_EDITOR
            Conditions.Add(condition as ScriptableCondition);
#endif
            int currentLen = conditions.Length;
            System.Array.Resize<ScriptableCondition>(ref conditions, currentLen + 1);

            conditions[currentLen] = condition as ScriptableCondition;
        }

        public override bool RuntimeEval(IRuntimeContext context)
        {
            return true;
        }

        public override ICondition GetChildCondition(int i)
        {
            return conditions[i];
        }

        public override int NumChildren()
        {
            return conditions.Length;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ScriptableOrCondition))]
    public class OrConditionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Save"))
            {
                ScriptableOrCondition cond = (target as ScriptableOrCondition);
                cond.conditions = cond.Conditions.ToArray();
            }
        }
    }

#endif
}
