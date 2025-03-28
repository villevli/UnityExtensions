using UnityEngine;
using UnityEditor;
using VLExtensions;

namespace VLExtensionsEditor
{
    [CustomPropertyDrawer(typeof(SingleLineAttribute))]
    public class SingleLineAttributeDrawer : PropertyDrawer
    {
        private static bool ShouldUseOnProperty(SerializedProperty prop)
        {
            switch (prop.propertyType)
            {
                case SerializedPropertyType.Generic:
                case SerializedPropertyType.Vector2:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.Vector4:
                case SerializedPropertyType.Rect:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.Bounds:
                case SerializedPropertyType.BoundsInt:
                    return true;
                default:
                    return false;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as SingleLineAttribute;
            if (attr.hideLabel)
                label = GUIContent.none;

            if (ShouldUseOnProperty(property))
            {
                label = EditorGUI.BeginProperty(position, label, property);
                position = EditorGUI.PrefixLabel(position, label);

                var end = property.GetEndProperty();
                var iterator = property.Copy();
                iterator.Next(true); // first child

                // Count child properties
                int childCount = 0;
                var start = iterator.Copy();
                do
                {
                    childCount++;
                }
                while (start.Next(false) && !SerializedProperty.EqualContents(start, end));

                // Draw child properties on one line
                Rect childPos = position;
                float spacing = 4;
                float itemWidth = (position.width + spacing) / childCount;
                childPos.width = itemWidth - spacing;
                using var indentScope = new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel);
                do
                {
                    if (attr.hideFieldLabels)
                    {
                        EditorGUI.PropertyField(childPos, iterator, GUIContent.none, true);
                    }
                    else
                    {
                        EditorGUIUtility.labelWidth = Mathf.Min(childPos.width / 2, EditorStyles.label.CalcSize(new GUIContent(iterator.displayName)).x + 3);
                        EditorGUI.PropertyField(childPos, iterator, true);
                    }
                    childPos.x += itemWidth;
                }
                while (iterator.Next(false) && !SerializedProperty.EqualContents(iterator, end));

                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (ShouldUseOnProperty(property))
            {
                var end = property.GetEndProperty();
                var iterator = property.Copy();
                iterator.Next(true); // first child

                float maxHeight = 0;
                do
                {
                    var height = EditorGUI.GetPropertyHeight(iterator, true);
                    if (height > maxHeight)
                        maxHeight = height;
                }
                while (iterator.Next(false) && !SerializedProperty.EqualContents(iterator, end));
                return maxHeight;
            }
            else
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
        }
    }
}
