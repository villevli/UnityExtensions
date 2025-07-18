using System;
using UnityEngine;
using VLExtensions;

namespace VLExtensionsSamples
{
    public class SavedPrefsSample : MonoBehaviour
    {
        private bool BoolValue
        {
            get => SavedPrefs.GetBool(nameof(SavedPrefsSample) + nameof(BoolValue));
            set => SavedPrefs.SetBool(nameof(SavedPrefsSample) + nameof(BoolValue), value);
        }

        private long LongValue
        {
            get => SavedPrefs.GetLong(nameof(SavedPrefsSample) + nameof(LongValue));
            set => SavedPrefs.SetLong(nameof(SavedPrefsSample) + nameof(LongValue), value);
        }

        private double DoubleValue
        {
            get => SavedPrefs.GetDouble(nameof(SavedPrefsSample) + nameof(DoubleValue));
            set => SavedPrefs.SetDouble(nameof(SavedPrefsSample) + nameof(DoubleValue), value);
        }

        private DateTimeOffset DateTimeValue
        {
            get => SavedPrefs.GetDateTime(nameof(SavedPrefsSample) + nameof(DateTimeValue));
            set => SavedPrefs.SetDateTime(nameof(SavedPrefsSample) + nameof(DateTimeValue), value);
        }

        private DayOfWeek EnumValue
        {
            get => SavedPrefs.GetEnum(nameof(SavedPrefsSample) + nameof(EnumValue), DayOfWeek.Monday);
            set => SavedPrefs.SetEnum(nameof(SavedPrefsSample) + nameof(EnumValue), value);
        }
    }
}
