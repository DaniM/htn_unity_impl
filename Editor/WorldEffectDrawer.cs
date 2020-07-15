using System;
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

#endif

namespace AI
{
#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(WorldEffect))]
    public class WorldEffectDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty world = property.FindPropertyRelative("WorldContext");
            if (world.objectReferenceValue == null)
            {
                return 2 * EditorGUIUtility.singleLineHeight;
            }
            else
            {
                return 4 * EditorGUIUtility.singleLineHeight;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //base.OnGUI(position, property, label);

            SerializedProperty world = property.FindPropertyRelative("WorldContext");

            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), world, new GUIContent("World: "));
            position.y += EditorGUIUtility.singleLineHeight;
            if (world.objectReferenceValue == null)
            {
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Assign a world context");
                position.y += EditorGUIUtility.singleLineHeight;
            }
            else
            {

                WorldDefinition worldDef = (world.objectReferenceValue as WorldDefinition);
                string[] options = worldDef.Order.ToArray();

                if (options.Length > 0)
                {
                    SerializedProperty variable = property.FindPropertyRelative("Variable");
                    string variableName = variable.stringValue;
                    int selected = 0;
                    for (int i = 0; i < options.Length; i++)
                    {
                        if (options[i] == variableName)
                        {
                            selected = i;
                            break;
                        }
                    }

                    selected = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Variable: ", selected, options);
                    variable.stringValue = options[selected];
                    position.y += EditorGUIUtility.singleLineHeight;


                    SerializedProperty operation = property.FindPropertyRelative("Op");
                    EditorUtils.EnumDrawer<EffectOperation>(position, operation, new GUIContent("Operation: "));
                    //EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), operation, new GUIContent("Operation: "));
                    position.y += EditorGUIUtility.singleLineHeight;
                    if (operation.intValue < 2)
                    {
                        SerializedProperty value = property.FindPropertyRelative("Value");

                        // constant
                        if (worldDef.World.IsEnum(variable.stringValue))
                        {
                            options = worldDef.World.enums[variable.stringValue];
                            if (options.Length > 0 && operation.intValue == 0)
                            {
                                value.intValue = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Value: ", value.intValue, options);
                                position.y += EditorGUIUtility.singleLineHeight;
                            }
                            else
                            {
                                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Invalid operation on an enum");
                                position.y += EditorGUIUtility.singleLineHeight;
                            }
                        }
                        else
                        {
                            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), value, new GUIContent("Value: "));
                            position.y += EditorGUIUtility.singleLineHeight;
                        }
                    }
                    else
                    {
                        // variable
                        SerializedProperty source = property.FindPropertyRelative("SourceVariable");
                        variableName = variable.stringValue;
                        selected = 0;
                        for (int i = 0; i < options.Length; i++)
                        {
                            if (options[i] == variableName)
                            {
                                selected = i;
                                break;
                            }
                        }

                        selected = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "Source: ", selected, options);
                        source.stringValue = options[selected];
                        position.y += EditorGUIUtility.singleLineHeight;
                    }
                }
                else
                {
                    EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "No clauses on the current world");
                    position.y += EditorGUIUtility.singleLineHeight;
                }

            }
        }
    }

#endif
}
