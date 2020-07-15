using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AI
{
    public enum ArithmeticComparisonType
    {
        EQ,
        NEQ,
        LT,
        LE,
        GT,
        GE
    };

    public enum CompareTo
    {
        CMP_WITH_CONSTANT,
        CMP_WITH_FIELD
    };

    /**/
    [System.Serializable]
    public class ConstantComparison
    {
        public ArithmeticComparisonType ComparisonType;

        public int Value;

        public ConstantComparison() { }

        public ConstantComparison(ArithmeticComparisonType cmp, int value)
        {
            ComparisonType = cmp;
            Value = value;
        }

        public bool Compare(int v)
        {
            switch (ComparisonType)
            {
                case ArithmeticComparisonType.EQ:
                    return v == Value;
                case ArithmeticComparisonType.NEQ:
                    return v != Value;
                case ArithmeticComparisonType.LT:
                    return v < Value;
                case ArithmeticComparisonType.LE:
                    return v <= Value;
                case ArithmeticComparisonType.GT:
                    return v > Value;
                case ArithmeticComparisonType.GE:
                    return v >= Value;
                default:
                    return false;
            }
        }
    }
}

//#if UNITY_EDITOR

//[CustomPropertyDrawer(typeof(ChestsContents.ChestCard))]
//public class ChestCardDrawer : PropertyDrawer
//{
//    public static string[] NAMES;

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        //base.OnGUI(position, property, label);

//        EditorGUI.BeginProperty(position, label, property);

//        //if (property.isExpanded)
//        {
//            EditorGUI.BeginChangeCheck();

//            SerializedProperty sfield = property.FindPropertyRelative("CardOddityMask");
//            sfield.intValue = EditorGUI.MaskField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), sfield.intValue, Enum.GetNames(typeof(CardOddity)));

//            sfield = property.FindPropertyRelative("CardTypeMask");
//            //position.y += 16;
//            sfield.intValue = EditorGUI.MaskField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight), sfield.intValue, Enum.GetNames(typeof(CardType)));


//            //position.y += 16;
//            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 3, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("CardId"));

//            //position.y += 16;
//            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 4, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("qty"));

//            EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight * 5, position.width, EditorGUIUtility.singleLineHeight), property.FindPropertyRelative("weight"));

//            if (EditorGUI.EndChangeCheck())
//            {
//                property.serializedObject.ApplyModifiedProperties();
//            }
//        }
//        //else
//        //{

//        //}

//        EditorGUI.EndProperty();
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        //if ( property.isExpanded )
//        {
//            return EditorGUIUtility.singleLineHeight * 6;
//        }
//        //else
//        //{
//        //    return EditorGUIUtility.singleLineHeight;
//        //}
//    }
//}

//#endif
