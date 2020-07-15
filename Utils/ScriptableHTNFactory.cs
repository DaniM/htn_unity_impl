using System;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using System.Text;

#endif

namespace AI
{
    public abstract class ScriptableHTNFactory : ScriptableObject, HTNFactory
    {
        public abstract ITaskNode Build();
    }

#if UNITY_EDITOR

#if UNITY_EDITOR

    [CustomEditor(typeof(ScriptableHTNFactory))]
    public class ScriptableHTNFactoryInspector : Editor
    {
        string tree = "";
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            tree = EditorGUILayout.TextArea(tree);
            if (GUILayout.Button("Print HTN"))
            {
                BotHTNFactory htnF = (target as BotHTNFactory);
                StringBuilder sb = new StringBuilder();
                AI.Utils.PrintTreeStructure(sb, htnF.Build(), 0);
                tree = sb.ToString();
            }
        }
    }

#endif

#endif
}
