using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AI
{
    [CreateAssetMenu(menuName = "Assets/AI/Conditions/Atomic Condition")]
    public class ScriptableAtomicCondition : ScriptableCondition
    {
        [HideInInspector]
        public string Variable;

        [HideInInspector]
        public ConstantComparison comparison;

        public override void AddCondition(ICondition condition)
        {
            throw new System.NotImplementedException("Atomic condition");
        }

        public override bool Eval(int[] worldState)
        {
            int value = WorldContext.World.FindValueInWorldState(Variable, worldState);
            return comparison.Compare(value);
        }

        public override ICondition GetChildCondition(int i)
        {
            throw new System.NotImplementedException("Atomic condition");
        }

        public override int NumChildren()
        {
            return 0;
        }

        public override bool RuntimeEval(IRuntimeContext context)
        {
            return true;
        }
    }

#if UNITY_EDITOR


    [CustomEditor( typeof( ScriptableAtomicCondition ) )]
    public class AtomicConditionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            ScriptableAtomicCondition condition = target as ScriptableAtomicCondition;

            EditorGUI.BeginChangeCheck();

            if (!condition.WorldContext)
            {
                GUILayout.Label("Assign a context (World Definition)");

                SerializedProperty context = serializedObject.FindProperty("WorldContext");

                EditorGUILayout.PropertyField(context);
            }
            else
            {
                SerializedProperty context = serializedObject.FindProperty("WorldContext");

                EditorGUILayout.PropertyField(context);

                WorldDefinition world = context.objectReferenceValue as WorldDefinition;

                SerializedProperty variable = serializedObject.FindProperty("Variable");
                int idx = 0;
                string[] options = world.Order.ToArray();
                string currentValue = variable.stringValue;
                if ( !string.IsNullOrEmpty( currentValue ) )
                {
                    for (int i = 0; i < options.Length; i++)
                    {
                        if ( options[i] == currentValue )
                        {
                            idx = i;
                            break;
                        }
                    }
                }

                idx = EditorGUILayout.Popup( "Variable: ", idx, options );
                currentValue = options[idx];
                variable.stringValue = currentValue;


                SerializedProperty comparation = serializedObject.FindProperty("comparison.ComparisonType");

                EditorGUILayout.PropertyField(comparation);

                SerializedProperty Value = serializedObject.FindProperty("comparison.Value");

                if (world.World.enums.ContainsKey(currentValue))
                {
                    idx = Value.intValue;
                    options = world.World.enums[currentValue];

                    //currentValue = Value.stringValue;

                    if (!string.IsNullOrEmpty(currentValue))
                    {
                        for (int i = 0; i < options.Length; i++)
                        {
                            if (options[i] == currentValue)
                            {
                                idx = i;
                                break;
                            }
                        }
                    }

                    Value.intValue = EditorGUILayout.Popup("Value:", idx, options);
                }
                else
                {
                    EditorGUILayout.PropertyField(Value);
                }
            }


            if ( EditorGUI.EndChangeCheck() )
            {
                serializedObject.ApplyModifiedProperties();
            }

            if ( GUILayout.Button( "Save" ) )
            {
                EditorUtility.SetDirty( target );
            }
        }
    }

#endif

}
