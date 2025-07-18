using System;
using System.Globalization;
using UnityEngine;

namespace VLExtensions
{
    /// <summary>
    /// Set preference values by key that are saved persistently between sessions.
    /// A wrapper around Unity's PlayerPrefs with more supported types.
    /// Don't call these every frame if you care about performance.
    /// </summary>
    public static class SavedPrefs
    {
        public static bool HasKey(string key)
            => PlayerPrefs.HasKey(key);
        public static void DeleteKey(string key)
            => PlayerPrefs.DeleteKey(key);
        public static void Save()
            => PlayerPrefs.Save();

        public static string GetString(string key, string defaultValue = default)
            => PlayerPrefs.GetString(key, defaultValue);
        public static void SetString(string key, string value)
            => PlayerPrefs.SetString(key, value);

        public static int GetInt(string key, int defaultValue = default)
            => PlayerPrefs.GetInt(key, defaultValue);
        public static void SetInt(string key, int value)
            => PlayerPrefs.SetInt(key, value);

        public static float GetFloat(string key, float defaultValue = default)
            => PlayerPrefs.GetFloat(key, defaultValue);
        public static void SetFloat(string key, float value)
            => PlayerPrefs.SetFloat(key, value);

        public static bool GetBool(string key, bool defaultValue = default)
            => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
        public static void SetBool(string key, bool value)
            => PlayerPrefs.SetInt(key, value ? 1 : 0);

        public static long GetLong(string key, long defaultValue = default)
        {
            if (long.TryParse(PlayerPrefs.GetString(key, ""), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out long val))
                return val;
            return defaultValue;
        }

        public static void SetLong(string key, long value)
        {
            PlayerPrefs.SetString(key, value.ToString(NumberFormatInfo.InvariantInfo));
        }

        public static double GetDouble(string key, double defaultValue = default)
        {
            if (double.TryParse(PlayerPrefs.GetString(key, ""), NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double val))
                return val;
            return defaultValue;
        }

        public static void SetDouble(string key, double value)
        {
            PlayerPrefs.SetString(key, value.ToString(NumberFormatInfo.InvariantInfo));
        }

        public static DateTimeOffset GetDateTime(string key, DateTimeOffset defaultValue = default)
        {
            if (PlayerPrefs.HasKey(key))
                return DateTimeOffset.FromUnixTimeMilliseconds(GetLong(key));
            return defaultValue;
        }

        public static void SetDateTime(string key, DateTimeOffset value)
        {
            SetLong(key, value.ToUnixTimeMilliseconds());
        }

        public static T GetEnum<T>(string key, T defaultValue = default) where T : struct
        {
            var strVal = PlayerPrefs.GetString(key, null);
            if (strVal != null && Enum.TryParse(strVal, out T val))
                return val;
            else
                return defaultValue;
        }

        public static void SetEnum<T>(string key, T value) where T : struct
        {
            PlayerPrefs.SetString(key, value.ToString());
        }
    }
}
