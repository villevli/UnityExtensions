using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using VLExtensions;

namespace VLExtensionsEditor
{
    [CustomPropertyDrawer(typeof(SDateTime))]
    public class SDateTimeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tsProp = property.FindPropertyRelative("timestamp");
            var tsRect = position;
            tsRect.width -= 20;

            DateTimeOffset dtValue = default;
            string dtString = "";
            try
            {
                dtValue = SDateTime.ToDateTimeOffset(tsProp.stringValue);
                dtString = SDateTime.ToString(dtValue);
            }
            catch (Exception)
            {
            }

            label = EditorGUI.BeginProperty(tsRect, label, property);
            EditorGUI.BeginChangeCheck();
            dtString = EditorGUI.TextField(tsRect, label, dtString);
            if (EditorGUI.EndChangeCheck())
            {
                if (string.IsNullOrWhiteSpace(dtString))
                {
                    dtValue = default;
                }
                else if (DateTimeOffset.TryParse(dtString, CultureInfo.InvariantCulture,
                                                 DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                                                 out var parsedDt))
                {
                    dtValue = parsedDt;
                }
                else if (long.TryParse(dtString, out var parsedTs))
                {
                    dtValue = DateTimeOffset.FromUnixTimeSeconds(parsedTs);
                }
                else if (double.TryParse(dtString, out var parsedTsd))
                {
                    dtValue = DateTimeOffset.FromUnixTimeSeconds((long)parsedTsd);
                }
                tsProp.stringValue = SDateTime.ToString(dtValue);
            }
            EditorGUI.EndProperty();

            var optionsRect = position;
            optionsRect.xMin = position.xMax - 20;
            if (EditorGUI.DropdownButton(optionsRect, GUIContent.none, FocusType.Keyboard))
            {
                EditorGUI.FocusTextInControl("");
                void SetTimestamp(DateTimeOffset dateTime)
                {
                    tsProp.stringValue = SDateTime.ToString(dateTime);
                    tsProp.serializedObject.ApplyModifiedProperties();
                }

                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Now"), false,
                    () => SetTimestamp(DateTimeOffset.UtcNow));
                menu.AddItem(new GUIContent("Today"), false,
                    () => SetTimestamp(DateTime.UtcNow.Date));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy Unix Time Seconds"), false,
                    () => GUIUtility.systemCopyBuffer = SDateTime.ToDateTimeOffset(tsProp.stringValue).ToUnixTimeSeconds().ToString());
                menu.ShowAsContext();
            }
        }
    }
}
