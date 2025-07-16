using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
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

            if (Mode == DisplayMode.Normal || Mode == DisplayMode.Human)
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
                case DisplayMode.Human:
                    return ToHumanReadable(value);
                default:
                    return ToDouble(value, mode).ToString(NumberFormatInfo.InvariantInfo);
            }
        }

        private static bool TryParse(string str, DisplayMode mode, out TimeSpan value)
        {
            // TODO: Support expressions like "+01:02:00", "+1 day", "-2 hours" or "*2" (multiply by 2), "/2" (divide by 2)
            switch (mode)
            {
                case DisplayMode.Normal:
                    return TimeSpan.TryParse(str, CultureInfo.InvariantCulture, out value)
                        || TimeSpan.TryParse(str, CultureInfo.CurrentCulture, out value);
                case DisplayMode.Human:
                    return TryParseHumanReadable(str, out value);
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

        private static string ToHumanReadable(TimeSpan value)
        {
            static void Append(StringBuilder sb, double val, string unit)
            {
                if (sb.Length > 0)
                    sb.Append(' ');
                sb.Append(val.ToString(NumberFormatInfo.InvariantInfo));
                sb.Append(unit);
            }
            var sb = new StringBuilder();
            if (value.Days != 0)
                Append(sb, value.Days, "d");
            if (value.Hours != 0)
                Append(sb, value.Hours, "h");
            if (value.Minutes != 0)
                Append(sb, value.Minutes, "min");
            if (value.Seconds != 0 || value.Milliseconds != 0)
                Append(sb, value.Seconds + value.Milliseconds / 1000d, "s");
            return sb.ToString();
        }

        // Support strings like "1d 2h 3min 4s 5ms" or "1 day 2 hours 3.5 minutes"
        private static bool TryParseHumanReadable(string str, out TimeSpan result)
        {
            var tokens = new List<string>();
            var tokenBuilder = new StringBuilder();
            int lastTokenType = 0; // 1 == number, 2 == unit

            foreach (var ci in str)
            {
                if (char.IsWhiteSpace(ci))
                    continue;

                var c = ci;
                if (c == ',')
                    c = '.';

                int tokenType = (char.IsDigit(c) || c == '-' || c == '.') ? 1 : char.IsLetter(c) ? 2 : 0;

                if (tokenType != lastTokenType && tokenBuilder.Length > 0)
                {
                    tokens.Add(tokenBuilder.ToString());
                    tokenBuilder.Clear();
                }

                if (tokenType != 0)
                    tokenBuilder.Append(c);

                lastTokenType = tokenType;
            }

            if (tokenBuilder.Length > 0)
                tokens.Add(tokenBuilder.ToString());

            result = TimeSpan.Zero;
            bool parsed = false;
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (double.TryParse(tokens[i], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var parsedDouble)
                    && TryParseWithUnitString(parsedDouble, tokens[i + 1], out var parsedTs)
                )
                {
                    result += parsedTs;
                    i++;
                    parsed = true;
                }
            }
            // Return true if anything was parsed succesfully. The full string can contain unparseable parts
            return parsed;
        }

        private static bool TryParseWithUnitString(double value, string unit, out TimeSpan result)
        {
            if (unit.Equals("d", StringComparison.OrdinalIgnoreCase)
                || unit.StartsWith("day", StringComparison.OrdinalIgnoreCase))
            {
                result = TimeSpan.FromDays(value);
                return true;
            }
            if (unit.Equals("h", StringComparison.OrdinalIgnoreCase)
                || unit.StartsWith("hour", StringComparison.OrdinalIgnoreCase))
            {
                result = TimeSpan.FromHours(value);
                return true;
            }
            if (unit.Equals("m", StringComparison.OrdinalIgnoreCase)
                || unit.StartsWith("min", StringComparison.OrdinalIgnoreCase))
            {
                result = TimeSpan.FromMinutes(value);
                return true;
            }
            if (unit.Equals("s", StringComparison.OrdinalIgnoreCase)
                || unit.StartsWith("sec", StringComparison.OrdinalIgnoreCase))
            {
                result = TimeSpan.FromSeconds(value);
                return true;
            }
            if (unit.Equals("ms", StringComparison.OrdinalIgnoreCase)
                || unit.StartsWith("mil", StringComparison.OrdinalIgnoreCase))
            {
                result = TimeSpan.FromMilliseconds(value);
                return true;
            }
            result = default;
            return false;
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
            Human,
            Seconds,
            Minutes,
            Hours,
            Days,
            Ticks
        }
    }
}
