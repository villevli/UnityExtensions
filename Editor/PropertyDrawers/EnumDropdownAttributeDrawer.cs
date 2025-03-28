using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using VLExtensions;

namespace VLExtensionsEditor
{
    [CustomPropertyDrawer(typeof(EnumDropdownAttribute))]
    public class EnumDropdownAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            var attr = attribute as EnumDropdownAttribute;
            bool isFlagsEnum = attr.enumType?.GetCustomAttribute<FlagsAttribute>() != null;

            if (attr.enumType == null || !attr.enumType.IsEnum)
            {
                EditorGUI.LabelField(position, label, new GUIContent($"{nameof(attr.enumType)} must be set a to a valid enum type!"));
            }
            else if (attr.defaultValue == null)
            {
                EditorGUI.LabelField(position, label, new GUIContent($"{nameof(attr.defaultValue)} must be set a to a valid enum value!"));
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                Enum enumVal;
                if (Enum.TryParse(attr.enumType, property.stringValue, true, out var enumObj))
                    enumVal = (Enum)enumObj;
                else
                    enumVal = attr.defaultValue;
                EditorGUI.BeginChangeCheck();
                if (isFlagsEnum)
                    enumVal = EditorGUI.EnumFlagsField(position, label, enumVal);
                else
                    enumVal = EditorGUI.EnumPopup(position, label, enumVal);
                if (EditorGUI.EndChangeCheck())
                    property.stringValue = enumVal.ToString();
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                Enum enumVal = (Enum)Enum.ToObject(attr.enumType, property.intValue);
                EditorGUI.BeginChangeCheck();
                if (isFlagsEnum)
                    enumVal = EditorGUI.EnumFlagsField(position, label, enumVal);
                else
                    enumVal = EditorGUI.EnumPopup(position, label, enumVal);
                if (EditorGUI.EndChangeCheck())
                    property.intValue = Convert.ToInt32(enumVal);
            }
            else
            {
                EditorGUI.LabelField(position, label, new GUIContent($"{nameof(EnumDropdownAttribute)} only works on string or int properties!"));
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }
    }
}
