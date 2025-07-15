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
            var tsProp = property.FindPropertyRelative("data");
            var tsRect = position;
            tsRect.width -= 70;

            DateTimeOffset dtValue = default;
            string dtString = "";
            try
            {
                dtValue = SDateTime.ToDateTimeOffset(tsProp.stringValue);
                dtString = ToString(dtValue, Mode);
            }
            catch (Exception)
            {
            }

            label = EditorGUI.BeginProperty(tsRect, label, property);
            EditorGUI.BeginChangeCheck();
            dtString = EditorGUI.DelayedTextField(tsRect, label, dtString);
            if (EditorGUI.EndChangeCheck())
            {
                if (string.IsNullOrWhiteSpace(dtString))
                    dtValue = default;
                else if (TryParse(dtString, Mode, out var parsedDt))
                    dtValue = parsedDt;
                tsProp.stringValue = SDateTime.ToString(dtValue.ToUniversalTime());
            }
            EditorGUI.EndProperty();

            var tzRect = position;
            tzRect.xMin = position.xMax - 70;
            tzRect.width = 50;
            Mode = (DisplayMode)EditorGUI.EnumPopup(tzRect, Mode, "Button");

            var optionsRect = position;
            optionsRect.xMin = position.xMax - 20;
            if (EditorGUI.DropdownButton(optionsRect, GUIContent.none, FocusType.Keyboard))
            {
                EditorGUI.FocusTextInControl("");
                void SetTimestamp(DateTimeOffset dateTime)
                {
                    tsProp.stringValue = SDateTime.ToString(dateTime.ToUniversalTime());
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

        private static string ToString(DateTimeOffset value, DisplayMode mode)
        {
            if (value == default)
                return "";

            switch (mode)
            {
                case DisplayMode.UTC:
                default:
                    return SDateTime.ToString(value);
                case DisplayMode.Local:
                    return SDateTime.ToString(value.LocalDateTime);
                case DisplayMode.Unix:
                    return (value.ToUnixTimeMilliseconds() / 1000d).ToString("0.###", NumberFormatInfo.InvariantInfo);
            }
        }

        private static bool TryParse(string str, DisplayMode mode, out DateTimeOffset value)
        {
            // TODO: Support expressions like "+1 day" or "-2 hours"
            switch (mode)
            {
                case DisplayMode.UTC:
                default:
                    return DateTimeOffset.TryParse(str, DateTimeFormatInfo.InvariantInfo,
                                                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                                                    out value)
                        || DateTimeOffset.TryParse(str, DateTimeFormatInfo.CurrentInfo,
                                                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                                                    out value);
                case DisplayMode.Local:
                    return DateTimeOffset.TryParse(str, DateTimeFormatInfo.InvariantInfo,
                                                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                                                    out value)
                        || DateTimeOffset.TryParse(str, DateTimeFormatInfo.CurrentInfo,
                                                    DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal,
                                                    out value);
                case DisplayMode.Unix:
                    if (ExpressionEvaluator.Evaluate(str, out double parsedDouble))
                    {
                        value = DateTimeOffset.FromUnixTimeMilliseconds((long)(parsedDouble * 1000));
                        return true;
                    }
                    value = default;
                    return false;
            }
        }

        DisplayMode Mode
        {
            get => (DisplayMode)SessionState.GetInt(nameof(SDateTimeDrawer) + nameof(Mode), (int)DisplayMode.UTC);
            set => SessionState.SetInt(nameof(SDateTimeDrawer) + nameof(Mode), (int)value);
        }

        enum DisplayMode
        {
            UTC,
            Local,
            Unix
        }
    }
}
