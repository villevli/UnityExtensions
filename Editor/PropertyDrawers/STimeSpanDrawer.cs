using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using VLExtensions;

namespace VLExtensionsEditor
{
    [CustomPropertyDrawer(typeof(STimeSpan))]
    public class STimeSpanDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tsProp = property.FindPropertyRelative("data");
            var tsRect = position;
            tsRect.width -= 80;

            TimeSpan tsValue = STimeSpan.ToTimeSpan(tsProp.stringValue);

            label = EditorGUI.BeginProperty(tsRect, label, property);

            if (Mode == DisplayMode.Normal)
            {
                string tsString = ToString(tsValue, Mode);

                EditorGUI.BeginChangeCheck();
                tsString = EditorGUI.TextField(tsRect, label, tsString);
                if (EditorGUI.EndChangeCheck())
                {
                    if (string.IsNullOrWhiteSpace(tsString))
                        tsValue = default;
                    else if (TryParse(tsString, Mode, out var parsedDt))
                        tsValue = parsedDt;
                    tsProp.stringValue = STimeSpan.ToString(tsValue);
                }
            }
            else
            {
                double tsDouble = ToDouble(tsValue, Mode);

                EditorGUI.BeginChangeCheck();
                tsDouble = EditorGUI.DoubleField(tsRect, label, tsDouble);
                if (EditorGUI.EndChangeCheck())
                {
                    tsValue = ToTimeSpan(tsDouble, Mode);
                    tsProp.stringValue = STimeSpan.ToString(tsValue);
                }
            }

            EditorGUI.EndProperty();

            var tzRect = position;
            tzRect.xMin = position.xMax - 80;
            tzRect.width = 60;
            Mode = (DisplayMode)EditorGUI.EnumPopup(tzRect, Mode, "Button");

            var optionsRect = position;
            optionsRect.xMin = position.xMax - 20;
            if (EditorGUI.DropdownButton(optionsRect, GUIContent.none, FocusType.Keyboard))
            {
                EditorGUI.FocusTextInControl("");
                void SetValue(TimeSpan timeSpan)
                {
                    tsProp.stringValue = STimeSpan.ToString(timeSpan);
                    tsProp.serializedObject.ApplyModifiedProperties();
                }

                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Time Of Day Now"), false,
                    () => SetValue(DateTime.UtcNow.TimeOfDay));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Copy As Seconds"), false,
                    () => GUIUtility.systemCopyBuffer = STimeSpan.ToTimeSpan(tsProp.stringValue).TotalSeconds.ToString(NumberFormatInfo.InvariantInfo));
                menu.ShowAsContext();
            }
        }


        private static string ToString(TimeSpan value, DisplayMode mode)
        {
            switch (mode)
            {
                case DisplayMode.Normal:
                    return STimeSpan.ToString(value);
                default:
                    return ToDouble(value, mode).ToString(NumberFormatInfo.InvariantInfo);
            }
        }

        private static bool TryParse(string str, DisplayMode mode, out TimeSpan value)
        {
            // TODO: Support strings like "1d 2h 3min" or "1 day 2 hours 3 minutes"
            // TODO: Support expressions like "+1 day" or "-2 hours"
            switch (mode)
            {
                case DisplayMode.Normal:
                    return TimeSpan.TryParse(str, CultureInfo.InvariantCulture, out value)
                        || TimeSpan.TryParse(str, CultureInfo.CurrentCulture, out value);
                case DisplayMode.Ticks:
                    if (long.TryParse(str, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out var parsedLong))
                    {
                        value = ToTimeSpan(parsedLong, mode);
                        return true;
                    }
                    value = default;
                    return false;
                default:
                    if (double.TryParse(str, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var parsedDouble))
                    {
                        value = ToTimeSpan(parsedDouble, mode);
                        return true;
                    }
                    value = default;
                    return false;
            }
        }

        private static double ToDouble(TimeSpan value, DisplayMode mode)
        {
            return mode switch
            {
                DisplayMode.Seconds => value.TotalSeconds,
                DisplayMode.Minutes => value.TotalMinutes,
                DisplayMode.Hours => value.TotalHours,
                DisplayMode.Days => value.TotalDays,
                DisplayMode.Ticks => value.Ticks,
                _ => value.TotalSeconds,
            };
        }

        private static TimeSpan ToTimeSpan(double value, DisplayMode mode)
        {
            return mode switch
            {
                DisplayMode.Seconds => TimeSpan.FromSeconds(value),
                DisplayMode.Minutes => TimeSpan.FromMinutes(value),
                DisplayMode.Hours => TimeSpan.FromHours(value),
                DisplayMode.Days => TimeSpan.FromDays(value),
                DisplayMode.Ticks => TimeSpan.FromTicks((long)value),
                _ => TimeSpan.FromSeconds(value),
            };
        }

        DisplayMode Mode
        {
            get => (DisplayMode)SessionState.GetInt(nameof(STimeSpanDrawer) + nameof(Mode), (int)DisplayMode.Normal);
            set => SessionState.SetInt(nameof(STimeSpanDrawer) + nameof(Mode), (int)value);
        }

        enum DisplayMode
        {
            Normal,
            Seconds,
            Minutes,
            Hours,
            Days,
            Ticks
        }
    }
}
