using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;

#if UNITY_EDITOR

using UnityEditor;
using System.Text;
using System.IO;

#endif


namespace AI
{


    [CreateAssetMenu(menuName = "Assets/AI/World Definition")]
    public class WorldDefinition : ScriptableObject
    {
#if UNITY_EDITOR

        public TextAsset JSON;

#endif


        [System.Serializable]
        public class StringList : ReorderableArray<string> { }

        [System.Serializable]
        public class EnumVariable
        {
            public string EnumName;
            public StringList EnumValues = new StringList();

            public EnumVariable() { }
            public EnumVariable(string name, StringList values)
            {
                EnumName = name;
                EnumValues.Clear();
                for (int i = 0; i < values.Length; i++)
                {
                    EnumValues.Add( values[i] );
                }
            }
        }

        [System.Serializable]
        public class EnumList : ReorderableArray<EnumVariable> { }

        [Reorderable]
        public EnumList Enums = new EnumList();

        [Reorderable]
        public StringList Ints = new StringList();

        [Reorderable]
        public StringList Order = new StringList();

        private void OnEnable()
        {
            World = ExportToWorld();
        }

        public void ImportFromWorld( WorldStateDefinition world )
        {
            World = world;

            Enums.Clear();
            foreach (var item in World.enums)
            {
                StringList values = new StringList();
                for (int i = 0; i < item.Value.Length; i++)
                {
                    values.Add( item.Value[i] );
                }

                Enums.Add(new EnumVariable(item.Key, values ));
            }

            Ints.Clear();
            foreach (var item in World.ints)
            {
                Ints.Add( item );
            }

            Order.Clear();
            foreach (var item in World.order)
            {
                Order.Add( item );
            }
        }

        public WorldStateDefinition ExportToWorld()
        {
            WorldStateDefinition wsd = new WorldStateDefinition();

            wsd.enums.Clear();
            foreach (var item in Enums)
            {
                wsd.enums.Add(item.EnumName, item.EnumValues.ToArray());
            }

            //wsd.ints.Clear();
            wsd.SetInts(Ints.ToArray());

            //wsd.order.Clear();
            wsd.SetOrder(Order.ToArray());


            return wsd;
        }

        [HideInInspector]
        public WorldStateDefinition World = new WorldStateDefinition();

        public void SetValue(string variable, int value, int[] ws)
        {
            World.SetValueInWorldState(variable, value, ws);
        }

        public void SetValue(int variableIdx, int value, int[] ws)
        {
            ws[variableIdx] = value;
        }

        public void SetEnumValue(string variable, string value, int[] ws)
        {
            World.SetEnumValueInWorldState(variable, value, ws);
        }

        public void AddValue(string variable, int value, int[] ws)
        {
            int idx = World.FindIndex(variable);
            if ( idx >= 0 )
            {
                ws[idx] += value;
            }
        }

        public void AddValue(int variableIdx, int value, int[] ws)
        {
            ws[variableIdx] += value;
        }

        public int FindIndex(string variable)
        {
            return World.FindIndex( variable );
        }

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(WorldDefinition))]
    public class WorldDefinitionInspector : Editor
    {
        string json;

        private void Awake()
        {
            json = "";
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            WorldDefinition wdef = target as WorldDefinition;

            if (GUILayout.Button("Dump order"))
            {
                wdef.Order.Clear();
                foreach (var item in wdef.Enums)
                {
                    wdef.Order.Add( item.EnumName );
                }

                foreach (var item in wdef.Ints)
                {
                    wdef.Order.Add( item );
                }
            }

            if ( wdef.JSON != null && GUILayout.Button( "Import" ) )
            {
                WorldStateDefinition def = JSONUtils.JSON_Deserialize<WorldStateDefinition>(wdef.JSON.text);
                wdef.ImportFromWorld(def);
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Export"))
            {
                WorldStateDefinition def = wdef.ExportToWorld();
                json = JSONUtils.JSON_Serialize<WorldStateDefinition>(def);
            }

            if ( !string.IsNullOrEmpty( json ) )
            {
                EditorGUILayout.TextArea( json );
            }

            if ( GUILayout.Button( "Save" ) )
            {
                
                WorldStateDefinition def = wdef.ExportToWorld();
                //SerializedProperty property = serializedObject.FindProperty("World");
                wdef.World = def;
                EditorUtility.SetDirty(target);
            }

            if (GUILayout.Button("Generate enums & constants"))
            {

                string path = AssetDatabase.GetAssetPath(target);
                string dir = Path.GetDirectoryName( path );
                string filepath = Path.Combine(dir, string.Format( "{0}_Data.cs", target.name ));
                ExportCode( filepath, wdef.name, wdef.World );
                AssetDatabase.Refresh();
            }
        }

        public void ExportCode(string filepath, string name, WorldStateDefinition def)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("public static class {0}\n{{\n", name);
            foreach (var _enum in def.enums)
            {
                SerializeEnum(sb, _enum.Key, _enum.Value);
                sb.Append("\n");
            }

            SerializeIndexes( sb, def.order );

            sb.Append("}\n");
            File.WriteAllText( filepath, sb.ToString() );
        }

        private void SerializeEnum(StringBuilder sb, string enumName, string[] enumValues)
        {
            StringBuilder enumValuesSb = new StringBuilder();
            for (int i = 0; i < enumValues.Length - 1; i++)
            {
                enumValuesSb.AppendFormat("\t{0} = {1},\n", enumValues[i], i);
            }

            enumValuesSb.AppendFormat("\t{0} = {1}", enumValues[enumValues.Length-1], enumValues.Length - 1);

            sb.AppendFormat("public enum {0}\n{{\n{1}\n}}\n", enumName, enumValuesSb.ToString());
        }

        private void SerializeIndexes(StringBuilder sb, string[] order)
        {
            for (int i = 0; i < order.Length; i++)
            {
                sb.AppendFormat( "public const int {0}_INDEX = {1};\n", order[i].ToUpperInvariant(), i );
            }
        }
    }

#endif
}
