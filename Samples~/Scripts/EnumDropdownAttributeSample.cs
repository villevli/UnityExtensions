using System;
using UnityEngine;
using VLExtensions;

namespace VLExtensionsSamples
{
    public class EnumDropdownAttributeSample : MonoBehaviour
    {
        [EnumDropdown(typeof(DayOfWeek))]
        public string _string;
        [EnumDropdown(typeof(DayOfWeek))]
        public int _int;
    }
}
