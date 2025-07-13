using System;
using System.Collections.Generic;
using UnityEngine;
using VLExtensions;

namespace VLExtensionsSamples
{
    public class SDictionarySample : MonoBehaviour
    {
        [Serializable]
        public class MyData
        {
            public string Name;
            public float Value1;
            public int Value2;
            public List<string> List = new();
        }

        public SDictionary<string, MyData> _stringData = new();
        public SDictionary<DayOfWeek, int> _weekDays = new();
    }
}
