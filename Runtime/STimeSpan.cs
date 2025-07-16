using System;
using System.Globalization;
using UnityEngine;

namespace VLExtensions
{
    /// <summary>
    /// TimeSpan value that can be serialized in Unity and displayed in inspector.
    /// Serializes into a standard and culture invariant string.
    /// </summary>
    [Serializable]
    public struct STimeSpan : IFormattable, ISerializationCallbackReceiver
    {
        [SerializeField]
        [Delayed]
        private string data;

        private TimeSpan value;

        private STimeSpan(TimeSpan timeSpan)
        {
            value = timeSpan;
            data = default;
        }

        public readonly TimeSpan ToTimeSpan() => value;

        public static STimeSpan FromTimeSpan(TimeSpan value) => new(value);

        public readonly string ToString(string format, IFormatProvider formatProvider)
        {
            return ToTimeSpan().ToString(format, formatProvider);
        }

        public override readonly string ToString()
        {
            return ToTimeSpan().ToString();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            data = ToString(value);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            value = ToTimeSpan(data);
        }

        public static implicit operator TimeSpan(STimeSpan v) => v.ToTimeSpan();
        public static implicit operator STimeSpan(TimeSpan v) => FromTimeSpan(v);

        /// <summary>
        /// Converts the <see cref="TimeSpan"/> to a string in standard formatting.
        /// Not affected by the current culture.
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static string ToString(TimeSpan timeSpan)
        {
            return timeSpan.ToString();
        }

        /// <summary>
        /// Converts the string representation of a time span to it's <see cref="TimeSpan"/> equivalent.
        /// If conversion fails, returns the default <see cref="TimeSpan"/> value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return default;
            if (TimeSpan.TryParse(str, CultureInfo.InvariantCulture, out var parsedDt))
                return parsedDt;
            return default;
        }
    }
}
