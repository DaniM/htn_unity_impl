using System;
using UnityEditor;
using UnityEngine;

namespace AI
{
    public static class EditorUtils
    {
        public static void EnumDrawer<T>(ref Rect position, SerializedProperty property, GUIContent label) where T : Enum
        {
            //if (property.intValue == 0)
            {
                string[] options = Enum.GetNames(typeof(T));
                int selected = property.intValue;
                
                property.intValue = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label.text , selected, options);
            }
            //else
            //{
            //    EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, label);
                
            //}
            position.y += EditorGUIUtility.singleLineHeight;
        }

        public static void EnumDrawer<T>( Rect position, SerializedProperty property, GUIContent label) where T : Enum
        {
            //if (property.intValue == 0)
            {
                string[] options = Enum.GetNames(typeof(T));
                int selected = property.intValue;

                property.intValue = EditorGUI.Popup(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), label.text, selected, options);
            }
            //else
            //{
            //    EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property, label);

            //}
        }
    }
}