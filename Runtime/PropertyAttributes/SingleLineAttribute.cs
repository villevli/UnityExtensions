using System;
using UnityEngine;

namespace VLExtensions
{
    /// <summary>
    /// Add this attribute to a serialized field to draw it on a single line even if it's a struct with multiple fields.
    /// Don't use for structs with too many fields and keep the field names short or set <see cref="hideFieldLabels"/> to true.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SingleLineAttribute : PropertyAttribute
    {
        /// <summary>
        /// Hide the name of the property. Useful in lists to hide the 'Element 1', 'Element 2' labels.
        /// </summary>
        public bool hideLabel { get; set; } = false;
        /// <summary>
        /// Hide the names of the child properties. Useful when they are redundant. e.g. enum fields.
        /// Note that float and int fields will lose the label dragging and context menu functionality.
        /// </summary>
        public bool hideFieldLabels { get; set; } = false;
    }
}
