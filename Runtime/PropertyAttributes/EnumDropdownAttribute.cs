using System;
using UnityEngine;

namespace VLExtensions
{
    /// <summary>
    /// Add this attribute to a string or int field to draw it as an enum dropdown.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EnumDropdownAttribute : PropertyAttribute
    {
        public Type enumType { get; set; }
        public Enum defaultValue { get; set; }

        /// <param name="enumType"></param>
        /// <param name="defaultValue">Value shown if the underlying value cannot be parsed as the enum. Defaults to 0.</param>
        public EnumDropdownAttribute(Type enumType, object defaultValue = default)
        {
            // Errors are shown in inspector via EnumDropdownAttributeDrawer
            if (enumType == null)
                return;
            this.enumType = enumType;
            try
            {
                this.defaultValue = defaultValue != null ?
                      (Enum)Enum.ToObject(enumType, defaultValue)
                    : (Enum)Enum.ToObject(enumType, 0);
            }
            catch { }
        }
    }
}
