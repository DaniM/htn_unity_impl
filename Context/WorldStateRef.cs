using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;
using System;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AI
{
	[CreateAssetMenu(menuName = "Assets/AI/World State Ref")]
	public class WorldStateRef : ScriptableObject
	{
        [Serializable]
		public class Clause
		{
			public string Name;
			public int Value;

            public Clause() { }

            public Clause(string name, int value)
            {
                Name = name;
                Value = value;
            }
		}

		//public class ClauseList : ReorderableArray<Clause> { }

		public WorldDefinition World;

        [HideInInspector]
		public List<Clause> Clauses;

		public int[] BuildState()
		{
			int[] ws = World.World.CreateWorldState();
            for (int i = 0; i < Clauses.Count; i++)
            {
				int idx = World.World.FindIndex( Clauses[i].Name );
                if ( idx >= 0 )
                {
					ws[idx] = Clauses[i].Value;
                }
            }
			return ws;
        }

        public void ResetState(int[] ws)
        {
            for (int i = 0; i < Clauses.Count; i++)
            {
                int idx = World.World.FindIndex(Clauses[i].Name);
                if (idx >= 0)
                {
                    ws[idx] = Clauses[i].Value;
                }
            }
        }
	}

#if UNITY_EDITOR

	[CustomEditor(typeof(WorldStateRef))]
	public class WorldDefaultStateInspector : Editor
	{
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();

            WorldStateRef state = target as WorldStateRef;

            if ( !state.World )
            {
				GUILayout.Label("Assign a context (World Definition)");
				return;
            }

            for (int i = 0; i < state.World.Order.Count; i++)
            {
				string variable = state.World.Order[i];
				if (state.World.World.IsEnum(variable))
				{
                    int idx = findClauseIdx(variable, state.Clauses);
                    if (idx >= 0)
                    {
                        int value = state.Clauses[idx].Value;
                        string[] options = state.World.World.enums[variable];
                        state.Clauses[idx].Value = EditorGUILayout.Popup( variable , value, options);
                    }
                    else
                    {
                        string[] options = state.World.World.enums[variable];
                        int value = EditorGUILayout.Popup(variable, 0, options);
                        if ( value != 0 )
                        {
                            state.Clauses.Add( new WorldStateRef.Clause( variable, value ) );
                        }
                    }
                }
				else
                {
                    int idx = findClauseIdx(variable, state.Clauses);
                    if (idx >= 0)
                    {
                        int value = state.Clauses[idx].Value;
                        state.Clauses[idx].Value = EditorGUILayout.IntField(variable, value);
                    }
                    else
                    {
                        int value = EditorGUILayout.IntField(variable, 0);
                        if (value != 0)
                        {
                            state.Clauses.Add(new WorldStateRef.Clause(variable, value));
                        }
                    }
                }
			}

            if ( EditorGUI.EndChangeCheck() )
            {
                serializedObject.ApplyModifiedProperties();
            }

            if ( GUILayout.Button( "Save" ) )
            {
				EditorUtility.SetDirty(target);
			}
		}

        int findClauseIdx(string name, List<WorldStateRef.Clause> clauses) {

            for (int i = 0; i < clauses.Count; i++)
            {
                if ( clauses[i].Name == name )
                {
                    return i;
                }
            }
            return -1;
        }
    }

#endif
}