using System;
using System.Globalization;
using UnityEngine;

namespace VLExtensions
{
    /// <summary>
    /// DateTime value that can be serialized in Unity and displayed in inspector.
    /// Serializes the value in UTC into a standard and culture invariant string.
    /// </summary>
    [Serializable]
    public struct SDateTime : IFormattable, ISerializationCallbackReceiver
    {
        [SerializeField]
        [Delayed]
        private string data;

        private DateTimeOffset value;

        public SDateTime(DateTime dateTime)
            : this(dateTime == default ? default : (DateTimeOffset)dateTime)
        {
        }

        public SDateTime(DateTimeOffset dateTime)
        {
            value = dateTime.ToUniversalTime();
            data = default;
        }

        public readonly DateTime ToDateTime() => value.UtcDateTime;
        public static SDateTime FromDateTime(DateTime value) => new(value);

        public readonly DateTimeOffset ToDateTimeOffset() => value;
        public static SDateTime FromDateTimeOffset(DateTimeOffset value) => new(value);

        public readonly string ToString(string format, IFormatProvider formatProvider)
        {
            return ToDateTime().ToString(format, formatProvider);
        }

        public override readonly string ToString()
        {
            return ToDateTime().ToString();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            data = ToString(value);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            value = ToDateTimeOffset(data);
        }

        public static implicit operator DateTime(SDateTime v) => v.ToDateTime();
        public static implicit operator SDateTime(DateTime v) => FromDateTime(v);

        public static implicit operator DateTimeOffset(SDateTime v) => v.ToDateTimeOffset();
        public static implicit operator SDateTime(DateTimeOffset v) => FromDateTimeOffset(v);

        /// <summary>
        /// Converts the <see cref="DateTime"/> to a string in standard formatting.
        /// Not affected by the current culture.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToString(DateTime dateTime)
        {
            if (dateTime == default)
                return "";
            return ToString((DateTimeOffset)dateTime);
        }

        /// <summary>
        /// Converts the <see cref="DateTimeOffset"/> to a string in standard formatting.
        /// Not affected by the current culture.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToString(DateTimeOffset dateTime)
        {
            if (dateTime == default)
                return "";
            if (dateTime.Millisecond != 0)
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo);
            else
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Converts the string representation of a date and time to it's <see cref="DateTime"/> equivalent.
        /// Always returns a UTC datetime. If conversion fails, returns the default <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(string str)
        {
            var dto = ToDateTimeOffset(str);
            if (dto == default)
                return default;
            return dto.UtcDateTime;
        }

        /// <summary>
        /// Converts the string representation of a date and time to it's <see cref="DateTimeOffset"/> equivalent.
        /// If conversion fails, returns the default <see cref="DateTimeOffset"/> value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return default;
            if (TryParseDateTimeOffset(str, out var value))
                return value;
            return default;
        }

        public static bool TryParseDateTimeOffset(string str, out DateTimeOffset result)
        {
            if (DateTimeOffset.TryParse(str, DateTimeFormatInfo.InvariantInfo,
                                        DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                                        out result))
            {
                return true;
            }
            // As fallback try to interpret as unix time.
            if (double.TryParse(str, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out var parsedDouble))
            {
                try
                {
                    result = DateTimeOffset.FromUnixTimeMilliseconds((long)(parsedDouble * 1000));
                    return true;
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
            result = default;
            return false;
        }
    }
}
