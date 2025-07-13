using UnityEditor;
using UnityEngine;
using VLExtensions;

namespace VLExtensionsEditor
{
    [CustomPropertyDrawer(typeof(SDictionary<,>), true)]
    public class SDictionaryDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var dataProp = property.FindPropertyRelative("_data");
            EditorGUI.PropertyField(position, dataProp, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var dataProp = property.FindPropertyRelative("_data");
            return EditorGUI.GetPropertyHeight(dataProp, label);
        }
    }

    [CustomPropertyDrawer(typeof(SDictionary<,>.SKeyValuePair), true)]
    public class SKeyValuePairDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = GUIContent.none;
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            // Leave some space at the left side for opening the right click context menu
            position.xMin += 10;

            var keyProp = property.FindPropertyRelative("Key");
            var valueProp = property.FindPropertyRelative("Value");

            using var indentScope = new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel);

            if (valueProp.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUI.PropertyField(position, keyProp, GUIContent.none, true);
                EditorGUI.PropertyField(position, valueProp, GUIContent.none, true);
            }
            else
            {
                var childPos = position;
                float spacing = 4;
                float itemWidth = (position.width + spacing) / 2;
                childPos.width = itemWidth - spacing;
                EditorGUI.PropertyField(childPos, keyProp, GUIContent.none, true);
                childPos.x += itemWidth;
                EditorGUI.PropertyField(childPos, valueProp, GUIContent.none, true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("Value");
            return EditorGUI.GetPropertyHeight(valueProp, label);
        }
    }
}
