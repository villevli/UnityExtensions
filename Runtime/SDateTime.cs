using System;
using System.Globalization;
using UnityEngine;

namespace VLExtensions
{
    /// <summary>
    /// DateTime value that can be serialized in Unity and displayed in inspector.
    /// Serializes into a standard and culture invariant string.
    /// </summary>
    [Serializable]
    public struct SDateTime : IFormattable
    {
        [SerializeField]
        private string timestamp;

        private SDateTime(string timestamp)
        {
            this.timestamp = timestamp;
        }

        public SDateTime(DateTime dateTime)
            : this(ToString(dateTime))
        {
        }

        public readonly DateTime ToDateTime() => ToDateTime(timestamp);

        public static SDateTime FromDateTime(DateTime value) => new(value);

        public readonly string ToString(string format, IFormatProvider formatProvider)
        {
            return ToDateTime().ToString(format, formatProvider);
        }

        public override readonly string ToString()
        {
            return ToDateTime().ToString();
        }

        public static implicit operator DateTime(SDateTime v) => v.ToDateTime();
        public static implicit operator SDateTime(DateTime v) => FromDateTime(v);

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
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
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
            if (!string.IsNullOrWhiteSpace(str)
                && DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture,
                                           DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal,
                                           out var parsedDt))
            {
                return parsedDt;
            }

            return default;
        }
    }
}
