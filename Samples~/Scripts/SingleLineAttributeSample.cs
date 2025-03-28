using System;
using System.Collections.Generic;
using UnityEngine;
using VLExtensions;

namespace VLExtensionsSamples
{
    public class SingleLineAttributeSample : MonoBehaviour
    {
        [Serializable]
        public struct TestStruct
        {
            public DayOfWeek Day;
            public float Value;
            public UnityEngine.Object Object;
        }

        [Header("These struct fields are drawn on a single line each")]
        [SingleLine]
        public TestStruct _singleLine;

        [SingleLine(hideFieldLabels = true)]
        public TestStruct _hiddenFieldNames;

        [SingleLine(hideLabel = true, hideFieldLabels = true)]
        public List<TestStruct> _listWithNoLabels = new() { new(), new() };

        [Header("Also makes some built in types more compact")]
        [SingleLine]
        public Vector4 _vector4;
        [SingleLine]
        public Rect _rect;
        [SingleLine]
        public Bounds _bounds;
    }
}
